﻿// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if WIN8METRO

using System;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using WinRTPointerDeviceType = Windows.Devices.Input.PointerDeviceType;
using WinRTPointerPoint = Windows.UI.Input.PointerPoint;

namespace SharpDX.Toolkit.Input
{
    /// <summary>
    /// Represents a specific <see cref="MousePlatform"/> implementation for the WinRT platform
    /// </summary>
    internal class MousePlatformWinRT : MousePlatform
    {
        // keep the mouse cursor location from last event
        private int pointerX;
        private int pointerY;

        /// <summary>
        /// Initializes a new instance of <see cref="MousePlatformWinRT"/> class.
        /// </summary>
        /// <param name="nativeWindow">A reference to <see cref="CoreWindow"/> or <see cref="UIElement"/> class.</param>
        internal MousePlatformWinRT(object nativeWindow) : base(nativeWindow) { }

        /// <summary>
        /// Binds to specific events of the provided CoreWindow
        /// </summary>
        /// <param name="nativeWindow">A reference to <see cref="CoreWindow"/> or <see cref="UIElement"/> class.</param>
        /// <exception cref="ArgumentNullException">Is thrown when <paramref name="nativeWindow"/> is null.</exception>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="nativeWindow"/> is not a <see cref="CoreWindow"/> and not an <see cref="UIElement"/></exception>
        protected override void BindWindow(object nativeWindow)
        {
            if (nativeWindow == null) throw new ArgumentNullException("nativeWindow");
            var window = nativeWindow as CoreWindow;
            if (window != null)
            {
                window.PointerPressed += HandleWindowPointerEvent;
                window.PointerReleased += HandleWindowPointerEvent;
                window.PointerWheelChanged += HandleWindowPointerEvent;
                window.PointerMoved += HandleWindowPointerEvent;
                return;
            }

            var uiElement = nativeWindow as UIElement;
            if (uiElement != null)
            {
                uiElement.PointerPressed += HandleUIElementPointerEvent;
                uiElement.PointerReleased += HandleUIElementPointerEvent;
                uiElement.PointerWheelChanged += HandleUIElementPointerEvent;
                uiElement.PointerMoved += HandleUIElementPointerEvent;
                return;
            }

            throw new ArgumentException("Should be an instance of either CoreWindow or UIElement", "nativeWindow");
        }

        /// <summary>
        /// Returns the mouse cursor position from cached values
        /// </summary>
        /// <param name="nativeWindow">A reference to <see cref="CoreWindow"/> or <see cref="UIElement"/> class.</param>
        /// <returns>The location of mouse cursor</returns>
        protected override DrawingPoint GetLocationInternal(object nativeWindow)
        {
            return new DrawingPoint(pointerX, pointerY);
        }

        /// <summary>
        /// Handles the <see cref="CoreWindow.PointerPressed"/>, <see cref="CoreWindow.PointerReleased"/>, <see cref="CoreWindow.PointerWheelChanged"/> and <see cref="CoreWindow.PointerMoved"/> events
        /// </summary>
        /// <param name="sender">Sender widow. Not used</param>
        /// <param name="args">Pointer event arguments.</param>
        private void HandleWindowPointerEvent(CoreWindow sender, PointerEventArgs args)
        {
            var p = args.CurrentPoint;
            // if the current device is not a mouse - ignore it
            if (p.PointerDevice.PointerDeviceType != WinRTPointerDeviceType.Mouse) return;

            UpdateMouse(p);

            args.Handled = true;
        }

        /// <summary>
        /// Handles the <see cref="UIElement.PointerPressed"/>, <see cref="UIElement.PointerReleased"/>, <see cref="UIElement.PointerWheelChanged"/> and <see cref="UIElement.PointerMoved"/> events
        /// </summary>
        /// <param name="sender">Sender element.</param>
        /// <param name="args">Pointer event arguments.</param>
        /// <exception cref="ArgumentNullException">Is thrown if either <paramref name="sender"/>  or <paramref name="args"/> are null</exception>
        /// <exception cref="InvalidCastException">Is thrown if <paramref name="sender"/> is not an <see cref="UIElement"/></exception>
        private void HandleUIElementPointerEvent(object sender, PointerRoutedEventArgs args)
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (args == null) throw new ArgumentNullException("args");

            var p = args.GetCurrentPoint((UIElement)sender);
            // if the current device is not a mouse - ignore it
            if (p.PointerDevice.PointerDeviceType != WinRTPointerDeviceType.Mouse) return;

            UpdateMouse(p);

            args.Handled = true;
        }

        /// <summary>
        /// Raises corresponding events to update mouse state
        /// </summary>
        /// <param name="p"><see cref="PointerPoint"/> instance that contains all needed information</param>
        private void UpdateMouse(WinRTPointerPoint p)
        {
            // adjust mouse position from Device-Independent-Pixels to physical pixels
            var dipFactor = DisplayProperties.LogicalDpi / 96.0f;
            pointerX = (int)(p.Position.X * dipFactor);
            pointerY = (int)(p.Position.Y * dipFactor);

            // update mouse wheel delta
            OnMouseWheel(p.Properties.MouseWheelDelta);

            var properties = p.Properties;

            // update mouse buttons state
            RaiseButtonChange(properties.IsLeftButtonPressed, MouseButton.Left);
            RaiseButtonChange(properties.IsMiddleButtonPressed, MouseButton.Middle);
            RaiseButtonChange(properties.IsRightButtonPressed, MouseButton.Right);
            RaiseButtonChange(properties.IsXButton1Pressed, MouseButton.XButton1);
            RaiseButtonChange(properties.IsXButton2Pressed, MouseButton.XButton2);
        }

        /// <summary>
        /// Raises corresponding event depending on mouse button state
        /// </summary>
        /// <param name="isDown">A flag that indicates if the button is pressed or not.</param>
        /// <param name="button">Mouse button whose state needs to be updated.</param>
        private void RaiseButtonChange(bool isDown, MouseButton button)
        {
            if (isDown)
                OnMouseDown(button);
            else
                OnMouseUp(button);
        }
    }
}

#endif
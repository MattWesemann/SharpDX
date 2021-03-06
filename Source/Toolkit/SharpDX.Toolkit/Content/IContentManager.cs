// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
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

using System;

namespace SharpDX.Toolkit.Content
{
    /// <summary>
    /// The content manager interface provides a service to load and store content data (texture, songs, effects...etc.).
    /// </summary>
    public interface IContentManager
    {
        /// <summary>
        /// Gets the service provider associated with the ContentManager.
        /// </summary>
        /// <value>The service provider.</value>
        /// <remarks>
        /// The service provider can be used by some <see cref="IContentReader"/> when for example a <see cref="SharpDX.Toolkit.Graphics.GraphicsDevice"/> needs to be 
        /// used to instantiate a content.
        /// </remarks>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Loads an asset that has been processed by the Content Pipeline.  Reference page contains code sample.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetNameWithExtension">Full asset name (with its extension)</param>
        /// <returns>``0.</returns>
        /// <exception cref="SharpDX.Toolkit.Content.AssetNotFoundException">If the asset was not found from all <see cref="IContentResolver"/>.</exception>
        /// <exception cref="System.NotSupportedException">If no content reader was suitable to decode the asset.</exception>
        T Load<T>(string assetNameWithExtension);

        /// <summary>
        /// Unloads all data that was loaded by this ContentManager. All data will be disposed.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="ContentManager.Load{T}"/> method, this method is not threadsafe and must be called by a single caller at a single time.
        /// </remarks>
        void Unload();
    }
}
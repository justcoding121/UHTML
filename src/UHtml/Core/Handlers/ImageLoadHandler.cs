using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Entities;
using UHtml.Core.Utils;

namespace UHtml.Core.Handlers
{
    /// <summary>
    /// Handler for all loading image logic.<br/>
    /// <p>
    /// Loading by <see cref="HtmlImageLoadEventArgs"/>.<br/>
    /// Loading by file path.<br/>
    /// Loading by URI.<br/>
    /// </p>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Supports sync and async image loading.
    /// </para>
    /// <para>
    /// If the image object is created by the handler on calling dispose of the handler the image will be released, this
    /// makes release of unused images faster as they can be large.<br/>
    /// Disposing image load handler will also cancel download of image from the web.
    /// </para>
    /// </remarks>
    internal sealed class ImageLoadHandler : IDisposable
    {
        #region Fields and Consts

        /// <summary>
        /// the container of the html to handle load image for
        /// </summary>
        private readonly HtmlContainerInt htmlContainer;

        /// <summary>
        /// callback raised when image load process is complete with image or without
        /// </summary>
        private readonly ActionInt<RImage, RRect, bool> loadCompleteCallback;

        /// <summary>
        /// Must be open as long as the image is in use
        /// </summary>
        private Stream imageFileStream;

        /// <summary>
        /// the image instance of the loaded image
        /// </summary>
        private RImage image;

        /// <summary>
        /// the image rectangle restriction as returned from image load event
        /// </summary>
        private RRect imageRectangle;

        /// <summary>
        /// to know if image load event callback was sync or async raised
        /// </summary>
        private bool asyncCallback;

        /// <summary>
        /// flag to indicate if to release the image object on box dispose (only if image was loaded by the box)
        /// </summary>
        private bool releaseImageObject;

        /// <summary>
        /// is the handler has been disposed
        /// </summary>
        private bool disposed;

        #endregion

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="htmlContainer">the container of the html to handle load image for</param>
        /// <param name="loadCompleteCallback">callback raised when image load process is complete with image or without</param>
        public ImageLoadHandler(HtmlContainerInt htmlContainer, ActionInt<RImage, RRect, bool> loadCompleteCallback)
        {
            ArgChecker.AssertArgNotNull(htmlContainer, "htmlContainer");
            ArgChecker.AssertArgNotNull(loadCompleteCallback, "loadCompleteCallback");

            this.htmlContainer = htmlContainer;
            this.loadCompleteCallback = loadCompleteCallback;
        }

        /// <summary>
        /// the image instance of the loaded image
        /// </summary>
        public RImage Image
        {
            get { return image; }
        }

        /// <summary>
        /// the image rectangle restriction as returned from image load event
        /// </summary>
        public RRect Rectangle
        {
            get { return imageRectangle; }
        }

        /// <summary>
        /// Set image of this image box by analyzing the src attribute.<br/>
        /// Load the image from inline base64 encoded string.<br/>
        /// Or from calling property/method on the bridge object that returns image or URL to image.<br/>
        /// Or from file path<br/>
        /// Or from URI.
        /// </summary>
        /// <remarks>
        /// File path and URI image loading is executed async and after finishing calling <see cref="ImageLoadComplete"/>
        /// on the main thread and not thread-pool.
        /// </remarks>
        /// <param name="src">the source of the image to load</param>
        /// <param name="attributes">the collection of attributes on the element to use in event</param>
        /// <returns>the image object (null if failed)</returns>
        public void LoadImage(string src, Dictionary<string, string> attributes)
        {
            try
            {
                var args = new HtmlImageLoadEventArgs(src, attributes, OnHtmlImageLoadEventCallback);
                htmlContainer.RaiseHtmlImageLoadEvent(args);
                asyncCallback = !htmlContainer.AvoidAsyncImagesLoading;

                if (!args.Handled)
                {
                    if (!string.IsNullOrEmpty(src))
                    {
                        if (src.StartsWith("data:image", StringComparison.CurrentCultureIgnoreCase))
                        {
                            SetFromInlineData(src);
                        }
                        else
                        {
                            SetImageFromPath(src);
                        }
                    }
                    else
                    {
                        ImageLoadComplete(false);
                    }
                }
            }
            catch (Exception ex)
            {
                htmlContainer.ReportError(HtmlRenderErrorType.Image, "Exception in handling image source", ex);
                ImageLoadComplete(false);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            disposed = true;
            ReleaseObjects();
        }


        #region Private methods

        /// <summary>
        /// Set the image using callback from load image event, use the given data.
        /// </summary>
        /// <param name="path">the path to the image to load (file path or uri)</param>
        /// <param name="image">the image to load</param>
        /// <param name="imageRectangle">optional: limit to specific rectangle of the image and not all of it</param>
        private void OnHtmlImageLoadEventCallback(string path, object image, RRect imageRectangle)
        {
            if (!disposed)
            {
                this.imageRectangle = imageRectangle;

                if (image != null)
                {
                    this.image = htmlContainer.Adapter.ConvertImage(image);
                    ImageLoadComplete(asyncCallback);
                }
                else if (!string.IsNullOrEmpty(path))
                {
                    SetImageFromPath(path);
                }
                else
                {
                    ImageLoadComplete(asyncCallback);
                }
            }
        }

        /// <summary>
        /// Load the image from inline base64 encoded string data.
        /// </summary>
        /// <param name="src">the source that has the base64 encoded image</param>
        private void SetFromInlineData(string src)
        {
            image = GetImageFromData(src);
            if (image == null)
                htmlContainer.ReportError(HtmlRenderErrorType.Image, "Failed extract image from inline data");
            releaseImageObject = true;
            ImageLoadComplete(false);
        }

        /// <summary>
        /// Extract image object from inline base64 encoded data in the src of the html img element.
        /// </summary>
        /// <param name="src">the source that has the base64 encoded image</param>
        /// <returns>image from base64 data string or null if failed</returns>
        private RImage GetImageFromData(string src)
        {
            var s = src.Substring(src.IndexOf(':') + 1).Split(new[] { ',' }, 2);
            if (s.Length == 2)
            {
                int imagePartsCount = 0, base64PartsCount = 0;
                foreach (var part in s[0].Split(new[] { ';' }))
                {
                    var pPart = part.Trim();
                    if (pPart.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                        imagePartsCount++;
                    if (pPart.Equals("base64", StringComparison.OrdinalIgnoreCase))
                        base64PartsCount++;
                }

                if (imagePartsCount > 0)
                {
                    byte[] imageData = base64PartsCount > 0 ? Convert.FromBase64String(s[1].Trim()) : new UTF8Encoding().GetBytes(Uri.UnescapeDataString(s[1].Trim()));
                    return htmlContainer.Adapter.ImageFromStream(new MemoryStream(imageData));
                }
            }
            return null;
        }

        /// <summary>
        /// Load image from path of image file or URL.
        /// </summary>
        /// <param name="path">the file path or uri to load image from</param>
        private void SetImageFromPath(string path)
        {
            var uri = CommonUtils.TryGetUri(path);
            if (uri != null && uri.Scheme != "file")
            {
                SetImageFromUrl(uri);
            }
            else
            {
                var fileInfo = CommonUtils.TryGetFileInfo(uri != null ? uri.AbsolutePath : path);
                if (fileInfo != null)
                {
                    SetImageFromFile(fileInfo);
                }
                else
                {
                    htmlContainer.ReportError(HtmlRenderErrorType.Image, "Failed load image, invalid source: " + path);
                    ImageLoadComplete(false);
                }
            }
        }

        /// <summary>
        /// Load the image file on thread-pool thread and calling <see cref="ImageLoadComplete"/> after.
        /// </summary>
        /// <param name="source">the file path to get the image from</param>
        private void SetImageFromFile(string src)
        {
            if (StorageUtils.FileExists(src))
            {
                if (htmlContainer.AvoidAsyncImagesLoading)
                    LoadImageFromFile(src);
                else
                    Task.Run(()=> LoadImageFromFile(src));
            }
            else
            {
                ImageLoadComplete();
            }
        }

        /// <summary>
        /// Load the image file on thread-pool thread and calling <see cref="ImageLoadComplete"/> after.<br/>
        /// Calling <see cref="ImageLoadComplete"/> on the main thread and not thread-pool.
        /// </summary>
        /// <param name="source">the file path to get the image from</param>
        private void LoadImageFromFile(string source)
        {
            try
            {
                var file = FileSystem.Current.GetFileFromPathAsync(source).Result;
                var imageFileStream = file.OpenAsync(FileAccess.ReadAndWrite).Result;
                lock (loadCompleteCallback)
                {
                    this.imageFileStream = imageFileStream;
                    if (!disposed)
                        image = htmlContainer.Adapter.ImageFromStream(this.imageFileStream);
                    releaseImageObject = true;
                }
                ImageLoadComplete();
            }
            catch (Exception ex)
            {
                htmlContainer.ReportError(HtmlRenderErrorType.Image, "Failed to load image from disk: " + source, ex);
                ImageLoadComplete();
            }
        }

        /// <summary>
        /// Load image from the given URI by downloading it.<br/>
        /// Create local file name in temp folder from the URI, if the file already exists use it as it has already been downloaded.
        /// If not download the file.
        /// </summary>
        private void SetImageFromUrl(Uri source)
        {
            var filePath = CommonUtils.GetLocalfilePath(source);
            if (StorageUtils.FileExists(filePath) && filePath.Length > 0)
            {
                SetImageFromFile(filePath);
            }
            else
            {
                htmlContainer.GetImageDownloader().DownloadImage(source, filePath, !htmlContainer.AvoidAsyncImagesLoading, OnDownloadImageCompleted);
            }
        }

        /// <summary>
        /// On download image complete to local file use <see cref="LoadImageFromFile"/> to load the image file.<br/>
        /// If the download canceled do nothing, if failed report error.
        /// </summary>
        private void OnDownloadImageCompleted(Uri imageUri, string filePath, Exception error, bool canceled)
        {
            if (!canceled && !disposed)
            {
                if (error == null)
                {
                    LoadImageFromFile(filePath);
                }
                else
                {
                    htmlContainer.ReportError(HtmlRenderErrorType.Image, "Failed to load image from URL: " + imageUri, error);
                    ImageLoadComplete();
                }
            }
        }

        /// <summary>
        /// Flag image load complete and request refresh for re-layout and invalidate.
        /// </summary>
        private void ImageLoadComplete(bool async = true)
        {
            // can happen if some operation return after the handler was disposed
            if (disposed)
                ReleaseObjects();
            else
                loadCompleteCallback(image, imageRectangle, async);
        }

        /// <summary>
        /// Release the image and client objects.
        /// </summary>
        private void ReleaseObjects()
        {
            lock (loadCompleteCallback)
            {
                if (releaseImageObject && image != null)
                {
                    image.Dispose();
                    image = null;
                }
                if (imageFileStream != null)
                {
                    imageFileStream.Dispose();
                    imageFileStream = null;
                }
            }
        }

        #endregion
    }
}
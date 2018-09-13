using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Entities;
using UHtml.Core.Handlers;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    /// <summary>
    /// CSS box for iframe element.<br/>
    /// If the iframe is of embedded YouTube or Vimeo video it will show image with play.
    /// </summary>
    internal sealed class CssBoxFrame : CssBox
    {
        #region Fields and Consts

        /// <summary>
        /// the image word of this image box
        /// </summary>
        private readonly CssRectImage imageWord;

        /// <summary>
        /// is the iframe is of embeded video
        /// </summary>
        private readonly bool isVideo;

        /// <summary>
        /// the title of the video
        /// </summary>
        private string videoTitle;

        /// <summary>
        /// the url of the video thumbnail image
        /// </summary>
        private string videoImageUrl;

        /// <summary>
        /// link to the video on the site
        /// </summary>
        private string videoLinkUrl;

        /// <summary>
        /// handler used for image loading by source
        /// </summary>
        private ImageLoadHandler imageLoadHandler;

        /// <summary>
        /// is image load is finished, used to know if no image is found
        /// </summary>
        private bool imageLoadingComplete;
        #endregion

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="parent">the parent box of this box</param>
        /// <param name="tag">the html tag data of this box</param>
        public CssBoxFrame(CssBox parent, HtmlTag tag)
            : base(parent, tag)
        {
            imageWord = new CssRectImage(this);
            Words.Add(imageWord);

            Uri uri;
            if (Uri.TryCreate(GetAttribute("src"), UriKind.Absolute, out uri))
            {
                //    if (uri.Host.IndexOf("youtube.com", StringComparison.OrdinalIgnoreCase) > -1)
                //    {
                //        isVideo = true;
                //        LoadYoutubeDataAsync(uri);
                //    }
                //    else if (uri.Host.IndexOf("vimeo.com", StringComparison.OrdinalIgnoreCase) > -1)
                //    {
                //        isVideo = true;
                //        LoadVimeoDataAsync(uri);
                //    }
                throw new NotImplementedException();
            }

            if (!isVideo)
            {
                SetErrorBorder();
            }
        }

        /// <summary>
        /// Is the css box clickable ("a" element is clickable)
        /// </summary>
        public override bool IsClickable
        {
            get { return true; }
        }

        /// <summary>
        /// Get the href link of the box (by default get "href" attribute)
        /// </summary>
        public override string HrefLink
        {
            get { return videoLinkUrl ?? GetAttribute("src"); }
        }

        /// <summary>
        /// is the iframe is of embeded video
        /// </summary>
        public bool IsVideo
        {
            get { return isVideo; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (imageLoadHandler != null)
                imageLoadHandler.Dispose();
            base.Dispose();
        }


        #region Private methods

        /// <summary>
        /// Load YouTube video data (title, image, link) by calling YouTube API.
        /// </summary>
        private void LoadYoutubeDataAsync(Uri uri)
        {
            //Task.Run(() =>
            // {
            //     try
            //     {
            //         var apiUri = new Uri(string.Format("http://gdata.youtube.com/feeds/api/videos/{0}?v=2&alt=json", uri.Segments[2]));

            //         var handler = IocModule.Container.GetInstance<HttpClientHandler>();
            //         var client = new HttpClient(handler);
            //         HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUri);
            //         var response = client.SendAsync(request).Result;
            //         var byteArray = response.Content.ReadAsByteArrayAsync().Result;
            //         var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            //         OnDownloadYoutubeApiCompleted(client, responseString);

            //     }
            //     catch (Exception ex)
            //     {
            //         HandleDataLoadFailure(ex, "YouTube");
            //         HtmlContainer.ReportError(HtmlRenderErrorType.Iframe, "Failed to get youtube video data: " + uri, ex);
            //         HtmlContainer.RequestRefresh(false);
            //     }
            // });
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse YouTube API response to get video data (title, image, link).
        /// </summary>
        //private void OnDownloadYoutubeApiCompleted(HttpClient sender, string result)
        //{

        //    try
        //    {

        //        var idx = result.IndexOf("\"media$title\"", StringComparison.Ordinal);
        //        if (idx > -1)
        //        {
        //            idx = result.IndexOf("\"$t\"", idx);
        //            if (idx > -1)
        //            {
        //                idx = result.IndexOf('"', idx + 4);
        //                if (idx > -1)
        //                {
        //                    var endIdx = result.IndexOf('"', idx + 1);
        //                    while (result[endIdx - 1] == '\\')
        //                        endIdx = result.IndexOf('"', endIdx + 1);
        //                    if (endIdx > -1)
        //                    {
        //                        videoTitle = result.Substring(idx + 1, endIdx - idx - 1).Replace("\\\"", "\"");
        //                    }
        //                }
        //            }
        //        }

        //        idx = result.IndexOf("\"media$thumbnail\"", StringComparison.Ordinal);
        //        if (idx > -1)
        //        {
        //            var iidx = result.IndexOf("sddefault", idx);
        //            if (iidx > -1)
        //            {
        //                if (string.IsNullOrEmpty(Width))
        //                    Width = "640px";
        //                if (string.IsNullOrEmpty(Height))
        //                    Height = "480px";
        //            }
        //            else
        //            {
        //                iidx = result.IndexOf("hqdefault", idx);
        //                if (iidx > -1)
        //                {
        //                    if (string.IsNullOrEmpty(Width))
        //                        Width = "480px";
        //                    if (string.IsNullOrEmpty(Height))
        //                        Height = "360px";
        //                }
        //                else
        //                {
        //                    iidx = result.IndexOf("mqdefault", idx);
        //                    if (iidx > -1)
        //                    {
        //                        if (string.IsNullOrEmpty(Width))
        //                            Width = "320px";
        //                        if (string.IsNullOrEmpty(Height))
        //                            Height = "180px";
        //                    }
        //                    else
        //                    {
        //                        iidx = result.IndexOf("default", idx);
        //                        if (string.IsNullOrEmpty(Width))
        //                            Width = "120px";
        //                        if (string.IsNullOrEmpty(Height))
        //                            Height = "90px";
        //                    }
        //                }
        //            }

        //            iidx = result.LastIndexOf("http:", iidx, StringComparison.Ordinal);
        //            if (iidx > -1)
        //            {
        //                var endIdx = result.IndexOf('"', iidx);
        //                if (endIdx > -1)
        //                {
        //                    videoImageUrl = result.Substring(iidx, endIdx - iidx).Replace("\\\"", "\"").Replace("\\", "");
        //                }
        //            }
        //        }

        //        idx = result.IndexOf("\"link\"", StringComparison.Ordinal);
        //        if (idx > -1)
        //        {
        //            idx = result.IndexOf("http:", idx);
        //            if (idx > -1)
        //            {
        //                var endIdx = result.IndexOf('"', idx);
        //                if (endIdx > -1)
        //                {
        //                    videoLinkUrl = result.Substring(idx, endIdx - idx).Replace("\\\"", "\"").Replace("\\", "");
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        HtmlContainer.ReportError(HtmlRenderErrorType.Iframe, "Failed to parse YouTube video response", ex);
        //    }

        //    HandlePostApiCall(sender);
        //}

        ///// <summary>
        ///// Load Vimeo video data (title, image, link) by calling Vimeo API.
        ///// </summary>
        //private void LoadVimeoDataAsync(Uri uri)
        //{
        //    Task.Run(() =>
        //    {
        //        try
        //        {
        //            var apiUri = new Uri(string.Format("http://vimeo.com/api/v2/video/{0}.json", uri.Segments[2]));

        //            var handler = IocModule.Container.GetInstance<HttpClientHandler>();
        //            var client = new HttpClient(handler);
        //            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUri);
        //            var response = client.SendAsync(request).Result;
        //            var byteArray = response.Content.ReadAsByteArrayAsync().Result;
        //            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
        //            OnDownloadVimeoApiCompleted(client, responseString);
        //        }
        //        catch (Exception ex)
        //        {
        //            HandleDataLoadFailure(ex, "Vimeo");
        //            imageLoadingComplete = true;
        //            SetErrorBorder();
        //            HtmlContainer.ReportError(HtmlRenderErrorType.Iframe, "Failed to get vimeo video data: " + uri, ex);
        //            HtmlContainer.RequestRefresh(false);
        //        }
        //    });
        //}

        /// <summary>
        /// Parse Vimeo API response to get video data (title, image, link).
        /// </summary>
        private void OnDownloadVimeoApiCompleted(object sender, string result)
        {
            try
            {

                var idx = result.IndexOf("\"title\"", StringComparison.Ordinal);
                if (idx > -1)
                {
                    idx = result.IndexOf('"', idx + 7);
                    if (idx > -1)
                    {
                        var endIdx = result.IndexOf('"', idx + 1);
                        while (result[endIdx - 1] == '\\')
                            endIdx = result.IndexOf('"', endIdx + 1);
                        if (endIdx > -1)
                        {
                            videoTitle = result.Substring(idx + 1, endIdx - idx - 1).Replace("\\\"", "\"");
                        }
                    }
                }

                idx = result.IndexOf("\"thumbnail_large\"", StringComparison.Ordinal);
                if (idx > -1)
                {
                    if (string.IsNullOrEmpty(Width))
                        Width = "640";
                    if (string.IsNullOrEmpty(Height))
                        Height = "360";
                }
                else
                {
                    idx = result.IndexOf("thumbnail_medium", idx);
                    if (idx > -1)
                    {
                        if (string.IsNullOrEmpty(Width))
                            Width = "200";
                        if (string.IsNullOrEmpty(Height))
                            Height = "150";
                    }
                    else
                    {
                        idx = result.IndexOf("thumbnail_small", idx);
                        if (string.IsNullOrEmpty(Width))
                            Width = "100";
                        if (string.IsNullOrEmpty(Height))
                            Height = "75";
                    }
                }
                if (idx > -1)
                {
                    idx = result.IndexOf("http:", idx);
                    if (idx > -1)
                    {
                        var endIdx = result.IndexOf('"', idx);
                        if (endIdx > -1)
                        {
                            videoImageUrl = result.Substring(idx, endIdx - idx).Replace("\\\"", "\"").Replace("\\", "");
                        }
                    }
                }

                idx = result.IndexOf("\"url\"", StringComparison.Ordinal);
                if (idx > -1)
                {
                    idx = result.IndexOf("http:", idx);
                    if (idx > -1)
                    {
                        var endIdx = result.IndexOf('"', idx);
                        if (endIdx > -1)
                        {
                            videoLinkUrl = result.Substring(idx, endIdx - idx).Replace("\\\"", "\"").Replace("\\", "");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                HtmlContainer.ReportError(HtmlRenderErrorType.Iframe, "Failed to parse Vimeo video response", ex);
            }

            HandlePostApiCall(sender);
        }

        /// <summary>
        /// Handle error occurred during video data load to handle if the video was not found.
        /// </summary>
        /// <param name="ex">the exception that occurred during data load web request</param>
        /// <param name="source">the name of the video source (YouTube/Vimeo/Etc.)</param>
        private void HandleDataLoadFailure(Exception ex, string source)
        {
            //var webError = ex as WebException;
            //var webResponse = webError != null ? webError.Response as HttpWebResponse : null;
            //if (webResponse != null && webResponse.StatusCode == HttpStatusCode.NotFound)
            //{
            //    videoTitle = "The video is not found, possibly removed by the user.";
            //}
            //else
            //{
            //    HtmlContainer.ReportError(HtmlRenderErrorType.Iframe, "Failed to load " + source + " video data", ex);
            //}

            throw new NotImplementedException();
        }

        /// <summary>
        /// Create image handler for downloading video image if found and release the WebClient instance used for API call.
        /// </summary>
        private void HandlePostApiCall(object sender)
        {
            //try
            //{
            //    if (videoImageUrl == null)
            //    {
            //        imageLoadingComplete = true;
            //        SetErrorBorder();
            //    }

            //    var webClient = (HttpClient)sender;
            //    webClient.Dispose();

            //    HtmlContainer.RequestRefresh(IsLayoutRequired());
            //}
            //catch
            //{ }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g">the device to draw to</param>
        internal override void PaintImp(RGraphics g)
        {
            if (videoImageUrl != null && imageLoadHandler == null)
            {
                imageLoadHandler = new ImageLoadHandler(HtmlContainer, OnLoadImageComplete);
                imageLoadHandler.LoadImage(videoImageUrl, HtmlTag != null ? HtmlTag.Attributes : null);
            }

            var rects = CommonUtils.GetFirstValueOrDefault(Rectangles);

            RPoint offset = (HtmlContainer != null && !IsFixed) ? HtmlContainer.ScrollOffset : RPoint.Empty;
            rects.Offset(offset);

            var clipped = RenderUtils.ClipGraphicsByOverflow(g, this);

            PaintBackground(g, rects, true, true);

            BordersDrawHandler.DrawBoxBorders(g, this, rects, true, true);

            var word = Words[0];
            var tmpRect = word.Rectangle;
            tmpRect.Offset(offset);
            tmpRect.Height -= ActualBorderTopWidth + ActualBorderBottomWidth + ActualPaddingTop + ActualPaddingBottom;
            tmpRect.Y += ActualBorderTopWidth + ActualPaddingTop;
            tmpRect.X = Math.Floor(tmpRect.X);
            tmpRect.Y = Math.Floor(tmpRect.Y);
            var rect = tmpRect;

            DrawImage(g, offset, rect);

            DrawTitle(g, rect);

            DrawPlay(g, rect);

            if (clipped)
                g.PopClip();
        }

        /// <summary>
        /// Draw video image over the iframe if found.
        /// </summary>
        private void DrawImage(RGraphics g, RPoint offset, RRect rect)
        {
            if (imageWord.Image != null)
            {
                if (rect.Width > 0 && rect.Height > 0)
                {
                    if (imageWord.ImageRectangle == RRect.Empty)
                        g.DrawImage(imageWord.Image, rect);
                    else
                        g.DrawImage(imageWord.Image, rect, imageWord.ImageRectangle);

                    if (imageWord.Selected)
                    {
                        g.DrawRectangle(GetSelectionBackBrush(g, true), imageWord.Left + offset.X, imageWord.Top + offset.Y, imageWord.Width + 2, DomUtils.GetCssLineBoxByWord(imageWord).LineHeight);
                    }
                }
            }
            else if (isVideo && !imageLoadingComplete)
            {
                RenderUtils.DrawImageLoadingIcon(g, HtmlContainer, rect);
                if (rect.Width > 19 && rect.Height > 19)
                {
                    g.DrawRectangle(g.GetPen(RColor.LightGray), rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
        }

        /// <summary>
        /// Draw video title on top of the iframe if found.
        /// </summary>
        private void DrawTitle(RGraphics g, RRect rect)
        {
            if (videoTitle != null && imageWord.Width > 40 && imageWord.Height > 40)
            {
                var font = HtmlContainer.Adapter.GetFont("Arial", 9f, RFontStyle.Regular);
                g.DrawRectangle(g.GetSolidBrush(RColor.FromArgb(160, 0, 0, 0)), rect.X1, rect.Y1, rect.Width, ActualFont.Height + 7);

                var titleRect = new RRect(rect.X1 + 3, rect.Y1 + 3, rect.Width - 6, rect.Height - 6);
                g.DrawString(videoTitle, font, RColor.WhiteSmoke, titleRect.Location, RSize.Empty, false);
            }
        }

        /// <summary>
        /// Draw play over the iframe if we found link url.
        /// </summary>
        private void DrawPlay(RGraphics g, RRect rect)
        {
            if (isVideo && imageWord.Width > 70 && imageWord.Height > 50)
            {
                var prevMode = g.SetAntiAliasSmoothingMode();

                var size = new RSize(60, 40);
                var left = rect.X1 + (rect.Width - size.Width) / 2;
                var top = rect.Y1 + (rect.Height - size.Height) / 2;
                g.DrawRectangle(g.GetSolidBrush(RColor.FromArgb(160, 0, 0, 0)), left, top, size.Width, size.Height);

                RPoint[] points =
                {
                    new RPoint(left + size.Width / 3f + 1,top + 3 * size.Height / 4f),
                    new RPoint(left + size.Width / 3f + 1, top + size.Height / 4f),
                    new RPoint(left + 2 * size.Width / 3f + 1, top + size.Height / 2f)
                };
                g.DrawPolygon(g.GetSolidBrush(RColor.White), points);

                g.ReturnPreviousSmoothingMode(prevMode);
            }
        }

        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        /// <param name="g">the device to use</param>
        internal override void MeasureWordsSize(RGraphics g)
        {
            if (!_wordsSizeMeasured)
            {
                MeasureWordSpacing(g);
                _wordsSizeMeasured = true;
            }
            CssLayoutEngine.MeasureImageSize(imageWord);
        }

        /// <summary>
        /// Set error image border on the image box.
        /// </summary>
        private void SetErrorBorder()
        {
            SetAllBorders(CssConstants.Solid, "2px", "#A0A0A0");
            BorderRightColor = BorderBottomColor = "#E3E3E3";
        }

        /// <summary>
        /// On image load process is complete with image or without update the image box.
        /// </summary>
        /// <param name="image">the image loaded or null if failed</param>
        /// <param name="rectangle">the source rectangle to draw in the image (empty - draw everything)</param>
        /// <param name="async">is the callback was called async to load image call</param>
        private void OnLoadImageComplete(RImage image, RRect rectangle, bool async)
        {
            imageWord.Image = image;
            imageWord.ImageRectangle = rectangle;
            imageLoadingComplete = true;
            _wordsSizeMeasured = false;

            if (imageLoadingComplete && image == null)
            {
                SetErrorBorder();
            }

            if (async)
            {
                HtmlContainer.RequestRefresh(IsLayoutRequired());
            }
        }

        private bool IsLayoutRequired()
        {
            var width = new CssLength(Width);
            var height = new CssLength(Height);
            return (width.Number <= 0 || width.Unit != CssUnit.Pixels) || (height.Number <= 0 || height.Unit != CssUnit.Pixels);
        }

        #endregion
    }
}
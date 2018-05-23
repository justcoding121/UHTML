using System;
using System.Globalization;
using System.IO;
using UHtml.Adapters;
using UHtml.Core.Dom;
using UHtml.Core.Entities;
using UHtml.Core.Utils;

namespace UHtml.Core.Handlers
{
    /// <summary>
    /// Handle context menu.
    /// </summary>
    internal sealed class ContextMenuHandler : IDisposable
    {
        #region Fields and Consts

        /// <summary>
        /// select all text
        /// </summary>
        private static readonly string selectAll;

        /// <summary>
        /// copy selected text
        /// </summary>
        private static readonly string copy;

        /// <summary>
        /// copy the link source
        /// </summary>
        private static readonly string copyLink;

        /// <summary>
        /// open link (as left mouse click)
        /// </summary>
        private static readonly string openLink;

        /// <summary>
        /// copy the source of the image
        /// </summary>
        private static readonly string copyImageLink;

        /// <summary>
        /// copy image to clipboard
        /// </summary>
        private static readonly string copyImage;

        /// <summary>
        /// save image to disk
        /// </summary>
        private static readonly string saveImage;

        /// <summary>
        /// open video in browser
        /// </summary>
        private static readonly string openVideo;

        /// <summary>
        /// copy video url to browser
        /// </summary>
        private static readonly string copyVideoUrl;

        /// <summary>
        /// the selection handler linked to the context menu handler
        /// </summary>
        private readonly SelectionHandler selectionHandler;

        /// <summary>
        /// the html container the handler is on
        /// </summary>
        private readonly HtmlContainerInt htmlContainer;

        /// <summary>
        /// the last context menu shown
        /// </summary>
        private RContextMenu contextMenu;

        /// <summary>
        /// the control that the context menu was shown on
        /// </summary>
        private RControl parentControl;

        /// <summary>
        /// the css rectangle that context menu shown on
        /// </summary>
        private CssRect currentRect;

        /// <summary>
        /// the css link box that context menu shown on
        /// </summary>
        private CssBox currentLink;

        #endregion

        /// <summary>
        /// Init context menu items strings.
        /// </summary>
        static ContextMenuHandler()
        {
            if (CultureInfo.CurrentUICulture.Name.StartsWith("fr", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Tout sélectionner";
                copy = "Copier";
                copyLink = "Copier l'adresse du lien";
                openLink = "Ouvrir le lien";
                copyImageLink = "Copier l'URL de l'image";
                copyImage = "Copier l'image";
                saveImage = "Enregistrer l'image sous...";
                openVideo = "Ouvrir la vidéo";
                copyVideoUrl = "Copier l'URL de l'vidéo";
            }
            else if (CultureInfo.CurrentUICulture.Name.StartsWith("de", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Alle auswählen";
                copy = "Kopieren";
                copyLink = "Link-Adresse kopieren";
                openLink = "Link öffnen";
                copyImageLink = "Bild-URL kopieren";
                copyImage = "Bild kopieren";
                saveImage = "Bild speichern unter...";
                openVideo = "Video öffnen";
                copyVideoUrl = "Video-URL kopieren";
            }
            else if (CultureInfo.CurrentUICulture.Name.StartsWith("it", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Seleziona tutto";
                copy = "Copia";
                copyLink = "Copia indirizzo del link";
                openLink = "Apri link";
                copyImageLink = "Copia URL immagine";
                copyImage = "Copia immagine";
                saveImage = "Salva immagine con nome...";
                openVideo = "Apri il video";
                copyVideoUrl = "Copia URL video";
            }
            else if (CultureInfo.CurrentUICulture.Name.StartsWith("es", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Seleccionar todo";
                copy = "Copiar";
                copyLink = "Copiar dirección de enlace";
                openLink = "Abrir enlace";
                copyImageLink = "Copiar URL de la imagen";
                copyImage = "Copiar imagen";
                saveImage = "Guardar imagen como...";
                openVideo = "Abrir video";
                copyVideoUrl = "Copiar URL de la video";
            }
            else if (CultureInfo.CurrentUICulture.Name.StartsWith("ru", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Выбрать все";
                copy = "Копировать";
                copyLink = "Копировать адрес ссылки";
                openLink = "Перейти по ссылке";
                copyImageLink = "Копировать адрес изображения";
                copyImage = "Копировать изображение";
                saveImage = "Сохранить изображение как...";
                openVideo = "Открыть видео";
                copyVideoUrl = "Копировать адрес видео";
            }
            else if (CultureInfo.CurrentUICulture.Name.StartsWith("sv", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Välj allt";
                copy = "Kopiera";
                copyLink = "Kopiera länkadress";
                openLink = "Öppna länk";
                copyImageLink = "Kopiera bildens URL";
                copyImage = "Kopiera bild";
                saveImage = "Spara bild som...";
                openVideo = "Öppna video";
                copyVideoUrl = "Kopiera video URL";
            }
            else if (CultureInfo.CurrentUICulture.Name.StartsWith("hu", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Összes kiválasztása";
                copy = "Másolás";
                copyLink = "Hivatkozás címének másolása";
                openLink = "Hivatkozás megnyitása";
                copyImageLink = "Kép URL másolása";
                copyImage = "Kép másolása";
                saveImage = "Kép mentése másként...";
                openVideo = "Videó megnyitása";
                copyVideoUrl = "Videó URL másolása";
            }
            else if (CultureInfo.CurrentUICulture.Name.StartsWith("cs", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Vybrat vše";
                copy = "Kopírovat";
                copyLink = "Kopírovat adresu odkazu";
                openLink = "Otevřít odkaz";
                copyImageLink = "Kopírovat URL snímku";
                copyImage = "Kopírovat snímek";
                saveImage = "Uložit snímek jako...";
                openVideo = "Otevřít video";
                copyVideoUrl = "Kopírovat URL video";
            }
            else if (CultureInfo.CurrentUICulture.Name.StartsWith("da", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Vælg alt";
                copy = "Kopiér";
                copyLink = "Kopier link-adresse";
                openLink = "Åbn link";
                copyImageLink = "Kopier billede-URL";
                copyImage = "Kopier billede";
                saveImage = "Gem billede som...";
                openVideo = "Åbn video";
                copyVideoUrl = "Kopier video-URL";
            }
            else if (CultureInfo.CurrentUICulture.Name.StartsWith("nl", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Alles selecteren";
                copy = "Kopiëren";
                copyLink = "Link adres kopiëren";
                openLink = "Link openen";
                copyImageLink = "URL Afbeelding kopiëren";
                copyImage = "Afbeelding kopiëren";
                saveImage = "Bewaar afbeelding als...";
                openVideo = "Video openen";
                copyVideoUrl = "URL video kopiëren";
            }
            else if (CultureInfo.CurrentUICulture.Name.StartsWith("fi", StringComparison.OrdinalIgnoreCase))
            {
                selectAll = "Valitse kaikki";
                copy = "Kopioi";
                copyLink = "Kopioi linkin osoite";
                openLink = "Avaa linkki";
                copyImageLink = "Kopioi kuvan URL";
                copyImage = "Kopioi kuva";
                saveImage = "Tallena kuva nimellä...";
                openVideo = "Avaa video";
                copyVideoUrl = "Kopioi video URL";
            }
            else
            {
                selectAll = "Select all";
                copy = "Copy";
                copyLink = "Copy link address";
                openLink = "Open link";
                copyImageLink = "Copy image URL";
                copyImage = "Copy image";
                saveImage = "Save image as...";
                openVideo = "Open video";
                copyVideoUrl = "Copy video URL";
            }
        }

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="selectionHandler">the selection handler linked to the context menu handler</param>
        /// <param name="htmlContainer">the html container the handler is on</param>
        public ContextMenuHandler(SelectionHandler selectionHandler, HtmlContainerInt htmlContainer)
        {
            ArgChecker.AssertArgNotNull(selectionHandler, "selectionHandler");
            ArgChecker.AssertArgNotNull(htmlContainer, "htmlContainer");

            this.selectionHandler = selectionHandler;
            this.htmlContainer = htmlContainer;
        }

        /// <summary>
        /// Show context menu clicked on given rectangle.
        /// </summary>
        /// <param name="parent">the parent control to show the context menu on</param>
        /// <param name="rect">the rectangle that was clicked to show context menu</param>
        /// <param name="link">the link that was clicked to show context menu on</param>
        public void ShowContextMenu(RControl parent, CssRect rect, CssBox link)
        {
            try
            {
                DisposeContextMenu();

                parentControl = parent;
                currentRect = rect;
                currentLink = link;
                contextMenu = htmlContainer.Adapter.GetContextMenu();

                if (rect != null)
                {
                    bool isVideo = false;
                    if (link != null)
                    {
                        isVideo = link is CssBoxFrame && ((CssBoxFrame)link).IsVideo;
                        var linkExist = !string.IsNullOrEmpty(link.HrefLink);
                        contextMenu.AddItem(isVideo ? openVideo : openLink, linkExist, OnOpenLinkClick);
                        if (htmlContainer.IsSelectionEnabled)
                        {
                            contextMenu.AddItem(isVideo ? copyVideoUrl : copyLink, linkExist, OnCopyLinkClick);
                        }
                        contextMenu.AddDivider();
                    }

                    if (rect.IsImage && !isVideo)
                    {
                        contextMenu.AddItem(saveImage, rect.Image != null, OnSaveImageClick);
                        if (htmlContainer.IsSelectionEnabled)
                        {
                            contextMenu.AddItem(copyImageLink, !string.IsNullOrEmpty(currentRect.OwnerBox.GetAttribute("src")), OnCopyImageLinkClick);
                            contextMenu.AddItem(copyImage, rect.Image != null, OnCopyImageClick);
                        }
                        contextMenu.AddDivider();
                    }

                    if (htmlContainer.IsSelectionEnabled)
                    {
                        contextMenu.AddItem(copy, rect.Selected, OnCopyClick);
                    }
                }

                if (htmlContainer.IsSelectionEnabled)
                {
                    contextMenu.AddItem(selectAll, true, OnSelectAllClick);
                }

                if (contextMenu.ItemsCount > 0)
                {
                    contextMenu.RemoveLastDivider();
                    contextMenu.Show(parent, parent.MouseLocation);
                }
            }
            catch (Exception ex)
            {
                htmlContainer.ReportError(HtmlRenderErrorType.ContextMenu, "Failed to show context menu", ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            DisposeContextMenu();
        }


        #region Private methods

        /// <summary>
        /// Dispose of the last used context menu.
        /// </summary>
        private void DisposeContextMenu()
        {
            try
            {
                if (contextMenu != null)
                    contextMenu.Dispose();
                contextMenu = null;
                parentControl = null;
                currentRect = null;
                currentLink = null;
            }
            catch
            { }
        }

        /// <summary>
        /// Handle link click.
        /// </summary>
        private void OnOpenLinkClick(object sender, EventArgs eventArgs)
        {
            try
            {
                currentLink.HtmlContainer.HandleLinkClicked(parentControl, parentControl.MouseLocation, currentLink);
            }
            catch (HtmlLinkClickedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                htmlContainer.ReportError(HtmlRenderErrorType.ContextMenu, "Failed to open link", ex);
            }
            finally
            {
                DisposeContextMenu();
            }
        }

        /// <summary>
        /// Copy the href of a link to clipboard.
        /// </summary>
        private void OnCopyLinkClick(object sender, EventArgs eventArgs)
        {
            try
            {
                htmlContainer.Adapter.SetToClipboard(currentLink.HrefLink);
            }
            catch (Exception ex)
            {
                htmlContainer.ReportError(HtmlRenderErrorType.ContextMenu, "Failed to copy link url to clipboard", ex);
            }
            finally
            {
                DisposeContextMenu();
            }
        }

        /// <summary>
        /// Open save as dialog to save the image
        /// </summary>
        private void OnSaveImageClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var imageSrc = currentRect.OwnerBox.GetAttribute("src");
                htmlContainer.Adapter.SaveToFile(currentRect.Image, Path.GetFileName(imageSrc) ?? "image", Path.GetExtension(imageSrc) ?? "png");
            }
            catch (Exception ex)
            {
                htmlContainer.ReportError(HtmlRenderErrorType.ContextMenu, "Failed to save image", ex);
            }
            finally
            {
                DisposeContextMenu();
            }
        }

        /// <summary>
        /// Copy the image source to clipboard.
        /// </summary>
        private void OnCopyImageLinkClick(object sender, EventArgs eventArgs)
        {
            try
            {
                htmlContainer.Adapter.SetToClipboard(currentRect.OwnerBox.GetAttribute("src"));
            }
            catch (Exception ex)
            {
                htmlContainer.ReportError(HtmlRenderErrorType.ContextMenu, "Failed to copy image url to clipboard", ex);
            }
            finally
            {
                DisposeContextMenu();
            }
        }

        /// <summary>
        /// Copy image object to clipboard.
        /// </summary>
        private void OnCopyImageClick(object sender, EventArgs eventArgs)
        {
            try
            {
                htmlContainer.Adapter.SetToClipboard(currentRect.Image);
            }
            catch (Exception ex)
            {
                htmlContainer.ReportError(HtmlRenderErrorType.ContextMenu, "Failed to copy image to clipboard", ex);
            }
            finally
            {
                DisposeContextMenu();
            }
        }

        /// <summary>
        /// Copy selected text.
        /// </summary>
        private void OnCopyClick(object sender, EventArgs eventArgs)
        {
            try
            {
                selectionHandler.CopySelectedHtml();
            }
            catch (Exception ex)
            {
                htmlContainer.ReportError(HtmlRenderErrorType.ContextMenu, "Failed to copy text to clipboard", ex);
            }
            finally
            {
                DisposeContextMenu();
            }
        }

        /// <summary>
        /// Select all text.
        /// </summary>
        private void OnSelectAllClick(object sender, EventArgs eventArgs)
        {
            try
            {
                selectionHandler.SelectAll(parentControl);
            }
            catch (Exception ex)
            {
                htmlContainer.ReportError(HtmlRenderErrorType.ContextMenu, "Failed to select all text", ex);
            }
            finally
            {
                DisposeContextMenu();
            }
        }

        #endregion
    }
}
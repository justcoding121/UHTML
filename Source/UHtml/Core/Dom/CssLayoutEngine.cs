using System;
using System.Collections.Generic;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Parse;
using UHtml.Core.Utils;
using System.Linq;

namespace UHtml.Core.Dom
{
    /// <summary>
    /// Helps on CSS Layout.
    /// </summary>
    internal static class CssLayoutEngine
    {
        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.<br/>
        /// </summary>
        /// <param name="g">Device context to use</param>
        internal static void LayoutBoxes(RGraphics g, CssBox box)
        {

            if (box.Display != CssConstants.None)
            {
                box.RectanglesReset();
                box.MeasureWordsSize(g);
            }


            if (box.Display == CssConstants.Block
                || box.Display == CssConstants.ListItem
                || box.Display == CssConstants.Table
                || box.Display == CssConstants.InlineTable
                || box.Display == CssConstants.InlineBlock
                || box.Display == CssConstants.TableCell)
            {
                if (box.Display != CssConstants.TableCell)
                {
                    var prevSibling = DomUtils.GetPreviousSibling(box);

                    SetBoxSize(box);


                    double left;
                    double top;


                    left = box.ContainingBlock.Location.X + box.ContainingBlock.ActualBorderLeftWidth
                         + box.ContainingBlock.ActualPaddingLeft + box.ActualMarginLeft;

                    if (prevSibling == null && box.ParentBox != null)
                    {
                        top = box.ParentBox.ClientTop + box.MarginTopCollapse(prevSibling);
                    }
                    else
                    {
                        if (prevSibling != null)
                        {
                            top = prevSibling.ActualBottom + box.MarginTopCollapse(prevSibling);
                        }
                        else
                        {
                            top = box.MarginTopCollapse(prevSibling);
                        }

                    }
                    // top = (prevSibling == null && ParentBox != null ? ParentBox.ClientTop : ParentBox == null ? Location.Y : 0) + MarginTopCollapse(prevSibling) + (prevSibling != null ? prevSibling.ActualBottom + prevSibling.ActualBorderBottomWidth : 0);
                    box.Location = new RPoint(left, top);
                }

                //If we're talking about a table here..
                if (box.Display == CssConstants.Table || box.Display == CssConstants.InlineTable)
                {
                    CssTableLayoutEngine.PerformLayout(g, box);
                }
                else
                {
                    //If there's just inline boxes, create LineBoxes
                    if (DomUtils.ContainsInlinesOnly(box))
                    {
                        double curX = box.Location.X - box.ActualMarginLeft;
                        double curY = box.Location.Y - box.ActualMarginTop;

                        double startX = curX
                            + box.ActualMarginLeft
                            + box.ActualPaddingLeft
                            + box.ActualBorderLeftWidth;

                        double startY = curY
                            + box.ActualMarginTop
                            + box.ActualPaddingTop
                            + box.ActualBorderTopWidth;

                        curX = startX;
                        curY = startY;

                        double currentMaxBottom = curY;

                        box.LineBoxes.Clear();
                        var currentLineBox = new CssLineBox(box);

                        //This will automatically set the bottom of this block
                        LayoutInlineBoxes(g, box, ref currentLineBox,
                            box,
                            startX,
                            startY,
                            ref curX,
                            ref curY,
                            box.ActualRight
                            - box.ActualBorderRightWidth
                            - box.ActualPaddingRight,
                            ref currentMaxBottom
                            );

                        //Gets the rectangles for each line-box
                        foreach (var linebox in box.LineBoxes)
                        {
                            ApplyHorizontalAlignment(g, linebox);
                            ApplyRightToLeft(box, linebox);
                            BubbleRectangles(box, linebox);
                            ApplyVerticalAlignment(g, linebox);
                            linebox.AssignRectanglesToBoxes();
                        }

                        if (box.Height == CssConstants.Auto)
                        {
                            box.ActualBottom = currentMaxBottom
                                + box.ActualBorderBottomWidth
                                + box.ActualPaddingBottom;

                        }

                    }
                    else if (box.Boxes.Count > 0)
                    {
                        foreach (var childBox in box.Boxes)
                        {
                            childBox.PerformLayoutImp(g);
                        }

                        box.ActualRight = box.CalculateActualRight();
                        box.ActualBottom = box.MarginBottomCollapse();
                    }
                    else
                    {
                        box.ActualBottom = Math.Max(box.ActualBottom, box.Location.Y + box.ActualHeight);
                    }
                }
            }
            else
            {
                if (box.Display == CssConstants.Inline)
                {
                    var prevSibling = DomUtils.GetPreviousSibling(box);
                    if (prevSibling != null)
                    {
                        if (box.Location == RPoint.Empty)
                            box.Location = prevSibling.Location;
                        box.ActualBottom = prevSibling.ActualBottom;
                    }
                }


            }

            box.CreateListItemBox(g);

            if (!box.IsFixed)
            {
                var actualWidth = Math.Max(box.GetMinimumWidth() + box.GetWidthMarginDeep(box), box.Size.Width < 90999 ? box.ActualRight - box.HtmlContainer.Root.Location.X : 0);
                box.HtmlContainer.ActualSize = CommonUtils.Max(box.HtmlContainer.ActualSize, new RSize(actualWidth, box.ActualBottom - box.HtmlContainer.Root.Location.Y));
            }
        }

        /// <summary>
        /// Creates line boxes for the specified blockbox
        /// Assuming all immediate children of this box are inline boxes
        /// Inline boxes don't create new lines unless explicitly specified or limited by windows/parent width
        /// </summary>
        /// <param name="g"></param>
        /// <param name="blockBox"></param>
        public static void LayoutInlineBoxes(RGraphics g, CssBox closestBlockAncestor,
            ref CssLineBox currentLineBox,
            CssBox currentBox,
            double startX, double startY,
            ref double curX, ref double curY, double maxRight, ref double currentMaxBottom)
        {
            ArgChecker.AssertArgNotNull(g, "g");
            ArgChecker.AssertArgNotNull(currentBox, "blockBox");

            if (currentBox.Display != CssConstants.None)
            {
                currentBox.RectanglesReset();
                currentBox.MeasureWordsSize(g);
            }


            //loop through each inline box
            foreach (CssBox box in currentBox.Boxes)
            {

                if (box.Display == CssConstants.None)
                    continue;

                box.Location = new RPoint(curX + box.ActualMarginLeft,
                                        curY + box.ActualMarginTop);

                curX = curX + box.ActualMarginLeft
                    + box.ActualPaddingLeft
                    + box.ActualBorderLeftWidth;

                curY = curY + box.ActualMarginTop
                    + box.ActualPaddingTop
                    + box.ActualBorderTopWidth;


                box.RectanglesReset();
                box.MeasureWordsSize(g);


                SetInlineBoxSize(box);

                var localMaxRight = maxRight;

                //set x,y location
                if (box.Width != CssConstants.Auto)
                {
                    //break to new line if exceeds maximum width
                    if (curX + box.ActualWidth > maxRight)
                    {
                        box.Location = new RPoint(startX + box.ActualMarginLeft,
                                  currentMaxBottom + box.ActualMarginTop);

                        curX = startX + box.ActualMarginLeft
                            + box.ActualPaddingLeft
                            + box.ActualBorderLeftWidth;

                        curY = currentMaxBottom + box.ActualMarginTop
                            + box.ActualPaddingTop
                            + box.ActualBorderTopWidth;


                    }

                    //if this box have a size, then limit right to its size
                    localMaxRight = box.ActualRight
                                - box.ActualPaddingRight
                                - box.ActualBorderRightWidth;
                }

                //position words within local max right
                //box bottom should be updated by this method
                //as text wrap to new lines increase bottom
                LayoutWords(g, closestBlockAncestor, ref currentLineBox, box, startX, startY,
                     ref curX, ref curY, localMaxRight, ref currentMaxBottom);

                if (box.Boxes.Count > 0)
                {
                    if (DomUtils.ContainsInlinesOnly(box))
                    {
                        curX = curX
                          + box.ActualMarginLeft
                          + box.ActualPaddingLeft
                          + box.ActualBorderLeftWidth;

                        curY = curY
                            + box.ActualMarginTop
                            + box.ActualPaddingTop
                            + box.ActualBorderTopWidth;
            

                        //since parent is an inline box all child inlines will use
                        //the closest box ancestor as startX, startY
                        LayoutInlineBoxes(g,
                            closestBlockAncestor,
                            ref currentLineBox,
                            box, startX, startY,
                            ref curX, ref curY, localMaxRight, ref currentMaxBottom);
                    }
                    else
                    {
                        foreach (var childBox in box.Boxes)
                        {
                            LayoutBoxes(g, childBox);
                            currentMaxBottom = Math.Max(currentMaxBottom, childBox.ActualBottom);
                        }

                    }
                }

            }

            //layout any words in this box
            LayoutWords(g, closestBlockAncestor,  ref currentLineBox, currentBox,
                startX, startY,
                ref curX, ref curY, maxRight, ref currentMaxBottom);

           

        }

        /// <summary>
        /// Should update the actual bottom of the box is height is auto
        /// </summary>
        /// <param name="g"></param>
        /// <param name="box"></param>
        /// <param name="curX"></param>
        /// <param name="curY"></param>
        /// <param name="rightLimit"></param>
        private static void LayoutWords(RGraphics g,
            CssBox closestBlockAncestor, ref CssLineBox currentLineBox, CssBox box,
            double startX, double startY,
            ref double curX, ref double curY,
            double rightLimit, ref double currentMaxBottom)
        {

            double localMaxLineBottom = currentMaxBottom;


            if (box.Words.Count > 0)
            {
                box.FirstHostingLineBox = currentLineBox;

                if (DomUtils.DoesBoxHasWhitespace(box))
                    curX += box.ActualWordSpacing;

                foreach (var word in box.Words)
                {
                    if ((box.WhiteSpace != CssConstants.NoWrap
                        && box.WhiteSpace != CssConstants.Pre
                        && curX + word.Width > rightLimit
                         && (box.WhiteSpace != CssConstants.PreWrap || !word.IsSpaces))
                        || word.IsLineBreak)
                    {

                        curX = startX;
                        curY = localMaxLineBottom;

                        currentLineBox = new CssLineBox(closestBlockAncestor);

                    }

                    currentLineBox.ReportExistanceOf(word);

                    word.Left = curX;
                    word.Top = curY;

                    curX = word.Left + word.FullWidth;
                    localMaxLineBottom = Math.Max(localMaxLineBottom, word.Bottom);

                }

                //set x,y location
                if (box.Height == CssConstants.Auto)
                {

                    box.ActualBottom = localMaxLineBottom
                        + box.ActualBorderBottomWidth
                        + box.ActualPaddingBottom;


                }

                currentMaxBottom = box.ActualBottom;

                if (box.Width == CssConstants.Auto)
                {
                    box.ActualRight = curX
                        + box.ActualBorderRightWidth
                        + box.ActualPaddingRight;
                }

                box.LastHostingLineBox = currentLineBox;


            }

        }

        /// <summary>
        /// Set Width & Height for Box
        /// </summary>
        /// <param name="box"></param>
        private static void SetInlineBoxSize(CssBox box)
        {
            if (box.Height != CssConstants.Auto && !string.IsNullOrEmpty(box.Height))
            {
                double height = CssValueParser.ParseLength(box.Height, box.ContainingBlock.Size.Height, box);
                box.Size = new RSize(box.Size.Width
                        , height
                        + box.ActualBorderTopWidth
                        + box.ActualPaddingTop
                        + box.ActualBorderBottomWidth
                        + box.ActualPaddingBottom);
            }

            //overrride with custom width
            if (box.Width != CssConstants.Auto && !string.IsNullOrEmpty(box.Width))
            {
                double width = CssValueParser.ParseLength(box.Width, box.ContainingBlock.Size.Width, box);
                box.Size = new RSize(width
                        + box.ActualBorderLeftWidth
                        + box.ActualPaddingLeft
                        + box.ActualBorderRightWidth
                        + box.ActualPaddingRight
                        , box.Size.Height);

            }

        }


        /// <summary>
        /// Set Width & Height for Box
        /// </summary>
        /// <param name="box"></param>
        private static void SetBoxSize(CssBox box)
        {
            if (box.Height != CssConstants.Auto && !string.IsNullOrEmpty(box.Height))
            {
                double height = CssValueParser.ParseLength(box.Height, box.ContainingBlock.Size.Height, box);
                box.Size = new RSize(box.Size.Width
                        , height
                        + box.ActualBorderTopWidth
                        + box.ActualPaddingTop
                        + box.ActualBorderBottomWidth
                        + box.ActualPaddingBottom);
            }
            else
            {
                // must be separate
                //because the margin can be calculated by percentage of the width
                box.Size = new RSize(box.Size.Width
                , box.ContainingBlock.Size.Height > 0 ?
                  box.ContainingBlock.Size.Height
                - box.ContainingBlock.ActualBorderTopWidth
                - box.ContainingBlock.ActualPaddingTop
                - box.ContainingBlock.ActualBorderBottomWidth
                - box.ContainingBlock.ActualPaddingBottom
                - box.ActualMarginTop
                - box.ActualMarginBottom : 0);

            }
            //overrride with custom width
            if (box.Width != CssConstants.Auto && !string.IsNullOrEmpty(box.Width))
            {
                double width = CssValueParser.ParseLength(box.Width, box.ContainingBlock.Size.Width, box);
                box.Size = new RSize(width
                        + box.ActualBorderLeftWidth
                        + box.ActualPaddingLeft
                        + box.ActualBorderRightWidth
                        + box.ActualPaddingRight
                        , box.Size.Height);

            }
            else
            {

                // must be separate
                //because the margin can be calculated by percentage of the width
                box.Size = new RSize(box.ContainingBlock.Size.Width > 0 ?
                    box.ContainingBlock.Size.Width
                - box.ContainingBlock.ActualBorderLeftWidth
                - box.ContainingBlock.ActualPaddingLeft
                - box.ContainingBlock.ActualBorderRightWidth
                - box.ContainingBlock.ActualPaddingRight
                - box.ActualMarginLeft
                - box.ActualMarginRight : 0
                , box.Size.Height);
            }
        }


        /// <summary>
        /// Applies special vertical alignment for table-cells
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        public static void ApplyCellVerticalAlignment(RGraphics g, CssBox cell)
        {
            ArgChecker.AssertArgNotNull(g, "g");
            ArgChecker.AssertArgNotNull(cell, "cell");

            if (cell.VerticalAlign == CssConstants.Top || cell.VerticalAlign == CssConstants.Baseline)
                return;

            double cellbot = cell.ClientBottom;
            double bottom = cell.GetMaximumBottom(cell, 0f);
            double dist = 0f;

            if (cell.VerticalAlign == CssConstants.Bottom)
            {
                dist = cellbot - bottom;
            }
            else if (cell.VerticalAlign == CssConstants.Middle)
            {
                dist = (cellbot - bottom) / 2;
            }

            foreach (CssBox b in cell.Boxes)
            {
                b.OffsetTop(dist);
            }

        }

        /// <summary>
        /// Measure image box size by the width\height set on the box and the actual rendered image size.<br/>
        /// If no image exists for the box error icon will be set.
        /// </summary>
        /// <param name="imageWord">the image word to measure</param>
        public static void MeasureImageSize(CssRectImage imageWord)
        {
            ArgChecker.AssertArgNotNull(imageWord, "imageWord");
            ArgChecker.AssertArgNotNull(imageWord.OwnerBox, "imageWord.OwnerBox");

            var width = new CssLength(imageWord.OwnerBox.Width);
            var height = new CssLength(imageWord.OwnerBox.Height);

            bool hasImageTagWidth = width.Number > 0 && width.Unit == CssUnit.Pixels;
            bool hasImageTagHeight = height.Number > 0 && height.Unit == CssUnit.Pixels;
            bool scaleImageHeight = false;

            if (hasImageTagWidth)
            {
                imageWord.Width = width.Number;
            }
            else if (width.Number > 0 && width.IsPercentage)
            {
                imageWord.Width = width.Number * imageWord.OwnerBox.ContainingBlock.Size.Width;
                scaleImageHeight = true;
            }
            else if (imageWord.Image != null)
            {
                imageWord.Width = imageWord.ImageRectangle == RRect.Empty ? imageWord.Image.Width : imageWord.ImageRectangle.Width;
            }
            else
            {
                imageWord.Width = hasImageTagHeight ? height.Number / 1.14f : 20;
            }

            var maxWidth = new CssLength(imageWord.OwnerBox.MaxWidth);
            if (maxWidth.Number > 0)
            {
                double maxWidthVal = -1;
                if (maxWidth.Unit == CssUnit.Pixels)
                {
                    maxWidthVal = maxWidth.Number;
                }
                else if (maxWidth.IsPercentage)
                {
                    maxWidthVal = maxWidth.Number * imageWord.OwnerBox.ContainingBlock.Size.Width;
                }

                if (maxWidthVal > -1 && imageWord.Width > maxWidthVal)
                {
                    imageWord.Width = maxWidthVal;
                    scaleImageHeight = !hasImageTagHeight;
                }
            }

            if (hasImageTagHeight)
            {
                imageWord.Height = height.Number;
            }
            else if (imageWord.Image != null)
            {
                imageWord.Height = imageWord.ImageRectangle == RRect.Empty ? imageWord.Image.Height : imageWord.ImageRectangle.Height;
            }
            else
            {
                imageWord.Height = imageWord.Width > 0 ? imageWord.Width * 1.14f : 22.8f;
            }

            if (imageWord.Image != null)
            {
                // If only the width was set in the html tag, ratio the height.
                if ((hasImageTagWidth && !hasImageTagHeight) || scaleImageHeight)
                {
                    // Divide the given tag width with the actual image width, to get the ratio.
                    double ratio = imageWord.Width / imageWord.Image.Width;
                    imageWord.Height = imageWord.Image.Height * ratio;
                }
                // If only the height was set in the html tag, ratio the width.
                else if (hasImageTagHeight && !hasImageTagWidth)
                {
                    // Divide the given tag height with the actual image height, to get the ratio.
                    double ratio = imageWord.Height / imageWord.Image.Height;
                    imageWord.Width = imageWord.Image.Width * ratio;
                }
            }

            imageWord.Height += imageWord.OwnerBox.ActualBorderBottomWidth + imageWord.OwnerBox.ActualBorderTopWidth + imageWord.OwnerBox.ActualPaddingTop + imageWord.OwnerBox.ActualPaddingBottom;
        }
        /// <summary>
        /// Adjust the position of absolute elements by letf and top margins.
        /// </summary>
        private static void AdjustAbsolutePosition(CssBox box, double left, double top)
        {
            left += box.ActualMarginLeft;
            top += box.ActualMarginTop;
            if (box.Words.Count > 0)
            {
                foreach (var word in box.Words)
                {
                    word.Left += left;
                    word.Top += top;
                }
            }
            else
            {
                foreach (var b in box.Boxes)
                    AdjustAbsolutePosition(b, left, top);
            }
        }

        /// <summary>
        /// Recursively creates the rectangles of the blockBox, by bubbling from deep to outside of the boxes 
        /// in the rectangle structure
        /// </summary>
        private static void BubbleRectangles(CssBox box, CssLineBox line)
        {
            if (box.Words.Count > 0)
            {
                double x = Single.MaxValue, y = Single.MaxValue, r = Single.MinValue, b = Single.MinValue;
                List<CssRect> words = line.WordsOf(box);

                if (words.Count > 0)
                {
                    foreach (CssRect word in words)
                    {
                        // handle if line is wrapped for the first text element where parent has left margin\padding
                        var left = word.Left;

                        if (box == box.ParentBox.Boxes[0] && word == box.Words[0] && word == line.Words[0] && line != line.OwnerBox.LineBoxes[0] && !word.IsLineBreak)
                            left -= box.ParentBox.ActualMarginLeft + box.ParentBox.ActualBorderLeftWidth + box.ParentBox.ActualPaddingLeft;


                        x = Math.Min(x, left);
                        r = Math.Max(r, word.Right);
                        y = Math.Min(y, word.Top);
                        b = Math.Max(b, word.Bottom);
                    }
                    line.UpdateRectangle(box, x, y, r, b);
                }
            }
            else
            {
                foreach (CssBox b in box.Boxes)
                {
                    BubbleRectangles(b, line);
                }
            }
        }

        /// <summary>
        /// Applies vertical and horizontal alignment to words in lineboxes
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineBox"></param>
        private static void ApplyHorizontalAlignment(RGraphics g, CssLineBox lineBox)
        {
            switch (lineBox.OwnerBox.TextAlign)
            {
                case CssConstants.Right:
                    ApplyRightAlignment(g, lineBox);
                    break;
                case CssConstants.Center:
                    ApplyCenterAlignment(g, lineBox);
                    break;
                case CssConstants.Justify:
                    ApplyJustifyAlignment(g, lineBox);
                    break;
                default:
                    ApplyLeftAlignment(g, lineBox);
                    break;
            }
        }

        /// <summary>
        /// Applies right to left direction to words
        /// </summary>
        /// <param name="blockBox"></param>
        /// <param name="lineBox"></param>
        private static void ApplyRightToLeft(CssBox blockBox, CssLineBox lineBox)
        {
            if (blockBox.Direction == CssConstants.Rtl)
            {
                ApplyRightToLeftOnLine(lineBox);
            }
            else
            {
                foreach (var box in lineBox.RelatedBoxes)
                {
                    if (box.Direction == CssConstants.Rtl)
                    {
                        ApplyRightToLeftOnSingleBox(lineBox, box);
                    }
                }
            }
        }

        /// <summary>
        /// Applies RTL direction to all the words on the line.
        /// </summary>
        /// <param name="line">the line to apply RTL to</param>
        private static void ApplyRightToLeftOnLine(CssLineBox line)
        {
            if (line.Words.Count > 0)
            {
                double left = line.Words[0].Left;
                double right = line.Words[line.Words.Count - 1].Right;

                foreach (CssRect word in line.Words)
                {
                    double diff = word.Left - left;
                    double wright = right - diff;
                    word.Left = wright - word.Width;
                }
            }
        }

        /// <summary>
        /// Applies RTL direction to specific box words on the line.
        /// </summary>
        /// <param name="lineBox"></param>
        /// <param name="box"></param>
        private static void ApplyRightToLeftOnSingleBox(CssLineBox lineBox, CssBox box)
        {
            int leftWordIdx = -1;
            int rightWordIdx = -1;
            for (int i = 0; i < lineBox.Words.Count; i++)
            {
                if (lineBox.Words[i].OwnerBox == box)
                {
                    if (leftWordIdx < 0)
                        leftWordIdx = i;
                    rightWordIdx = i;
                }
            }

            if (leftWordIdx > -1 && rightWordIdx > leftWordIdx)
            {
                double left = lineBox.Words[leftWordIdx].Left;
                double right = lineBox.Words[rightWordIdx].Right;

                for (int i = leftWordIdx; i <= rightWordIdx; i++)
                {
                    double diff = lineBox.Words[i].Left - left;
                    double wright = right - diff;
                    lineBox.Words[i].Left = wright - lineBox.Words[i].Width;
                }
            }
        }

        /// <summary>
        /// Applies vertical alignment to the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineBox"></param>
        private static void ApplyVerticalAlignment(RGraphics g, CssLineBox lineBox)
        {
            double baseline = Single.MinValue;
            foreach (var box in lineBox.Rectangles.Keys)
            {
                baseline = Math.Max(baseline, lineBox.Rectangles[box].Y1);
            }

            var boxes = new List<CssBox>(lineBox.Rectangles.Keys);
            foreach (CssBox box in boxes)
            {
                //Important notes on http://www.w3.org/TR/CSS21/tables.html#height-layout
                switch (box.VerticalAlign)
                {
                    case CssConstants.Sub:
                        lineBox.SetBaseLine(g, box, baseline + lineBox.Rectangles[box].Height * .5f);
                        break;
                    case CssConstants.Super:
                        lineBox.SetBaseLine(g, box, baseline - lineBox.Rectangles[box].Height * .2f);
                        break;
                    case CssConstants.TextTop:

                        break;
                    case CssConstants.TextBottom:

                        break;
                    case CssConstants.Top:

                        break;
                    case CssConstants.Bottom:

                        break;
                    case CssConstants.Middle:

                        break;
                    default:
                        //case: baseline
                        lineBox.SetBaseLine(g, box, baseline);
                        break;
                }
            }
        }

        /// <summary>
        /// Applies centered alignment to the text on the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineBox"></param>
        private static void ApplyJustifyAlignment(RGraphics g, CssLineBox lineBox)
        {
            if (lineBox.Equals(lineBox.OwnerBox.LineBoxes[lineBox.OwnerBox.LineBoxes.Count - 1]))
                return;

            double indent = lineBox.Equals(lineBox.OwnerBox.LineBoxes[0]) ? lineBox.OwnerBox.ActualTextIndent : 0f;
            double textSum = 0f;
            double words = 0f;
            double availWidth = lineBox.OwnerBox.ClientRectangle.Width - indent;

            // Gather text sum
            foreach (CssRect w in lineBox.Words)
            {
                textSum += w.Width;
                words += 1f;
            }

            if (words <= 0f)
                return; //Avoid Zero division
            double spacing = (availWidth - textSum) / words; //Spacing that will be used
            double curx = lineBox.OwnerBox.ClientLeft + indent;

            foreach (CssRect word in lineBox.Words)
            {
                word.Left = curx;
                curx = word.Right + spacing;

                if (word == lineBox.Words[lineBox.Words.Count - 1])
                {
                    word.Left = lineBox.OwnerBox.ClientRight - word.Width;
                }
            }
        }

        /// <summary>
        /// Applies centered alignment to the text on the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line"></param>
        private static void ApplyCenterAlignment(RGraphics g, CssLineBox line)
        {
            if (line.Words.Count == 0)
                return;

            CssRect lastWord = line.Words[line.Words.Count - 1];
            double right = line.OwnerBox.ActualRight - line.OwnerBox.ActualPaddingRight - line.OwnerBox.ActualBorderRightWidth;
            double diff = right - lastWord.Right - lastWord.OwnerBox.ActualBorderRightWidth - lastWord.OwnerBox.ActualPaddingRight;
            diff /= 2;

            if (diff > 0)
            {
                foreach (CssRect word in line.Words)
                {
                    word.Left += diff;
                }

                if (line.Rectangles.Count > 0)
                {
                    foreach (CssBox b in ToList(line.Rectangles.Keys))
                    {
                        RRect r = line.Rectangles[b];
                        line.Rectangles[b] = new RRect(r.X + diff, r.Y, r.Width, r.Height);
                    }
                }
            }
        }

        /// <summary>
        /// Applies right alignment to the text on the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line"></param>
        private static void ApplyRightAlignment(RGraphics g, CssLineBox line)
        {
            if (line.Words.Count == 0)
                return;


            CssRect lastWord = line.Words[line.Words.Count - 1];
            double right = line.OwnerBox.ActualRight - line.OwnerBox.ActualPaddingRight - line.OwnerBox.ActualBorderRightWidth;
            double diff = right - lastWord.Right - lastWord.OwnerBox.ActualBorderRightWidth - lastWord.OwnerBox.ActualPaddingRight;

            if (diff > 0)
            {
                foreach (CssRect word in line.Words)
                {
                    word.Left += diff;
                }

                if (line.Rectangles.Count > 0)
                {
                    foreach (CssBox b in ToList(line.Rectangles.Keys))
                    {
                        RRect r = line.Rectangles[b];
                        line.Rectangles[b] = new RRect(r.X + diff, r.Y, r.Width, r.Height);
                    }
                }
            }
        }

        /// <summary>
        /// Simplest alignment, just arrange words.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line"></param>
        private static void ApplyLeftAlignment(RGraphics g, CssLineBox line)
        {
            //No alignment needed.

            //foreach (LineBoxRectangle r in line.Rectangles)
            //{
            //    double curx = r.Left + (r.Index == 0 ? r.OwnerBox.ActualPaddingLeft + r.OwnerBox.ActualBorderLeftWidth / 2 : 0);

            //    if (r.SpaceBefore) curx += r.OwnerBox.ActualWordSpacing;

            //    foreach (BoxWord word in r.Words)
            //    {
            //        word.Left = curx;
            //        word.Top = r.Top;// +r.OwnerBox.ActualPaddingTop + r.OwnerBox.ActualBorderTopWidth / 2;

            //        curx = word.Right + r.OwnerBox.ActualWordSpacing;
            //    }
            //}
        }

        /// <summary>
        /// todo: optimizate, not creating a list each time
        /// </summary>
        private static List<T> ToList<T>(IEnumerable<T> collection)
        {
            List<T> result = new List<T>();
            foreach (T item in collection)
            {
                result.Add(item);
            }
            return result;
        }


    }
}
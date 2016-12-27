using System;
using System.Collections.Generic;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    /// <summary>
    /// Helps on CSS Layout.
    /// </summary>
    internal static class CssLayoutEngine
    {
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
        /// Creates line boxes for the specified blockbox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="blockBox"></param>
        public static void CreateLineBoxes(RGraphics g, CssBox blockBox)
        {
            ArgChecker.AssertArgNotNull(g, "g");
            ArgChecker.AssertArgNotNull(blockBox, "blockBox");

            blockBox.LineBoxes.Clear();

            // handle limiting block height when overflow is hidden
            if (blockBox.Height != null && blockBox.Height != CssConstants.Auto
                && blockBox.Overflow == CssConstants.Hidden
                && blockBox.ActualBottom - blockBox.Location.Y > blockBox.ActualHeight)
            {
                blockBox.ActualBottom = blockBox.Location.Y + blockBox.ActualHeight;
            }

            //Get the start x and y of the blockBox
            double startX = blockBox.Location.X
                + blockBox.ActualPaddingLeft
                + blockBox.ActualBorderLeftWidth
                + blockBox.ActualTextIndent;

            double startY = blockBox.Location.Y
                + blockBox.ActualPaddingTop
                + blockBox.ActualBorderTopWidth;


            //Flow words and boxes
            FlowInlineBoxes(g, blockBox, startX, startY);


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

            //float top = cell.ClientTop;
            //float bottom = cell.ClientBottom;
            //bool middle = cell.VerticalAlign == CssConstants.Middle;

            //foreach (LineBox line in cell.LineBoxes)
            //{
            //    for (int i = 0; i < line.RelatedBoxes.Count; i++)
            //    {

            //        double diff = bottom - line.RelatedBoxes[i].Rectangles[line].Bottom;
            //        if (middle) diff /= 2f;
            //        RectangleF r = line.RelatedBoxes[i].Rectangles[line];
            //        line.RelatedBoxes[i].Rectangles[line] = new RectangleF(r.X, r.Y + diff, r.Width, r.Height);

            //    }

            //    foreach (BoxWord word in line.Words)
            //    {
            //        double gap = word.Top - top;
            //        word.Top = bottom - gap - word.Height;
            //    }
            //}
        }


        #region Private methods

        /// <summary>
        /// Recursively flows the content of the box using the inline model
        /// </summary>
        /// <param name="g">Device Info</param>
        /// <param name="parentBox">Blockbox that contains the text flow</param>
        /// <param name="limitRight">Maximum reached right</param>
        /// <param name="linespacing">Space to use between rows of text</param>
        /// <param name="startx">x starting coordinate for when breaking lines of text</param>
        /// <param name="line">Current linebox being used</param>
        /// <param name="curx">Current x coordinate that will be the left of the next word</param>
        /// <param name="cury">Current y coordinate that will be the top of the next word</param>
        /// <param name="maxRight">Maximum right reached so far</param>
        /// <param name="maxbottom">Maximum bottom reached so far</param>
        private static void FlowInlineBoxes(RGraphics g, CssBox parentBox, double startX, double startY)
        {

            var curX = startX;
            var curY = startY;

            double limitRight = parentBox.ActualRight
              - parentBox.ActualPaddingRight
              - parentBox.ActualBorderRightWidth;

            //loop through each inline box
            foreach (CssBox box in parentBox.Boxes)
            {
                double leftspacing = (box.Position != CssConstants.Absolute
                    && box.Position != CssConstants.Fixed) ?
                    box.ActualMarginLeft + box.ActualBorderLeftWidth + box.ActualPaddingLeft
                    : 0;

                double rightspacing = (box.Position != CssConstants.Absolute
                    && box.Position != CssConstants.Fixed) ?
                    box.ActualMarginRight + box.ActualBorderRightWidth + box.ActualPaddingRight
                    : 0;

                box.RectanglesReset();
                box.MeasureWordsSize(g);

                //init curX after left spacing
                curX += leftspacing;


                LayoutWords(g, parentBox, startX, startY, box, ref curX, ref curY,
                    limitRight, leftspacing, rightspacing);


                foreach (var childBox in box.Boxes)
                {
                    if (!childBox.IsInline)
                    {
                        childBox.PerformLayout(g);
                    }
                    else
                    {
                        if (childBox.Boxes.Count > 0)
                        {
                            FlowInlineBoxes(g, childBox, curX, curY);
                        }
                        else
                        {
                            LayoutWords(g, box, startX, startY, childBox,
                                ref curX, ref curY, limitRight, leftspacing, rightspacing);
                        }

                    }
                }

                //Gets the rectangles for each line-box
                foreach (var linebox in parentBox.LineBoxes)
                {
                    ApplyHorizontalAlignment(g, linebox);
                    ApplyRightToLeft(parentBox, linebox);
                    BubbleRectangles(parentBox, linebox);
                    ApplyVerticalAlignment(g, linebox);
                    linebox.AssignRectanglesToBoxes();
                }

                curX += rightspacing;
            }

            if (parentBox.Height == CssConstants.Auto)
            {
                parentBox.Size = new RSize(parentBox.Size.Width, (curY - startY)
                    + parentBox.ActualPaddingBottom
                    + parentBox.ActualBorderBottomWidth);
            }


        }

        private static void LayoutWords(RGraphics g, CssBox parentBox,
            double startX, double startY,
            CssBox box, ref double curX, ref double curY, double limitRight,
            double leftSpacing, double rightSpacing)
        {
            if(box.GetAttribute("id") == "check1" || box.GetAttribute("id") == "check2")
            {
                box.Size = new RSize(150, 40);
            }

            if (box.Words.Count > 0)
            {
                var lineBox = new CssLineBox(box);

                bool wrapNoWrapBox = false;

                //no line breaks so draw straight
                if (box.WhiteSpace == CssConstants.NoWrap && curX > startX)
                {
                    var boxRight = curX;

                    foreach (var word in box.Words)
                    {
                        boxRight += word.FullWidth;
                    }

                    //set NoWrap flag to true
                    if (boxRight > limitRight)
                    {
                        wrapNoWrapBox = true;
                    }
                }

                //?
                //if last sibling was inline
                //and first word of this line has a space then add it
                if (DomUtils.DoesBoxHasWhitespace(box))
                {
                    curX += box.ActualWordSpacing;
                }

                foreach (var word in box.Words)
                {
                    //if (maxbottom - cury < parentBox.ActualLineHeight)
                    //{
                    //    maxbottom += parentBox.ActualLineHeight - (maxbottom - cury);
                    //}

                    if ((box.WhiteSpace != CssConstants.NoWrap
                        && box.WhiteSpace != CssConstants.Pre
                        && curX + word.Width + rightSpacing > limitRight
                         && (box.WhiteSpace != CssConstants.PreWrap || !word.IsSpaces))
                        || curX + box.ActualWidth > limitRight
                        || word.IsLineBreak || wrapNoWrapBox)
                    {
                        wrapNoWrapBox = false;
                        curX = startX;

                        // handle if line is wrapped for the first text element where parent has left margin\padding
                        //if (b == box.Boxes[0] && !word.IsLineBreak && (word == b.Words[0] || (box.ParentBox != null && box.ParentBox.IsBlock)))
                        //    curx += box.ActualMarginLeft + box.ActualBorderLeftWidth + box.ActualPaddingLeft;

                        curY = startY + parentBox.ActualLineHeight;

                        lineBox = new CssLineBox(box);

                        if (word.IsImage || word.Equals(box.FirstWord))
                        {
                            curX += leftSpacing;
                        }
                    }

                    lineBox.ReportExistanceOf(word);

                    word.Left = curX;
                    word.Top = curY;

                    //?
                    if (!parentBox.IsFixed && parentBox.PageBreakInside == CssConstants.Avoid)
                    {
                        word.BreakPage();
                    }

                    curX = word.Left + word.FullWidth;

                    if (box.Position == CssConstants.Absolute)
                    {
                        word.Left += parentBox.ActualMarginLeft;
                        word.Top += parentBox.ActualMarginTop;
                    }
                }


            }
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

        #endregion
    }
}
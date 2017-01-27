using System;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    internal static partial class CssLayoutEngine
    {
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
                box.FirstHostingLineBox = currentLineBox;

                if (box.Display == CssConstants.None)
                    continue;

                //inline don't respect top & bottom Margins
                box.Location = new RPoint(curX + box.ActualMarginLeft,
                                        box.IsInline ? curY : curY + box.ActualMarginTop);

                curX = curX + box.ActualMarginLeft
                    + box.ActualPaddingLeft
                    + box.ActualBorderLeftWidth;


                box.RectanglesReset();
                box.MeasureWordsSize(g);


                SetInlineBoxSize(box);

                var localMaxRight = maxRight;
                var localMinLeft = startX;
                //inlines don't respect Box Width & Height
                if (box.IsInlineBlock)
                {
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

                            currentLineBox = new CssLineBox(closestBlockAncestor);
                        }

                        //if this box have a size, then limit right to its size
                        localMaxRight = box.ActualRight
                                    - box.ActualPaddingRight
                                    - box.ActualBorderRightWidth;

                        localMinLeft = curX;
                    }
                }

                //position words within local max right
                //box bottom should be updated by this method
                //as text wrap to new lines increase bottom
                LayoutWords(g, closestBlockAncestor, ref currentLineBox, box,
                     ref curX, ref curY, localMinLeft, localMaxRight, ref currentMaxBottom);

                if (box.Boxes.Count > 0)
                {
                    if (DomUtils.ContainsInlinesOnly(box))
                    {
                        if(box.IsInline)
                        {
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
                            box.LineBoxes.Clear();

                            var lineBox = new CssLineBox(box);

                            box.FirstHostingLineBox = lineBox;
                            
                            //since parent is an inline box all child inlines will use
                            //the closest box ancestor as startX, startY
                            LayoutInlineBoxes(g,
                                closestBlockAncestor,
                                ref lineBox,
                                box, curX, curY,
                                ref curX, ref curY, localMaxRight, ref currentMaxBottom);

                            curX = box.ActualRight;

                            box.LastHostingLineBox = lineBox;

                            //Gets the rectangles for each line-box
                            foreach (var linebox in box.LineBoxes)
                            {
                                ApplyHorizontalAlignment(g, linebox);
                                ApplyRightToLeft(box, linebox);
                                BubbleRectangles(box, linebox);
                                //ApplyVerticalAlignment(g, linebox);
                                linebox.AssignRectanglesToBoxes();
                            }
                        }
                      
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

                box.LastHostingLineBox = currentLineBox;
            }
        }
    }
}

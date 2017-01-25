using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    internal static partial class CssLayoutEngine
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
    }
}

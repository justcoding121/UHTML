using System;
using System.Collections.Generic;
using System.Linq;
using UHtml.Adapters;
using UHtml.Adapters.Entities;

namespace UHtml.Core.Dom
{
    /// <summary>
    /// Represents a line of text.
    /// </summary>
    /// <remarks>
    /// To learn more about line-boxes see CSS spec:
    /// http://www.w3.org/TR/CSS21/visuren.html
    /// </remarks>
    internal sealed class CssLineBox
    {
        #region Fields and Consts

        private readonly List<CssRect> words;
        private readonly CssBox ownerBox;
        private readonly Dictionary<CssBox, RRect> rects;
        private readonly List<CssBox> relatedBoxes;

        #endregion

        /// <summary>
        /// Creates a new LineBox
        /// </summary>
        public CssLineBox(CssBox ownerBox)
        {
            rects = new Dictionary<CssBox, RRect>();
            relatedBoxes = new List<CssBox>();
            words = new List<CssRect>();
            this.ownerBox = ownerBox;
            this.ownerBox.LineBoxes.Add(this);
        }

        internal void AdjustMaxBottom()
        {

        }

        /// <summary>
        /// Gets a list of boxes related with the linebox. 
        /// To know the words of the box inside this linebox, use the <see cref="WordsOf"/> method.
        /// </summary>
        public List<CssBox> RelatedBoxes
        {
            get { return relatedBoxes; }
        }

        /// <summary>
        /// Gets the words inside the linebox
        /// </summary>
        public List<CssRect> Words
        {
            get { return words; }
        }

        /// <summary>
        /// Gets the owner box
        /// </summary>
        public CssBox OwnerBox
        {
            get { return ownerBox; }
        }

        /// <summary>
        /// Gets a List of rectangles that are to be painted on this linebox
        /// </summary>
        public Dictionary<CssBox, RRect> Rectangles
        {
            get { return rects; }
        }

        /// <summary>
        /// Get the height of this box line (the max height of all the words)
        /// </summary>
        public double LineHeight
        {
            get
            {
                double height = 0;
                foreach (var rect in rects)
                {
                    height = Math.Max(height, rect.Value.Height);
                }
                return height;
            }
        }

        /// <summary>
        /// Get the bottom of this box line (the max bottom of all the words)
        /// </summary>
        public double LineBottom
        {
            get
            {
                double bottom = 0;
                foreach (var rect in rects)
                {
                    bottom = Math.Max(bottom, rect.Value.Y2);
                }
                return bottom;
            }
        }

        /// <summary>
        /// Lets the linebox add the word an its box to their lists if necessary.
        /// </summary>
        /// <param name="word"></param>
        internal void ReportExistanceOf(CssRect word)
        {
            if (!Words.Contains(word))
            {
                Words.Add(word);
            }

            if (!RelatedBoxes.Contains(word.OwnerBox))
            {
                RelatedBoxes.Add(word.OwnerBox);
            }
        }

        /// <summary>
        /// Lets the linebox add the word an its box to their lists if necessary.
        /// </summary>
        /// <param name="word"></param>
        internal void ReportExistanceOfBox(CssBox box)
        {
            if (!RelatedBoxes.Contains(box))
            {
                RelatedBoxes.Add(box);
            }
        }

        /// <summary>
        /// Return the words of the specified box that live in this linebox
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        internal List<CssRect> WordsOf(CssBox box)
        {
            var r = new List<CssRect>();
            foreach (CssRect word in Words)
                if (word.OwnerBox.Equals(box))
                    r.Add(word);

            return r;
        }

        /// <summary>
        /// Return the words of the specified box and child inline boxes that live in this linebox
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        internal List<CssRect> WordsOf(CssBox box, List<CssRect> r)
        {
            foreach (CssRect word in Words)
                if (word.OwnerBox.Equals(box))
                    r.Add(word);

            foreach (var child in box.Boxes)
            {
                if(child.Display == "inline")
                {
                    WordsOf(child, r);
                }
            }

            return r;
        }

        /// <summary>
        /// Updates the specified rectangle of the specified box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="r"></param>
        /// <param name="b"></param>
        internal void UpdateRectangle(CssBox box, double x, double y, double r, double b)
        {
            //double leftspacing = box.ActualBorderLeftWidth + box.ActualPaddingLeft;
            //double rightspacing = box.ActualBorderRightWidth + box.ActualPaddingRight;
            //double topspacing = box.ActualBorderTopWidth + box.ActualPaddingTop;
            //double bottomspacing = box.ActualBorderBottomWidth + box.ActualPaddingTop;

            //x -= leftspacing;
            //r += rightspacing;

            //if (!box.IsImage)
            //{
            //    y -= topspacing;
            //    b += bottomspacing;
            //}

            if (RelatedBoxes.Any(bb => bb == box))
            {
                if (!Rectangles.ContainsKey(box))
                {
                    Rectangles.Add(box, RRect.FromCoordinates(x, y, r, b));
                }
                else
                {
                    RRect f = Rectangles[box];
                    Rectangles[box] = RRect.FromCoordinates(
                        Math.Min(f.X, x), Math.Min(f.Y, y),
                        Math.Max(f.X2, r), Math.Max(f.Y2, b));
                }
            }

            //inline blocks don't need to propagate up the size of child rectangles?
            //only propogate up when parent box is inline?
            if (box.ParentBox != null && box.ParentBox.IsInline)
            {
                UpdateRectangle(box.ParentBox, x, y, r, b);
            }
        }

        /// <summary>
        /// Copies the rectangles to their specified box
        /// </summary>
        internal void AssignRectanglesToBoxes()
        {
            foreach (CssBox b in Rectangles.Keys)
            {
                b.Rectangles.Add(this, Rectangles[b]);
            }
        }

        /// <summary>
        /// Sets the baseline of the words of the specified box to certain height
        /// </summary>
        /// <param name="g">Device info</param>
        /// <param name="b">box to check words</param>
        /// <param name="baseline">baseline</param>
        internal void SetBaseLine(RGraphics g, CssBox b, double baseline)
        {
            if (!Rectangles.ContainsKey(b))
            {
                return;
            }

            var r = Rectangles[b];
            var diff = baseline - r.Y2;

            ////TODO: Aqui me quede, checar poniendo "by the" con un font-size de 3em
            List<CssRect> ws = WordsOf(b, new List<CssRect>());

            if (ws.Count > 0)
            {
                foreach (var word in ws)
                {
                    if (!word.IsImage)
                    {
                        word.Top += diff;
                    }
                }
            }

            if(b.IsInline)
            {
                r.Y += diff;
            }

            if (b.IsInlineBlock)
            {
                moveBox(b, diff);
            }
            
        }


        private static void moveBox(CssBox currentBox, double yDiff)
        {
          
            currentBox.Location = new RPoint(currentBox.Location.X, currentBox.Location.Y + yDiff);


            if (currentBox.Words.Count > 0)
            {
                foreach (var word in currentBox.Words)
                {
                    word.Left = word.Left;
                    word.Top = word.Top + yDiff;
                }
            }


            foreach (var box in currentBox.Boxes)
            {
                moveBox(box, yDiff);
            }
        }

        /// <summary>
        /// Check if the given word is the last selected word in the line.<br/>
        /// It can either be the last word in the line or the next word has no selection.
        /// </summary>
        /// <param name="word">the word to check</param>
        /// <returns></returns>
        public bool IsLastSelectedWord(CssRect word)
        {
            for (int i = 0; i < words.Count - 1; i++)
            {
                if (words[i] == word)
                {
                    return !words[i + 1].Selected;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the words of the linebox
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string[] ws = new string[Words.Count];
            for (int i = 0; i < ws.Length; i++)
            {
                ws[i] = Words[i].Text;
            }
            return string.Join(" ", ws);
        }
    }
}
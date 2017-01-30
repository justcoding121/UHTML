using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHtml.Core.Dom
{
    internal class CurrentProgress
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public CssLineBox CurrentLineBox { get; set; }
    }
}

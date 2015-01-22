using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace svgcheck
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] != null)
            {
                // will read a single SVG document and write out checked+pruned version
                SVGChecker.Check(args[0]);
            }
        }
    }
}

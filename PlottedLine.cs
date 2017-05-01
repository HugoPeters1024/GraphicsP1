using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    /// <summary>
    /// A function wrapper for a plotted line.
    /// </summary>
    class PlottedLine
    {
        public float translation;
        public float Function(float x)
        {
            return (float)Math.Sin(x - translation) + (float)Math.Sin((x - translation) * 5);
        }

        public PlottedLine()
        {
            translation = 0;
        }
    }
}

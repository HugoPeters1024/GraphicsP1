using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    /// <summary>
    /// A type that stores 2 integer values
    /// </summary>
    class Point
    {
        public int x, y;

        /// <summary> 
        /// Create a 0 initiated Point 
        /// </summery>
        public Point()
        {
            x = 0;
            y = 0;
        }

        public Point(int n)
        {
            x = n;
            y = n;
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.x + p2.x, p1.y + p2.y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.x - p2.x, p1.y - p2.y);
        }
    }
}

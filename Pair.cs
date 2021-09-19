using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace square_algorithm
{
    class Pair
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }

        public Pair(Point p1, Point p2)
        {
            Point1 = p1;
            Point2 = p2;
        }
    }
}

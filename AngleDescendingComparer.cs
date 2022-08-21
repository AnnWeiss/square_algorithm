using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace square_algorithm
{
    class AngleDescendingComparer : IComparer<PointAndAngle>
    {
        public int Compare(PointAndAngle o2, PointAndAngle o1)
        {
            double a = o1.angle;
            double b = o2.angle;

            if (a > b)
            {
                return 1;
            }
            else if (a < b)
            {
                return -1;
            }
            return 0;
        }
    }
}

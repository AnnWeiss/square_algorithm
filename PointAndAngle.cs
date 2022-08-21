using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace square_algorithm
{
    class PointAndAngle
    {
        public Point point;
        public double angle;
        public PointAndAngle(Point point)
        {
            this.point = point;
        }
        public Point getPoint()
        {
            return point;
        }
        public void setAngle(double angle)
        {
            this.angle = angle;
        }
    }
}

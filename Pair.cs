using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace square_algorithm
{
    public class Pair : IEquatable<Pair>
    {
        public Point Point1;
        public Point Point2;

        public Pair(Point p1, Point p2)
        {
            Point1 = p1;
            Point2 = p2;
        }
        public Pair()
        {

        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Pair);
        }

        public bool Equals(Pair other)
        {
            return other != null &&
                   (EqualityComparer<Point>.Default.Equals(Point1, other.Point1) &&
                   EqualityComparer<Point>.Default.Equals(Point2, other.Point2)) ||
                   (EqualityComparer<Point>.Default.Equals(Point1, other.Point2) &&
                   EqualityComparer<Point>.Default.Equals(Point2, other.Point1));
        }

        public override int GetHashCode()
        {
            int hashCode = 363529913;
            hashCode = hashCode * -1521134295 + Point1.GetHashCode();
            hashCode = hashCode * -1521134295 + Point2.GetHashCode();
            return hashCode;
        }
    }
}

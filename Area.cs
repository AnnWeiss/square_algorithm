using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace square_algorithm
{
    public class Area
    {
        public Rectangle rect;
        public List<Point> points;
        public List<Point> vertices;
        public bool wasVisited;
        public List<Pair> pairsList;

        public Area(Rectangle _rect)
        {
            rect = _rect;
            points = new List<Point>();
            vertices = new List<Point>();
            pairsList = new List<Pair>();
        }
        public Point getVertice(int value)
        {
            return vertices[value];
        }
        public List<Point> getListPoints()
        {
            return points;
        }
    }
}

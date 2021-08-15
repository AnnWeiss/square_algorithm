using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace square_algorithm
{
    class Area
    {
        public Rectangle rect { get; set; }
        public List<Point> points { get; set; }
        public List<Point> vertices { get; set; }
        public bool wasVisited { get; set; }

        public Area(Rectangle _rect)
        {
            rect = _rect;
            points = new List<Point>();
            vertices = new List<Point>();
            wasVisited = false;
        }
        public Point getVertice(int value)
        {
            return vertices[value];
        }
    }
}

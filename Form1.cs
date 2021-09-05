using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace square_algorithm
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        List<Area> areasList = new List<Area>();
        List<Point> pointsList = new List<Point>();
        List<Nums> numsList = new List<Nums>();
        Queue lineQueue = new Queue();
        public struct Nums
        {
            public int num { get; set; }
            public int upperNum { get; set; }
            public int upperRightNum { get; set; }
            public int rightNum { get; set; }
            public int downNum { get; set; }
            public int downRightNum { get; set; }
            public Nums(int _num, int _upperNum, int _upperRightNum, int _rightNum, int _downNum, int _downRightNum)
            {
                num = _num;
                upperNum = _upperNum;
                upperRightNum = _upperRightNum;
                rightNum = _rightNum;
                downNum = _downNum;
                downRightNum = _downRightNum;
            }

            public void searchForNeighbors(int areaCount, int number)
            {
                num = number;
                upperNum = number - areaCount;
                upperRightNum = number - areaCount + 1;
                rightNum = number + 1;
                downNum = number + areaCount;
                downRightNum = number + areaCount + 1;
            }
        }
        public int Rotate(Point A, Point B, Point C)
        {
            return ((B.X - A.X) * (C.Y - B.Y) - (B.Y - A.Y) * (C.X - B.X));
        }
        public Form1()
        {
            areasList = new List<Area>();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void CreateBitmapAtRuntime()
        {
            int areasize = 65;
            int areasCount = Convert.ToInt32(textBox1.Text);
            if (areasCount < 2 || areasCount > 10)
            {
                throw new Exception("Введите число от 2 до 10");
            }
            mainPictureBox.Size = new Size(areasize*areasCount, areasize * areasCount);
            bmp = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            Graphics flagGraphics = Graphics.FromImage(bmp);
            flagGraphics.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
            mainPictureBox.Image = bmp;
            mainPictureBox.Invalidate();
            
        }

        public void GenerateAreas()
        {
            areasList.Clear();
            int areasCount = Convert.ToInt32(textBox1.Text);
            int w = bmp.Width / areasCount;
            int h = bmp.Height / areasCount;
            for (int i = 0; i < bmp.Height; i = i + h)
            {
                for (int j = 0; j < bmp.Width; j = j + w)
                {
                    areasList.Add(new Area(new Rectangle(j, i, w, h)));
                }
            }
            DrawAreas();
        }
        public void DrawAreas()
        {
            Pen blackPen = new Pen(Color.Black, 1);
            Graphics flagGraphics = Graphics.FromImage(bmp);
            foreach (Area a in areasList)
            {
                flagGraphics.DrawRectangle(blackPen, a.rect);
            }
            GeneratePoints();
        }

        public void GeneratePoints()
        {
            Random rnd = new Random();
            int maxXvalue = bmp.Size.Width;
            int maxYvalue = bmp.Size.Height;
            int pointsCount = Convert.ToInt32(textBox2.Text);
            if (pointsCount < 2 || pointsCount > 10000)
            {
                throw new Exception("Введите число от 2 до 10000");
            }
            int pointsIterator = 0;
            pointsList.Clear();
            while (pointsIterator < pointsCount)
            {
                int x = rnd.Next(0, maxXvalue);
                int y = rnd.Next(0, maxYvalue);
                pointsList.Add(new Point(x, y));
                pointsIterator++;
            }
            DrawRandomPoints();
        }

        public void DrawRandomPoints()
        {
            Graphics flagGraphics = Graphics.FromImage(bmp);
            foreach (Point p in pointsList)
            {
                flagGraphics.FillEllipse(Brushes.Red, p.X-2, p.Y-2, 4, 4);
            }
            PointBelongs();
        }

        public void PointBelongs()
        {
            int areasCount = Convert.ToInt32(textBox1.Text);
            int w = bmp.Width / areasCount;
            int h = bmp.Height / areasCount;
            foreach (Point p in pointsList)
            {
                int x1 = p.X / w;
                int y1 = p.Y / h;
                int areaNumber;
                y1 *= areasCount;
                areaNumber = x1 + y1;
                areasList[areaNumber].points.Add(p);
            }
            //принадлежность вершин к прямоугольникам
            for (int k = 0; k < areasList.Count; k++)
            {
                areasList[k].vertices.Add(new Point(areasList[k].rect.X, areasList[k].rect.Y));
                areasList[k].vertices.Add(new Point(areasList[k].rect.X+w, areasList[k].rect.Y));
                areasList[k].vertices.Add(new Point(areasList[k].rect.X, areasList[k].rect.Y+h));
                areasList[k].vertices.Add(new Point(areasList[k].rect.X+w, areasList[k].rect.Y+h));
            }
            FindBaseLine();
        }

        public void FindBaseLine()
        {
            if (pointsList.Count < 2)
            {
                throw new Exception("Точек меньше двух!");
            }
            int indexStartPoint = 0;
            for (int i = 0; i < pointsList.Count; i++)
            {
                if (pointsList[i].X < pointsList[indexStartPoint].X)
                {
                    indexStartPoint = i;
                }
            }
            int indexNextPoint = indexStartPoint == 0 ? 1 : 0;
            for (int i = 0; i < pointsList.Count; i++)
            {
                if ((indexStartPoint!=indexNextPoint) && (Rotate(pointsList[indexStartPoint],pointsList[indexNextPoint],pointsList[i])>0))
                {
                    indexNextPoint = i;
                }
            }
            if (indexNextPoint==indexStartPoint)
            {
                throw new Exception("Ошибка алгоритма");
            }
            Pen blackPen = new Pen(Color.Green, 1);
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawLine(blackPen, pointsList[indexStartPoint].X, pointsList[indexStartPoint].Y,
                                            pointsList[indexNextPoint].X, pointsList[indexNextPoint].Y);
            }
            //нужные области для базовой линии
            int areaNumberSP = -1, areaNumberNP = -1;
            for (int i = 0; i < areasList.Count; i++)
            {
                if (areasList[i].rect.Contains(pointsList[indexStartPoint]))
                {
                    areaNumberSP = i;
                }
                if (areasList[i].rect.Contains(pointsList[indexNextPoint]))
                {
                    areaNumberNP = i;
                }
            }
            //для линии соседи
            numsList.Clear();
            //вершины областей iSP iNP
            int areasCount = Convert.ToInt32(textBox1.Text);
            int xI1 = -1, yJ1 = -1, xI2=-1, yJ2=-1;
            int[,] Matrix = new int[areasCount, areasCount];
            int k = 0;
            for (int i = 0; i < areasCount; i++)
            {
                for (int j = 0; j < areasCount; j++)
                {
                    Matrix[i, j] = k++; //заполняю матрицу
                    if (Matrix[i, j] == areaNumberSP)
                    {
                        xI1 = i;
                        yJ1 = j;
                    }
                    if (Matrix[i, j] == areaNumberNP)
                    {
                        xI2 = i;
                        yJ2 = j;
                    }
                }
            }
            if (xI1 > xI2)//swap
            {
                xI1 = xI1 + xI2;
                xI2 = xI1 - xI2;
                xI1 = xI1 - xI2;
            }
            if (yJ1 > yJ2)
            {
                yJ1 = yJ1 + yJ2;
                yJ2 = yJ1 - yJ2;
                yJ1 = yJ1 - yJ2;
            }
            //рассматриваем в диапазоне
            for (int i = xI1; i <= xI2; i++)
            {
                int one = 0, two = 0, three = 0, four = 0;
                for (int j = yJ1; j <= yJ2; j++)
                {
                    int val = Matrix[i,j];
                    one = Rotate(pointsList[indexStartPoint], pointsList[indexNextPoint], areasList[val].getVertice(0));
                    two = Rotate(pointsList[indexStartPoint], pointsList[indexNextPoint], areasList[val].getVertice(1));
                    three = Rotate(pointsList[indexStartPoint], pointsList[indexNextPoint], areasList[val].getVertice(2));
                    four = Rotate(pointsList[indexStartPoint], pointsList[indexNextPoint], areasList[val].getVertice(3));

                    if ((one < 0 && two < 0 && three < 0 && four < 0) || (one > 0 && two > 0 && three > 0 && four > 0))
                    {
                        continue;
                    }
                    else
                    {
                        int area1 = Matrix[i, j];
                        Nums numms3 = new Nums(-1,-1, -1, -1, -1, -1);
                        numms3.searchForNeighbors(areasCount, area1);
                        numsList.Add(numms3);
                    }
                }
                foreach(Nums n in numsList)
                {
                    areasList[n.num].wasVisited = true;
                }
            }
            lineQueue.Enqueue(pointsList[indexStartPoint]);
            lineQueue.Enqueue(pointsList[indexNextPoint]);
            Triangulation(ref numsList, ref lineQueue, areasCount);
        }
        public void Triangulation(ref List<Nums> nums, ref Queue lineQueue, int arcount)
        {
            if (lineQueue.Count >= 2)
            {
                Point A = (Point)lineQueue.Dequeue();
                Point B = (Point)lineQueue.Dequeue();
                nums = isEmptyArea(ref nums, arcount, A, B);

                lineQueue.Clear();
            }
        }
        public List<Nums> isEmptyArea(ref List<Nums> nums, int arcount, Point SP, Point NP)
        {
            List<Nums> newList = new List<Nums>();
            newList = cellsBypassing(ref nums, arcount);
            int pCount = 0;
            for (int i = 0; i < newList.Count; i++)
            {
                int z = areasList[newList[i].num].getListPoints().Count;
                pCount += z;
                if (pCount > 0)
                {
                    setTriangle(SP, NP, ref newList);
                    break;
                }
            }
            if (pCount == 0 && newList.Count != 0)
            {
                isEmptyArea(ref newList, arcount, SP, NP);
            }
            return newList;
        }

        public double getLineLength(Point A, Point B)
        {
            double xVal = Math.Pow(B.X - A.X, 2);
            double yVal = Math.Pow(B.Y - A.Y, 2);
            double dist = Math.Sqrt(xVal + yVal);
            return dist;
        }
        public void setTriangle(Point A, Point B, ref List<Nums> nums)
        {
            double AB = getLineLength(A, B); //c
            double finalGamma = 0;
            int a = 0, n = 0;
            for (int i = 0; i < nums.Count; i++)
            {
                List<Point> newList = new List<Point>();
                newList = areasList[nums[i].num].getListPoints();
                
                for (int k = 0; k < newList.Count; k++)//перебор листа поинтов num ректангла
                {
                    int val = Rotate(A, B, newList[k]);
                    if (val < 0)
                    {
                        double AC = getLineLength(A, newList[k]); //b
                        double CB = getLineLength(newList[k], B); //a
                        double gamma = Math.Acos((CB * CB + AC * AC - AB * AB) / (2 * CB * AC)) * 180 / Math.PI;
                        if (gamma > finalGamma)
                        {
                            finalGamma = gamma;
                            a = k;
                            n = i;
                        }
                    }
                }
            }
            if (finalGamma > 0)
            {
                Graphics flagGraphics = Graphics.FromImage(bmp);
                Pen bluePen = new Pen(Color.Blue, 1);
                flagGraphics.DrawLine(bluePen, A.X, A.Y,
                                                areasList[nums[n].num].points[a].X, areasList[nums[n].num].points[a].Y);
                flagGraphics.DrawLine(bluePen, areasList[nums[n].num].points[a].X, areasList[nums[n].num].points[a].Y, B.X, B.Y);
                //закинем новые базовые линии в очередь
                lineQueue.Enqueue(A);
                lineQueue.Enqueue(areasList[nums[n].num].points[a]);
                lineQueue.Enqueue(areasList[nums[n].num].points[a]);
                lineQueue.Enqueue(B);
            }
           
        }
        public List<Nums> cellsBypassing(ref List<Nums> nums, int arcount)
        {
            int size = arcount * arcount;
            List<Nums> numsList2 = new List<Nums>();

            for (int i = 0; i < nums.Count; i++)
            {
                int URN = nums[i].upperRightNum;
                URN %= arcount;

                int RN = nums[i].rightNum;
                RN %= arcount;

                int DRN = nums[i].downRightNum;
                DRN %= arcount;

                if (areasList[nums[i].num].wasVisited == false)
                {
                    areasList[nums[i].num].wasVisited = true;
                }
                if ( nums[i].upperNum >= 0 && areasList[nums[i].upperNum].wasVisited == false)
                {
                    areasList[nums[i].upperNum].wasVisited = true;
                    Nums numms3 = new Nums(-1, -1, -1, -1, -1, -1);
                    numms3.searchForNeighbors(arcount, nums[i].upperNum);
                    numsList2.Add(numms3);
                }
                if (nums[i].upperRightNum > 0 && URN > 0 && areasList[nums[i].upperRightNum].wasVisited == false)
                {
                    areasList[nums[i].upperRightNum].wasVisited = true;
                    Nums numms3 = new Nums(-1, -1, -1, -1, -1, -1);
                    numms3.searchForNeighbors(arcount, nums[i].upperRightNum);
                    numsList2.Add(numms3);
                }
                if (nums[i].rightNum >= 0 && RN > 0 && areasList[nums[i].rightNum].wasVisited == false)
                {
                    areasList[nums[i].rightNum].wasVisited = true;
                    Nums numms3 = new Nums(-1, -1, -1, -1, -1, -1);
                    numms3.searchForNeighbors(arcount, nums[i].rightNum);
                    numsList2.Add(numms3);
                }
                if (nums[i].downNum >= 0 && nums[i].downNum < size && areasList[nums[i].downNum].wasVisited == false)
                {
                    areasList[nums[i].downNum].wasVisited = true;
                    Nums numms3 = new Nums(-1, -1, -1, -1, -1, -1);
                    numms3.searchForNeighbors(arcount, nums[i].downNum);
                    numsList2.Add(numms3);
                }
                if (nums[i].downRightNum >= 0 && nums[i].downRightNum < size && DRN > 0 && areasList[nums[i].downRightNum].wasVisited == false)
                {
                    areasList[nums[i].downRightNum].wasVisited = true;
                    Nums numms3 = new Nums(-1, -1, -1, -1, -1, -1);
                    numms3.searchForNeighbors(arcount, nums[i].downRightNum);
                    numsList2.Add(numms3);
                }
            }
            return numsList2;
        }

        private void genButton_Click(object sender, EventArgs e)
        {
            CreateBitmapAtRuntime();
            GenerateAreas();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

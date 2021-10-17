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
        List<Nums> numsByPass = new List<Nums>();//сюда будет закидываться обход клеток
        Queue<Point> lineQueue = new Queue<Point>();
        List<Pair> pairsList = new List<Pair>();
        HashSet<Point> allreadyAddedPoints = new HashSet<Point>();
        public struct Nums
        {
            public int num { get; set; }
            public int upperNum { get; set; }
            public int upperRightNum { get; set; }
            public int rightNum { get; set; }
            public int downNum { get; set; }
            public int downRightNum { get; set; }

            public void searchForNeighbors(ref int areaCount, int number)
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
                MessageBox.Show("Введите число от 2 до 10");
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
        }
        public void DrawAreas()
        {
            Pen blackPen = new Pen(Color.Black, 1);
            Graphics flagGraphics = Graphics.FromImage(bmp);
            foreach (Area a in areasList)
            {
                flagGraphics.DrawRectangle(blackPen, a.rect);
            }
        }

        public void GeneratePoints()
        {
            Random rnd = new Random();
            int maxXvalue = bmp.Size.Width;
            int maxYvalue = bmp.Size.Height;
            int pointsCount = Convert.ToInt32(textBox2.Text);
            if (pointsCount < 2 || pointsCount > 10000)
            {
                MessageBox.Show("Введите число от 2 до 10000");
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
        }

        public void DrawRandomPoints()
        {
            Graphics flagGraphics = Graphics.FromImage(bmp);
            foreach (Point p in pointsList)
            {
                flagGraphics.FillEllipse(Brushes.Red, p.X-2, p.Y-2, 4, 4);
            }
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
        }

        public void FindBaseLine()
        {
            if (pointsList.Count < 2)
            {
                MessageBox.Show("Точек меньше двух!");
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
            if (indexNextPoint == indexStartPoint)
            {
                MessageBox.Show("Ошибка алгоритма");
            }
            Point SP = new Point();
            SP = pointsList[indexStartPoint];
            Point NP = new Point();
            NP = pointsList[indexNextPoint];
            //нужные области для базовой линии
            int areaNumberSP = -1, areaNumberNP = -1;
            for (int i = 0; i < areasList.Count; i++)
            {
                if (areasList[i].rect.Contains(SP))
                {
                    areaNumberSP = i;
                }
                if (areasList[i].rect.Contains(NP))
                {
                    areaNumberNP = i;
                }
            }
            //вершины областей iSP iNP
            int areasCount = Convert.ToInt32(textBox1.Text);
            int xI1 = -1, yJ1 = -1, xI2=-1, yJ2=-1;
            int[,] Matrix = new int[areasCount, areasCount];
            int k = 0;
            for (int i = 0; i < areasCount; i++)
            {
                for (int j = 0; j < areasCount; j++)
                {
                    Matrix[i, j] = k++;
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
            List<Nums> numsList = new List<Nums>();//лист клеток с первой базовой линией
            numsList.Clear();
            for (int i = xI1; i <= xI2; i++)
            {
                int one = 0, two = 0, three = 0, four = 0;
                for (int j = yJ1; j <= yJ2; j++)
                {
                    int val = Matrix[i,j];
                    one = Rotate(SP, NP, areasList[val].getVertice(0));
                    two = Rotate(SP, NP, areasList[val].getVertice(1));
                    three = Rotate(SP, NP, areasList[val].getVertice(2));
                    four = Rotate(SP, NP, areasList[val].getVertice(3));

                    if ((one < 0 && two < 0 && three < 0 && four < 0) || (one > 0 && two > 0 && three > 0 && four > 0))
                    {
                        continue;
                    }
                    else
                    {
                        int area1 = Matrix[i, j];
                        Nums numms3 = new Nums();
                        numms3.searchForNeighbors(ref areasCount, area1);
                        numsList.Add(numms3);
                    }
                }
            }
            foreach (Nums n in numsList)
            {
                areasList[n.num].wasVisited = true;
            }
            //порядок обхода вызывается здесь..
            addCellsBypass(ref numsList, ref areasCount, ref SP, ref NP);
            lineQueue.Enqueue(SP);
            lineQueue.Enqueue(NP);
            allreadyAddedPoints.Add(SP);
            allreadyAddedPoints.Add(NP);
            pairsList.Add(new Pair(SP, NP));
            Triangulation(ref lineQueue, ref areasCount);
        }
        public void addCellsBypass(ref List<Nums> list, ref int arcount, ref Point SP, ref Point NP)//задается нужный порядок обхода от базовой линии
        {
            List<Nums> numsListsec = new List<Nums>();
            numsListsec = cellsBypassing(ref list, ref arcount, ref SP, ref NP);//создались соседи для листа
            //проверка на выход из зоны базовой линии, удаление лишних элементов
            for (int i = 0; i < numsListsec.Count; i++)
            {
                int one = 0, two = 0, three = 0, four = 0;
                one = Rotate(SP, NP, areasList[numsListsec[i].num].getVertice(0));
                two = Rotate(SP, NP, areasList[numsListsec[i].num].getVertice(1));
                three = Rotate(SP, NP, areasList[numsListsec[i].num].getVertice(2));
                four = Rotate(SP, NP, areasList[numsListsec[i].num].getVertice(3));

                if (one > 0 && two > 0 && three > 0 && four > 0)
                {
                    numsListsec.RemoveAt(i);
                }
            }

            int f = list.Count % 2;//проверка на четность
            if (f > 0)//нечет
            {
                for (int i = 0; i < list.Count; i++)//порядок обхода задается здесь
                {
                    numsByPass.Add(list[i]);
                    int a = list.Count - i - 1;
                    if (i == a)
                    {
                        break;
                    }
                    numsByPass.Add(list[a]);
                }
            }
            if (f == 0)//чет
            {
                int a = -1;
                for (int i = 0; i < list.Count; i++)//порядок обхода задается здесь
                {
                    if (i == a)
                    {
                        break;
                    }
                    numsByPass.Add(list[i]);
                    a = list.Count - i - 1;
                    numsByPass.Add(list[a]);
                }
            }
            if (numsListsec.Count != 0)
            {
                addCellsBypass(ref numsListsec, ref arcount, ref SP, ref NP);
            }
        }
        public void Triangulation(ref Queue<Point> lineQueue, ref int arcount)
        {
            if (lineQueue.Count >= 2)
            {
                Point A = (Point)lineQueue.Dequeue();
                Point B = (Point)lineQueue.Dequeue();
                setTriangle(ref A, ref B, ref numsByPass);
                if (lineQueue.Count > 0)
                {
                    Triangulation(ref lineQueue, ref arcount);
                }
                numsByPass.Clear();
            }
        }

        public double getLineLength(Point A, Point B)
        {
            double xVal = Math.Pow(B.X - A.X, 2);
            double yVal = Math.Pow(B.Y - A.Y, 2);
            double dist = Math.Sqrt(xVal + yVal);
            return dist;
        }
        void setTriangle(ref Point A, ref Point B, ref List<Nums> numsByPass)
        {
            double AB = getLineLength(A, B); //c
            double finalGamma = 0;
            int a = 0, n = 0;
            List<Point> newListPoints = new List<Point>();
            for (int i = 0; i < numsByPass.Count; i++)
            {
                newListPoints = areasList[numsByPass[i].num].getListPoints();
                for (int k = 0; k < newListPoints.Count; k++)//перебор листа поинтов num ректангла
                {
                    int val = Rotate(A, B, newListPoints[k]);
                    if (val < 0)
                    {
                        double AC = getLineLength(A, newListPoints[k]); //b
                        double CB = getLineLength(newListPoints[k], B); //a
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
                Pair new1 = new Pair(A, areasList[numsByPass[n].num].points[a]);
                Pair new2 = new Pair(areasList[numsByPass[n].num].points[a], B);
                if (!isContainLine(ref new1))
                {
                    lineQueue.Enqueue(A);
                    lineQueue.Enqueue(areasList[numsByPass[n].num].points[a]);
                    pairsList.Add(new1);
                }
                if (!isContainLine(ref new2))
                {
                    lineQueue.Enqueue(areasList[numsByPass[n].num].points[a]);
                    lineQueue.Enqueue(B);
                    pairsList.Add(new2);
                }
            }
        }
        public void drawLine(ref List<Pair> pairsList)
        {
            Graphics flagGraphics = Graphics.FromImage(bmp);
            Pen bluePen = new Pen(Color.Blue, 1);
            foreach (Pair p in pairsList)
            {
                flagGraphics.DrawLine(bluePen, p.Point1, p.Point2);
            }
            pairsList.Clear();
        }
        bool isContainLine(ref Pair line)
        {
            foreach (Pair p in pairsList)
            {
                if ((p.Point1 == line.Point1 && p.Point2 == line.Point2))
                {
                    return true;
                }
            }
            return false;
        }
        public List<Nums> cellsBypassing(ref List<Nums> nums, ref int arcount, ref Point A, ref Point B)//огибающая
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
                    Nums numms3 = new Nums();
                    numms3.searchForNeighbors(ref arcount, nums[i].upperNum);
                    numsList2.Add(numms3);
                }
                if (nums[i].upperRightNum > 0 && URN > 0 && areasList[nums[i].upperRightNum].wasVisited == false)
                {
                    areasList[nums[i].upperRightNum].wasVisited = true;
                    Nums numms3 = new Nums();
                    numms3.searchForNeighbors(ref arcount, nums[i].upperRightNum);
                    numsList2.Add(numms3);
                }
                if (nums[i].rightNum >= 0 && RN > 0 && areasList[nums[i].rightNum].wasVisited == false)
                {
                    areasList[nums[i].rightNum].wasVisited = true;
                    Nums numms3 = new Nums();
                    numms3.searchForNeighbors(ref arcount, nums[i].rightNum);
                    numsList2.Add(numms3);
                }
                if (nums[i].downRightNum >= 0 && nums[i].downRightNum < size && DRN > 0 && areasList[nums[i].downRightNum].wasVisited == false)
                {
                    areasList[nums[i].downRightNum].wasVisited = true;
                    Nums numms3 = new Nums();
                    numms3.searchForNeighbors(ref arcount, nums[i].downRightNum);
                    numsList2.Add(numms3);
                }
                if (nums[i].downNum >= 0 && nums[i].downNum < size && areasList[nums[i].downNum].wasVisited == false)
                {
                    areasList[nums[i].downNum].wasVisited = true;
                    Nums numms3 = new Nums();
                    numms3.searchForNeighbors(ref arcount, nums[i].downNum);
                    numsList2.Add(numms3);
                }
            }
            return numsList2;
        }

        private void genButton_Click(object sender, EventArgs e)
        {
            CreateBitmapAtRuntime();
            GenerateAreas();
            DrawAreas();
            GeneratePoints();
            DrawRandomPoints();
            PointBelongs();
            FindBaseLine();
            drawLine(ref pairsList);
            allreadyAddedPoints.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FindBaseLine();
        }
    }
}

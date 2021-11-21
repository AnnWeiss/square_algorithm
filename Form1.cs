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
        public class Nums
        {
            public int mainNum;
            public List<int> numsList = new List<int>();
            public int sizeOfArea; //areasCount*areasCount
            public Nums(int size)
            {
                sizeOfArea = size;
            }
            public void RightLeft(int areaCount, int number, int one)
            {
                mainNum = number;
                int up = number - areaCount;
                int down = number + areaCount;
                int side = number + one;
                var isOneRow = side / areaCount == number / areaCount;
                if (up > 0)
                {
                    numsList.Add(up);
                }
                if (isOneRow && up > 0)
                {
                    numsList.Add(up + one);
                }
                if (isOneRow)
                {
                    numsList.Add(side);
                }
                if (isOneRow && down < sizeOfArea)
                {
                    numsList.Add(down + one);
                }
                if (down < sizeOfArea)
                {
                    numsList.Add(down);
                }
            }
            public void UpDown(int areaCount, int number)
            {
                mainNum = number;
                int value = number + areaCount;//up or down
                int sideLeft = number - 1;
                var isOneRowLeft = sideLeft / areaCount == number / areaCount;
                int sideRight = number + 1;
                var isOneRowRight = sideRight / areaCount == number / areaCount;
                
                if (isOneRowLeft)
                {
                    numsList.Add(sideLeft);
                }
                if (isOneRowLeft && value > 0 && value < sizeOfArea)
                {
                    numsList.Add(value - 1);
                }
                if (value > 0 && value < sizeOfArea)
                {
                    numsList.Add(value);
                }
                if (isOneRowRight && value > 0 && value < sizeOfArea)
                {
                    numsList.Add(value + 1);
                }
                if (isOneRowRight)
                {
                    numsList.Add(sideRight);
                }
            }
            public void UpRightLeft(int areaCount, int number,int one)
            {
                mainNum = number;
                int up = number - areaCount;
                int side = number + one;
                var isOneRow = side / areaCount == number / areaCount;
                if (up > 0)
                {
                    numsList.Add(up);
                }
                if (isOneRow && up > 0)
                {
                    numsList.Add(up + one);
                }
                if (isOneRow)
                {
                    numsList.Add(side);
                }
            }
            public void DownRightLeft(int areaCount, int number,int one)
            {
                mainNum = number;
                int down = number + areaCount;
                int side = number + one;
                var isOneRow = side / areaCount == number / areaCount;
                if (isOneRow)
                {
                    numsList.Add(side);
                }
                if (isOneRow && down < sizeOfArea)
                {
                    numsList.Add(down + one);
                }
                if (down < sizeOfArea)
                {
                    numsList.Add(down);
                }
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
            List<Nums> numsList = new List<Nums>();//лист клеток с первой базовой линией
            numsList.Clear();

            Point NVector = new Point();
            NVector = getNormalVector(SP, NP); //вектор нормали к базовой линии
            //NVector.X = 1; NVector.Y = 0;

            for (int i = xI1; i <= xI2; i++)//рассматриваем в диапазоне, пропускаем клетки, через которые прямая не проходит
            {
                int one, two, three, four;
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
                        int area = Matrix[i, j];
                        addNums(NVector, areasCount, area, ref numsList);
                    }
                }
            }
            foreach (Nums n in numsList)
            {
                areasList[n.mainNum].wasVisited = true;
            }
            addCellsBypass(ref numsList, areasCount, SP, NP, NVector);//порядок обхода вызывается здесь..
            lineQueue.Enqueue(SP);
            lineQueue.Enqueue(NP);
            allreadyAddedPoints.Add(SP);
            allreadyAddedPoints.Add(NP);
            pairsList.Add(new Pair(SP, NP));
            Triangulation(ref lineQueue);
        }
        public void addNums(Point NVector, int areasCount, int area, ref List<Nums> numsList)
        {
            Nums numms = new Nums(areasCount*areasCount);
            if (NVector.X > 0 && NVector.Y > 0) //вправо вверх
            {
                numms.UpRightLeft(areasCount, area, 1);
                numsList.Add(numms);
            }
            if (NVector.X > 0 && NVector.Y < 0) //вправо вниз
            {
                numms.DownRightLeft(areasCount, area, 1);
                numsList.Add(numms);
            }
            if (NVector.X < 0 && NVector.Y < 0) //влево вниз
            {
                numms.DownRightLeft(areasCount, area, -1);
                numsList.Add(numms);
            }
            if (NVector.X < 0 && NVector.Y > 0) //влево вверх
            {
                numms.UpRightLeft(areasCount, area, -1);
                numsList.Add(numms);
            }
            if (NVector.X == 0 && NVector.Y < 0) //вниз
            {
                numms.UpDown(areasCount, area);
                numsList.Add(numms);
            }
            if (NVector.X == 0 && NVector.Y > 0) //вверх
            {
                numms.UpDown(-areasCount, area);
                numsList.Add(numms);
            }
            if (NVector.X > 0 && NVector.Y == 0) //вправо
            {
                numms.RightLeft(areasCount, area, 1);
                numsList.Add(numms);
            }
            if (NVector.X < 0 && NVector.Y == 0) //влево
            {
                numms.RightLeft(areasCount, area, -1);
                numsList.Add(numms);
            }
        }
        public Point getNormalVector(Point sp, Point np)
        {
            np.X = np.X - sp.X; np.Y = np.Y - sp.Y;//конец минус начало
            int a = np.Y; int b = -np.X;
            np.X = a; np.Y = b;
            return np;//возврат координат конца вектора, повернутые на 90 градусов во внеш.сторону
        }
        public List<Nums> createNeighbors(ref List<Nums> list, Point NVecor, int areasCount)
        {
            List<Nums> numsListsec = new List<Nums>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].numsList.Count; j++)
                {
                    addNums(NVecor, areasCount, list[i].numsList[j], ref numsListsec);
                }
            }
            return numsListsec;
        }
        public void addCellsBypass(ref List<Nums> list, int arcount, Point SP, Point NP, Point NVector)//задается нужный порядок обхода от базовой линии
        {
            List<Nums> numsListsec = new List<Nums>();
            list = deleteDublicates(ref list);
            //для каждого объекта в его листе создать для элементов соседей
            numsListsec = createNeighbors(ref list,NVector,arcount);
            //проверка на выход из зоны базовой линии, удаление лишних элементов
            for (int i = 0; i < numsListsec.Count; i++)
            {
                int one, two, three, four;
                one = Rotate(SP, NP, areasList[numsListsec[i].mainNum].getVertice(0));
                two = Rotate(SP, NP, areasList[numsListsec[i].mainNum].getVertice(1));
                three = Rotate(SP, NP, areasList[numsListsec[i].mainNum].getVertice(2));
                four = Rotate(SP, NP, areasList[numsListsec[i].mainNum].getVertice(3));

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
                addCellsBypass(ref numsListsec, arcount, SP, NP, NVector);
            }
        }
        public List<Nums> deleteDublicates(ref List<Nums> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].numsList.Count; j++)
                {
                    if (!areasList[list[i].numsList[j]].wasVisited)
                    {
                        areasList[list[i].numsList[j]].wasVisited = true;
                    }
                    else
                    {
                        list[i].numsList.RemoveAt(j);
                    }
                }
            }
            return list;
        }
        public void Triangulation(ref Queue<Point> lineQueue)
        {
            if (lineQueue.Count >= 2)
            {
                Point A = lineQueue.Dequeue();
                Point B = lineQueue.Dequeue();
                setTriangle(in A, in B, ref numsByPass);
                if (lineQueue.Count > 0)
                {
                    Triangulation(ref lineQueue);
                }
                numsByPass.Clear();
            }
        }

        public double getLineLength(in Point A, in Point B)
        {
            double xVal = Math.Pow(B.X - A.X, 2);
            double yVal = Math.Pow(B.Y - A.Y, 2);
            double dist = Math.Sqrt(xVal + yVal);
            return dist;
        }
        void setTriangle(in Point A, in Point B, ref List<Nums> numsByPass)
        {
            double AB = getLineLength(A, B); //c
            double finalGamma = 0;
            int a = 0, n = 0;
            List<Point> newListPoints = new List<Point>();
            for (int i = 0; i < numsByPass.Count; i++)
            {
                newListPoints = areasList[numsByPass[i].mainNum].getListPoints();
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
                Pair new1 = new Pair(A, areasList[numsByPass[n].mainNum].points[a]);
                Pair new2 = new Pair(areasList[numsByPass[n].mainNum].points[a], B);
                if (!isContainLine(ref new1))
                {
                    lineQueue.Enqueue(A);
                    lineQueue.Enqueue(areasList[numsByPass[n].mainNum].points[a]);
                    pairsList.Add(new1);
                }
                if (!isContainLine(ref new2))
                {
                    lineQueue.Enqueue(areasList[numsByPass[n].mainNum].points[a]);
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
        /*public List<Nums> checkCellsBypassing(ref List<Nums> nums, in int arcount, ref Point NVector)
        {
            int size = arcount * arcount;
            List<Nums> numsList2 = new List<Nums>();
            return numsList2;
        }*/

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

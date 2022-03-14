using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace square_algorithm
{
    public class Nums
    {
        public int mainNum;
        public List<int> numsList = new List<int>();
        static public int sizeOfArea; //areasCount*areasCount
        public Nums(int size)
        {
            sizeOfArea = size;
        }
        public void setSquare_Down_Or_RightDown(int areaCount, int number)
        {
            mainNum = number;
            int down = number - areaCount;
            int up = number + areaCount;
            int sideRight = number + 1;
            int sideLeft = number - 1;
            var isOneRowRight = sideRight / areaCount == number / areaCount;
            var isOneRowLeft = sideLeft / areaCount == number / areaCount;
            if (isOneRowRight && down >= 0)
            {
                numsList.Add(down + 1);
            }
            if (isOneRowRight)
            {
                numsList.Add(sideRight);
            }
            if (isOneRowRight && up < sizeOfArea)
            {
                numsList.Add(up + 1);
            }
            if (up < sizeOfArea)
            {
                numsList.Add(up);
            }
            if (isOneRowLeft && up < sizeOfArea && sideLeft >= 0)
            {
                numsList.Add(up - 1);
            }
            if (isOneRowLeft && sideLeft >= 0)
            {
                numsList.Add(sideLeft);
            }
            if (isOneRowLeft && down >= 0)
            {
                numsList.Add(down - 1);
            }
            if (down >= 0)
            {
                numsList.Add(down);
            }
        }
        public void setSquare_Left_Or_LeftDown(int areaCount, int number)
        {
            mainNum = number;
            int down = number - areaCount;
            int up = number + areaCount;
            int sideRight = number + 1;
            int sideLeft = number - 1;
            var isOneRowRight = sideRight / areaCount == number / areaCount;
            var isOneRowLeft = sideLeft / areaCount == number / areaCount;
            if (isOneRowLeft && down >= 0)
            {
                numsList.Add(down - 1);
            }
            if (down >= 0)
            {
                numsList.Add(down);
            }
            if (isOneRowRight && down >= 0)
            {
                numsList.Add(down + 1);
            }
            if (isOneRowRight)
            {
                numsList.Add(sideRight);
            }
            if (isOneRowRight && up < sizeOfArea)
            {
                numsList.Add(up + 1);
            }
            if (up < sizeOfArea)
            {
                numsList.Add(up);
            }
            if (isOneRowLeft && up < sizeOfArea && sideLeft >= 0)
            {
                numsList.Add(up - 1);
            }
            if (isOneRowLeft && sideLeft >= 0)
            {
                numsList.Add(sideLeft);
            }
        }
        public void setSquare_Up_Or_UpLeft(int areaCount, int number)
        {
            mainNum = number;
            int down = number - areaCount;
            int up = number + areaCount;
            int sideRight = number + 1;
            int sideLeft = number - 1;
            var isOneRowRight = sideRight / areaCount == number / areaCount;
            var isOneRowLeft = sideLeft / areaCount == number / areaCount;
            if (isOneRowLeft && up < sizeOfArea && sideLeft >= 0)
            {
                numsList.Add(up - 1);
            }
            if (isOneRowLeft && sideLeft >= 0)
            {
                numsList.Add(sideLeft);
            }
            if (isOneRowLeft && down >= 0)
            {
                numsList.Add(down - 1);
            }
            if (down >= 0)
            {
                numsList.Add(down);
            }
            if (isOneRowRight && down >= 0)
            {
                numsList.Add(down + 1);
            }
            if (isOneRowRight)
            {
                numsList.Add(sideRight);
            }
            if (isOneRowRight && up < sizeOfArea)
            {
                numsList.Add(up + 1);
            }
            if (up < sizeOfArea)
            {
                numsList.Add(up);
            }
        }
        public void setSquare_Right_Or_RightUp(int areaCount, int number)
        {
            mainNum = number;
            int down = number - areaCount;
            int up = number + areaCount;
            int sideRight = number + 1;
            int sideLeft = number - 1;
            var isOneRowRight = sideRight / areaCount == number / areaCount;
            var isOneRowLeft = sideLeft / areaCount == number / areaCount;
            if (isOneRowRight && up < sizeOfArea)
            {
                numsList.Add(up + 1);
            }
            if (up < sizeOfArea)
            {
                numsList.Add(up);
            }
            if (isOneRowLeft && up < sizeOfArea && sideLeft >= 0)
            {
                numsList.Add(up - 1);
            }
            if (isOneRowLeft && sideLeft >= 0)
            {
                numsList.Add(sideLeft);
            }
            if (isOneRowLeft && down >= 0)
            {
                numsList.Add(down - 1);
            }
            if (down >= 0)
            {
                numsList.Add(down);
            }
            if (isOneRowRight && down >= 0)
            {
                numsList.Add(down + 1);
            }
            if (isOneRowRight)
            {
                numsList.Add(sideRight);
            }
        }
    }
}

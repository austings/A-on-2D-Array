using System;
using System.Collections.Generic;
using System.Text;

namespace Tiles
{
    class TileNode
    {
        private int myValue;
        private int[] myPosition;
        private bool locked;
        private bool amIRightPosition;

        public TileNode(int value, int[] position, bool locked)
        {
            myValue = value;
            myPosition = position;
            this.locked = locked;
            if (locked)
                amIRightPosition = true;
            else
                amIRightPosition = false;
        }

        public int[] getMyPosition()
        {
            return myPosition;
        }

        public void updateValue(int newValue)
        {
            myValue = newValue;
        }

        public int getValue()
        {
            return myValue;
        }

        public void rightPosition(bool value)
        {
            amIRightPosition = value;
        }

        public bool isRightPos()
        {
            return amIRightPosition;
        }

        public bool getLocked()
        {
            return locked;
        }

        public void lockThis()
        {
            locked = true;
        }

        public void checkLock()
        {
            if (amIRightPosition)
            {
                //Console.WriteLine(myValue + " " + myPosition[0] + " " + myPosition[1]);
                lockThis();
            }
        }

        public bool isLocked()
        {
            return locked;
        }

    }
}

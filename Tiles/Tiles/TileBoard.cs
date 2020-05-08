using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tiles
{

    class TileBoard
    {
        private TileNode[,] myBoard = new TileNode[5, 5];
        private List<string> myMoves = new List<string>();
        private TileBoard parent;
        public int mDis;
        public int myG;
        public int myF;
        public int[] xPosition;
        public bool markForDelete = false;

        public TileBoard(TileNode[,] board, int g, List<string> moveList, int[] startPosition, TileBoard parent)
        {
            myBoard = board;
            myMoves = moveList;
            myG = g;
            xPosition = startPosition;
            mDis = mDistance();
            myF = g + mDis;
            this.parent = parent;
        }

        public TileBoard getParent()
        {
            return parent;
        }

        public override bool Equals(object obj)
        {
            var item = obj as TileBoard;

            if (item == null)
            {
                return false;
            }

            return this.myBoard.Equals(item.myBoard);
        }

        public void setParent(TileBoard tb)
        {
            parent = tb;
        }

        /* Manhattan distance heuristic
         * resource https://stackoverflow.com/questions/46507074/calculating-manhattan-distance-within-a-2d-array
         * https://stackoverflow.com/questions/29781359/how-to-find-manhattan-distance-in-a-continuous-two-dimensional-matrix
         */
        public int mDistance()
        {
            int distance = 0;
            int weightedValue = getNumberOfLockedCells();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    //Manhattan - as described by Wiki is calculated by distance in rows plus distance in columns
                    //In the usual case, which is to say a grid without wraparound, 
                    //we define the Manhattan distance from i, j to r, c as
                    //abs(r - i) + abs(c - j)
                    //adding 'weights' to make the system prioritize top left numbers first.
                    int itemValue = myBoard[i, j].getValue();

                    int thisDistance = 0;
                    int distanceFromX = (Math.Abs(i - xPosition[0]) + Math.Abs(j - xPosition[1]));
                    int goalI = (itemValue - 1) / 5;
                    int goalJ = (itemValue -1) % 5;

                    //Console.WriteLine(weightedValue);
                    if (itemValue == 1)
                        thisDistance = (Math.Abs(i - goalI) + Math.Abs(j - goalJ)) * distanceFromX * 100;
                    else
                    {
                        if (itemValue == 2 | itemValue == 6)
                            thisDistance = (Math.Abs(i - goalI) + Math.Abs(j - goalJ))* distanceFromX* 50;
                        else
                        {
                            if (itemValue == 3 | itemValue == 7 || itemValue == 11)
                                thisDistance = (Math.Abs(i - goalI) + Math.Abs(j - goalJ))* distanceFromX * 25;
                            else
                            {
                                if (itemValue == 4 | itemValue == 8 || itemValue == 12 || itemValue == 16)
                                    thisDistance = (Math.Abs(i - goalI) + Math.Abs(j - goalJ)) * distanceFromX * 10;
                                else
                                {
                                    if (itemValue == 5 | itemValue == 9 || itemValue == 13 || itemValue == 17 || itemValue == 20)
                                        thisDistance = (Math.Abs(i - goalI) + Math.Abs(j - goalJ)) * distanceFromX * 5;
                                    else
                                        thisDistance = (Math.Abs(i - goalI) + Math.Abs(j - goalJ)) * distanceFromX;
                                }
                            }
                        }
                    }
                    if (thisDistance ==0)
                        myBoard[i, j].rightPosition(true);
                    else
                        myBoard[i, j].rightPosition(false);
                    distance += thisDistance;
                }
            }

            return distance + weightedValue;
        }


        public int getNumberOfLockedCells()
        {
            Console.Write("Locked:");
            int value = 24;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (myBoard[i, j].isLocked())
                    {
                        Console.Write(myBoard[i, j].getValue()+" ");
                        value--;
                    }

                }
            }
            return value;
        }

        //get integer array of this board
        public TileNode[,] getBoard()
        {
            return myBoard;
        }

        public List<string> getMyMoves()
        {
            return myMoves;
        }

        public void setMyMoves(List<string> moves)
        {
            myMoves = moves;
        }

        //move the X in 'direction'
        public TileBoard moveTile(string direction)
        {
            TileNode[,] newBoard = new TileNode[5, 5];
            //copy moves and board
            for (int i = 0;i<5;i++)
            {
                for(int j =0;j<5;j++)
                {
                    newBoard[i, j] = new TileNode(myBoard[i,j].getValue(),myBoard[i,j].getMyPosition(),myBoard[i,j].getLocked());
                }
            }

            List<string> newMove = new List<string>();
            foreach (string s in myMoves)
                newMove.Add(s);

            int[] newXPos = new int[] { 0, 0 };

            // build new XPos
            switch (direction)
            {
                case "L":
                    newXPos[0] = xPosition[0];
                    newXPos[1] = xPosition[1] -1;
                    break;
                case "U":
                    newXPos[0] = xPosition[0] - 1;
                    newXPos[1] = xPosition[1];
                    break;
                case "R":
                    newXPos[0] = xPosition[0];
                    newXPos[1] = xPosition[1] + 1;
                    break;
                case "D":
                    newXPos[0] = xPosition[0] + 1;
                    newXPos[1] = xPosition[1];
                    break;
                default:
                    break;
            }
            newBoard[newXPos[0], newXPos[1]].updateValue(25);
            newBoard[xPosition[0], xPosition[1]].updateValue(myBoard[newXPos[0], newXPos[1]].getValue());
            newMove.Add(direction.ToString());

            TileBoard returnMe = new TileBoard(newBoard, myG+1, newMove,newXPos, this);

            returnMe.lockNodes();

            return returnMe;
        }

        //Lock items starting from the top left that were correctly placed
        public void lockNodes()
        {
            for (int i =0;i<5;i++)
            {
                for(int j =0;j<5;j++)
                {
                    determineLock(new int[] {i,j});
                }

            }
        }

        public bool determineLock(int[] pos)
        {
            if (pos[0] == 0 & pos[1] == 0)
            {
                myBoard[pos[0], pos[1]].checkLock();
                return myBoard[pos[0], pos[1]].isLocked();
            }
            else
            {
                if (pos[0] == 0 & pos[1] != 0)
                {
                    if (determineLock(new int[] { pos[0], pos[1] - 1 }))
                    {
                        myBoard[pos[0], pos[1]].checkLock();
                        return myBoard[pos[0], pos[1]].isLocked();
                    }
                }
                else
                {
                    if ((pos[0] != 0 & pos[1] == 0))
                    {
                        if (determineLock(new int[] { pos[0] - 1, pos[1] }))
                        {
                            myBoard[pos[0], pos[1]].checkLock();
                            return myBoard[pos[0], pos[1]].isLocked();
                        }
                    }
                    else
                    {
                        if (determineLock(new int[] { pos[0] - 1, pos[1] })& determineLock(new int[] { pos[0], pos[1]-1 }))
                        {
                            myBoard[pos[0], pos[1]].checkLock();
                            return myBoard[pos[0], pos[1]].isLocked();
                        }
                    }
                }
            }
            return false;
        }

        //returns X if last move is null
        public string getLastMove()
        {
            if(myMoves.Count!=0)
                return myMoves[myMoves.Count - 1];
            return "X";
        }

    }
}

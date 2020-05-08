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
        public int myG;
        public int myF;
        public int[] xPosition;
        public bool markForDelete = false;

        public TileBoard(TileNode[,] board, int g, List<string> moveList, int[] startPosition, TileBoard parent)
        {
            myBoard = board;
            myMoves = moveList;
            myG = g;
            myF = g + mDistance();
            xPosition = startPosition;
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
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    //Manhattan - as described by Wiki is calculated by distance in rows plus distance in columns
                    //In the usual case, which is to say a grid without wraparound, 
                    //we define the Manhattan distance from i, j to r, c as
                    //abs(r - i) + abs(c - j)
                    int thisDistance = 0;
                    int goalI = (myBoard[i,j].getValue()-1) / 5;
                    int goalJ = (myBoard[i,j].getValue()-1) % 5;
                    thisDistance = Math.Abs(i - goalI) + Math.Abs(j - goalJ);
                    if(thisDistance ==0)
                        myBoard[i, j].rightPosition(true);
                    else
                        myBoard[i, j].rightPosition(false);
                    distance += thisDistance;
                }
            }

            return distance;
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

            lockNodes(newBoard);

            return new TileBoard(newBoard, myG+1, newMove,newXPos, this);
        }

        //Lock items starting from the top left that were correctly placed
        public void lockNodes(TileNode[,] board)
        {
            for (int i =0;i<5;i++)
            {
                for(int j =0;j<5;j++)
                {
                    if(i==0&j==0)
                    {
                        board[i, j].checkLock();
                    }
                    //on the top row, we can lock only if the item to the left is locked
                    if(i==0&j!=0)
                    {
                        board[i, j - 1].checkLock();
                        if (board[i, j - 1].isLocked())
                            board[i, j].checkLock();
                        else
                            break;
                    }
                    if(i!=0)
                    {
                        //everything else we can lock if the item above it is locked
                        board[i-1, j].checkLock();
                        if (board[i-1, j].isLocked())
                            board[i, j].checkLock();
                        else
                            break;
                    }
                }

            }
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

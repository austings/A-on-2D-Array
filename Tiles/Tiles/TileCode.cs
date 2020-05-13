using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Threading;

namespace Tiles
{
    class TileCode
    {
        private List<TileBoard> map;
        private List<TileBoard> scannedItems;
        private int[] position;//position of the 'X'

        static void Main(string[] args)
        {
            TileCode runMe = new TileCode();

            runMe.run();

        }

        public void run()
        {
            //Establish Driver
            IWebDriver driver = new ChromeDriver("C:\\Users\\Austin\\source\\repos\\ConsoleApp1\\ConsoleApp1\\bin\\Debug");
            driver.Navigate().GoToUrl("https://hack.ainfosec.com/");
            Actions action = new Actions(driver);
            position = new int[] { 0, 0 };
            map = new List<TileBoard>();
            scannedItems = new List<TileBoard>();

            //try to enter a previously given ID number
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(25);
            IWebElement idBox = driver.FindElement(By.XPath("/html/body/site-nav/div/nav/div/div/ul/li[2]/div/div/button"));
            idBox.Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            idBox = driver.FindElement(By.XPath("/html/body/site-help-modal/div/div/div/div[2]/resume-session-form/div/input"));
            System.Threading.Thread.Sleep(1000);
            idBox.SendKeys("80d4a8eb-c553-4c47-9058-4bed9b594976");
            System.Threading.Thread.Sleep(1000);
            driver.FindElement(By.XPath("/html/body/site-help-modal/div/div/div/div[2]/resume-session-form/button")).Click();
            System.Threading.Thread.Sleep(5000);//refresh page
            driver.Navigate().Refresh();
            

            //navigate to the tile exercise
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(25);
            IWebElement tileIcon = driver.FindElement(By.XPath("//challenge-list[@id='hackerchallenge-challenge-list']/div[3]/div/challenge-card[3]/div/div/div/div[2]/div/button"));
            action.MoveToElement(tileIcon).Perform();
            tileIcon.Click();
            System.Threading.Thread.Sleep(250);

            //retrieve the current board
            IWebElement text = driver.FindElement(By.XPath("/html/body/div[3]/div[4]/challenge-list/div[3]/div/challenge-card[3]/challenge-modal/div/div/div/div[2]/div[1]/p[3]"));
            string boardTextForm = text.Text.ToString();
            TileNode[,] startBoard = new TileNode[5, 5];
            Regex rx = new Regex(@"\d+|X");
            MatchCollection m = rx.Matches(boardTextForm);
            int i = 0;
            int j = 0;
            foreach (Match match in m)
            {
                if (match.Value != "X")
                {
                    startBoard[i, j] = new TileNode(Int32.Parse(match.Value),new int[] { i,j},false);

                }
                else
                {
                    startBoard[i, j] = new TileNode(25, new int[] { i, j },false); 
                    position = new int[] { i, j };

                }

                j++;
                if (j > 4)
                {
                    i++;
                    j = 0;
                }

            }

            //save the board we copied
            TileBoard start = new TileBoard(startBoard, 0, new List<string>(),position, null);
            map.Add(start);
            PrintBoard(start);

            TileBoard solution = A_star();
            foreach (string move in solution.getMyMoves())
                Console.Write(move);


                //enter the correct combination of steps
                do
            {
                try
                {

                    //Click into the input field
                    foreach (string move in solution.getMyMoves())
                    {
                        IWebElement inputBox = driver.FindElement(By.XPath("/html/body/div[3]/div[4]/challenge-list/div[3]/div/challenge-card[3]/challenge-modal/div/div/div/div[2]/div[2]/div/input"));
                        IWebElement submit = driver.FindElement(By.XPath("/html/body/div[3]/div[4]/challenge-list/div[3]/div/challenge-card[3]/challenge-modal/div/div/div/div[3]/button"));

                        inputBox.Clear();
                        inputBox.SendKeys(move);
                        submit.Click();
                        System.Threading.Thread.Sleep(500);
                    }



                }
                catch (StaleElementReferenceException e)
                {
                    Console.WriteLine("Stale Element.. Retrying");
                }
                catch (ElementNotInteractableException e)
                {
                    Console.WriteLine("Element Not Interactable.. Retrying");
                }
                catch (WebDriverException e)
                {
                    Console.WriteLine("WebDriverException.. Retrying");
                }

            }
            while (1 < 2);
        }

        public TileBoard A_star()
        {

            TileBoard goalBoard = null;

            while (goalBoard == null)//run until we find a solution
            {
                //get the board with the smallest f //maybe try 
                TileBoard currentBoard = map[0];
                int index = 0;
                for (int i = 0; i < map.Count; i++)
                {
                    if (map[i].myF < currentBoard.myF)
                    {
                        currentBoard = map[i];
                        index = i;
                    }
                }
                int mDis = currentBoard.mDis;
                PrintBoard(currentBoard);
                //System.Threading.Thread.Sleep(50);
                Console.WriteLine(mDis);
                //if this is the goalBoard, save it
                if (mDis == 0)
                {
                    goalBoard = currentBoard;
                    break;
                }



                //for each neighbor of current
                map.RemoveAt(index);

                List<TileBoard> neighbors = getNeighbors(currentBoard);
                foreach(TileBoard n in neighbors)
                {

                    //int lowestF = 1000;
                    //if (n.myF < lowestF)//try and speed up by looking at the best neighbors only
                    //{
                    //lowestF = n.myF;
                    bool inOpenList = false;
                        foreach (TileBoard open in map)
                        {
                            if (compareBoards(open, n))
                            {
                                inOpenList = true;
                                break;
                            }
                        }

                        if (inOpenList)
                        {
                            foreach (TileBoard o in map)
                            {
                                if (n.myG < o.myG & compareBoards(n, o))
                                {
                                    o.setParent(n.getParent());
                                    o.setMyMoves(n.getMyMoves());
                                    o.myF = n.myF;
                                    o.myG = n.myG;
                                }
                            }

                        }
                        else
                        {
                            bool inClosedList = false;
                            foreach (TileBoard closed in scannedItems)
                            {
                                if (compareBoards(closed, n))
                                {
                                    inClosedList = true;
                                    break;

                                }
                            }
                            if (!inClosedList)
                                map.Add(n);
                        }
                }
                scannedItems.Add(currentBoard);

    
            }

            return goalBoard;
        }

        public bool compareBoards(TileBoard a, TileBoard b)
        {
            //could use a search algorithm here to speed it up?
            bool match = true;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (a.getBoard()[i, j].getValue() != b.getBoard()[i, j].getValue())
                    {
                        match = false;
                        break;
                    }
                }
                if (!match)
                    break;
            }

            return match;
        }

        public List<TileBoard> getNeighbors(TileBoard board)
        {
            List<TileBoard> neighbors = new List<TileBoard>();
            string lastMove = board.getLastMove();//prevent moving back and forth

            if(lastMove != "U"&board.xPosition[0]!=4)
            {
                neighbors.Add(board.moveTile("D"));
            }

            if (lastMove != "L" & board.xPosition[1] != 4)
            {
                neighbors.Add(board.moveTile("R"));
            }
            int posx = board.xPosition[1];
            int posy = board.xPosition[0];

            if (lastMove != "D" & board.xPosition[0] != 0)
            {
                TileNode node = board.getBoard()[posx, posy-1];
                if(!node.isLocked())
                    neighbors.Add(board.moveTile("U"));
            }

            if(lastMove != "R" & board.xPosition[1] != 0)
            {
                TileNode node = board.getBoard()[posx -1, posy];
                if (!node.isLocked())
                    neighbors.Add(board.moveTile("L"));
            }

            return neighbors;
        }

        public void PrintBoard(TileBoard b)
        {

            //Print the board state out
            Console.WriteLine("Board State:");
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {

                    Console.Write(b.getBoard()[i, j].getValue() + " ");
                }
                Console.Write("\n");
            }
        }

    }

    
}

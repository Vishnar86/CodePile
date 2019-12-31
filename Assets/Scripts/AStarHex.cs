using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarHex : MonoBehaviour
{
    public InputController inputs; // user input script
    public GameObject HexCell;    // object to use for the hexcell
    public GameObject SqrCell;     // object to use for the square cell
    public Vector2 GridSize = new Vector2(5, 5);  //default grid size
    public int ObstructionChance = 20;  // % chance a cell is an obstacle
    public GameObject MapHolder;  // map container object

    public bool isDiaganal;  // allow diaganol movement on  asquare gride or disallow it
    public bool isSqr;       // is the grid a square grid?

    public float mS;  // place holder for the stop watch

    public Cell[] Grid;  // holds the cells of the grid.

    public void GenHex()
    {
        StartCoroutine("GenerateHex");
    }

    
    // create a hex grid
    public IEnumerator GenerateHex()
    {
        //destroy previous grid before creating a new one if one existed prior.
        if(Grid.Length > 0)
        {
            foreach(Cell hex in Grid)
            {
                Destroy(hex.gameObject);
            }
        }
        
        List<Cell> grid = new List<Cell>();

        float t = GridSize.x * GridSize.y;
        float p = 0;

        int l = 0;

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                GameObject obj = Instantiate(HexCell, MapHolder.transform.position, Quaternion.identity, MapHolder.transform);
                Cell h = obj.GetComponent<Cell>();

                h.MoveCost = 1;
                h.GridCord = new Vector2(x + 1, y + 1);
                h.GridIndex = grid.Count;
                obj.name = h.GridCord.ToString();

                //find even and odd cells to position them appropriatly for the hex grid
                if (x % 2 == 0)
                {
                    obj.transform.position = new Vector3(x * .6f + 1, y * .64f + 1, 0);
                }
                else
                {
                    obj.transform.position = new Vector3(x * .6f + 1, (y * .64f) - .32f + 1, 0);
                }




                /// do not touch
                #region Neighbor maths

                bool isTop = false;
                bool isBottom = false;
                bool isLeft = false;
                bool isRight = false;

                if (h.GridCord.x == 1)
                {
                    isLeft = true;
                }
                else if (h.GridCord.x == GridSize.x)
                {
                    isRight = true;
                }

                if (h.GridCord.y == 1)
                {
                    isBottom = true;
                }
                else if (h.GridCord.y == GridSize.y)
                {
                    isTop = true;
                }

                int gridY = Mathf.RoundToInt(GridSize.y);

                int first = h.GridIndex + 1;
                int second = h.GridIndex + gridY;
                int third = h.GridIndex + gridY - 1;
                int fourth = h.GridIndex - 1;
                int fifth = h.GridIndex - gridY - 1;
                int sixth = h.GridIndex - gridY;

                if (h.GridCord.x % 2 == 0)
                {
                    first = h.GridIndex + 1;
                    second = h.GridIndex + gridY;
                    third = h.GridIndex + gridY - 1;
                    fourth = h.GridIndex - 1;
                    fifth = h.GridIndex - gridY - 1;
                    sixth = h.GridIndex - gridY;
                }
                else
                {
                    first = h.GridIndex + 1;
                    second = h.GridIndex + gridY + 1;
                    third = h.GridIndex + gridY;
                    fourth = h.GridIndex - 1;
                    fifth = h.GridIndex - gridY;
                    sixth = h.GridIndex - gridY + 1;
                }

                if (isTop && isLeft)
                {
                    h.Neighbors = new int[]
                    {
                        third,
                        fourth
                    };
                }
                else if (isTop && isRight)
                {
                    if (GridSize.y % 2 == 0)
                    {
                        h.Neighbors = new int[]
                        {
                        fourth,
                        fifth,
                        sixth
                        };
                    }
                    else
                    {
                        h.Neighbors = new int[]
                        {
                        fourth,
                        fifth
                        };
                    }
                }
                else if (isBottom && isLeft)
                {
                    h.Neighbors = new int[]
                    {
                        first,
                        second,
                        third
                    };
                }
                else if (isBottom && isRight)
                {
                    if (GridSize.y % 2 == 0)
                    {
                        h.Neighbors = new int[]
                        {
                        first,
                        sixth
                        };
                    }
                    else
                    {
                        h.Neighbors = new int[]
                        {
                        first,
                        fifth,
                        sixth
                        };
                    }
                }
                else if (isTop)
                {
                    if (h.GridCord.x % 2 == 0)
                    {
                        h.Neighbors = new int[]
                        {
                        second,
                        third,
                        fourth,
                        fifth,
                        sixth
                        };
                    }
                    else
                    {
                        h.Neighbors = new int[]
                        {
                        third,
                        fourth,
                        fifth
                        };
                    }
                }
                else if (isBottom)
                {
                    if (h.GridCord.x % 2 == 0)
                    {
                        h.Neighbors = new int[]
                        {
                        first,
                        second,
                        sixth
                        };
                    }
                    else
                    {
                        h.Neighbors = new int[]
                        {
                        first,
                        second,
                        third,
                        fifth,
                        sixth
                        };
                    }

                }
                else if (isLeft)
                {
                    h.Neighbors = new int[]
                    {
                        first,
                        second,
                        third,
                        fourth
                    };
                }
                else if (isRight)
                {
                    h.Neighbors = new int[]
                    {
                        first,
                        fourth,
                        fifth,
                        sixth
                    };
                }
                else
                {
                    h.Neighbors = new int[]
                    {
                        first,
                        second,
                        third,
                        fourth,
                        fifth,
                        sixth
                    };
                }
                grid.Add(h);
                #endregion    // do not touch, defines hex neighbors.
                /// do not touch

                l++;  // loop counter
                p++;  // total cells created so far

                if (l > 500)
                {
                    yield return new WaitForEndOfFrame();
                    inputs.Progress = Mathf.RoundToInt((p / t) * 100);
                    l = 0;
                }
            }
        }

        Grid = grid.ToArray();

        yield return new WaitForEndOfFrame();

        inputs.Progress = 100;

        GenerateObstructions();
    }

    // call this to start the coroutine for a square grid
    public void GenSqr()
    {
        StartCoroutine("GenerateSqr");
    }

    // create a square grid
    public IEnumerator GenerateSqr()
    {

        // remove current grid if one exists before generating anew one
        if (Grid.Length > 0)
        {
            foreach (Cell hex in Grid)
            {
                Destroy(hex.gameObject);
            }
        }
        List<Cell> grid = new List<Cell>();

        float t = GridSize.x * GridSize.y;
        float p = 0;

        int l = 0;

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                GameObject obj = Instantiate(SqrCell, MapHolder.transform.position, Quaternion.identity, MapHolder.transform);
                Cell h = obj.GetComponent<Cell>();

                h.MoveCost = 1;
                h.GridCord = new Vector2(x + 1, y + 1);
                h.GridIndex = grid.Count;
                obj.name = h.GridCord.ToString();

                obj.transform.position = new Vector3(x * .65f + 1, y * .65f + 1, 0);

                h.Neighbors = GetNeighborsSqr(h);
                
                grid.Add(h);


                //// used for calculating % of generation compelete
                l++;
                p++;

                if (l > 500)
                {
                    yield return new WaitForEndOfFrame();
                    inputs.Progress = Mathf.RoundToInt((p / t) * 100);
                    l = 0;
                }
                ////
            }
        }

        Grid = grid.ToArray();

        yield return new WaitForEndOfFrame();

        inputs.Progress = 100;

        GenerateObstructions();
    }

    // denote some cells as unwalkable
    public void GenerateObstructions()
    {
        foreach(Cell hex in Grid)
        {
            int roll = Random.Range(1, 100);
           
            if (roll < ObstructionChance)
            {
                hex.GetComponentInChildren<SpriteRenderer>().color = Color.black;
                hex.IsWalkable = false;
            }
        }
    }


    public void AssignNeighbors()
    {
        foreach(Cell cell in Grid)
        {
            cell.Neighbors = GetNeighborsSqr(cell);
        }
    }

    private int[] GetNeighborsSqr(Cell h)
    {
            bool isTop = false;
            bool isBottom = false;
            bool isLeft = false;
            bool isRight = false;

            if (h.GridCord.x == 1)
            {
                isLeft = true;
            }
            else if (h.GridCord.x == GridSize.x)
            {
                isRight = true;
            }

            if (h.GridCord.y == 1)
            {
                isBottom = true;
            }
            else if (h.GridCord.y == GridSize.y)
            {
                isTop = true;
            }
            int gridY = Mathf.RoundToInt(GridSize.y);

            int first = h.GridIndex + 1;
            int second = h.GridIndex - 1;
            int third = h.GridIndex + gridY;
            int fourth = h.GridIndex - gridY;

            int fifth = h.GridIndex + gridY + 1;
            int sixth = h.GridIndex + gridY - 1;
            int seventh = h.GridIndex - gridY + 1;
            int eighth = h.GridIndex - gridY - 1;

            if (isDiaganal)
            {
                if (isTop && isLeft)
                {
                return new int[]
                    {
                            second,
                            third,
                            sixth
                    };
                }
                else if (isTop && isRight)
                {
                return new int[]
                    {
                            second,
                            fourth,
                            eighth
                    };
                }
                else if (isBottom && isLeft)
                {
                return new int[]
                    {
                            first,
                            third,
                            fifth
                    };
                }
                else if (isBottom && isRight)
                {
                return new int[]
                    {
                            first,
                            fourth,
                            seventh
                    };
                }
                else if (isTop)
                {
                return new int[]
                    {
                            second,
                            third,
                            fourth,
                            sixth,
                            eighth
                    };
                }
                else if (isBottom)
                {
                return new int[]
                    {
                            first,
                            third,
                            fourth,
                            fifth,
                            sixth
                    };

                }
                else if (isLeft)
                {
                return new int[]
                    {
                            first,
                            second,
                            third,
                            fifth,
                            sixth
                    };
                }
                else if (isRight)
                {
                return new int[]
                    {
                            first,
                            second,
                            fourth,
                            seventh,
                            eighth
                    };
                }
                else
                {
                return new int[]
                    {
                            first,
                            second,
                            third,
                            fourth,
                            fifth,
                            sixth,
                            seventh,
                            eighth
                    };
                }
            }
            else
            {
                if (isTop && isLeft)
                {
                return new int[]
                    {
                            second,
                            third
                    };
                }
                else if (isTop && isRight)
                {
                return new int[]
                    {
                            second,
                            fourth
                    };
                }
                else if (isBottom && isLeft)
                {
                return new int[]
                    {
                            first,
                            third
                    };
                }
                else if (isBottom && isRight)
                {
                return new int[]
                    {
                            first,
                            fourth
                    };
                }
                else if (isTop)
                {
                return new int[]
                    {
                            second,
                            third,
                            fourth
                    };
                }
                else if (isBottom)
                {
                return new int[]
                    {
                            first,
                            third,
                            fourth
                    };

                }
                else if (isLeft)
                {
                return new int[]
                    {
                            first,
                            second,
                            third
                    };
                }
                else if (isRight)
                {
                return new int[]
                    {
                            first,
                            second,
                            fourth
                    };
                }
                else
                {
                    return new int[]
                    {
                            first,
                            second,
                            third,
                            fourth
                    };
                }
            }
    }

    // meat and potatoes calculate the path.
    public int[] GetPath(Vector2 start, Vector2 end)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        int maxLoop = 100000;
        int loop = 0;

        List<int> path = new List<int>();

        Dictionary<int,int> Open = new Dictionary<int, int>();  // open list to hold searchable nodes from the grid
        HashSet<int> Closed = new HashSet<int>();               // closed list of nodes that have been searched and should hnot be rechecked.
        HashSet<int> Used = new HashSet<int>();                 // if cell has been on either open or closed they get dumped here(this is to save on calls of haset<>.contains(X). thus calling it only once per itteration rather than twice.

        // quick math to identify the index of the start and end nodes using there vector location.
        int S = Mathf.RoundToInt(((start.x - 1) * GridSize.y) + start.y) - 1;
        int E = Mathf.RoundToInt(((end.x - 1) * GridSize.y) + end.y) - 1;
        int cur = E;  // the node we will check for neighbors of

        Closed.Add(E);  // this node should only be checked this one time, placed in closed
        Used.Add(E);    // this node has been used and thus should not be checked for having any neighbors.

        // check to see if point is reachable or dead ends //
        bool sReachable = false;  
        bool eReachable = false;

        foreach(int i in Grid[E].Neighbors)
        {
            if(Grid[i].IsWalkable)
            {
                eReachable = true;
                break;
            }
        }

        foreach (int i in Grid[S].Neighbors)
        {
            if (Grid[i].IsWalkable)
            {
                sReachable = true;
                break;
            }
        }
        
        if (!Grid[S].IsWalkable || !Grid[E].IsWalkable || !sReachable || !eReachable)
        {
            return new int[] { 1,1 };
        }
        /////////////////////////////////////////////////////////////////////////


        // main loop to iterate through till apath is found
        while (cur != S && loop < maxLoop)
        {
            //  extra check to confirm we reached the goal or not. (redundant, will remove later)
            if (cur == S)
            {
                break;
            }
            float distCostA = Vector2.Distance(Grid[S].gameObject.transform.position, Grid[cur].gameObject.transform.position) * 2;  // distance from start to currently checked node
            float distCostB = Vector2.Distance(Grid[E].gameObject.transform.position, Grid[cur].gameObject.transform.position);  // distance from end  to currently checked node

            Grid[cur].Cost = (distCostB + Grid[cur].MoveCost) + distCostA + Grid[cur].Cost; //assign the cost for the current node thus far 

            // check through the array of neighbors in the cell.Neighbors variable that was assigned in generation, and add them to the open list
            foreach (int index in Grid[cur].Neighbors)
            {
                float cost = (distCostB + Grid[index].MoveCost) + distCostA + Grid[cur].Cost;  // assign the step cost to the neighbor

                if ((!Used.Contains(index) && Grid[index].IsWalkable))
                {
                    Grid[index].Cost = cost;

                    Grid[index].Previous = cur;  // basicly a pointer saying "we came from taht direction and should go back that way to establish the path"
                    Open.Add(index, index);
                    Used.Add(index);
                }
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////

            // find the neighbor of the lowest cost, and make them the current node to be checked for the path.
            int lowest = cur;

            foreach (KeyValuePair<int, int> i in Open)
            {
                if (Grid[lowest].Cost > Grid[i.Value].Cost)
                {
                    lowest = i.Value;
                }
            }

            Open.Remove(lowest);
            Closed.Add(lowest);
            cur = lowest;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////

            loop++;
        }
        ///////////////////////////////////////////////////

        if (loop >= maxLoop)
        {
            return new int[] { 1, 1 };
        }

        // finish up by fliping the closed list and following the "previous" pointers to find a path back.
        while (!path.Contains(E))
        {
            path.Add(cur);
            cur = Grid[cur].Previous;
        }
        watch.Stop();

        mS = watch.ElapsedMilliseconds;
        //////////////////////////////////////////////////////////////////////////////////////////////

        return path.ToArray();  // return array of indexes to be used to mark/travel the path.
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    public AStarHex Pathing;
    public Camera Main;
    public float Speed;
    public bool isSquare = true;
    public bool IsSqr
    {
        set
        {
            isSquare = value;
            Pathing.isSqr = value;
        }
        get { return isSquare; }
    }
    public bool isDiaganal = true;
    public bool IsDiag
    {
        set
        {
            isDiaganal = value;
            Pathing.isDiaganal = value;
            Pathing.AssignNeighbors();
        }
        get
        {
            return isDiaganal;
        }
    }
    public InputField InputX;
    public InputField InputY;

    public Text Time;
    public Text Length;
    public Text Loading;

    public Image Blocker;

    public int Progress;


    private float ConX
    {
        get { return Pathing.GridSize.x * .6f + 1; }
    }

    private float ConY
    {
        get { return Pathing.GridSize.y * .6f + 1; }
    }

    private Cell PointA;
    private Cell PointB;
    private int[] path;

	
	void Start ()
    {
        InputY.text = Pathing.GridSize.y.ToString();
        InputX.text = Pathing.GridSize.x.ToString();

        Generate();
	}
	
	void Update ()
    {
        if (!IsPointerOverUIElement())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cell h = ClickSelect();

                if (h != null)
                {
                    if (PointA == null)
                    {
                        PointA = h;
                        PointA.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                        //Pathing.HexGrid[PointA.GridIndex].gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                    }
                    else if (PointA.GridCord == h.GridCord)
                    {
                        //PointA.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                        PointA = null;
                        if (PointB != null)
                        {
                            if (PointB.GetComponent<Cell>().IsWalkable)
                            {
                                PointB.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                            }
                            else
                            {
                                PointB.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.black;
                            }
                            PointB = null;
                        }
                        ClearPath();
                    }
                    else
                    {
                        ClearPath();
                        PointA.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                        if (PointB != null)
                        {
                            if (PointB.GetComponent<Cell>().IsWalkable)
                            {
                                PointB.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                            }
                            else
                            {
                                PointB.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.black;
                            }
                        }
                        PointB = h;
                        //PointB.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                        path = Pathing.GetPath(PointA.GridCord, PointB.GridCord);
                        PaintPath();

                        Time.text = "ms: " + Pathing.mS.ToString();
                        Length.text = "Path Length: " + path.Length.ToString();
                    }
                }
                //Debug.Log(PointA.GridCord + " " + PointB.GridCord);

            }
            else if (Input.GetMouseButtonDown(1))
            {
                Cell h = ClickSelect();

                if (h != null)
                {
                    SpriteRenderer sr = h.GetComponentInChildren<SpriteRenderer>();
                    if (h.IsWalkable)
                    {
                        h.IsWalkable = false;
                        sr.color = Color.black;
                    }
                    else
                    {
                        h.IsWalkable = true;
                        sr.color = Color.white;
                    }
                }
            }
        }

        if (Progress > 0)
        {
            Blocker.gameObject.SetActive(true);
            
            Loading.text = "Loading: " + Progress + "%";

            if (Progress == 100)
            {
                Loading.text = "";
                Progress = 0;
                Blocker.gameObject.SetActive(false);
            }
        }

        float H = Input.GetAxisRaw("Horizontal");
        float V = Input.GetAxisRaw("Vertical");

        Main.transform.position += new Vector3(H, V, 0f) * UnityEngine.Time.deltaTime * Speed;

        Vector3 main = Main.transform.position;

        if (main.x > ConX)
        {
            Main.transform.position = new Vector3(ConX, main.y, main.z);
        }
        else if (main.x < 1)
        {
            Main.transform.position = new Vector3(1, main.y, main.z);
        }
        else if (main.y > ConY)
        {
            Main.transform.position = new Vector3(main.x, ConY, main.z);
        }
        else if (main.y < 1)
        {
            Main.transform.position = new Vector3(main.x, 1, main.z);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void NewSize()
    {
        Pathing.GridSize = new Vector2(int.Parse(InputX.text), int.Parse(InputY.text));

        ClearPath();
        PointA = null;
        PointB = null;
    }

    public void Generate()
    {
        Pathing.isDiaganal = IsDiag;

        if (IsSqr)
        {
            Pathing.GenSqr();
        }
        else
        {
            Pathing.GenHex();
        }
    }

    public Cell ClickSelect()
    {
        //Converting Mouse Pos to 2D (vector2) World Pos
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);
        
        if (hit.collider != null && hit.collider.GetComponent<Cell>() != null)
        {
            return hit.transform.gameObject.GetComponent<Cell>();
        }
        else return null;
    }

    private void PaintPath()
    {
        if(path[0] == path[1])
        { return; }
        if (path != null)
        {
            foreach (int i in path)
            {
                Pathing.Grid[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }
        }
    }

    private void ClearPath()
    {
        if (path != null)
        {
            foreach (int i in path)
            {
                if (Pathing.Grid[i].IsWalkable)
                {
                    Pathing.Grid[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                }
                else
                {
                    Pathing.Grid[i].gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.black;
                }
            }
        }

        path = null;
    }

    public static bool IsPointerOverUIElement()
    {
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}

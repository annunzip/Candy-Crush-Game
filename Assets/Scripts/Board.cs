using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Board : MonoBehaviour {
    public List<Candy> candy = new List<Candy>();
    public int GridWidth;
    public int GridHeight;
    public GameObject candyPrefab;
    public Candy lastCandy;
    public Vector3 candy1Start, candy1End, candy2Start, candy2End;
    public bool isSwapping = false;
    public Candy candy1, candy2;
    public float startTime;
    public float SwapRate = 2;
    public int AmountToMatch = 3;
    public bool isMatched = false;
    public bool swapBack = false;
    public GUIText scoreText;
    public int score;

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }
    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }


    // Use this for initialization
    void Start ()
    {
        score = 0;
        UpdateScore();
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                GameObject g = Instantiate(candyPrefab,new Vector3(x,y,0), Quaternion.identity) as GameObject;
                g.transform.parent = gameObject.transform;
                candy.Add(g.GetComponent<Candy>());   
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isMatched)
        {
            for (int i = 0; i < candy.Count; i++)
            {
                if (candy[i].isMatched)
                {
                    candy[i].CreateCandy();
                    candy[i].transform.position = new Vector3(
                        candy[i].transform.position.x,
                        candy[i].transform.position.y + 7,
                        candy[i].transform.position.z);
                }
            }
            isMatched = false;
        }
        else if (isSwapping)
        {
            MoveCandy(candy1, candy1End, candy1Start);
            MoveNegCandy(candy2, candy2End, candy2Start);
            if ((Vector3.Distance(candy1.transform.position, candy1End) < .1f) || (Vector3.Distance(candy2.transform.position, candy2End) < .1f))
            {
                candy1.transform.position = candy1End;
                candy2.transform.position = candy2End;

                lastCandy = null;
                isSwapping = false;
                TogglePhysics(false);
                if (!swapBack)
                {
                    candy1.ToggleSelector();
                    candy2.ToggleSelector();
                    CheckMatch();
                }
                else
                {
                    swapBack = false;
                }
            }
        }
        else if (!DetermineBoardState())
        {
            for (int i = 0; i < candy.Count; i++)
            {
                CheckForNearbyMatches(candy[i]);
            }
            if (!DoesBoardContainMatches())
            {
                isMatched = true;
                for (int i = 0; i < candy.Count; i++)
                {
                    candy[i].isMatched = true;
                }

            }
        }
	}
    public bool DetermineBoardState()
    {
        for (int i = 0; i < candy.Count; i++)
        {
            if (candy[i].transform.localPosition.y > 4)
            {
                return true;
            }
            else if(candy[i].GetComponent<Rigidbody>().velocity.y > .1f)
            {
                return true; 
            }
        }
        return false; 
    }
    public void CheckMatch()
    {
        List<Candy> candy1List = new List<Candy>();
        List<Candy> candy2List = new List<Candy>();
        ConstructMatchList(candy1.color, candy1, candy1.XCoord, candy1.YCoord, ref candy1List);
        FixMatchList(candy1, candy1List);
        ConstructMatchList(candy2.color, candy2, candy2.XCoord, candy2.YCoord, ref candy2List);
        FixMatchList(candy2, candy2List);
        if (!isMatched)
        {
            swapBack = true;
            ResetGems();
        }
        

    }

    public void ResetGems()
    {
        candy1Start = candy1.transform.position;
        candy1End = candy2.transform.position;

        candy2Start = candy2.transform.position;
        candy2End = candy1.transform.position;

        startTime = Time.time;
        TogglePhysics(true);
        isSwapping = true;
    }

    public void CheckForNearbyMatches(Candy g)
    {
        List<Candy> candyList = new List<Candy>();
        ConstructMatchList(g.color, g, g.XCoord, g.YCoord, ref candyList);
        FixMatchList(g, candyList);
    }
    public void ConstructMatchList(string color, Candy candy, int XCoord, int YCoord, ref List<Candy> MatchList)
    {
        if (candy == null)
        {
            return;
        }
        else if (candy.color != color)
        {
            return;
        }
        else if (MatchList.Contains(candy))
        {
            return;
        }
        else
        {
            MatchList.Add(candy);
            if (XCoord == candy.XCoord || YCoord == candy.YCoord)
            {
                foreach (Candy g in candy.Neighbors)
                {
                    ConstructMatchList(color, g, XCoord, YCoord, ref MatchList);
                }
            }
        }
    }
    public bool DoesBoardContainMatches()
    {
        TogglePhysics(true);
        for (int i = 0; i < candy.Count; i++)
        {
            for (int j = 0; j < candy.Count; j++)
            {
                if (candy[i].IsNeighborWith(candy[j]))
                {
                    Candy g = candy[i];
                    Candy f = candy[j];
                    Vector3 GTemp = g.transform.position;
                    Vector3 FTemp = f.transform.position;
                    List<Candy> tempNeighbors = new List<Candy>(g.Neighbors);
                    g.transform.position = FTemp;
                    f.transform.position = GTemp;
                    g.Neighbors = f.Neighbors;
                    f.Neighbors = tempNeighbors;
                    List<Candy> testListG = new List<Candy>();
                    ConstructMatchList(g.color, g, g.XCoord, g.YCoord, ref testListG);

                    if (TestMatchList(g, testListG))
                    {
                        g.transform.position = GTemp;
                        f.transform.position = FTemp;
                        f.Neighbors = g.Neighbors;
                        g.Neighbors = tempNeighbors;
                        TogglePhysics(false); 
                        return true;
                    }
                    List<Candy> testListF = new List<Candy>();
                    ConstructMatchList(f.color, f, f.XCoord, f.YCoord, ref testListF);
                    if (TestMatchList(f, testListF))
                    {
                        g.transform.position = GTemp;
                        f.transform.position = FTemp;
                        f.Neighbors = g.Neighbors;
                        g.Neighbors = tempNeighbors;
                        TogglePhysics(false);
                        return true; 
                    }

                    g.transform.position = GTemp;
                    f.transform.position = FTemp;
                    f.Neighbors = g.Neighbors;
                    g.Neighbors = tempNeighbors;
                    TogglePhysics(false);
                }
            }
        }
        return false; 
    }
    public void FixMatchList(Candy candy, List<Candy> ListToFix)
    {
        List<Candy> rows = new List<Candy>();
        List<Candy> cols = new List<Candy>();

        for (int i = 0; i < ListToFix.Count; i++)
        {
            if (candy.XCoord == ListToFix[i].XCoord)
            {
                rows.Add(ListToFix[i]);
            }
            if (candy.YCoord == ListToFix[i].YCoord)
            {
                cols.Add(ListToFix[i]);
            }
        }
        if (rows.Count >= AmountToMatch)
        {
            GameObject.Find("Game").GetComponent<UI>().AddScore(1);
            isMatched = true;
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].isMatched = true;
            }  
        }
        if (cols.Count >= AmountToMatch)
        {
            GameObject.Find("Game").GetComponent<UI>().AddScore(1);
            isMatched = true;
            for (int i = 0; i < cols.Count; i++)
            {
                cols[i].isMatched = true;
            }
        }
    }

    public bool TestMatchList(Candy candy, List<Candy> ListToFix)
    {
        List<Candy> rows = new List<Candy>();
        List<Candy> cols = new List<Candy>();

        for (int i = 0; i < ListToFix.Count; i++)
        {
            if (candy.XCoord == ListToFix[i].XCoord)
            {
                rows.Add(ListToFix[i]);
            } 
            if(candy.YCoord == ListToFix[i].YCoord)
            {
                cols.Add(ListToFix[i]); 
            }
        }
        if (rows.Count >= AmountToMatch)
        {
            return true;   
        }
        if(cols.Count >= AmountToMatch)
        {
            return true; 
           }
        return false; 
        }

    public void MoveCandy(Candy candyToMove,Vector3 toPos, Vector3 fromPos)
    {
        Vector3 center = (fromPos + toPos) * .5f;
        center -= new Vector3(0, 0, .1f);
        Vector3 riseRelCenter = fromPos - center;   
        Vector3 setRelCenter = toPos - center;
        float fracComplete = (Time.time - startTime) / SwapRate;
        candyToMove.transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        candyToMove.transform.position += center; 
    }
    public void MoveNegCandy(Candy candyToMove, Vector3 toPos, Vector3 fromPos)
    {
        Vector3 center = (fromPos - toPos) * .5f;
        center -= new Vector3(0, 0, -.1f);
        Vector3 riseRelCenter = fromPos - center;
        Vector3 setRelCenter = toPos - center;
        float fracComplete = (Time.time - startTime) / SwapRate;
        candyToMove.transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        candyToMove.transform.position += center;
    }
    public void TogglePhysics(bool isOn)
    {
        for (int i = 0; i < candy.Count; i++)
        {
            candy[i].GetComponent<Rigidbody>().isKinematic = isOn;  
        }
    }
    public void SwapCandy(Candy currentCandy)
    {
        if (lastCandy == null)
        {
            lastCandy = currentCandy;
        }
        else if (lastCandy == currentCandy)
        {
            lastCandy = null;
        }
        else
        {
            if(lastCandy.IsNeighborWith(currentCandy))
            {
                candy1Start = lastCandy.transform.position;
                candy1End = currentCandy.transform.position;

                candy2Start = currentCandy.transform.position;
                candy2End = lastCandy.transform.position;

                startTime = Time.time;
                TogglePhysics(true);
                candy1 = lastCandy;
                candy2 = currentCandy;
                isSwapping = true;
            }
            else
            {
                lastCandy.ToggleSelector();
                lastCandy = currentCandy;
             
            }
        }
    }
}

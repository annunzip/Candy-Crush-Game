using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Candy : MonoBehaviour {

    public GameObject sphere;
    public GameObject selector;
    public bool isMatched = false; 
    public bool isSelected = false; 
    string[] candyMats = { "Black", "Blue", "Green", "Purple", "Yellow", "Orange", "Red" };
    public string color = ""; 
    public List<Candy> Neighbors = new List<Candy>();

    public int XCoord
    {
        get
        {
            return Mathf.RoundToInt(transform.localPosition.x);
        }
    }
    public int YCoord
    {
        get
        {
            return Mathf.RoundToInt(transform.localPosition.y);
        }
    }
    // Use this for initialization
    void Start() {
        CreateCandy();
    }
	// Update is called once per frame
	void Update () {
	
	}

    
    public void ToggleSelector()
    {
        isSelected = !isSelected;
        selector.SetActive(isSelected); 
    }
    public void CreateCandy()
    {
        color = candyMats[Random.Range(0, candyMats.Length)];
        Material m = Resources.Load("Materials/" + color) as Material;
        sphere.GetComponent<Renderer>().material = m;
        isMatched = false; 
    }
    public void AddNeighbor(Candy g)
    {
        if (!Neighbors.Contains(g))
        {
            Neighbors.Add(g);
        }
    }

    public bool IsNeighborWith(Candy g)
    {
        if(Neighbors.Contains(g))
        {
            return true;
        }
        return false;
    }
    public void RemoveNeighbor(Candy g)
    {
        Neighbors.Remove(g);
    }
    void OnMouseDown()
    {
        if (GameObject.Find("Board").GetComponent<Board>().DetermineBoardState()) return; 
            if (!GameObject.Find("Board").GetComponent<Board>().isSwapping)
        {
            ToggleSelector();
            GameObject.Find("Board").GetComponent<Board>().SwapCandy(this);
        }
   } 
}

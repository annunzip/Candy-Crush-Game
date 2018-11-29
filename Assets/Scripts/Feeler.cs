using UnityEngine;
using System.Collections;

public class Feeler : MonoBehaviour
{
    public Candy owner;
    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Candy")
        {
            owner.AddNeighbor(c.GetComponent<Candy>());
        }
    }
    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Candy")
        {
            owner.RemoveNeighbor(c.GetComponent<Candy>());
        }
    }
}

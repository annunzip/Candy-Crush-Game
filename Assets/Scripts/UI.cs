using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour
{
    public GameObject game; 

    public GUIText score;
    public int CurrentScore = 0;
    public GameObject TitleScreen;

    public void AddScore(int amountToAdd)
    {
        CurrentScore += amountToAdd;
        score.text = "Score: " + CurrentScore.ToString();
    }
}
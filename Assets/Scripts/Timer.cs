using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    float timer;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }

    void OnGUI()
    {
        GUIStyle MyStyle = new GUIStyle();
        MyStyle.fontSize = 25;
        MyStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(540, 75, 250, 100), "Time: " + timer, MyStyle);
    }
}

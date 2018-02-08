using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    int w;
    int h;
    float deltaTime = 0.0f;
    GUIStyle style;
    Rect rect;
    float msec;
    float fps;
    string text;

    private void Awake()
    {
        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        w = Screen.width;
        h = Screen.height;
        rect = new Rect(0, 0, w, h * 2 / 100);
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;

        text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        style.fontSize = h * 2 / 100;
        GUI.Label(rect, text, style);
    }
}
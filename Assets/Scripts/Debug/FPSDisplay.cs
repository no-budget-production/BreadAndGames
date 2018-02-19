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
    int enemies;
    float time;

    private void Awake()
    {
        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        enemies = GameManager.Instance.Enemies.Count;
    }

    void OnGUI()
    {
        w = Screen.width;
        h = Screen.height;
        rect = new Rect(0, 0, w, h * 50);
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;

        float time = Time.realtimeSinceStartup;

        text = string.Format("{0:0.0} ms ({1:0.} fps) {2} enemies {3:0.00} RT", msec, fps, enemies, time);

        style.fontSize = h * 2 / 100;
        GUI.Label(rect, text, style);
    }
}
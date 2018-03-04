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
    float min;
    float sec;
    //int objectCount;
    //int objectsInView;

    private void Awake()
    {
        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        //enemies = GameManager.Instance.Enemies.Count;


    }

    private void OnEnable()
    {
        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    void OnGUI()
    {
        w = Screen.width;
        h = Screen.height;
        rect = new Rect(0, 0, w, h * 50);
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;

        //time = Time.realtimeSinceStartup;

        //GameObject[] allGameobjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        //objectCount = allGameobjects.Length;
        //for (int i = 0; i < objectCount; i++)
        //{
        //    var tempObject = allGameobjects[i].GetComponent<Renderer>();
        //    if (tempObject != null)
        //    {
        //        if (allGameobjects[i].GetComponent<Renderer>().isVisible)
        //        {
        //            objectsInView++;
        //        }
        //    }
        //}

        text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps/*, enemies, time, objectCount, objectsInView*/);

        style.fontSize = h * 2 / 100;
        GUI.Label(rect, text, style);
    }
}
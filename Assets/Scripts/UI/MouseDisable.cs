using UnityEngine;

public class MouseDisable : MonoBehaviour
{
    public float hideAfterSeconds = 3f;
    public float thresholdInPixels = 3f;

    private float _lastTime;
    private Vector3 _lastMousePos;

    private Vector3 inputMousePosition;
    private Vector3 dx;
    private bool move;
    private float timeSinceLevelLoad;

    private float iniHideTime = -100f;

    private void Awake()
    {

        iniHideTime = -hideAfterSeconds;
    }

    private void Start()
    {
        Cursor.visible = false;

        TimeOutMouse();

        _lastMousePos = Input.mousePosition;
    }

    private void Update()
    {
        inputMousePosition = Input.mousePosition;
        dx = inputMousePosition - _lastMousePos;
        move = (dx.sqrMagnitude > (thresholdInPixels * thresholdInPixels));
        _lastMousePos = inputMousePosition;

        timeSinceLevelLoad = Time.timeSinceLevelLoad;

        if (move)
        {
            _lastTime = timeSinceLevelLoad;
        }

        Cursor.visible = (timeSinceLevelLoad - _lastTime) < hideAfterSeconds;
    }

    public void TimeOutMouse()
    {
        Cursor.visible = false;
        _lastMousePos = inputMousePosition;
        _lastTime = iniHideTime;
    }
}
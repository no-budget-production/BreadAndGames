using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class reloadTimer : MonoBehaviour
{
    public Image fillImg;
    float timeAmt = 1.5f;
    float time;

    // Use this for initialization
    void Start()
    {
        fillImg = GetComponentInChildren<Image>();
        time = timeAmt;
    }

    // Update is called once per frame
    public void circleReload()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            fillImg.fillAmount = time / timeAmt;
        }
    }
}
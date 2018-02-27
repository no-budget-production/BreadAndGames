using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

public class ReviveSelf : Skill
{
    public int howMutchToGet;
    public float ReviveHealthMulti;
    public float workRadius;

    private ActivateWheel UI;
    private findIt[] block;
    private int youGetIt = 0;

    // Use this for initialization
    void Start()
    {
        WhileDead = true;
        UI = FindObjectOfType<ActivateWheel>();
    }

    // Update is called once per frame

    public override void OneShoot()
    {
        block = FindObjectsOfType<findIt>();
        var wheel = UI.GetComponentInChildren<ReviveWheelSpin>();
        foreach (findIt b in block)
        {
            var check = Mathf.Abs(b.GetComponent<RectTransform>().eulerAngles.z);
            if (check <= workRadius || check >= (360 - workRadius))
            {
                Debug.Log("check: " + check);
                youGetIt++;
                Destroy(b.gameObject);
                wheel.changeDirection();
            }
        }
        if (howMutchToGet <= youGetIt)
        {
            Deactivate();
        }
    }

    public void Deactivate()
    {
        block = FindObjectsOfType<findIt>();
        foreach (findIt b in block)
        {
            Destroy(b.gameObject);
        }
        Character.GetHealth(Character.MaxHealth * ReviveHealthMulti);
        youGetIt = 0;
        UI.Deactivate();
    }
}

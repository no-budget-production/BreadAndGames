using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

public class ReviveSelf : Skill
{
    public int howMutchToGet;
    public float ReviveHealthMulti;
    public int workRadius;

    private ActivateWheel UI;
    private ReviveWheelSpin wheel;
    private findIt[] block;
    private int youGetIt;

    // Use this for initialization
    void Start () {
        WhileDead = true;
        UI = FindObjectOfType<ActivateWheel>();
        wheel = FindObjectOfType<ReviveWheelSpin>();
    }
	
	// Update is called once per frame

	public override void OneShoot() {
        block = FindObjectsOfType<findIt>();
        foreach (findIt b in block)
        {
            var check = wheel.GetComponent<RectTransform>().rotation.z - b.GetComponent<RectTransform>().rotation.z;
            check = Mathf.Abs(check);
            if (check <= workRadius)
            {
                youGetIt++;
                wheel.changeDirection();
            }
        }
        if (howMutchToGet <= youGetIt)
        {

            Character.GetHealth(Character.MaxHealth * ReviveHealthMulti);
            
            foreach (findIt b in block)
            {
                Destroy(b.gameObject);
            }
            UI.Deactivate();
        }
	}
}

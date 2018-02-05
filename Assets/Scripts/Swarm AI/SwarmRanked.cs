using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmRanked : MonoBehaviour {
    
    public PlayerController target;
    public float range;

    //clip_size && reload
    public int clip_size = 5;
    public int max_clip_size = 5;
    public int reload_time = 60;
    private int current_reload_time;

    //Attack Speed
    public int cooldown = 10;
    public int current_cd = 0;
    public int current_delay = -1;
    public int startup = 0;

    //Damage
    public float damage = 10;

    // Use this for initialization
    void Start () {
        current_reload_time = reload_time;
    }
	
	// Update is called once per frame
	void Update () {

        if (clip_size > 0)
        {
            var currentPosition = gameObject.transform.position;
            PlayerController[] targetChild = target.GetComponentsInChildren<PlayerController>();
            foreach (PlayerController newTarget in targetChild)
            {
                var targetPosition = newTarget.transform.position;
                float dist = Vector3.Distance(currentPosition, targetPosition);
                if (dist <= range)
                {
                    if (current_cd == 0)
                    {
                        current_cd = cooldown;
                        current_delay = startup;
                    }
                    if (current_delay == 0)
                    {
                        Debug.Log("Shoot!");
                        clip_size--;
                        //Instantiate<Bullet>;
                    }
                    current_delay = Mathf.Max(-1, current_delay - 1);
                    if (current_delay == -1) current_cd = Mathf.Max(0, current_cd - 1);
                }
            }
        }
        else
        {
            current_reload_time--;
            if (current_reload_time == 0)
            {
                current_reload_time = reload_time;
                clip_size = max_clip_size;
            }
        }
    }
}

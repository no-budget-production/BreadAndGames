using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmRanged : MonoBehaviour {
    
    private Transform player_handler;
    GunSystem enemyGunSystem;
    public float range;

    //clip_size && reload
    public int clip_size = 5;
    public int max_clip_size = 5;
    
    // Use this for initialization
    void Start () {
        enemyGunSystem = enemyGunSystem.GetComponent<GunSystem>();
        player_handler = GameObject.Find("Player Handler").transform;
    }
	
	// Update is called once per frame
	void Update () {
        var currentPosition = gameObject.transform.position;
        PlayerController[] targetChild = player_handler.GetComponentsInChildren<PlayerController>();
        foreach (PlayerController newTarget in targetChild)
        {
            var targetPosition = newTarget.transform.position;
            float dist = Vector3.Distance(currentPosition, targetPosition);
            if (dist <= range)
            {
                Debug.Log("Shoot!");
                enemyGunSystem.Shoot();
            }
        }
    }
}

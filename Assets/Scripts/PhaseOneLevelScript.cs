using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseOneLevelScript : MonoBehaviour
{
    public SwarmSpawn[] spawner;

    public bool startArenaEvent;


    
	void Update ()
    {
        if (startArenaEvent)
        {
            for (int i = 0; i < spawner.Length; i++)
            {
                spawner[i].enabled = true;
            }


        }
	}

    private IEnumerator TimerFirstPhase(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

    }
}

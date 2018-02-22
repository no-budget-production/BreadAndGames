using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseOneLevelScript : MonoBehaviour
{
    
    public bool startArenaEvent;
    public bool Wave1Finshed;

    public int Wave1AmountOfEnemys;

    public ArenaSpawner[] spawner;

    void Start()
    {
        startArenaEvent = false;
        Wave1Finshed = false;
    }

    void Update ()
    {
        if (startArenaEvent)
        {
            Debug.Log(GameManager.Instance.Enemies.Count);
            if (!Wave1Finshed)
            {
                // Start of the first Wave
                for (int i = 0; i < spawner.Length; i++)
                {
                    spawner[i].SpawnRate = Random.Range(1.0f, 2.0f);
                    spawner[i].AmountToSpawn = Wave1AmountOfEnemys / spawner.Length;
                    spawner[i].enabled = true;
                }
                if (GameManager.Instance.Enemies.Count == 0)
                {
                    Wave1Finshed = true;
                }
            }


        }
	}

    private IEnumerator TimerFirstPhase(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

    }
}

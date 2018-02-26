using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    public HPPickUps HPPickUps;
    public int HealthPickUpsRequested;
    public float HealthPickUpThreshold;
    public bool HealthPickUpsSpawn;
    public int maxHealthPickUps;

    public void SpawnPickUps(Vector3 location)
    {
        if (GameManager.Instance.HealthPickUps.Count < maxHealthPickUps)
        {
            HPPickUps curHpPickUps = Instantiate(HPPickUps, location + HPPickUps.transform.position, Quaternion.identity);
            GameManager.Instance.HealthPickUps.Add(curHpPickUps);
        }
    }

    public void HealthRequestAdding(bool Add)
    {
        if (Add)
        {
            HealthPickUpsRequested++;
        }
        else
        {
            HealthPickUpsRequested--;
        }

        if (HealthPickUpsRequested > 0)
        {
            HealthPickUpsSpawn = true;
        }
        else
        {
            HealthPickUpsSpawn = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    public HPPickUps HPPickUps;

    public void SpawnPickUps(Vector3 location)
    {
        Debug.Log(location);
        if (GameManager.Instance.HealthPickUps.Count < GameManager.Instance.maxHealthPickUps)
        {
            HPPickUps curHpPickUps = Instantiate(HPPickUps, location + HPPickUps.transform.position, Quaternion.identity);
            GameManager.Instance.HealthPickUps.Add(curHpPickUps);
        }
    }
}

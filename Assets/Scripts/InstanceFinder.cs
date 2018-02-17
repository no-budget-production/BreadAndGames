using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class InstanceFinder : MonoBehaviour
{
    public InstanceRef instanceRef;

    public List<string> PlayerSpawnNames;

    private List<GameObject> tempGameObjectList = new List<GameObject>();
    private List<Transform> tempTransformList = new List<Transform>();

    private void Update()
    {
        Debug.Log("InstanceFinder Enabled - disable for builds and after Instances are found");

        //if (instanceRef == null)
        {
            Debug.Log("InstanceFinder instanceRef - Find Default ");

            var tempVar = GameObject.Find("Loader");

            if (tempVar == null)
            {
                Debug.Log("Loader not found in Scene");
            }

            instanceRef = tempVar.GetComponent<InstanceRef>();
        }

        //if (PlayerSpawnNames.Count == 0)
        {
            Debug.Log("InstanceFinder PlayerSpawnNames.Count - Setting to: MeleeSpawnPoint, ShooterSpawnPoint, SupportSpawnPoint");
            PlayerSpawnNames = new List<string>(3);
            PlayerSpawnNames.Add("MeleeSpawnPoint");
            PlayerSpawnNames.Add("ShooterSpawnPoint");
            PlayerSpawnNames.Add("SupportSpawnPoint");
        }

        //if (instanceRef.PlayerSpawns.Count != 3)
        {
            Debug.Log("InstanceFinder PlayerSpawns.Count - Set to Default 3");
            instanceRef.PlayerSpawns = new List<Transform>(3);

            Debug.Log("InstanceFinder PlayerSpawns - Setting to: MeleeSpawnPoint, ShooterSpawnPoint, SupportSpawnPoint");
            for (int i = 0; i < 3; i++)
            {
                instanceRef.PlayerSpawns.Add(GameObject.Find(PlayerSpawnNames[i]).transform);
            }
        }

        //if (instanceRef.SphereTriggers.Length == 0)
        {
            Debug.Log("InstanceFinder SphereTriggers.Length - Find all");

            tempGameObjectList.Clear();

            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.name.IndexOf("SphereTrigger") != -1)
                {
                    tempGameObjectList.Add(go);
                }
            }

            instanceRef.SphereTriggers = new GameObject[tempGameObjectList.Count];

            for (int j = 0; j < tempGameObjectList.Count; j++)
            {
                instanceRef.SphereTriggers[j] = tempGameObjectList[j];
            }
        }

        //if (instanceRef.ReinforcementPoints.Length == 0)
        {
            Debug.Log("InstanceFinder ReinforcementPoints.Length - Find all");

            tempTransformList.Clear();

            foreach (Transform t in Transform.FindObjectsOfType<Transform>())
            {
                if (t.name.IndexOf("ReinforcementPoint") != -1)
                {
                    if (!(t.name.IndexOf("ReinforcementPointHolder") != -1))
                    {
                        tempTransformList.Add(t);
                    }
                }
            }

            instanceRef.ReinforcementPoints = new Transform[tempTransformList.Count];

            for (int j = 0; j < tempTransformList.Count; j++)
            {
                instanceRef.ReinforcementPoints[j] = tempTransformList[j];
            }
        }

        //if (instanceRef.EnemyHolder == null)
        {
            Debug.Log("EnemyHolder - Find it");

            instanceRef.EnemyHolder = GameObject.Find("EnemyHolder").transform;
        }

        //if (instanceRef.SpawnHolder == null)
        {
            Debug.Log("SpawnHolder - Find it");

            instanceRef.SpawnHolder = GameObject.Find("SpawnHolder").transform;
        }


        //if (instanceRef.ClusterHolder == null)
        {
            Debug.Log("ClusterHolder - Find it");

            instanceRef.ClusterHolder = GameObject.Find("ClusterHolder").transform;
        }
    }
}

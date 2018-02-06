using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public string[] Tags;

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < Tags.Length; i++)
        {
            if (collision.gameObject.tag == Tags[i])
            {
                Debug.Log(Tags[i]);
            }
        }
    }
}

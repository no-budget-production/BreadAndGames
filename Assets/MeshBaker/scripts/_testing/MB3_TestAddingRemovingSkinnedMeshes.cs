using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MB3_TestAddingRemovingSkinnedMeshes : MonoBehaviour {
    public MB3_MeshBaker meshBaker;
    public GameObject[] g; 

    // Use this for initialization
    void Start() {
        StartCoroutine(TestScript());
    }

    IEnumerator TestScript() {
        Debug.Log("Test 1 adding 0,1,2");
        GameObject[] a2 = new GameObject[] { g[0], g[1], g[2] };
        meshBaker.AddDeleteGameObjects(a2, null, true);
        meshBaker.Apply();
        meshBaker.meshCombiner.CheckIntegrity();
        yield return new WaitForSeconds(3f);

        Debug.Log("Test 2 remove 1 and add 3,4,5");
        GameObject[] d1 = new GameObject[] { g[1] };
        a2 = new GameObject[] { g[3], g[4], g[5] };
        meshBaker.AddDeleteGameObjects(a2, d1, true);
        meshBaker.Apply();
        meshBaker.meshCombiner.CheckIntegrity();
        yield return new WaitForSeconds(3f);

        Debug.Log("Test 3 remove 0,2,5 and add 1");
        d1 = new GameObject[] { g[3], g[4], g[5] };
        a2 = new GameObject[] { g[1] };
        meshBaker.AddDeleteGameObjects(a2, d1, true);
        meshBaker.Apply();
        meshBaker.meshCombiner.CheckIntegrity();
        yield return new WaitForSeconds(3f);

        Debug.Log("Test 3 remove all remaining");
        d1 = new GameObject[] { g[0], g[1], g[2] };
        meshBaker.AddDeleteGameObjects(null, d1, true);
        meshBaker.Apply();
        meshBaker.meshCombiner.CheckIntegrity();
        yield return new WaitForSeconds(3f);

        Debug.Log("Test 3 add all");
        meshBaker.AddDeleteGameObjects(g, null, true);
        meshBaker.Apply();
        meshBaker.meshCombiner.CheckIntegrity();
        yield return new WaitForSeconds(1f);

        Debug.Log("Done");
    }
}
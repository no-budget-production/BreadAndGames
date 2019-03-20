using UnityEngine;
using System.Collections;
using DigitalOpus.MB.Core;

public class MB3_TestBakeAllWithSameMaterial : MonoBehaviour {
	public GameObject[] listOfObjsToCombineGood;
	public GameObject[] listOfObjsToCombineBad;

	// Use this for initialization
	void Start () {
		testCombine();
	}

	void testCombine(){
		MB3_MeshCombinerSingle mb = new MB3_MeshCombinerSingle();
		Debug.Log ("About to bake 1");
		mb.AddDeleteGameObjects(listOfObjsToCombineGood,null);
		mb.Apply();
		mb.UpdateGameObjects(listOfObjsToCombineGood);
		mb.Apply ();
		mb.AddDeleteGameObjects(null,listOfObjsToCombineGood);
		mb.Apply ();
		Debug.Log ("Did bake 1");
		Debug.Log ("About to bake 2 should get error that one material doesn't match");
		mb.AddDeleteGameObjects(listOfObjsToCombineBad,null);
		mb.Apply();
		Debug.Log ("Did bake 2");

		Debug.Log("Doing same with multi mesh combiner");
		MB3_MultiMeshCombiner mmb = new MB3_MultiMeshCombiner();
		Debug.Log ("About to bake 3");
		mmb.AddDeleteGameObjects(listOfObjsToCombineGood,null);
		mmb.Apply();
		mmb.UpdateGameObjects(listOfObjsToCombineGood);
		mmb.Apply ();
		mmb.AddDeleteGameObjects(null,listOfObjsToCombineGood);
		mmb.Apply ();
		Debug.Log ("Did bake 3");
		Debug.Log ("About to bake 4  should get error that one material doesn't match");
		mmb.AddDeleteGameObjects(listOfObjsToCombineBad,null);
		mmb.Apply();
		Debug.Log ("Did bake 4");
	}
}

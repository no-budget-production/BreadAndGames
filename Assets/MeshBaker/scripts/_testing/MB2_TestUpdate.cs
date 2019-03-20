using UnityEngine;
using System.Collections;

public class MB2_TestUpdate : MonoBehaviour {
	
	public MB3_MeshBaker meshbaker;
	public MB3_MultiMeshBaker multiMeshBaker;
	public GameObject[] objsToMove;
	public GameObject objWithChangingUVs;
	Vector2[] uvs;
	Mesh m;

   	void Start(){
	  //Add the objects to the combined mesh	
	  //Must have previously baked textures for these in the editor
      meshbaker.AddDeleteGameObjects(objsToMove, null, true);
	  meshbaker.AddDeleteGameObjects(new GameObject[]{objWithChangingUVs}, null, true);

	  MeshFilter mf = objWithChangingUVs.GetComponent<MeshFilter>();
	  m = mf.sharedMesh;
	  uvs = m.uv;
		
      //apply the changes we made this can be slow. See documentation
	  meshbaker.Apply();

	  //same with multi mesh baker
		multiMeshBaker.AddDeleteGameObjects(objsToMove, null, true);
		multiMeshBaker.AddDeleteGameObjects(new GameObject[]{objWithChangingUVs}, null, true);
		
		mf = objWithChangingUVs.GetComponent<MeshFilter>();
		m = mf.sharedMesh;
		uvs = m.uv;

		multiMeshBaker.Apply();	
	}
	
	void LateUpdate(){
		//Apply changes after this and other scripts have made changes
		//Only to vertecies, tangents and normals
		//Only want to call this once per frame since it is slow
		meshbaker.UpdateGameObjects(objsToMove,false);
		Vector2[] uvs2 = m.uv;
		for (int i = 0; i < uvs2.Length; i++){
			uvs2[i] = Mathf.Sin(Time.time) * uvs[i];
		}
		m.uv = uvs2;
		meshbaker.UpdateGameObjects(new GameObject[]{objWithChangingUVs},true,true,true,true,true,false,false,false,false);
		meshbaker.Apply(false,true,true,true,true,false,false,false,false,false);	

		// now multi mesh baker
		multiMeshBaker.UpdateGameObjects(objsToMove,false);
		uvs2 = m.uv;
		for (int i = 0; i < uvs2.Length; i++){
			uvs2[i] = Mathf.Sin(Time.time) * uvs[i];
		}
		m.uv = uvs2;
		multiMeshBaker.UpdateGameObjects(new GameObject[]{objWithChangingUVs},true,true,true,true,true,false,false,false,false);
		multiMeshBaker.Apply(false,true,true,true,true,false,false,false,false,false);

	}
}

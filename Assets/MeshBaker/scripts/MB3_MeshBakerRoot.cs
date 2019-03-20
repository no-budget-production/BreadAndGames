//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;

/// <summary>
/// Root class of all the baking Components
/// </summary>
public abstract class MB3_MeshBakerRoot : MonoBehaviour {

    public static bool DO_INTEGRITY_CHECKS = false;

    /**
     * Transparent shaders often require objects to be sorted along 
     */
    public class ZSortObjects
    {
        public Vector3 sortAxis;
        public class Item
        {
            public GameObject go;
            public Vector3 point;
        }

        public class ItemComparer : IComparer<Item>
        {
            public int Compare(Item a, Item b)
            {
                return (int)Mathf.Sign(b.point.z - a.point.z);
            }
        }

        public void SortByDistanceAlongAxis(List<GameObject> gos)
        {
            if (sortAxis == Vector3.zero)
            {
                Debug.LogError("The sort axis cannot be the zero vector.");
                return;
            }
            Debug.Log("Z sorting meshes along axis numObjs=" + gos.Count);
            List<Item> items = new List<Item>();
            Quaternion q = Quaternion.FromToRotation(sortAxis, Vector3.forward);
            for (int i = 0; i < gos.Count; i++)
            {
                if (gos[i] != null)
                {
                    Item item = new Item();
                    item.point = gos[i].transform.position;
                    item.go = gos[i];
                    item.point = q * item.point;
                    items.Add(item);
                }
            }
            items.Sort(new ItemComparer());

            for (int i = 0; i < gos.Count; i++)
            {
                gos[i] = items[i].go;
            }
        }
    }

    public Vector3 sortAxis;

	[HideInInspector] public abstract MB2_TextureBakeResults textureBakeResults{
		get;
		set;
	}
	
	//todo switch this to List<Renderer>
	public virtual List<GameObject> GetObjectsToCombine(){
		return null;	
	}
	
	public static bool DoCombinedValidate(MB3_MeshBakerRoot mom, MB_ObjsToCombineTypes objToCombineType, MB2_EditorMethodsInterface editorMethods, MB2_ValidationLevel validationLevel){
		if (mom.textureBakeResults == null){
			Debug.LogError("Need to set Texture Bake Result on " + mom);
			return false;
		}
		if (mom is MB3_MeshBakerCommon){
			MB3_MeshBakerCommon momMB = (MB3_MeshBakerCommon) mom;
			MB3_TextureBaker tb = momMB.GetTextureBaker();
			if (tb != null && tb.textureBakeResults != mom.textureBakeResults){
				Debug.LogWarning("Texture Bake Result on this component is not the same as the Texture Bake Result on the MB3_TextureBaker.");
			}
		}

		Dictionary<int,MB_Utility.MeshAnalysisResult> meshAnalysisResultCache = null;
		if (validationLevel == MB2_ValidationLevel.robust){
			meshAnalysisResultCache = new Dictionary<int, MB_Utility.MeshAnalysisResult>();
		}
		List<GameObject> objsToMesh = mom.GetObjectsToCombine();
		for (int i = 0; i < objsToMesh.Count; i++){
			GameObject go = objsToMesh[i];
			if (go == null){
				Debug.LogError("The list of objects to combine contains a null at position." + i + " Select and use [shift] delete to remove");
				return false;					
			}
			for (int j = i + 1; j < objsToMesh.Count; j++){
				if (objsToMesh[i] == objsToMesh[j]){
					Debug.LogError("The list of objects to combine contains duplicates at " + i + " and " + j);
					return false;	
				}
			}
			if (MB_Utility.GetGOMaterials(go).Length == 0){
				Debug.LogError("Object " + go + " in the list of objects to be combined does not have a material");
				return false;
			}
			Mesh m = MB_Utility.GetMesh(go);
			if (m == null){
				Debug.LogError("Object " + go + " in the list of objects to be combined does not have a mesh");
				return false;
			}
			if (m != null){ //This check can be very expensive and it only warns so only do this if we are in the editor.
				if (!Application.isEditor && 
				    Application.isPlaying &&
					mom.textureBakeResults.doMultiMaterial && 
					validationLevel >= MB2_ValidationLevel.robust){
					MB_Utility.MeshAnalysisResult mar;
					if (!meshAnalysisResultCache.TryGetValue(m.GetInstanceID(),out mar)){
						MB_Utility.doSubmeshesShareVertsOrTris(m,ref mar);
						meshAnalysisResultCache.Add (m.GetInstanceID(),mar);
					}
					if (mar.hasOverlappingSubmeshVerts){
						Debug.LogWarning("Object " + objsToMesh[i] + " in the list of objects to combine has overlapping submeshes (submeshes share vertices). If the UVs associated with the shared vertices are important then this bake may not work. If you are using multiple materials then this object can only be combined with objects that use the exact same set of textures (each atlas contains one texture). There may be other undesirable side affects as well. Mesh Master, available in the asset store can fix overlapping submeshes.");	
					}
				}
			}
		}

		
		List<GameObject> objs = objsToMesh;
		
		if (mom is MB3_MeshBaker)
		{
			objs = mom.GetObjectsToCombine();
			//if (((MB3_MeshBaker)mom).useObjsToMeshFromTexBaker && tb != null) objs = tb.GetObjectsToCombine(); 
			if (objs == null || objs.Count == 0)
			{
				Debug.LogError("No meshes to combine. Please assign some meshes to combine.");
				return false;
			}
			if (mom is MB3_MeshBaker && ((MB3_MeshBaker)mom).meshCombiner.renderType == MB_RenderType.skinnedMeshRenderer){
				if (!editorMethods.ValidateSkinnedMeshes(objs))
				{
					return false;
				}
			}
		}
		
		if (editorMethods != null){
			editorMethods.CheckPrefabTypes(objToCombineType, objsToMesh);
		}
		return true;
	}
}


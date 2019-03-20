//----------------------------------------------
//            MeshBaker
// Copyright © 2015-2016 Ian Deane
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using DigitalOpus.MB.Core;
using UnityEditor;


[CustomEditor(typeof(MB3_BoneWeightCopier))]
[CanEditMultipleObjects]
public class MB3_BoneWeightCopierEditor : Editor {
	SerializedProperty inputGameObjectProp;
    SerializedProperty outputPrefabProp;
    SerializedProperty radiusProp;
    SerializedProperty seamMeshProp;
    SerializedProperty targetMeshesProp;
    SerializedProperty outputFolderProp;

	GameObject copyOfInput;

    GUIContent radiusGC = new GUIContent("Radius", "Vertices in each Target Mesh that are within Radius of a vertex in the Seam Mesh will be assigned the same bone weight as the Seam Mesh vertex.");
    GUIContent seamMeshGC = new GUIContent("Seam Mesh", "The seam mesh should contain vertices for the seams with correct bone weights. Seams are the vertices that are shared by two skinned meshes that overlap. For example there should be a seam between an arm skinned mesh and a hand skinned mesh.");

    [MenuItem("GameObject/Create Other/Mesh Baker/Bone Weight Copier",false,1000)]
		public static GameObject CreateNewMeshBaker(){
			GameObject nmb = new GameObject("BoneWeightCopier");
			nmb.transform.position = Vector3.zero;
            nmb.AddComponent<MB3_BoneWeightCopier>();
			return nmb.gameObject;  
		}
		

    void OnEnable() {
        radiusProp = serializedObject.FindProperty("radius");
        seamMeshProp = serializedObject.FindProperty("seamMesh");
        //targetMeshesProp = serializedObject.FindProperty("targetMeshes");
        outputFolderProp = serializedObject.FindProperty("outputFolder");
		inputGameObjectProp = serializedObject.FindProperty("inputGameObject");
        outputPrefabProp = serializedObject.FindProperty("outputPrefab");
    }

    string ConvertAbsolutePathToUnityPath(string absolutePath) {
        if (absolutePath.StartsWith(Application.dataPath)) {
            return "Assets" + absolutePath.Substring(Application.dataPath.Length);
        }
        return null;
    }

    public override void OnInspectorGUI(){
        serializedObject.Update();
        MB3_BoneWeightCopier bwc = (MB3_BoneWeightCopier) target;


        EditorGUILayout.HelpBox("== BETA (please report problems) ==\n\n" +
                                "This tool helps create skinned mesh parts that can be mix and matched to customize characters. " +
                                "It adjusts the boneWeights, positions, normals and tangents at the joins to have the same values so there are no tears." +
                                "\n\n" +
                                "1) Model the skinned meshes and attach them all to the same rig. The rig may have multiple arms, hands, hair etc...\n" +
                                "2) At every seam (eg. between an arm mesh and hand mesh) there must be a set of vertices duplicated in both meshes.\n" +
                                "3) Create one additional mesh that contains another copy of all the geometry. This will be called the Seam Mesh\n" +
                                "4) Attach it to the rig and adjust the bone weights. This will be the master set of bone weights that will be copied to all the other skinned meshes.\n" +
		                        "5) Mark the seam vertices using the UV channel.\n" +
		                        "    verts with UV > (.5,.5) are seam verts\n" +
		                        "    verts with UV < (.5,.5) are ignored\n" +
                                "6) Import the model into Unity and create an instance of it in the scene\n" +
                                "7) Assign the Seam Mesh to the Seam Mesh field\n" +
                                "8) Assign the parent game object of all the skinned meshes to the Input Game Object field\n" +
                                "9) Adjust the radius\n" +
                                "10) Choose an output folder and save the meshes\n" +
		                        "11) Click 'Copy Bone Weights From Seam Mesh'\n", MessageType.Info);

        EditorGUILayout.PropertyField(radiusProp, radiusGC);
        EditorGUILayout.PropertyField(seamMeshProp, seamMeshGC);
		EditorGUILayout.PropertyField(inputGameObjectProp);
        EditorGUILayout.PropertyField(outputPrefabProp);
		//if (GUILayout.Button("Get Skinned Meshes From Input Game Object")) {
		//	GetSkinnedMeshesFromGameObject(bwc);
		//}
        //EditorGUILayout.PropertyField(targetMeshesProp,true);

        if (GUILayout.Button("Copy Bone Weights From Seam Mesh")) {
            CopyBoneWeightsFromSeamMeshToOtherMeshes(bwc);
        }
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Save Meshes To Project Folder", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(outputFolderProp.stringValue);
        if (GUILayout.Button("Browse For Output Folder")) {
            string path = EditorUtility.OpenFolderPanel("Browse For Output Folder", "", "");
            outputFolderProp.stringValue = path;
        }
        if (GUILayout.Button("Set Output Folder To Output Prefab Folder"))
        {
            string path = AssetDatabase.GetAssetPath(bwc.outputPrefab);
            path = new System.IO.FileInfo(path).Directory.FullName.Replace('\\','/');
            outputFolderProp.stringValue = path;
        }
        serializedObject.ApplyModifiedProperties();
	}

    public void CopyBoneWeightsFromSeamMeshToOtherMeshes(MB3_BoneWeightCopier bwc) {
        if (bwc.seamMesh == null) {
            Debug.LogError("The seamMesh cannot be null.");
            return;
        }

		UnityEngine.Object pr = (UnityEngine.Object) PrefabUtility.GetPrefabObject(bwc.seamMesh.gameObject);
		string assetPath = null;
		if (pr != null) {
			assetPath = AssetDatabase.GetAssetPath(pr);
		}
		if (assetPath != null){
			ModelImporter mi = (ModelImporter) AssetImporter.GetAtPath(assetPath);
			if (mi != null){
				if (mi.optimizeMesh){
					Debug.LogError(string.Format("The seam mesh has 'optimized' checked in the asset importer. This will result in no vertices. Uncheck 'optimized'."));
					return;
				}
			}
		}
        //todo check that output game object exists and is a prefab
        if (bwc.outputPrefab == null)
        {
            Debug.LogError(string.Format("The output game object must be assigned and must be a prefab of a game object in the project folder."));
            return;
        }
        if (PrefabUtility.GetPrefabType(bwc.outputPrefab) != PrefabType.Prefab)
        {
            Debug.LogError("The output game object must be a prefab. Create a prefab in the project and drag an empty game object to it.");
            return;
        }

        //duplicate the source prefab and the meshes
        if (copyOfInput != null) {
			DestroyImmediate(copyOfInput);
		}
		copyOfInput = (GameObject) GameObject.Instantiate(bwc.inputGameObject);
        SkinnedMeshRenderer[] targSkinnedMeshes = copyOfInput.GetComponentsInChildren<SkinnedMeshRenderer>();
        Mesh[] targs = new Mesh[targSkinnedMeshes.Length];
        for (int i = 0; i < targSkinnedMeshes.Length; i++)
        {
            if (targSkinnedMeshes[i].sharedMesh == null)
            {
                Debug.LogError(string.Format("Skinned Mesh {0} does not have a mesh", targSkinnedMeshes[i]));
                return;
            }
            MB_PrefabType pt = MBVersionEditor.GetPrefabType(targSkinnedMeshes[i].gameObject);
            if (pt == MB_PrefabType.modelPrefab)
            {
                Debug.LogError(string.Format("Target Mesh {0} is an imported model prefab. Can't modify these meshes because changes will be overwritten the next time the model is saved or reimported. Try instantiating the prefab and using skinned meshes from the scene instance.", i));
                return;
            }
            targs[i] = (Mesh) GameObject.Instantiate(targSkinnedMeshes[i].sharedMesh);
        }
        MB3_CopyBoneWeights.CopyBoneWeightsFromSeamMeshToOtherMeshes(bwc.radius, bwc.seamMesh.sharedMesh, targs);
		SaveMeshesToOutputFolderAndAssignToSMRs(targs, targSkinnedMeshes);

		EditorUtility.SetDirty (copyOfInput);
		PrefabUtility.ReplacePrefab(copyOfInput, bwc.outputPrefab, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
		AssetDatabase.SaveAssets ();
		DestroyImmediate (copyOfInput);
    }

    public void SaveMeshesToOutputFolderAndAssignToSMRs(Mesh[] targetMeshes, SkinnedMeshRenderer[] targetSMRs) {
        //validate meshes
        for (int i = 0; i < targetMeshes.Length; i++) {
			if (targetSMRs[i] == null) {
				Debug.LogError(string.Format("Target Mesh {0} is null", i));
                return;
            }

			if (targetSMRs[i].sharedMesh == null) {
					Debug.LogError(string.Format("Target Mesh {0} does not have a mesh", i));
                return;
            }
            MB_PrefabType pt = MBVersionEditor.GetPrefabType(targetMeshes[i]);
            if (pt == MB_PrefabType.modelPrefab) {
						Debug.LogError(string.Format("Target Mesh {0} is an imported model prefab. Can't modify these meshes because changes will be overwritten the next time the model is saved or reimported. Try instantiating the prefab and using skinned meshes from the scene instance.", i));
                return;
            }
            
        }
        //validate output folder
        if (outputFolderProp.stringValue == null) {
            Debug.LogError("Output folder must be set");
            return;
        }
        if (outputFolderProp.stringValue.StartsWith(Application.dataPath)) {
            string relativePath = "Assets" + outputFolderProp.stringValue.Substring(Application.dataPath.Length);
            string gid = AssetDatabase.AssetPathToGUID(relativePath);
            if (gid == null) {
                Debug.LogError("Output folder must be a folder in the Unity project Asset folder");
                return;
            }
        } else {
            Debug.LogError("Output folder must be a folder in the Unity project Asset folder");
            return;
        }
        for (int i = 0; i < targetMeshes.Length; i++) {
            Mesh m = targetMeshes[i];
            string pth = ConvertAbsolutePathToUnityPath(outputFolderProp.stringValue + "/" + targetMeshes[i].name + ".Asset");
            if (pth == null) {
                Debug.LogError("The output folder must be a folder in the project Assets folder.");
                return;
            }
			AssetDatabase.CreateAsset(m,pth);
			targetSMRs[i].sharedMesh = m;
			Debug.Log(string.Format("Created mesh at {0}. Updated Skinned Mesh {1} to use created mesh.", pth, targetSMRs[i].name));
        }
		AssetDatabase.SaveAssets ();
    }

}

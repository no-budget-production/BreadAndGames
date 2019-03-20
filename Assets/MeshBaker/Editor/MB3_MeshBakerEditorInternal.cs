//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using DigitalOpus.MB.Core;

using UnityEditor;

namespace DigitalOpus.MB.Core{
	
	public interface MB3_MeshBakerEditorWindowInterface{
		MonoBehaviour target{
			get;
			set;
		}	
	}
	
	public class MB3_MeshBakerEditorInternal{
        //add option to exclude skinned mesh renderer and mesh renderer in filter
        //example scenes for multi material
        private static GUIContent
            gc_outputOptoinsGUIContent = new GUIContent("Output"),
            gc_logLevelContent = new GUIContent("Log Level"),
			gc_openToolsWindowLabelContent = new GUIContent("Open Tools For Adding Objects", "Use these tools to find out what can be combined, discover problems with meshes, and quickly add objects."),
			gc_renderTypeGUIContent = new GUIContent("Renderer","The type of renderer to add to the combined mesh."),
			gc_objectsToCombineGUIContent = new GUIContent("Custom List Of Objects To Be Combined","You can add objects here that were not on the list in the MB3_TextureBaker as long as they use a material that is in the Texture Bake Results"),
			gc_textureBakeResultsGUIContent = new GUIContent("Texture Bake Result","When materials are combined a MB2_TextureBakeResult Asset is generated. Drag that Asset to this field to use the combined material."),
			gc_useTextureBakerObjsGUIContent = new GUIContent("Same As Texture Baker","Build a combined mesh using using the same list of objects that generated the Combined Material"),
			gc_lightmappingOptionGUIContent = new GUIContent("Lightmapping UVs","preserve current lightmapping: Use this if all source objects are lightmapped and you want to preserve it. All source objects must use the same lightmap. DOES NOT WORK IN UNITY 5.\n\n"+
																			 "generate new UV Layout: Use this if you want to bake a lightmap after the combined mesh has been generated\n\n" +
																			 "copy UV2 unchanged: Use this if UV2 is being used for something other than lightmaping.\n\n" + 
																			 "ignore UV2: A UV2 channel will not be generated for the combined mesh\n\n" +
                                                                             "copy UV2 unchanged to separate rects: Use this if your meshes include a custom lightmap that you want to use with the combined mesh.\n\n"),

            gc_combinedMeshPrefabGUIContent = new GUIContent("Combined Mesh Prefab","Create a new prefab asset an drag an empty game object to it. Drag the prefab asset to here."),
			gc_clearBuffersAfterBakeGUIContent = new GUIContent("Clear Buffers After Bake","Frees memory used by the MeshCombiner. Set to false if you want to update the combined mesh at runtime."),
			gc_doNormGUIContent = new GUIContent("Include Normals"),
			gc_doTanGUIContent = new GUIContent("Include Tangents"),
			gc_doColGUIContent = new GUIContent("Include Colors"),
            gc_doBlendShapeGUIContent = new GUIContent("Include Blend Shapes"),
            gc_doUVGUIContent = new GUIContent("Include UV"),
			gc_doUV3GUIContent = new GUIContent("Include UV3"),
            gc_doUV4GUIContent = new GUIContent("Include UV4"),
			gc_uv2HardAngleGUIContent = new GUIContent("  UV2 Hard Angle","Angles greater than 'hard angle' in degrees will be split."),
			gc_uv2PackingMarginUV3GUIContent = new GUIContent("  UV2 Packing Margin","The margin between islands in the UV layout measured in UV coordinates (0..1) not pixels"),
            gc_SortAlongAxis = new GUIContent("SortAlongAxis", "Transparent materials often require that triangles be rendered in a certain order. This will sort Game Objects along the specified axis. Triangles will be added to the combined mesh in this order."),
            gc_CenterMeshToBoundsCenter = new GUIContent("Center Mesh To Render Bounds", "Centers the verticies of the mesh about the render bounds center and translates the game object. This makes the combined meshes easier to work with. There is a performance and memory allocation cost to this so if you are frequently baking meshes at runtime disable it."),
            gc_OptimizeAfterBake = new GUIContent("Optimize After Bake", "This does the same thing that 'Optimize' does on the ModelImporter.");

        private SerializedObject meshBaker;
		private SerializedProperty  logLevel, lightmappingOption, combiner, outputOptions, textureBakeResults, useObjsToMeshFromTexBaker, renderType, fixOutOfBoundsUVs, objsToMesh, mesh;
		private SerializedProperty doNorm, doTan, doUV, doUV3, doUV4, doCol, doBlendShapes, clearBuffersAfterBake, uv2OutputParamsPackingMargin, uv2OutputParamsHardAngle, sortOrderAxis, centerMeshToBoundsCenter, optimizeAfterBake;
		bool showInstructions = false;
		bool showContainsReport = true;

        GUIStyle editorBoxBackgroundStyle = new GUIStyle();

        Texture2D editorBoxBackgroundColor;

        Color buttonColor = new Color(.8f, .8f, 1f, 1f);

        void _init (SerializedObject mb) { 
			this.meshBaker = mb;
            
			objsToMesh = meshBaker.FindProperty("objsToMesh");
			combiner = meshBaker.FindProperty("_meshCombiner");
            logLevel = combiner.FindPropertyRelative("_LOG_LEVEL");
            outputOptions = combiner.FindPropertyRelative("_outputOption");			
			renderType = combiner.FindPropertyRelative("_renderType");
			useObjsToMeshFromTexBaker = meshBaker.FindProperty("useObjsToMeshFromTexBaker");
			textureBakeResults = combiner.FindPropertyRelative("_textureBakeResults");
			lightmappingOption = combiner.FindPropertyRelative("_lightmapOption");
			doNorm = combiner.FindPropertyRelative("_doNorm");
			doTan = combiner.FindPropertyRelative("_doTan");
			doUV = combiner.FindPropertyRelative("_doUV");
			doUV3 = combiner.FindPropertyRelative("_doUV3");
            doUV4 = combiner.FindPropertyRelative("_doUV4");
            doCol = combiner.FindPropertyRelative("_doCol");
            doBlendShapes = combiner.FindPropertyRelative("_doBlendShapes");
            uv2OutputParamsPackingMargin = combiner.FindPropertyRelative ("uv2UnwrappingParamsPackMargin");
			uv2OutputParamsHardAngle = combiner.FindPropertyRelative ("uv2UnwrappingParamsHardAngle");
			clearBuffersAfterBake = meshBaker.FindProperty("clearBuffersAfterBake");
			mesh = combiner.FindPropertyRelative("_mesh");
            sortOrderAxis = meshBaker.FindProperty("sortAxis");
            centerMeshToBoundsCenter = combiner.FindPropertyRelative("_recenterVertsToBoundsCenter");
            optimizeAfterBake = combiner.FindPropertyRelative("_optimizeAfterBake");
        }

        public void OnEnable(SerializedObject meshBaker)
        {
            _init(meshBaker);
            bool isPro = EditorGUIUtility.isProSkin;
            Color32 backgroundColor = isPro
                ? new Color32(35, 35, 35, 255)
                : new Color32(174, 174, 174, 255);
            editorBoxBackgroundColor = MB3_MeshBakerEditorFunctions.MakeTex(8, 8, backgroundColor);
            editorBoxBackgroundStyle.normal.background = editorBoxBackgroundColor;
            editorBoxBackgroundStyle.border = new RectOffset(0, 0, 0, 0);
            editorBoxBackgroundStyle.margin = new RectOffset(5, 5, 5, 5);
            editorBoxBackgroundStyle.padding = new RectOffset(10, 10, 10, 10);
        }

        public void OnDisable()
        {
            if (editorBoxBackgroundColor != null) GameObject.DestroyImmediate(editorBoxBackgroundColor);
        }

        public void OnInspectorGUI(SerializedObject meshBaker, MB3_MeshBakerCommon target, System.Type editorWindowType){
			DrawGUI(meshBaker, target, editorWindowType);
		}
		
		public void DrawGUI(SerializedObject meshBaker, MB3_MeshBakerCommon target, System.Type editorWindowType){
            if (meshBaker == null)
            {
                return;
            }
            meshBaker.Update();

			showInstructions = EditorGUILayout.Foldout(showInstructions,"Instructions:");
			if (showInstructions){
				EditorGUILayout.HelpBox("1. Bake combined material(s).\n\n" +
										"2. If necessary set the 'Texture Bake Results' field.\n\n" +
										"3. Add scene objects or prefabs to combine or check 'Same As Texture Baker'. For best results these should use the same shader as result material.\n\n" +
										"4. Select options and 'Bake'.\n\n" +
										"6. Look at warnings/errors in console. Decide if action needs to be taken.\n\n" +
										"7. (optional) Disable renderers in source objects.", UnityEditor.MessageType.None);
				
				EditorGUILayout.Separator();
			}				
			
			MB3_MeshBakerCommon momm = (MB3_MeshBakerCommon) target;
			
			//mom.meshCombiner.LOG_LEVEL = (MB2_LogLevel) EditorGUILayout.EnumPopup("Log Level", mom.meshCombiner.LOG_LEVEL);
            EditorGUILayout.PropertyField(logLevel, gc_logLevelContent);
			
			EditorGUILayout.PropertyField(textureBakeResults, gc_textureBakeResultsGUIContent);
			if (textureBakeResults.objectReferenceValue != null){
				showContainsReport = EditorGUILayout.Foldout(showContainsReport, "Shaders & Materials Contained");
				if (showContainsReport){
					EditorGUILayout.HelpBox(((MB2_TextureBakeResults)textureBakeResults.objectReferenceValue).GetDescription(), MessageType.Info);	
				}
			}

            EditorGUILayout.BeginVertical(editorBoxBackgroundStyle);
			EditorGUILayout.LabelField("Objects To Be Combined",EditorStyles.boldLabel);
			if (momm.GetTextureBaker() != null){
				EditorGUILayout.PropertyField(useObjsToMeshFromTexBaker, gc_useTextureBakerObjsGUIContent);
			} else {
				useObjsToMeshFromTexBaker.boolValue = false;
				momm.useObjsToMeshFromTexBaker = false;
				GUI.enabled = false;
				EditorGUILayout.PropertyField(useObjsToMeshFromTexBaker, gc_useTextureBakerObjsGUIContent);
				GUI.enabled = true;
			}
			
			if (!momm.useObjsToMeshFromTexBaker){
				if (GUILayout.Button(gc_openToolsWindowLabelContent)){
					MB3_MeshBakerEditorWindowInterface mmWin = (MB3_MeshBakerEditorWindowInterface) EditorWindow.GetWindow(editorWindowType);
					mmWin.target = (MB3_MeshBakerRoot) target;
				}

                object[] objs = MB3_EditorMethods.DropZone("Drag & Drop Renderers Or Parents Here To Add Objects To Be Combined", 300, 50);
                MB3_EditorMethods.AddDroppedObjects(objs, momm);

                EditorGUILayout.PropertyField(objsToMesh,gc_objectsToCombineGUIContent, true);
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select Objects In Scene"))
                {
                    Selection.objects = momm.GetObjectsToCombine().ToArray();
                    if (momm.GetObjectsToCombine().Count > 0)
                    {
                        SceneView.lastActiveSceneView.pivot = momm.GetObjectsToCombine()[0].transform.position;
                    }
                }
                if (GUILayout.Button(gc_SortAlongAxis))
                {
                    MB3_MeshBakerRoot.ZSortObjects sorter = new MB3_MeshBakerRoot.ZSortObjects();
                    sorter.sortAxis = sortOrderAxis.vector3Value;
                    sorter.SortByDistanceAlongAxis(momm.GetObjectsToCombine());
                }
                EditorGUILayout.PropertyField(sortOrderAxis, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            } else {
				GUI.enabled = false;
				EditorGUILayout.PropertyField(objsToMesh,gc_objectsToCombineGUIContent, true);
				GUI.enabled = true;
			}
            EditorGUILayout.EndVertical();

			EditorGUILayout.LabelField("Output",EditorStyles.boldLabel);
			if (momm is MB3_MultiMeshBaker){
				MB3_MultiMeshCombiner mmc = (MB3_MultiMeshCombiner) momm.meshCombiner;
				mmc.maxVertsInMesh = EditorGUILayout.IntField("Max Verts In Mesh", mmc.maxVertsInMesh);	
			}
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(doNorm,gc_doNormGUIContent);
			EditorGUILayout.PropertyField(doTan,gc_doTanGUIContent);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(doUV,gc_doUVGUIContent);
			EditorGUILayout.PropertyField(doUV3,gc_doUV3GUIContent);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(doUV4, gc_doUV4GUIContent);
            EditorGUILayout.PropertyField(doCol,gc_doColGUIContent);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(doBlendShapes, gc_doBlendShapeGUIContent);

            if (momm.meshCombiner.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping) {
                if (MBVersion.GetMajorVersion() == 5) {
                    EditorGUILayout.HelpBox("The best choice for Unity 5 is to Ignore_UV2 or Generate_New_UV2 layout. Unity's baked GI will create the UV2 layout it wants. See manual for more information.", MessageType.Warning);
                }
            }
			if (momm.meshCombiner.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout){
				EditorGUILayout.HelpBox("Generating new lightmap UVs can split vertices which can push the number of vertices over the 64k limit.",MessageType.Warning);
			}
			EditorGUILayout.PropertyField(lightmappingOption,gc_lightmappingOptionGUIContent);
			if (momm.meshCombiner.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout){
				EditorGUILayout.PropertyField(uv2OutputParamsHardAngle, gc_uv2HardAngleGUIContent);
				EditorGUILayout.PropertyField(uv2OutputParamsPackingMargin, gc_uv2PackingMarginUV3GUIContent);
				EditorGUILayout.Separator();
			}

			EditorGUILayout.PropertyField(outputOptions,gc_outputOptoinsGUIContent);
			EditorGUILayout.PropertyField(renderType, gc_renderTypeGUIContent);
			if (momm.meshCombiner.outputOption == MB2_OutputOptions.bakeIntoSceneObject){
				//todo switch to renderer
				momm.meshCombiner.resultSceneObject = (GameObject) EditorGUILayout.ObjectField("Combined Mesh Object", momm.meshCombiner.resultSceneObject, typeof(GameObject), true);
				if (momm is MB3_MeshBaker){
					string l = "Mesh";
					if (mesh.objectReferenceValue != null) l += " ("+ mesh.objectReferenceValue.GetInstanceID() +")";
					EditorGUILayout.PropertyField(mesh,new GUIContent(l));
				}
			} else if (momm.meshCombiner.outputOption == MB2_OutputOptions.bakeIntoPrefab){
				momm.resultPrefab = (GameObject) EditorGUILayout.ObjectField(gc_combinedMeshPrefabGUIContent, momm.resultPrefab, typeof(GameObject), true);			
				if (momm is MB3_MeshBaker){
					string l = "Mesh";
					if (mesh.objectReferenceValue != null) l += " ("+ mesh.objectReferenceValue.GetInstanceID() +")";
					EditorGUILayout.PropertyField(mesh,new GUIContent(l));
				}
			} else if (momm.meshCombiner.outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace){
				EditorGUILayout.HelpBox("NEW! Try the BatchPrefabBaker component. It makes preparing a batch of prefabs for static/ dynamic batching much easier.",MessageType.Info);
				if (GUILayout.Button("Choose Folder For Bake In Place Meshes") ){
					string newFolder = EditorUtility.SaveFolderPanel("Folder For Bake In Place Meshes", Application.dataPath, "");	
					if (!newFolder.Contains(Application.dataPath)) Debug.LogWarning("The chosen folder must be in your assets folder.");
					momm.bakeAssetsInPlaceFolderPath = "Assets" + newFolder.Replace(Application.dataPath, "");
				}
				EditorGUILayout.LabelField("Folder For Meshes: " + momm.bakeAssetsInPlaceFolderPath);
			}

            EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(clearBuffersAfterBake, gc_clearBuffersAfterBakeGUIContent);
            EditorGUILayout.PropertyField(centerMeshToBoundsCenter, gc_CenterMeshToBoundsCenter);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(optimizeAfterBake, gc_OptimizeAfterBake);
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = buttonColor;
            if (GUILayout.Button("Bake")){
				bake(momm);
			}
            GUI.backgroundColor = oldColor;
	
			string enableRenderersLabel;
			bool disableRendererInSource = false;
			if (momm.GetObjectsToCombine().Count > 0){
				Renderer r = MB_Utility.GetRenderer(momm.GetObjectsToCombine()[0]);
				if (r != null && r.enabled) disableRendererInSource = true;
			}
			if (disableRendererInSource){
				enableRenderersLabel = "Disable Renderers On Source Objects";
			} else {
				enableRenderersLabel = "Enable Renderers On Source Objects";
			}
			if (GUILayout.Button(enableRenderersLabel)){
				momm.EnableDisableSourceObjectRenderers(!disableRendererInSource);
			}	
			
			meshBaker.ApplyModifiedProperties();		
			meshBaker.SetIsDifferentCacheDirty();
		}
			
		public static void updateProgressBar(string msg, float progress){
			EditorUtility.DisplayProgressBar("Combining Meshes", msg, progress);
		}
			
		public static bool bake(MB3_MeshBakerCommon mom){
			bool createdDummyTextureBakeResults = false;
			bool success = false;
			try{
				if (mom.meshCombiner.outputOption == MB2_OutputOptions.bakeIntoSceneObject ||
                    mom.meshCombiner.outputOption == MB2_OutputOptions.bakeIntoPrefab)
                {
					success = MB3_MeshBakerEditorFunctions.BakeIntoCombined(mom, out createdDummyTextureBakeResults);
				} else {
                    //bake meshes in place
					if (mom is MB3_MeshBaker){
						if (MB3_MeshCombiner.EVAL_VERSION){
							Debug.LogError("Bake Meshes In Place is disabled in the evaluation version.");
						} else {
							MB2_ValidationLevel vl = Application.isPlaying ? MB2_ValidationLevel.quick : MB2_ValidationLevel.robust; 
							if (!MB3_MeshBakerRoot.DoCombinedValidate(mom, MB_ObjsToCombineTypes.prefabOnly, new MB3_EditorMethods(), vl)) return false;
								
							List<GameObject> objsToMesh = mom.GetObjectsToCombine();
							success = MB3_BakeInPlace.BakeMeshesInPlace((MB3_MeshCombinerSingle)((MB3_MeshBaker)mom).meshCombiner, objsToMesh, mom.bakeAssetsInPlaceFolderPath, mom.clearBuffersAfterBake, updateProgressBar);
						}
					} else {
						Debug.LogError("Multi-mesh Baker components cannot be used for Bake In Place. Use an ordinary Mesh Baker object instead.");	
					}
				}
                mom.meshCombiner.CheckIntegrity();
            } catch(Exception e){
				Debug.LogError(e);	
			} finally {
				if (createdDummyTextureBakeResults && mom.textureBakeResults != null){
					MB_Utility.Destroy(mom.textureBakeResults);
					mom.textureBakeResults = null;
				}
				EditorUtility.ClearProgressBar();
			}
			return success;
		}
	}	
}

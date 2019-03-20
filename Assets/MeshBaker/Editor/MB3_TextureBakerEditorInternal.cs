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
using System.Text.RegularExpressions;
using DigitalOpus.MB.Core;

using UnityEditor;

namespace DigitalOpus.MB.Core
{
    public class MB3_TextureBakerEditorInternal
    {
        //add option to exclude skinned mesh renderer and mesh renderer in filter
        //example scenes for multi material

        private static GUIContent insertContent = new GUIContent("+", "add a material");
        private static GUIContent deleteContent = new GUIContent("-", "delete a material");
        private static GUILayoutOption buttonWidth = GUILayout.MaxWidth(20f);
        public static GUIContent noneContent = new GUIContent("");

        //private SerializedObject textureBaker;
        private SerializedProperty logLevel, textureBakeResults, maxTilingBakeSize, maxAtlasSize,
            doMultiMaterial, doMultiMaterialSplitAtlasesIfTooBig, doMultiMaterialIfOBUVs, considerMeshUVs, resultMaterial, resultMaterials, atlasPadding,
            resizePowerOfTwoTextures, customShaderProperties, objsToMesh, texturePackingAlgorithm, maxAtlasWidthOverride, maxAtlasHeightOverride, useMaxAtlasWidthOverride, useMaxAtlasHeightOverride,
            forcePowerOfTwoAtlas, considerNonTextureProperties, sortOrderAxis;

        bool resultMaterialsFoldout = true;
        bool showInstructions = false;
        bool showContainsReport = true;

        GUIStyle multipleMaterialBackgroundStyle = new GUIStyle();
        GUIStyle multipleMaterialBackgroundStyleDarker = new GUIStyle();
        GUIStyle editorBoxBackgroundStyle = new GUIStyle();


        Texture2D multipleMaterialBackgroundColor;
        Texture2D multipleMaterialBackgroundColorDarker;
        Texture2D editorBoxBackgroundColor;

        Color buttonColor = new Color(.8f, .8f, 1f, 1f);

        private static GUIContent
            createPrefabAndMaterialLabelContent = new GUIContent("Create Empty Assets For Combined Material", "Creates a material asset and a 'MB2_TextureBakeResult' asset. You should set the shader on the material. Mesh Baker uses the Texture properties on the material to decide what atlases need to be created. The MB2_TextureBakeResult asset should be used in the 'Texture Bake Result' field."),
            logLevelContent = new GUIContent("Log Level"),
            openToolsWindowLabelContent = new GUIContent("Open Tools For Adding Objects", "Use these tools to find out what can be combined, discover possible problems with meshes, and quickly add objects."),
            fixOutOfBoundsGUIContent = new GUIContent("Consider Mesh UVs", "(Was called 'fix out of bounds UVs') The textures will be sampled based on mesh uv rectangle as well as material tiling. This can have two effects:\n\n" +
                                                        "1) If the mesh only uses a small rectangle of it's source material (atlas). Only that small rectangle will be baked into the atlas.\n\n" +
                                                        "2) If the mesh has uvs outside the 0,1 range (tiling) then this tiling will be baked into the atlas."),
            resizePowerOfTwoGUIContent = new GUIContent("Resize Power-Of-Two Textures", "Shrinks textures so they have a clear border of width 'Atlas Padding' around them. Improves texture packing efficiency."),
            customShaderPropertyNamesGUIContent = new GUIContent("Custom Shader Propert Names", "Mesh Baker has a list of common texture properties that it looks for in shaders to generate atlases. Custom shaders may have texture properties not on this list. Add them here and Meshbaker will generate atlases for them."),
            combinedMaterialsGUIContent = new GUIContent("Combined Materials", "Use the +/- buttons to add multiple combined materials. You will also need to specify which materials on the source objects map to each combined material."),
            maxTilingBakeSizeGUIContent = new GUIContent("Max Tiling Bake Size", "This is the maximum size tiling textures will be baked to."),
            maxAtlasSizeGUIContent = new GUIContent("Max Atlas Size", "This is the maximum size of the atlas. If the atlas is larger than this textures being added will be shrunk."),

            objectsToCombineGUIContent = new GUIContent("Objects To Be Combined", "These can be prefabs or scene objects. They must be game objects with Renderer components, not the parent objects. Materials on these objects will baked into the combined material(s)"),
            textureBakeResultsGUIContent = new GUIContent("Texture Bake Result", "This asset contains a mapping of materials to UV rectangles in the atlases. It is needed to create combined meshes or adjust meshes so they can use the combined material(s). Create it using 'Create Empty Assets For Combined Material'. Drag it to the 'Texture Bake Result' field to use it."),
            texturePackingAgorithmGUIContent = new GUIContent("Texture Packer", "Unity's PackTextures: Atlases are always a power of two. Can crash when trying to generate large atlases. \n\n " +
                                                              "Mesh Baker Texture Packer: Atlases will be most efficient size and shape (not limited to a power of two). More robust for large atlases. \n\n" +
                                                              "Mesh Baker Texture Packer Fast: Same as Mesh Baker Texture Packer but creates atlases on the graphics card using RenderTextures instead of the CPU. Source textures can be compressed. May not be pixel perfect. \n\n" +
                                                              "Mesh Baker Texture Packer Horizontal (Experimental): Packs all images vertically to allow horizontal-only UV-tiling.\n\n" +
                                                              "Mesh Baker Texture Packer Vertical (Experimental): Packs all images horizontally other to allow vertical-only UV-tiling.\n\n"),

            configMultiMatFromObjsContent = new GUIContent("Build Source To Combined Mapping From \n Objects To Be Combined", "This will group the materials on your source objects by shader and create one source to combined mapping for each shader found. For example if combining trees then all the materials with the same bark shader will be grouped togther and all the materials with the same leaf material will be grouped together. You can adjust the results afterwards. \n\nIf fix out-of-bounds UVs is NOT checked then submeshes with UVs outside 0,0..1,1 will be mapped to their own submesh regardless of shader."),
            forcePowerOfTwoAtlasContent = new GUIContent("Force Power-Of-Two Atlas", "Forces atlas x and y dimensions to be powers of two with aspect ratio 1:1,1:2 or 2:1. Unity recommends textures be a power of two for everything but GUI textures."),
            considerNonTexturePropertiesContent = new GUIContent("Blend Non-Texture Properties", "Will blend non-texture properties such as _Color, _Glossiness with the textures. Objects with different non-texture property values will be copied into different parts of the atlas even if they use the same textures. This feature requires that TextureBlenders " +
                                                            "exist for the result material shader. It is easy to extend Mesh Baker by writing custom TextureBlenders. Default TextureBlenders exist for: \n" +
                                                             "  - Standard \n" +
                                                             "  - Diffuse \n" +
                                                             "  - Bump Diffuse\n"),
            gc_SortAlongAxis = new GUIContent("Sort Along Axis", "Transparent materials often require that triangles be rendered in a certain order. This will sort Game Objects along the specified axis. Triangles will be added to the combined mesh in this order."),
            gc_DoMultiMaterialSplitAtlasesIfTooBig = new GUIContent("Split Atlases If Textures Don't Fit", ""),
            gc_DoMultiMaterialSplitAtlasesIfOBUVs = new GUIContent("Put Meshes With Out Of Bounds UV On Submesh", ""),
            gc_overrideMaxAtlasWidth = new GUIContent("Override Max Atlas Width", "Set the maximum width of the atlas to this."),
            gc_overrideMaxAtlasHeight = new GUIContent("Override Max Atlas Height", "Set the maximum height of the atlas to this."),
            gc_useMaxAtlasWidthOverride = new GUIContent("Use Max Width Override", "Force the atlas width to not exceed the override value"),
            gc_useMaxAtlasHeightOverride = new GUIContent("Use Max Height Override", "Force the atlas width to not exceed the override value"),
            gc_atlasPadding = new GUIContent("Atlas Padding", "Number of pixels to pad around the edge of the atlas.");


        [MenuItem("GameObject/Create Other/Mesh Baker/TextureBaker", false, 100)]
        public static void CreateNewTextureBaker()
        {
            MB3_TextureBaker[] mbs = (MB3_TextureBaker[])Editor.FindObjectsOfType(typeof(MB3_TextureBaker));
            Regex regex = new Regex(@"\((\d+)\)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            int largest = 0;
            try
            {
                for (int i = 0; i < mbs.Length; i++)
                {
                    Match match = regex.Match(mbs[i].name);
                    if (match.Success)
                    {
                        int val = Convert.ToInt32(match.Groups[1].Value);
                        if (val >= largest)
                            largest = val + 1;
                    }
                }
            }
            catch (Exception e)
            {
                if (e == null) e = null; //Do nothing supress compiler warning
            }
            GameObject nmb = new GameObject("TextureBaker (" + largest + ")");
            nmb.transform.position = Vector3.zero;
            nmb.AddComponent<MB3_MeshBakerGrouper>();
            MB3_TextureBaker tb = nmb.AddComponent<MB3_TextureBaker>();
            tb.packingAlgorithm = MB2_PackingAlgorithmEnum.MeshBakerTexturePacker;
        }

        void _init(SerializedObject textureBaker)
        {
            //textureBaker = new SerializedObject(target);
            logLevel = textureBaker.FindProperty("LOG_LEVEL");
            doMultiMaterial = textureBaker.FindProperty("_doMultiMaterial");
            doMultiMaterialSplitAtlasesIfTooBig = textureBaker.FindProperty("_doMultiMaterialSplitAtlasesIfTooBig");
            doMultiMaterialIfOBUVs = textureBaker.FindProperty("_doMultiMaterialSplitAtlasesIfOBUVs");
            considerMeshUVs = textureBaker.FindProperty("_fixOutOfBoundsUVs");
            resultMaterial = textureBaker.FindProperty("_resultMaterial");
            resultMaterials = textureBaker.FindProperty("resultMaterials");
            atlasPadding = textureBaker.FindProperty("_atlasPadding");
            resizePowerOfTwoTextures = textureBaker.FindProperty("_resizePowerOfTwoTextures");
            customShaderProperties = textureBaker.FindProperty("_customShaderProperties");
            objsToMesh = textureBaker.FindProperty("objsToMesh");
            maxTilingBakeSize = textureBaker.FindProperty("_maxTilingBakeSize");
            maxAtlasSize = textureBaker.FindProperty("_maxAtlasSize");
            maxAtlasWidthOverride = textureBaker.FindProperty("_maxAtlasWidthOverride");
            maxAtlasHeightOverride = textureBaker.FindProperty("_maxAtlasHeightOverride");
            useMaxAtlasWidthOverride = textureBaker.FindProperty("_useMaxAtlasWidthOverride");
            useMaxAtlasHeightOverride = textureBaker.FindProperty("_useMaxAtlasHeightOverride");
            textureBakeResults = textureBaker.FindProperty("_textureBakeResults");
            texturePackingAlgorithm = textureBaker.FindProperty("_packingAlgorithm");
            forcePowerOfTwoAtlas = textureBaker.FindProperty("_meshBakerTexturePackerForcePowerOfTwo");
            considerNonTextureProperties = textureBaker.FindProperty("_considerNonTextureProperties");
            sortOrderAxis = textureBaker.FindProperty("sortAxis");
        }

        public void OnEnable(SerializedObject textureBaker)
        {
            _init(textureBaker);
            bool isPro = EditorGUIUtility.isProSkin;
            Color backgroundColor = isPro
                ? new Color32(35, 35, 35, 255)
                : new Color32(174, 174, 174, 255);
            multipleMaterialBackgroundColor = MB3_MeshBakerEditorFunctions.MakeTex(8, 8, backgroundColor);
            backgroundColor = isPro
                ? new Color32(50, 50, 50, 255)
                : new Color32(160, 160, 160, 255);
            multipleMaterialBackgroundColorDarker = MB3_MeshBakerEditorFunctions.MakeTex(8, 8, backgroundColor);
            backgroundColor = isPro
                ? new Color32(35, 35, 35, 255)
                : new Color32(174, 174, 174, 255);
            editorBoxBackgroundColor = MB3_MeshBakerEditorFunctions.MakeTex(8, 8, backgroundColor);
            multipleMaterialBackgroundStyle.normal.background = multipleMaterialBackgroundColor;
            multipleMaterialBackgroundStyleDarker.normal.background = multipleMaterialBackgroundColorDarker;
            editorBoxBackgroundStyle.normal.background = editorBoxBackgroundColor;
            editorBoxBackgroundStyle.border = new RectOffset(0, 0, 0, 0);
            editorBoxBackgroundStyle.margin = new RectOffset(5, 5, 5, 5);
            editorBoxBackgroundStyle.padding = new RectOffset(10, 10, 10, 10);

        }

        public void OnDisable()
        {
            if (multipleMaterialBackgroundColor != null) GameObject.DestroyImmediate(multipleMaterialBackgroundColor);
            if (multipleMaterialBackgroundColorDarker != null) GameObject.DestroyImmediate(multipleMaterialBackgroundColorDarker);
            if (editorBoxBackgroundColor != null) GameObject.DestroyImmediate(editorBoxBackgroundColor);
        }

        public void DrawGUI(SerializedObject textureBaker, MB3_TextureBaker momm, System.Type editorWindow)
        {
            if (textureBaker == null)
            {
                return;
            }
            textureBaker.Update();

            
            showInstructions = EditorGUILayout.Foldout(showInstructions, "Instructions:");
            if (showInstructions)
            {
                EditorGUILayout.HelpBox("1. Add scene objects or prefabs to combine. For best results these should use the same shader as result material.\n\n" +
                                        "2. Create Empty Assets For Combined Material(s)\n\n" +
                                        "3. Check that shader on result material(s) are correct.\n\n" +
                                        "4. Bake materials into combined material(s).\n\n" +
                                        "5. Look at warnings/errors in console. Decide if action needs to be taken.\n\n" +
                                        "6. You are now ready to build combined meshs or adjust meshes to use the combined material(s).", UnityEditor.MessageType.None);

            }
            
            EditorGUILayout.PropertyField(logLevel, logLevelContent);
            
            EditorGUILayout.Separator();
            
            EditorGUILayout.BeginVertical(editorBoxBackgroundStyle);
            EditorGUILayout.LabelField("Objects To Be Combined", EditorStyles.boldLabel);
            if (GUILayout.Button(openToolsWindowLabelContent))
            {
                MB3_MeshBakerEditorWindowInterface mmWin = (MB3_MeshBakerEditorWindowInterface)EditorWindow.GetWindow(editorWindow);
                mmWin.target = (MB3_MeshBakerRoot)momm;
            }

            object[] objs = MB3_EditorMethods.DropZone("Drag & Drop Renderers Or Parents Here To Add Objects To Be Combined", 300, 50);
            MB3_EditorMethods.AddDroppedObjects(objs, momm);

            EditorGUILayout.PropertyField(objsToMesh, objectsToCombineGUIContent, true);
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
            EditorGUILayout.EndVertical();
            

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            if (GUILayout.Button(createPrefabAndMaterialLabelContent))
            {
                string newPrefabPath = EditorUtility.SaveFilePanelInProject("Asset name", "", "asset", "Enter a name for the baked texture results");
                if (newPrefabPath != null)
                {
                    CreateCombinedMaterialAssets(momm, newPrefabPath);
                }
            }
            EditorGUILayout.PropertyField(textureBakeResults, textureBakeResultsGUIContent);
            if (textureBakeResults.objectReferenceValue != null)
            {
                showContainsReport = EditorGUILayout.Foldout(showContainsReport, "Shaders & Materials Contained");
                if (showContainsReport)
                {
                    EditorGUILayout.HelpBox(((MB2_TextureBakeResults)textureBakeResults.objectReferenceValue).GetDescription(), MessageType.Info);
                }
            }
            EditorGUILayout.PropertyField(doMultiMaterial, new GUIContent("Multiple Combined Materials"));

            if (momm.doMultiMaterial)
            {
                EditorGUILayout.BeginVertical(multipleMaterialBackgroundStyle);
                EditorGUILayout.LabelField("Source Material To Combined Mapping", EditorStyles.boldLabel);

                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 300;
                EditorGUILayout.PropertyField(doMultiMaterialIfOBUVs, gc_DoMultiMaterialSplitAtlasesIfOBUVs);
                EditorGUILayout.PropertyField(doMultiMaterialSplitAtlasesIfTooBig, gc_DoMultiMaterialSplitAtlasesIfTooBig);
                EditorGUIUtility.labelWidth = oldLabelWidth;


                if (GUILayout.Button(configMultiMatFromObjsContent))
                {
                    ConfigureMutiMaterialsFromObjsToCombine(momm, resultMaterials, textureBaker);
                }

                EditorGUILayout.BeginHorizontal();
                resultMaterialsFoldout = EditorGUILayout.Foldout(resultMaterialsFoldout, combinedMaterialsGUIContent);

                if (GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, buttonWidth))
                {
                    if (resultMaterials.arraySize == 0)
                    {
                        momm.resultMaterials = new MB_MultiMaterial[1];
                        momm.resultMaterials[0].considerMeshUVs = momm.fixOutOfBoundsUVs;
                    }
                    else
                    {
                        int idx = resultMaterials.arraySize - 1;
                        resultMaterials.InsertArrayElementAtIndex(idx);
                        resultMaterials.GetArrayElementAtIndex(idx + 1).FindPropertyRelative("considerMeshUVs").boolValue = momm.fixOutOfBoundsUVs;
                    }
                }
                if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth))
                {
                    resultMaterials.DeleteArrayElementAtIndex(resultMaterials.arraySize - 1);
                }
                EditorGUILayout.EndHorizontal();
                if (resultMaterialsFoldout)
                {
                    for (int i = 0; i < resultMaterials.arraySize; i++)
                    {
                        EditorGUILayout.Separator();
                        if (i % 2 == 1)
                        {
                            EditorGUILayout.BeginVertical(multipleMaterialBackgroundStyle);
                        }
                        else
                        {
                            EditorGUILayout.BeginVertical(multipleMaterialBackgroundStyleDarker);
                        }
                        string s = "";
                        if (i < momm.resultMaterials.Length && momm.resultMaterials[i] != null && momm.resultMaterials[i].combinedMaterial != null) s = momm.resultMaterials[i].combinedMaterial.shader.ToString();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("---------- submesh:" + i + " " + s, EditorStyles.boldLabel);
                        if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth))
                        {
                            resultMaterials.DeleteArrayElementAtIndex(i);
                        }
                        EditorGUILayout.EndHorizontal();
                        if (i < resultMaterials.arraySize)
                        {
                            EditorGUILayout.Separator();
                            SerializedProperty resMat = resultMaterials.GetArrayElementAtIndex(i);
                            EditorGUILayout.PropertyField(resMat.FindPropertyRelative("combinedMaterial"));
                            EditorGUILayout.PropertyField(resMat.FindPropertyRelative("considerMeshUVs"));
                            SerializedProperty sourceMats = resMat.FindPropertyRelative("sourceMaterials");
                            EditorGUILayout.PropertyField(sourceMats, true);
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.PropertyField(resultMaterial, new GUIContent("Combined Mesh Material"));
            }

            int labelWidth = 200;
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(editorBoxBackgroundStyle);
            EditorGUILayout.LabelField("Material Bake Options", EditorStyles.boldLabel);

            DrawPropertyFieldWithLabelWidth(atlasPadding, gc_atlasPadding, labelWidth);
            DrawPropertyFieldWithLabelWidth(maxAtlasSize, maxAtlasSizeGUIContent, labelWidth);
            DrawPropertyFieldWithLabelWidth(resizePowerOfTwoTextures, resizePowerOfTwoGUIContent, labelWidth);
            DrawPropertyFieldWithLabelWidth(maxTilingBakeSize, maxTilingBakeSizeGUIContent, labelWidth);
            EditorGUI.BeginDisabledGroup(momm.doMultiMaterial);
            DrawPropertyFieldWithLabelWidth(considerMeshUVs, fixOutOfBoundsGUIContent, labelWidth);
            EditorGUI.EndDisabledGroup();
            if (texturePackingAlgorithm.intValue == (int)MB2_PackingAlgorithmEnum.MeshBakerTexturePacker ||
                texturePackingAlgorithm.intValue == (int)MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
            {
                DrawPropertyFieldWithLabelWidth(forcePowerOfTwoAtlas, forcePowerOfTwoAtlasContent, labelWidth);
            }
            DrawPropertyFieldWithLabelWidth(considerNonTextureProperties, considerNonTexturePropertiesContent, labelWidth);
            if (texturePackingAlgorithm.intValue == (int)MB2_PackingAlgorithmEnum.UnitysPackTextures)
            {
                EditorGUILayout.HelpBox("Unity's texture packer has memory problems and frequently crashes the editor.", MessageType.Warning);
            }
            EditorGUILayout.PropertyField(texturePackingAlgorithm, texturePackingAgorithmGUIContent);
            if (MB3_TextureCombinerPipeline.USE_EXPERIMENTAL_HOIZONTALVERTICAL)
            {
                if (texturePackingAlgorithm.enumValueIndex == (int)MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal)
                {

                    EditorGUILayout.PropertyField(useMaxAtlasWidthOverride, gc_useMaxAtlasWidthOverride);
                    if (!useMaxAtlasWidthOverride.boolValue) EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(maxAtlasWidthOverride, gc_overrideMaxAtlasWidth);
                    if (!useMaxAtlasWidthOverride.boolValue) EditorGUI.EndDisabledGroup();
                }
                else if (texturePackingAlgorithm.enumValueIndex == (int)MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical)
                {
                    EditorGUILayout.PropertyField(useMaxAtlasHeightOverride, gc_useMaxAtlasHeightOverride);
                    if (!useMaxAtlasHeightOverride.boolValue) EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(maxAtlasHeightOverride, gc_overrideMaxAtlasHeight);
                    if (!useMaxAtlasHeightOverride.boolValue) EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUILayout.PropertyField(customShaderProperties, customShaderPropertyNamesGUIContent, true);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            Color oldColor = GUI.backgroundColor;
            GUI.color = buttonColor;
            if (GUILayout.Button("Bake Materials Into Combined Material"))
            {
                momm.CreateAtlases(updateProgressBar, true, new MB3_EditorMethods());
                EditorUtility.ClearProgressBar();
                if (momm.textureBakeResults != null) EditorUtility.SetDirty(momm.textureBakeResults);
            }
            GUI.backgroundColor = oldColor;
            textureBaker.ApplyModifiedProperties();
            if (GUI.changed)
            {
                textureBaker.SetIsDifferentCacheDirty();
            }
        }



        public void DrawPropertyFieldWithLabelWidth(SerializedProperty prop, GUIContent content, int labelWidth)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(content, GUILayout.Width(labelWidth));
            EditorGUILayout.PropertyField(prop, noneContent);
            EditorGUILayout.EndHorizontal();
        }

        public void updateProgressBar(string msg, float progress)
        {
            EditorUtility.DisplayProgressBar("Combining Meshes", msg, progress);
        }

        /* tried to see if the MultiMaterialConfig could be done using the GroupBy filters. Saddly it didn't work */
        public static void ConfigureMutiMaterialsFromObjsToCombine2(MB3_TextureBaker mom, SerializedProperty resultMaterials, SerializedObject textureBaker)
        {
            if (mom.GetObjectsToCombine().Count == 0)
            {
                Debug.LogError("You need to add some objects to combine before building the multi material list.");
                return;
            }
            if (resultMaterials.arraySize > 0)
            {
                Debug.LogError("You already have some source to combined material mappings configured. You must remove these before doing this operation.");
                return;
            }
            if (mom.textureBakeResults == null)
            {
                Debug.LogError("Texture Bake Result asset must be set before using this operation.");
                return;
            }

            //validate that the objects to be combined are valid
            for (int i = 0; i < mom.GetObjectsToCombine().Count; i++)
            {
                GameObject go = mom.GetObjectsToCombine()[i];
                if (go == null)
                {
                    Debug.LogError("Null object in list of objects to combine at position " + i);
                    return;
                }
                Renderer r = go.GetComponent<Renderer>();
                if (r == null || (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer)))
                {
                    Debug.LogError("GameObject at position " + i + " in list of objects to combine did not have a renderer");
                    return;
                }
                if (r.sharedMaterial == null)
                {
                    Debug.LogError("GameObject at position " + i + " in list of objects to combine has a null material");
                    return;
                }
            }

            IGroupByFilter[] filters = new IGroupByFilter[3];
            filters[0] = new GroupByOutOfBoundsUVs();
            filters[1] = new GroupByShader();
            filters[2] = new MB3_GroupByStandardShaderType();

            List<GameObjectFilterInfo> gameObjects = new List<GameObjectFilterInfo>();
            HashSet<GameObject> objectsAlreadyIncludedInBakers = new HashSet<GameObject>();
            for (int i = 0; i < mom.GetObjectsToCombine().Count; i++)
            {
                GameObjectFilterInfo goaw = new GameObjectFilterInfo(mom.GetObjectsToCombine()[i], objectsAlreadyIncludedInBakers, filters);
                if (goaw.materials.Length > 0) //don't consider renderers with no materials
                {
                    gameObjects.Add(goaw);
                }
            }

            //analyse meshes
            Dictionary<int, MB_Utility.MeshAnalysisResult> meshAnalysisResultCache = new Dictionary<int, MB_Utility.MeshAnalysisResult>();
            int totalVerts = 0;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                //string rpt = String.Format("Processing {0} [{1} of {2}]", gameObjects[i].go.name, i, gameObjects.Count);
                //EditorUtility.DisplayProgressBar("Analysing Scene", rpt + " A", .6f);
                Mesh mm = MB_Utility.GetMesh(gameObjects[i].go);
                int nVerts = 0;
                if (mm != null)
                {
                    nVerts += mm.vertexCount;
                    MB_Utility.MeshAnalysisResult mar;
                    if (!meshAnalysisResultCache.TryGetValue(mm.GetInstanceID(), out mar))
                    {

                        //EditorUtility.DisplayProgressBar("Analysing Scene", rpt + " Check Out Of Bounds UVs", .6f);
                        MB_Utility.hasOutOfBoundsUVs(mm, ref mar);
                        //Rect dummy = mar.uvRect;
                        MB_Utility.doSubmeshesShareVertsOrTris(mm, ref mar);
                        meshAnalysisResultCache.Add(mm.GetInstanceID(), mar);
                    }
                    if (mar.hasOutOfBoundsUVs)
                    {
                        int w = (int)mar.uvRect.width;
                        int h = (int)mar.uvRect.height;
                        gameObjects[i].outOfBoundsUVs = true;
                        gameObjects[i].warning += " [WARNING: has uvs outside the range (0,1) tex is tiled " + w + "x" + h + " times]";
                    }
                    if (mar.hasOverlappingSubmeshVerts)
                    {
                        gameObjects[i].submeshesOverlap = true;
                        gameObjects[i].warning += " [WARNING: Submeshes share verts or triangles. 'Multiple Combined Materials' feature may not work.]";
                    }
                }
                totalVerts += nVerts;
                //EditorUtility.DisplayProgressBar("Analysing Scene", rpt + " Validate OBuvs Multi Material", .6f);
                Renderer mr = gameObjects[i].go.GetComponent<Renderer>();
                if (!MB_Utility.AreAllSharedMaterialsDistinct(mr.sharedMaterials))
                {
                    gameObjects[i].warning += " [WARNING: Object uses same material on multiple submeshes. This may produce poor results when used with multiple materials or fix out of bounds uvs.]";
                }
            }

            List<GameObjectFilterInfo> objsNotAddedToBaker = new List<GameObjectFilterInfo>();

            Dictionary<GameObjectFilterInfo, List<List<GameObjectFilterInfo>>> gs2bakeGroupMap = MB3_MeshBakerEditorWindow.sortIntoBakeGroups3(gameObjects, objsNotAddedToBaker, filters, false, mom.maxAtlasSize);

            mom.resultMaterials = new MB_MultiMaterial[gs2bakeGroupMap.Keys.Count];
            string pth = AssetDatabase.GetAssetPath(mom.textureBakeResults);
            string baseName = Path.GetFileNameWithoutExtension(pth);
            string folderPath = pth.Substring(0, pth.Length - baseName.Length - 6);
            int k = 0;
            foreach (GameObjectFilterInfo m in gs2bakeGroupMap.Keys)
            {
                MB_MultiMaterial mm = mom.resultMaterials[k] = new MB_MultiMaterial();
                mm.sourceMaterials = new List<Material>();
                mm.sourceMaterials.Add(m.materials[0]);
                string matName = folderPath + baseName + "-mat" + k + ".mat";
                Material newMat = new Material(Shader.Find("Diffuse"));
                MB3_TextureBaker.ConfigureNewMaterialToMatchOld(newMat, m.materials[0]);
                AssetDatabase.CreateAsset(newMat, matName);
                mm.combinedMaterial = (Material)AssetDatabase.LoadAssetAtPath(matName, typeof(Material));
                k++;
            }
            MBVersionEditor.UpdateIfDirtyOrScript(textureBaker);
        }

        public class MultiMatSubmeshInfo
        {
            public Shader shader;
            public GameObjectFilterInfo.StandardShaderBlendMode stdBlendMode = GameObjectFilterInfo.StandardShaderBlendMode.NotApplicable;

            public MultiMatSubmeshInfo(Shader s, Material m)
            {
                shader = s;
                if (m.shader.name.StartsWith("Standard") && m.HasProperty("_Mode"))
                {
                    stdBlendMode = (GameObjectFilterInfo.StandardShaderBlendMode)m.GetFloat("_Mode");
                }
                else
                {
                    stdBlendMode = GameObjectFilterInfo.StandardShaderBlendMode.NotApplicable;
                }
            }

            public override bool Equals(object obj)
            {
                MultiMatSubmeshInfo b = (MultiMatSubmeshInfo)obj;
                return (stdBlendMode == b.stdBlendMode && shader == b.shader);
            }

            public override int GetHashCode()
            {
                return shader.GetHashCode() ^ (int)stdBlendMode;
            }
        }

        //posibilities
        //  using fixOutOfBoundsUVs or not 
        //  
        public static void ConfigureMutiMaterialsFromObjsToCombine(MB3_TextureBaker mom, SerializedProperty resultMaterials, SerializedObject textureBaker)
        {
            if (mom.GetObjectsToCombine().Count == 0)
            {
                Debug.LogError("You need to add some objects to combine before building the multi material list.");
                return;
            }
            if (resultMaterials.arraySize > 0)
            {
                Debug.LogError("You already have some source to combined material mappings configured. You must remove these before doing this operation.");
                return;
            }
            if (mom.textureBakeResults == null)
            {
                Debug.LogError("Texture Bake Result asset must be set before using this operation.");
                return;
            }
            Dictionary<MultiMatSubmeshInfo, List<List<Material>>> shader2Material_map = new Dictionary<MultiMatSubmeshInfo, List<List<Material>>>();
            Dictionary<Material, Mesh> obUVobject2mesh_map = new Dictionary<Material, Mesh>();

            //validate that the objects to be combined are valid
            for (int i = 0; i < mom.GetObjectsToCombine().Count; i++)
            {
                GameObject go = mom.GetObjectsToCombine()[i];
                if (go == null)
                {
                    Debug.LogError("Null object in list of objects to combine at position " + i);
                    return;
                }
                Renderer r = go.GetComponent<Renderer>();
                if (r == null || (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer)))
                {
                    Debug.LogError("GameObject at position " + i + " in list of objects to combine did not have a renderer");
                    return;
                }
                if (r.sharedMaterial == null)
                {
                    Debug.LogError("GameObject at position " + i + " in list of objects to combine has a null material");
                    return;
                }
            }

            //first pass put any meshes with obUVs on their own submesh if not fixing OB uvs
            if (mom.doMultiMaterialSplitAtlasesIfOBUVs)
            {
                for (int i = 0; i < mom.GetObjectsToCombine().Count; i++)
                {
                    GameObject go = mom.GetObjectsToCombine()[i];
                    Mesh m = MB_Utility.GetMesh(go);
                    MB_Utility.MeshAnalysisResult dummyMar = new MB_Utility.MeshAnalysisResult();
                    Renderer r = go.GetComponent<Renderer>();
                    for (int j = 0; j < r.sharedMaterials.Length; j++)
                    {
                        if (MB_Utility.hasOutOfBoundsUVs(m, ref dummyMar, j))
                        {
                            if (!obUVobject2mesh_map.ContainsKey(r.sharedMaterials[j]))
                            {
                                Debug.LogWarning("Object " + go + " submesh " + j + " uses UVs outside the range 0,0..1,1 to generate tiling. This object has been mapped to its own submesh in the combined mesh. It can share a submesh with other objects that use different materials if you use the fix out of bounds UVs feature which will bake the tiling");
                                obUVobject2mesh_map.Add(r.sharedMaterials[j], m);
                            }
                        }
                    }
                }
            }

            //second pass  put other materials without OB uvs in a shader to material map
            for (int i = 0; i < mom.GetObjectsToCombine().Count; i++)
            {
                Renderer r = mom.GetObjectsToCombine()[i].GetComponent<Renderer>();
                for (int j = 0; j < r.sharedMaterials.Length; j++)
                {
                    if (!obUVobject2mesh_map.ContainsKey(r.sharedMaterials[j]))
                    { //if not already added
                        if (r.sharedMaterials[j] == null) continue;
                        List<List<Material>> binsOfMatsThatUseShader = null;
                        MultiMatSubmeshInfo newKey = new MultiMatSubmeshInfo(r.sharedMaterials[j].shader, r.sharedMaterials[j]);
                        if (!shader2Material_map.TryGetValue(newKey, out binsOfMatsThatUseShader))
                        {
                            binsOfMatsThatUseShader = new List<List<Material>>();
                            binsOfMatsThatUseShader.Add(new List<Material>());
                            shader2Material_map.Add(newKey, binsOfMatsThatUseShader);
                        }
                        if (!binsOfMatsThatUseShader[0].Contains(r.sharedMaterials[j])) binsOfMatsThatUseShader[0].Add(r.sharedMaterials[j]);
                    }
                }
            }

            int numResMats = shader2Material_map.Count;
            //third pass for each shader grouping check how big the atlas would be and group into bins that would fit in an atlas
            if (mom.doMultiMaterialSplitAtlasesIfTooBig)
            {
                if (mom.packingAlgorithm == MB2_PackingAlgorithmEnum.UnitysPackTextures)
                {
                    Debug.LogWarning("Unity texture packer does not support splitting atlases if too big. Atlases will not be split.");
                }
                else
                {
                    numResMats = 0;
                    foreach (MultiMatSubmeshInfo sh in shader2Material_map.Keys)
                    {
                        List<List<Material>> binsOfMatsThatUseShader = shader2Material_map[sh];
                        List<Material> allMatsThatUserShader = binsOfMatsThatUseShader[0];//at this point everything is in the same list
                        binsOfMatsThatUseShader.RemoveAt(0);
                        MB3_TextureCombiner combiner = mom.CreateAndConfigureTextureCombiner();
                        combiner.saveAtlasesAsAssets = false;
                        if (allMatsThatUserShader.Count > 1) combiner.fixOutOfBoundsUVs = mom.fixOutOfBoundsUVs;
                        else combiner.fixOutOfBoundsUVs = false;

                        // Do the texture pack
                        List<AtlasPackingResult> packingResults = new List<AtlasPackingResult>();
                        Material tempMat = new Material(sh.shader);
                        combiner.CombineTexturesIntoAtlases(null, null, tempMat, mom.GetObjectsToCombine(), allMatsThatUserShader, null, packingResults, true, true);
                        for (int i = 0; i < packingResults.Count; i++)
                        {

                            List<MatsAndGOs> matsData = (List<MatsAndGOs>)packingResults[i].data;
                            List<Material> mats = new List<Material>();
                            for (int j = 0; j < matsData.Count; j++)
                            {
                                for (int kk = 0; kk < matsData[j].mats.Count; kk++)
                                {
                                    if (!mats.Contains(matsData[j].mats[kk].mat))
                                    {
                                        mats.Add(matsData[j].mats[kk].mat);
                                    }
                                }
                            }
                            binsOfMatsThatUseShader.Add(mats);
                        }
                        numResMats += binsOfMatsThatUseShader.Count;
                    }
                }
            }

            //build the result materials
            if (shader2Material_map.Count == 0 && obUVobject2mesh_map.Count == 0) Debug.LogError("Found no materials in list of objects to combine");
            mom.resultMaterials = new MB_MultiMaterial[numResMats + obUVobject2mesh_map.Count];
            string pth = AssetDatabase.GetAssetPath(mom.textureBakeResults);
            string baseName = Path.GetFileNameWithoutExtension(pth);
            string folderPath = pth.Substring(0, pth.Length - baseName.Length - 6);
            int k = 0;
            foreach (MultiMatSubmeshInfo sh in shader2Material_map.Keys)
            {
                foreach (List<Material> matsThatUse in shader2Material_map[sh])
                {
                    MB_MultiMaterial mm = mom.resultMaterials[k] = new MB_MultiMaterial();
                    mm.sourceMaterials = matsThatUse;
                    if (mm.sourceMaterials.Count == 1)
                    {
                        mm.considerMeshUVs = false;
                    }
                    else
                    {
                        mm.considerMeshUVs = mom.fixOutOfBoundsUVs;
                    }
                    string matName = folderPath + baseName + "-mat" + k + ".mat";
                    Material newMat = new Material(Shader.Find("Diffuse"));
                    if (matsThatUse.Count > 0 && matsThatUse[0] != null)
                    {
                        MB3_TextureBaker.ConfigureNewMaterialToMatchOld(newMat, matsThatUse[0]);
                    }
                    AssetDatabase.CreateAsset(newMat, matName);
                    mm.combinedMaterial = (Material)AssetDatabase.LoadAssetAtPath(matName, typeof(Material));
                    k++;
                }
            }
            foreach (Material m in obUVobject2mesh_map.Keys)
            {
                MB_MultiMaterial mm = mom.resultMaterials[k] = new MB_MultiMaterial();
                mm.sourceMaterials = new List<Material>();
                mm.sourceMaterials.Add(m);
                mm.considerMeshUVs = false;
                string matName = folderPath + baseName + "-mat" + k + ".mat";
                Material newMat = new Material(Shader.Find("Diffuse"));
                MB3_TextureBaker.ConfigureNewMaterialToMatchOld(newMat, m);
                AssetDatabase.CreateAsset(newMat, matName);
                mm.combinedMaterial = (Material)AssetDatabase.LoadAssetAtPath(matName, typeof(Material));
                k++;
            }
            MBVersionEditor.UpdateIfDirtyOrScript(textureBaker);
        }

        public static void CreateCombinedMaterialAssets(MB3_TextureBaker target, string pth)
        {
            MB3_TextureBaker mom = (MB3_TextureBaker)target;
            string baseName = Path.GetFileNameWithoutExtension(pth);
            if (baseName == null || baseName.Length == 0) return;
            string folderPath = pth.Substring(0, pth.Length - baseName.Length - 6);

            List<string> matNames = new List<string>();
            if (mom.doMultiMaterial)
            {
                for (int i = 0; i < mom.resultMaterials.Length; i++)
                {
                    matNames.Add(folderPath + baseName + "-mat" + i + ".mat");
                    AssetDatabase.CreateAsset(new Material(Shader.Find("Diffuse")), matNames[i]);
                    mom.resultMaterials[i].combinedMaterial = (Material)AssetDatabase.LoadAssetAtPath(matNames[i], typeof(Material));
                }
            }
            else
            {
                matNames.Add(folderPath + baseName + "-mat.mat");
                Material newMat = null;
                if (mom.GetObjectsToCombine().Count > 0 && mom.GetObjectsToCombine()[0] != null)
                {
                    Renderer r = mom.GetObjectsToCombine()[0].GetComponent<Renderer>();
                    if (r == null)
                    {
                        Debug.LogWarning("Object " + mom.GetObjectsToCombine()[0] + " does not have a Renderer on it.");
                    }
                    else
                    {
                        if (r.sharedMaterial != null)
                        {
                            newMat = new Material(r.sharedMaterial);
                            //newMat.shader = r.sharedMaterial.shader;					
                            MB3_TextureBaker.ConfigureNewMaterialToMatchOld(newMat, r.sharedMaterial);
                        }
                    }
                }
                else
                {
                    Debug.Log("If you add objects to be combined before creating the Combined Material Assets. Then Mesh Baker will create a result material that is a duplicate of the material on the first object to be combined. This saves time configuring the shader.");
                }
                if (newMat == null)
                {
                    newMat = new Material(Shader.Find("Diffuse"));
                }
                AssetDatabase.CreateAsset(newMat, matNames[0]);
                mom.resultMaterial = (Material)AssetDatabase.LoadAssetAtPath(matNames[0], typeof(Material));
            }
            //create the MB2_TextureBakeResults
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MB2_TextureBakeResults>(), pth);
            mom.textureBakeResults = (MB2_TextureBakeResults)AssetDatabase.LoadAssetAtPath(pth, typeof(MB2_TextureBakeResults));
            AssetDatabase.Refresh();
        }
    }
}
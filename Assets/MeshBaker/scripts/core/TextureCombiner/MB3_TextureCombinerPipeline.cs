//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

/*
    Test different texture packers
    Test lots of multiple material configs
    Try using on Coast scene
*/

/*
  
Notes on Normal Maps in Unity3d

Unity stores normal maps in a non standard format for some platforms. Think of the standard format as being english, unity's as being
french. The raw image files in the project folder are in english, the AssetImporter converts them to french. Texture2D.GetPixels returns 
french. This is a problem when we build an atlas from Texture2D objects and save the result in the project folder.
Unity wants us to flag this file as a normal map but if we do it is effectively translated twice.

Solutions:

    1) convert the normal map to english just before saving to project. Then set the normal flag and let the Importer do translation.
    This was rejected because Unity doesn't translate for all platforms. I would need to check with every version of Unity which platforms
    use which format.

    2) Uncheck "normal map" on importer before bake and re-check after bake. This is the solution I am using.

*/
namespace DigitalOpus.MB.Core
{
    public class MB3_TextureCombinerPipeline
    {
        public static bool USE_EXPERIMENTAL_HOIZONTALVERTICAL = true;

        public struct CreateAtlasForProperty
        {
            public bool allTexturesAreNull;
            public bool allTexturesAreSame;
            public bool allNonTexturePropsAreSame;

            public override string ToString()
            {
                return String.Format("AllTexturesNull={0} areSame={1} nonTexPropsAreSame={2}", allTexturesAreNull, allTexturesAreSame, allNonTexturePropsAreSame);
            }
        }

        public static ShaderTextureProperty[] shaderTexPropertyNames = new ShaderTextureProperty[] {
            new ShaderTextureProperty("_MainTex",false),
            new ShaderTextureProperty("_BumpMap",true),
            new ShaderTextureProperty("_Normal",true),
            new ShaderTextureProperty("_BumpSpecMap",false),
            new ShaderTextureProperty("_DecalTex",false),
            new ShaderTextureProperty("_Detail",false),
            new ShaderTextureProperty("_GlossMap",false),
            new ShaderTextureProperty("_Illum",false),
            new ShaderTextureProperty("_LightTextureB0",false),
            new ShaderTextureProperty("_ParallaxMap",false),
            new ShaderTextureProperty("_ShadowOffset",false),
            new ShaderTextureProperty("_TranslucencyMap",false),
            new ShaderTextureProperty("_SpecMap",false),
            new ShaderTextureProperty("_SpecGlossMap",false),
            new ShaderTextureProperty("_TranspMap",false),
            new ShaderTextureProperty("_MetallicGlossMap",false),
            new ShaderTextureProperty("_OcclusionMap",false),
            new ShaderTextureProperty("_EmissionMap",false),
            new ShaderTextureProperty("_DetailMask",false), 
//			new ShaderTextureProperty("_DetailAlbedoMap",false), 
//			new ShaderTextureProperty("_DetailNormalMap",true),
		};

        internal class TexturePipelineData
        {
            internal MB2_TextureBakeResults _textureBakeResults;
            internal int _atlasPadding = 1;
            internal int _maxAtlasWidth = 1;
            internal int _maxAtlasHeight = 1;
            internal bool _useMaxAtlasHeightOverride = false;
            internal bool _useMaxAtlasWidthOverride = false;
            internal bool _resizePowerOfTwoTextures = false;
            internal bool _fixOutOfBoundsUVs = false;
            internal int _maxTilingBakeSize = 1024;
            internal bool _saveAtlasesAsAssets = false;
            internal MB2_PackingAlgorithmEnum _packingAlgorithm = MB2_PackingAlgorithmEnum.UnitysPackTextures;
            internal bool _meshBakerTexturePackerForcePowerOfTwo = true;
            internal List<ShaderTextureProperty> _customShaderPropNames = new List<ShaderTextureProperty>();
            internal bool _normalizeTexelDensity = false;
            internal bool _considerNonTextureProperties = false;
            internal MB3_TextureCombinerNonTextureProperties nonTexturePropertyBlender;
            internal List<MB_TexSet> distinctMaterialTextures;
            internal List<GameObject> allObjsToMesh;
            internal List<Material> allowedMaterialsFilter;
            internal List<ShaderTextureProperty> texPropertyNames;
            internal CreateAtlasForProperty[] allTexturesAreNullAndSameColor;
            internal int numAtlases { get
                {
                    if (texPropertyNames != null) return texPropertyNames.Count;
                    else return 0;
                }
            }
            internal Material resultMaterial;

            internal bool OnlyOneTextureInAtlasReuseTextures()
            {
                if (distinctMaterialTextures != null &&
                    distinctMaterialTextures.Count == 1 &&
                    distinctMaterialTextures[0].thisIsOnlyTexSetInAtlas == true &&
                    !_fixOutOfBoundsUVs && 
                    !_considerNonTextureProperties)
                {
                    return true;
                }
                return false;
            }
        }

        internal static bool _ShouldWeCreateAtlasForThisProperty(int propertyIndex, bool considerNonTextureProperties, CreateAtlasForProperty[] allTexturesAreNullAndSameColor)
        {
            CreateAtlasForProperty v = allTexturesAreNullAndSameColor[propertyIndex];
            if (considerNonTextureProperties)
            {
                if (!v.allNonTexturePropsAreSame || !v.allTexturesAreNull)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (!v.allTexturesAreNull)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal static bool _CollectPropertyNames(TexturePipelineData data, MB2_LogLevel LOG_LEVEL)
        {
            //try custom properties remove duplicates
            for (int i = 0; i < data.texPropertyNames.Count; i++)
            {
                ShaderTextureProperty s = data._customShaderPropNames.Find(x => x.name.Equals(data.texPropertyNames[i].name));
                if (s != null)
                {
                    data._customShaderPropNames.Remove(s);
                }
            }

            Material m = data.resultMaterial;
            if (m == null)
            {
                Debug.LogError("Please assign a result material. The combined mesh will use this material.");
                return false;
            }

            //Collect the property names for the textures
            string shaderPropStr = "";
            for (int i = 0; i < shaderTexPropertyNames.Length; i++)
            {
                if (m.HasProperty(shaderTexPropertyNames[i].name))
                {
                    shaderPropStr += ", " + shaderTexPropertyNames[i].name;
                    if (!data.texPropertyNames.Contains(shaderTexPropertyNames[i])) data.texPropertyNames.Add(shaderTexPropertyNames[i]);
                    if (m.GetTextureOffset(shaderTexPropertyNames[i].name) != new Vector2(0f, 0f))
                    {
                        if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Result material has non-zero offset. This is may be incorrect.");
                    }
                    if (m.GetTextureScale(shaderTexPropertyNames[i].name) != new Vector2(1f, 1f))
                    {
                        if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Result material should have tiling of 1,1");
                    }
                }
            }

            for (int i = 0; i < data._customShaderPropNames.Count; i++)
            {
                if (m.HasProperty(data._customShaderPropNames[i].name))
                {
                    shaderPropStr += ", " + data._customShaderPropNames[i].name;
                    data.texPropertyNames.Add(data._customShaderPropNames[i]);
                    if (m.GetTextureOffset(data._customShaderPropNames[i].name) != new Vector2(0f, 0f))
                    {
                        if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Result material has non-zero offset. This is probably incorrect.");
                    }
                    if (m.GetTextureScale(data._customShaderPropNames[i].name) != new Vector2(1f, 1f))
                    {
                        if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Result material should probably have tiling of 1,1.");
                    }
                }
                else
                {
                    if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Result material shader does not use property " + data._customShaderPropNames[i].name + " in the list of custom shader property names");
                }
            }

            return true;
        }

        internal static bool _ShouldWeCreateAtlasForThisProperty(int propertyIndex, CreateAtlasForProperty[] allTexturesAreNullAndSameColor, TexturePipelineData data)
        {
            CreateAtlasForProperty v = allTexturesAreNullAndSameColor[propertyIndex];
            if (data._considerNonTextureProperties)
            {
                if (!v.allNonTexturePropsAreSame || !v.allTexturesAreNull)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (!v.allTexturesAreNull)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Some shaders like the Standard shader have texture properties like Emission which can be set on the material
        /// but are disabled using keywords. In these cases the textures should not be returned.
        /// </summary>
        public static Texture GetTextureConsideringStandardShaderKeywords(string shaderName, Material mat, string propertyName)
        {
            if (shaderName.Equals("Standard") || shaderName.Equals("Standard (Specular setup)") || shaderName.Equals("Standard (Roughness setup"))
            {
                if (propertyName.Equals("_EmissionMap"))
                {
                    if (mat.IsKeywordEnabled("_EMISSION"))
                    {
                        return mat.GetTexture(propertyName);
                    } else
                    {
                        return null;
                    }
                }
            }
            return mat.GetTexture(propertyName);
        }

        /// <summary>
        /// Fills distinctMaterialTextures (a list of TexSets) and usedObjsToMesh. Each TexSet is a rectangle in the set of atlases.
        /// If allowedMaterialsFilter is empty then all materials on allObjsToMesh will be collected and usedObjsToMesh will be same as allObjsToMesh
        /// else only materials in allowedMaterialsFilter will be included and usedObjsToMesh will be objs that use those materials.
        /// bool __step1_CollectDistinctMatTexturesAndUsedObjects;
        /// </summary>
        internal static IEnumerator __Step1_CollectDistinctMatTexturesAndUsedObjects(ProgressUpdateDelegate progressInfo,
                                                            MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult result,
                                                            TexturePipelineData data,
                                                            MB3_TextureCombiner combiner,
                                                            MB2_EditorMethodsInterface textureEditorMethods,
                                                            List<GameObject> usedObjsToMesh,
                                                            MB2_LogLevel LOG_LEVEL
            )
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            // Collect distinct list of textures to combine from the materials on objsToCombine
            bool outOfBoundsUVs = false;
            Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisResultsCache = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>(); //cache results
            for (int i = 0; i < data.allObjsToMesh.Count; i++)
            {
                GameObject obj = data.allObjsToMesh[i];
                if (progressInfo != null) progressInfo("Collecting textures for " + obj, ((float)i) / data.allObjsToMesh.Count / 2f);
                if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Collecting textures for object " + obj);

                if (obj == null)
                {
                    Debug.LogError("The list of objects to mesh contained nulls.");
                    result.success = false;
                    yield break;
                }

                Mesh sharedMesh = MB_Utility.GetMesh(obj);
                if (sharedMesh == null)
                {
                    Debug.LogError("Object " + obj.name + " in the list of objects to mesh has no mesh.");
                    result.success = false;
                    yield break;
                }

                Material[] sharedMaterials = MB_Utility.GetGOMaterials(obj);
                if (sharedMaterials.Length == 0)
                {
                    Debug.LogError("Object " + obj.name + " in the list of objects has no materials.");
                    result.success = false;
                    yield break;
                }

                //analyze mesh or grab cached result of previous analysis, stores one result for each submesh
                MB_Utility.MeshAnalysisResult[] mar;
                if (!meshAnalysisResultsCache.TryGetValue(sharedMesh.GetInstanceID(), out mar))
                {
                    mar = new MB_Utility.MeshAnalysisResult[sharedMesh.subMeshCount];
                    for (int j = 0; j < sharedMesh.subMeshCount; j++)
                    {
                        MB_Utility.hasOutOfBoundsUVs(sharedMesh, ref mar[j], j);
                        if (data._normalizeTexelDensity)
                        {
                            mar[j].submeshArea = GetSubmeshArea(sharedMesh, j);
                        }

                        if (data._fixOutOfBoundsUVs && !mar[j].hasUVs)
                        {
                            //assume UVs will be generated if this feature is being used and generated UVs will be 0,0,1,1
                            mar[j].uvRect = new Rect(0, 0, 1, 1);
                            Debug.LogWarning("Mesh for object " + obj + " has no UV channel but 'consider UVs' is enabled. Assuming UVs will be generated filling 0,0,1,1 rectangle.");
                        }
                    }
                    meshAnalysisResultsCache.Add(sharedMesh.GetInstanceID(), mar);
                }

                if (data._fixOutOfBoundsUVs && LOG_LEVEL >= MB2_LogLevel.trace)
                {
                    Debug.Log("Mesh Analysis for object " + obj + " numSubmesh=" + mar.Length + " HasOBUV=" + mar[0].hasOutOfBoundsUVs + " UVrectSubmesh0=" + mar[0].uvRect);
                }

                for (int matIdx = 0; matIdx < sharedMaterials.Length; matIdx++)
                { //for each submesh
                    if (progressInfo != null) progressInfo(String.Format("Collecting textures for {0} submesh {1}", obj, matIdx), ((float)i) / data.allObjsToMesh.Count / 2f);
                    Material mat = sharedMaterials[matIdx];

                    //check if this material is in the list of source materaials
                    if (data.allowedMaterialsFilter != null && !data.allowedMaterialsFilter.Contains(mat))
                    {
                        continue;
                    }

                    //Rect uvBounds = mar[matIdx].sourceUVRect;
                    outOfBoundsUVs = outOfBoundsUVs || mar[matIdx].hasOutOfBoundsUVs;

                    if (mat.name.Contains("(Instance)"))
                    {
                        Debug.LogError("The sharedMaterial on object " + obj.name + " has been 'Instanced'. This was probably caused by a script accessing the meshRender.material property in the editor. " +
                                       " The material to UV Rectangle mapping will be incorrect. To fix this recreate the object from its prefab or re-assign its material from the correct asset.");
                        result.success = false;
                        yield break;
                    }

                    if (data._fixOutOfBoundsUVs)
                    {
                        if (!MB_Utility.AreAllSharedMaterialsDistinct(sharedMaterials))
                        {
                            if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Object " + obj.name + " uses the same material on multiple submeshes. This may generate strange resultAtlasesAndRects especially when used with fix out of bounds uvs. Try duplicating the material.");
                        }
                    }

                    //need to set up procedural material before converting its texs to texture2D
                    /*
                    if (mat is ProceduralMaterial)
                    {
                        combiner._addProceduralMaterial((ProceduralMaterial)mat);
                    }
                    */


                    //collect textures scale and offset for each texture in objects material
                    MeshBakerMaterialTexture[] mts = new MeshBakerMaterialTexture[data.texPropertyNames.Count];
                    for (int propIdx = 0; propIdx < data.texPropertyNames.Count; propIdx++)
                    {
                        Texture tx = null;
                        Vector2 scale = Vector2.one;
                        Vector2 offset = Vector2.zero;
                        float texelDensity = 0f;
                        if (mat.HasProperty(data.texPropertyNames[propIdx].name))
                        {
                            Texture txx = GetTextureConsideringStandardShaderKeywords(data.resultMaterial.shader.name, mat, data.texPropertyNames[propIdx].name);
                            if (txx != null)
                            {
                                if (txx is Texture2D)
                                {
                                    tx = txx;
                                    TextureFormat f = ((Texture2D)tx).format;
                                    bool isNormalMap = false;
                                    if (!Application.isPlaying && textureEditorMethods != null) isNormalMap = textureEditorMethods.IsNormalMap((Texture2D)tx);
                                    if ((f == TextureFormat.ARGB32 ||
                                        f == TextureFormat.RGBA32 ||
                                        f == TextureFormat.BGRA32 ||
                                        f == TextureFormat.RGB24 ||
                                        f == TextureFormat.Alpha8) && !isNormalMap) //DXT5 does not work
                                    {
                                        //good
                                    }
                                    else
                                    {
                                        //TRIED to copy texture using tex2.SetPixels(tex1.GetPixels()) but bug in 3.5 means DTX1 and 5 compressed textures come out skewe
                                        if (Application.isPlaying && data._packingAlgorithm != MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
                                        {
                                            Debug.LogWarning("Object " + obj.name + " in the list of objects to mesh uses Texture " + tx.name + " uses format " + f + " that is not in: ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT. These textures cannot be resized at runtime. Try changing texture format. If format says 'compressed' try changing it to 'truecolor'");
                                            result.success = false;
                                            yield break;
                                        }
                                        else
                                        {
                                            tx = (Texture2D)mat.GetTexture(data.texPropertyNames[propIdx].name);
                                        }
                                    }
                                }
                                /*
                                else if (txx is ProceduralTexture)
                                {
                                    //if (!MBVersion.IsTextureFormatRaw(((ProceduralTexture)txx).format))
                                    //{
                                    //    Debug.LogError("Object " + obj.name + " in the list of objects to mesh uses a ProceduarlTexture that is not in a RAW format. Convert textures to RAW.");
                                    //    result.success = false;
                                    //    yield break;
                                    //}
                                    tx = txx;
                                }
                                */
                                else
                                {
                                    Debug.LogError("Object " + obj.name + " in the list of objects to mesh uses a Texture that is not a Texture2D. Cannot build atlases.");
                                    result.success = false;
                                    yield break;
                                }

                            }

                            if (tx != null && data._normalizeTexelDensity)
                            {
                                //todo this doesn't take into account tiling and out of bounds UV sampling
                                if (mar[propIdx].submeshArea == 0)
                                {
                                    texelDensity = 0f;
                                }
                                else
                                {
                                    texelDensity = (tx.width * tx.height) / (mar[propIdx].submeshArea);
                                }
                            }
                            scale = mat.GetTextureScale(data.texPropertyNames[propIdx].name);
                            offset = mat.GetTextureOffset(data.texPropertyNames[propIdx].name);
                        }
                        mts[propIdx] = new MeshBakerMaterialTexture(tx, offset, scale, texelDensity);
                    }

                    data.nonTexturePropertyBlender.CollectAverageValuesOfNonTextureProperties(data.resultMaterial, mat);

                    Vector2 obUVscale = new Vector2(mar[matIdx].uvRect.width, mar[matIdx].uvRect.height);
                    Vector2 obUVoffset = new Vector2(mar[matIdx].uvRect.x, mar[matIdx].uvRect.y);

                    //Add to distinct set of textures if not already there
                    MB_TextureTilingTreatment tilingTreatment = MB_TextureTilingTreatment.none;
                    if (data._fixOutOfBoundsUVs)
                    {
                        tilingTreatment = MB_TextureTilingTreatment.considerUVs;
                    }

                    MB_TexSet setOfTexs = new MB_TexSet(mts, obUVoffset, obUVscale, tilingTreatment);  //one of these per submesh
                    MatAndTransformToMerged matt = new MatAndTransformToMerged(new DRect(obUVoffset, obUVscale), data._fixOutOfBoundsUVs, mat);
                    setOfTexs.matsAndGOs.mats.Add(matt);
                    MB_TexSet setOfTexs2 = data.distinctMaterialTextures.Find(x => x.IsEqual(setOfTexs, data._fixOutOfBoundsUVs, data.nonTexturePropertyBlender));
                    if (setOfTexs2 != null)
                    {
                        setOfTexs = setOfTexs2;
                    }
                    else
                    {
                        data.distinctMaterialTextures.Add(setOfTexs);
                    }

                    if (!setOfTexs.matsAndGOs.mats.Contains(matt))
                    {
                        setOfTexs.matsAndGOs.mats.Add(matt);
                    }

                    if (!setOfTexs.matsAndGOs.gos.Contains(obj))
                    {
                        setOfTexs.matsAndGOs.gos.Add(obj);
                        if (!usedObjsToMesh.Contains(obj)) usedObjsToMesh.Add(obj);
                    }
                }
            }

            if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log(String.Format("Step1_CollectDistinctTextures collected {0} sets of textures fixOutOfBoundsUV={1} considerNonTextureProperties={2}", data.distinctMaterialTextures.Count, data._fixOutOfBoundsUVs, data._considerNonTextureProperties));

            if (data.distinctMaterialTextures.Count == 0)
            {
                Debug.LogError("None of the source object materials matched any of the allowed materials for this submesh.");
                result.success = false;
                yield break;
            }

            MB3_TextureCombinerMerging merger = new MB3_TextureCombinerMerging(data._considerNonTextureProperties, data.nonTexturePropertyBlender, data._fixOutOfBoundsUVs, LOG_LEVEL);
            merger.MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects(data.distinctMaterialTextures);

            if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Total time Step1_CollectDistinctTextures " + (sw.ElapsedMilliseconds).ToString("f5"));
            yield break;
        }

        private static CreateAtlasForProperty[] CalculateAllTexturesAreNullAndSameColor(MB3_TextureCombinerPipeline.TexturePipelineData data, MB2_LogLevel LOG_LEVEL)
        {
            // check if all textures are null and use same color for each atlas
            // will not generate an atlas if so
            CreateAtlasForProperty[] shouldWeCreateAtlasForProp = new CreateAtlasForProperty[data.texPropertyNames.Count];
            for (int propIdx = 0; propIdx < data.texPropertyNames.Count; propIdx++)
            {
                MeshBakerMaterialTexture firstTexture = data.distinctMaterialTextures[0].ts[propIdx];
                Color firstColor = Color.black;
                if (data._considerNonTextureProperties)
                {
                    firstColor = data.nonTexturePropertyBlender.GetColorAsItWouldAppearInAtlasIfNoTexture(data.distinctMaterialTextures[0].matsAndGOs.mats[0].mat, data.texPropertyNames[propIdx]);
                }
                int numTexturesExisting = 0;
                int numTexturesMatchinFirst = 0;
                int numNonTexturePropertiesMatchingFirst = 0;
                for (int j = 0; j < data.distinctMaterialTextures.Count; j++)
                {
                    if (!data.distinctMaterialTextures[j].ts[propIdx].isNull)
                    {
                        numTexturesExisting++;
                    }
                    if (firstTexture.AreTexturesEqual(data.distinctMaterialTextures[j].ts[propIdx]))
                    {
                        numTexturesMatchinFirst++;
                    }
                    if (data._considerNonTextureProperties)
                    {
                        Color colJ = data.nonTexturePropertyBlender.GetColorAsItWouldAppearInAtlasIfNoTexture(data.distinctMaterialTextures[j].matsAndGOs.mats[0].mat, data.texPropertyNames[propIdx]);
                        if (colJ == firstColor)
                        {
                            numNonTexturePropertiesMatchingFirst++;
                        }
                    }

                }
                shouldWeCreateAtlasForProp[propIdx].allTexturesAreNull = numTexturesExisting == 0;
                shouldWeCreateAtlasForProp[propIdx].allTexturesAreSame = numTexturesMatchinFirst == data.distinctMaterialTextures.Count;
                shouldWeCreateAtlasForProp[propIdx].allNonTexturePropsAreSame = numNonTexturePropertiesMatchingFirst == data.distinctMaterialTextures.Count;
                if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log(String.Format("AllTexturesAreNullAndSameColor prop: {0} createAtlas:{1}  val:{2}", data.texPropertyNames[propIdx].name, MB3_TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(propIdx, data._considerNonTextureProperties, shouldWeCreateAtlasForProp), shouldWeCreateAtlasForProp[propIdx]));
            }
            return shouldWeCreateAtlasForProp;
        }

        //Textures in each material (_mainTex, Bump, Spec ect...) must be same size
        //Calculate the best sized to use. Takes into account tiling
        //if only one texture in atlas re-uses original sizes	
        internal static IEnumerator CalculateIdealSizesForTexturesInAtlasAndPadding(ProgressUpdateDelegate progressInfo,
                                                                    MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult result,
                                                                    MB3_TextureCombinerPipeline.TexturePipelineData data,
                                                                    MB3_TextureCombiner combiner,
                                                                    MB2_EditorMethodsInterface textureEditorMethods,
                                                                    MB2_LogLevel LOG_LEVEL)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            MeshBakerMaterialTexture.readyToBuildAtlases = true;
            data.allTexturesAreNullAndSameColor = CalculateAllTexturesAreNullAndSameColor(data, LOG_LEVEL);

            //calculate size of rectangles in atlas
            int _padding = data._atlasPadding;
            if (data.distinctMaterialTextures.Count == 1 && data._fixOutOfBoundsUVs == false && data._considerNonTextureProperties == false)
            {
                if (LOG_LEVEL >= MB2_LogLevel.info) Debug.Log("All objects use the same textures in this set of atlases. Original textures will be reused instead of creating atlases.");
                _padding = 0;
                data.distinctMaterialTextures[0].SetThisIsOnlyTexSetInAtlasTrue();
                data.distinctMaterialTextures[0].SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(MB_TextureTilingTreatment.edgeToEdgeXY);
            }

            Debug.Assert(data.allTexturesAreNullAndSameColor.Length == data.texPropertyNames.Count, "allTexturesAreNullAndSameColor array must be the same length of texPropertyNames.");
            for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Calculating ideal sizes for texSet TexSet " + i + " of " + data.distinctMaterialTextures.Count);
                MB_TexSet txs = data.distinctMaterialTextures[i];
                txs.idealWidth = 1;
                txs.idealHeight = 1;
                int tWidth = 1;
                int tHeight = 1;
                Debug.Assert(txs.ts.Length == data.texPropertyNames.Count, "length of arrays in each element of distinctMaterialTextures must be texPropertyNames.Count");

                //get the best size all textures in a TexSet must be the same size.
                for (int propIdx = 0; propIdx < data.texPropertyNames.Count; propIdx++)
                {
                    if (MB3_TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(propIdx, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
                    {
                        MeshBakerMaterialTexture matTex = txs.ts[propIdx];
                        if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log(string.Format("Calculating ideal size for texSet {0} property {1}", i, data.texPropertyNames[propIdx].name));
                        if (!matTex.matTilingRect.size.Equals(Vector2.one) && data.distinctMaterialTextures.Count > 1)
                        {
                            if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Texture " + matTex.GetTexName() + "is tiled by " + matTex.matTilingRect.size + " tiling will be baked into a texture with maxSize:" + data._maxTilingBakeSize);
                        }

                        if (!txs.obUVscale.Equals(Vector2.one) && data.distinctMaterialTextures.Count > 1 && data._fixOutOfBoundsUVs)
                        {
                            if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Texture " + matTex.GetTexName() + " has out of bounds UVs that effectively tile by " + txs.obUVscale + " tiling will be baked into a texture with maxSize:" + data._maxTilingBakeSize);
                        }

                        if (matTex.isNull)
                        {
                            txs.SetEncapsulatingRect(propIdx, data._fixOutOfBoundsUVs);
                            if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log(String.Format("No source texture creating a 16x16 texture for {0} texSet {1} srcMat {2}", data.texPropertyNames[propIdx].name, i, txs.matsAndGOs.mats[0].GetMaterialName()));
                        }

                        if (!matTex.isNull)
                        {
                            Vector2 dim = MB3_TextureCombinerPipeline.GetAdjustedForScaleAndOffset2Dimensions(matTex, txs.obUVoffset, txs.obUVscale, data, LOG_LEVEL);
                            if ((int)(dim.x * dim.y) > tWidth * tHeight)
                            {
                                if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log("    matTex " + matTex.GetTexName() + " " + dim + " has a bigger size than " + tWidth + " " + tHeight);
                                tWidth = (int)dim.x;
                                tHeight = (int)dim.y;
                            }
                        }
                    }
                }

                if (data._resizePowerOfTwoTextures)
                {
                    if (tWidth <= _padding * 5)
                    {
                        Debug.LogWarning(String.Format("Some of the textures have widths close to the size of the padding. It is not recommended to use _resizePowerOfTwoTextures with widths this small.", txs.ToString()));
                    }
                    if (tHeight <= _padding * 5)
                    {
                        Debug.LogWarning(String.Format("Some of the textures have heights close to the size of the padding. It is not recommended to use _resizePowerOfTwoTextures with heights this small.", txs.ToString()));
                    }
                    if (IsPowerOfTwo(tWidth))
                    {
                        tWidth -= _padding * 2;
                    }
                    if (IsPowerOfTwo(tHeight))
                    {
                        tHeight -= _padding * 2;
                    }
                    if (tWidth < 1) tWidth = 1;
                    if (tHeight < 1) tHeight = 1;
                }
                if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log("    Ideal size is " + tWidth + " " + tHeight);
                txs.idealWidth = tWidth;
                txs.idealHeight = tHeight;
            }
            data._atlasPadding = _padding;
            if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Total time Step2 Calculate Ideal Sizes part1: " + sw.Elapsed.ToString());
            yield break;
        }

        internal static AtlasPackingResult[] RunTexturePackerOnly(TexturePipelineData data, bool doSplitIntoMultiAtlasIfTooBig, MB_ITextureCombinerPacker texturePacker, MB2_LogLevel LOG_LEVEL)
        {
            AtlasPackingResult[] apr = texturePacker.CalculateAtlasRectangles(data, doSplitIntoMultiAtlasIfTooBig, LOG_LEVEL); // __RuntTexturePackerOnly(data, texturePacker, LOG_LEVEL);
            //copy materials PackingResults
            for (int i = 0; i < apr.Length; i++)
            {
                List<MatsAndGOs> matsList = new List<MatsAndGOs>();
                apr[i].data = matsList;
                for (int j = 0; j < apr[i].srcImgIdxs.Length; j++)
                {
                    MB_TexSet ts = data.distinctMaterialTextures[apr[i].srcImgIdxs[j]];
                    matsList.Add(ts.matsAndGOs);
                }
            }

            return apr;
        }

        internal static MB_ITextureCombinerPacker CreatePacker(bool onlyOneTextureInAtlasReuseTextures, MB2_PackingAlgorithmEnum packingAlgorithm)
        {
            if (onlyOneTextureInAtlasReuseTextures)
            {
                return new MB3_TextureCombinerPackerOneTextureInAtlas();
            }
            else if (packingAlgorithm == MB2_PackingAlgorithmEnum.UnitysPackTextures)
            {
                return new MB3_TextureCombinerPackerUnity();
            }
            else if (packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal)
            {
                if (USE_EXPERIMENTAL_HOIZONTALVERTICAL)
                {
                    return new MB3_TextureCombinerPackerMeshBakerHorizontalVertical(MB3_TextureCombinerPackerMeshBakerHorizontalVertical.AtlasDirection.horizontal);
                } else
                {
                    return new MB3_TextureCombinerPackerMeshBaker();
                }
                
            }
            else if (packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical)
            {
                if (USE_EXPERIMENTAL_HOIZONTALVERTICAL)
                {
                    return new MB3_TextureCombinerPackerMeshBakerHorizontalVertical(MB3_TextureCombinerPackerMeshBakerHorizontalVertical.AtlasDirection.vertical);
                } else
                {
                    return new MB3_TextureCombinerPackerMeshBaker();
                }
            }
            else if (packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker)
            {
                return new MB3_TextureCombinerPackerMeshBaker();
            }
            else
            {
                return new MB3_TextureCombinerPackerMeshBakerFast();
            }
        }

        internal static IEnumerator __Step3_BuildAndSaveAtlasesAndStoreResults(MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult result,
            ProgressUpdateDelegate progressInfo,
            TexturePipelineData data,
            MB3_TextureCombiner combiner,
            MB_ITextureCombinerPacker packer,
            AtlasPackingResult atlasPackingResult,
            MB2_EditorMethodsInterface textureEditorMethods, MB_AtlasesAndRects resultAtlasesAndRects,
            StringBuilder report,
            MB2_LogLevel LOG_LEVEL)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //run the garbage collector to free up as much memory as possible before bake to reduce MissingReferenceException problems
            GC.Collect();
            Texture2D[] atlases = new Texture2D[data.numAtlases];
            if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("time Step 3 Create And Save Atlases part 1 " + sw.Elapsed.ToString());

            yield return packer.CreateAtlases(progressInfo, data, combiner, atlasPackingResult, atlases, textureEditorMethods, LOG_LEVEL);
            float t3 = sw.ElapsedMilliseconds;

            data.nonTexturePropertyBlender.AdjustNonTextureProperties(data.resultMaterial, data.texPropertyNames, data.distinctMaterialTextures, textureEditorMethods);

            if (data.distinctMaterialTextures.Count > 0) data.distinctMaterialTextures[0].AdjustResultMaterialNonTextureProperties(data.resultMaterial, data.texPropertyNames);

            if (progressInfo != null) progressInfo("Building Report", .7f);

            //report on atlases created
            StringBuilder atlasMessage = new StringBuilder();
            atlasMessage.AppendLine("---- Atlases ------");
            for (int i = 0; i < data.numAtlases; i++)
            {
                if (atlases[i] != null)
                {
                    atlasMessage.AppendLine("Created Atlas For: " + data.texPropertyNames[i].name + " h=" + atlases[i].height + " w=" + atlases[i].width);
                }
                else if (!_ShouldWeCreateAtlasForThisProperty(i, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
                {
                    atlasMessage.AppendLine("Did not create atlas for " + data.texPropertyNames[i].name + " because all source textures were null.");
                }
            }
            report.Append(atlasMessage.ToString());

            List<MB_MaterialAndUVRect> mat2rect_map = new List<MB_MaterialAndUVRect>();
            for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
            {
                MB_TexSet texSet = data.distinctMaterialTextures[i];
                List<MatAndTransformToMerged> mats = texSet.matsAndGOs.mats;
                Rect allPropsUseSameTiling_encapsulatingSamplingRect, propsUseDifferntTiling_obUVRect;
                texSet.GetRectsForTextureBakeResults(out allPropsUseSameTiling_encapsulatingSamplingRect, out propsUseDifferntTiling_obUVRect);
                for (int j = 0; j < mats.Count; j++)
                {
                    Rect allPropsUseSameTiling_sourceMaterialTiling = texSet.GetMaterialTilingRectForTextureBakerResults(j);
                    MB_MaterialAndUVRect key = new MB_MaterialAndUVRect(
                        mats[j].mat,
                        atlasPackingResult.rects[i],
                        texSet.allTexturesUseSameMatTiling,
                        allPropsUseSameTiling_sourceMaterialTiling,
                        allPropsUseSameTiling_encapsulatingSamplingRect,
                        propsUseDifferntTiling_obUVRect,
                        texSet.tilingTreatment,
                        mats[j].objName);
                    if (!mat2rect_map.Contains(key))
                    {
                        mat2rect_map.Add(key);
                    }
                }
            }

            resultAtlasesAndRects.atlases = atlases;                             // one per texture on result shader
            resultAtlasesAndRects.texPropertyNames = ShaderTextureProperty.GetNames(data.texPropertyNames); // one per texture on source shader
            resultAtlasesAndRects.mat2rect_map = mat2rect_map;

            if (progressInfo != null) progressInfo("Restoring Texture Formats & Read Flags", .8f);
            combiner._destroyAllTemporaryTextures();
            if (textureEditorMethods != null) textureEditorMethods.RestoreReadFlagsAndFormats(progressInfo);
            if (report != null && LOG_LEVEL >= MB2_LogLevel.info) Debug.Log(report.ToString());
            if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Time Step 3 Create And Save Atlases part 3 " + (sw.ElapsedMilliseconds - t3).ToString("f5"));
            if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Total time Step 3 Create And Save Atlases " + sw.Elapsed.ToString());
            yield break;
        }

        internal static StringBuilder GenerateReport(MB3_TextureCombinerPipeline.TexturePipelineData data)
        {
            //generate report want to do this before
            StringBuilder report = new StringBuilder();
            if (data.numAtlases > 0)
            {
                report = new StringBuilder();
                report.AppendLine("Report");
                for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
                {
                    MB_TexSet txs = data.distinctMaterialTextures[i];
                    report.AppendLine("----------");
                    report.Append("This set of textures will be resized to:" + txs.idealWidth + "x" + txs.idealHeight + "\n");
                    for (int j = 0; j < txs.ts.Length; j++)
                    {
                        if (!txs.ts[j].isNull)
                        {
                            report.Append("   [" + data.texPropertyNames[j].name + " " + txs.ts[j].GetTexName() + " " + txs.ts[j].width + "x" + txs.ts[j].height + "]");
                            if (txs.ts[j].matTilingRect.size != Vector2.one || txs.ts[j].matTilingRect.min != Vector2.zero) report.AppendFormat(" material scale {0} offset{1} ", txs.ts[j].matTilingRect.size.ToString("G4"), txs.ts[j].matTilingRect.min.ToString("G4"));
                            if (txs.obUVscale != Vector2.one || txs.obUVoffset != Vector2.zero) report.AppendFormat(" obUV scale {0} offset{1} ", txs.obUVscale.ToString("G4"), txs.obUVoffset.ToString("G4"));
                            report.AppendLine("");
                        }
                        else
                        {
                            report.Append("   [" + data.texPropertyNames[j].name + " null ");
                            if (!MB3_TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(j, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
                            {
                                report.Append("no atlas will be created all textures null]\n");
                            }
                            else
                            {
                                report.AppendFormat("a 16x16 texture will be created]\n");
                            }
                        }
                    }
                    report.AppendLine("");
                    report.Append("Materials using:");
                    for (int j = 0; j < txs.matsAndGOs.mats.Count; j++)
                    {
                        report.Append(txs.matsAndGOs.mats[j].mat.name + ", ");
                    }
                    report.AppendLine("");
                }
            }
            return report;
        }

        /*
        internal static AtlasPackingResult[] __RuntTexturePackerOnly(TexturePipelineData data, MB_ITextureCombinerPacker texturePacker, MB2_LogLevel LOG_LEVEL)
        {
            AtlasPackingResult[] packerRects;
            if (data.OnlyOneTextureInAtlasReuseTextures())
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Only one image per atlas. Will re-use original texture");
                packerRects = new AtlasPackingResult[1];
                AtlasPadding[] paddings = new AtlasPadding[] { new AtlasPadding(data._atlasPadding) };
                packerRects[0] = new AtlasPackingResult(paddings);
                packerRects[0].rects = new Rect[1];
                packerRects[0].srcImgIdxs = new int[] { 0 };
                packerRects[0].rects[0] = new Rect(0f, 0f, 1f, 1f);

                MeshBakerMaterialTexture dmt = null;
                if (data.distinctMaterialTextures[0].ts.Length > 0)
                {
                    dmt = data.distinctMaterialTextures[0].ts[0];

                }
                packerRects[0].atlasX = dmt.isNull ? 16 : dmt.width;
                packerRects[0].atlasY = dmt.isNull ? 16 : dmt.height;
                packerRects[0].usedW = dmt.isNull ? 16 : dmt.width;
                packerRects[0].usedH = dmt.isNull ? 16 : dmt.height;
            }
            else
            {
                List<Vector2> imageSizes = new List<Vector2>();
                List<AtlasPadding> paddings = new List<AtlasPadding>();
                for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
                {
                    imageSizes.Add(new Vector2(data.distinctMaterialTextures[i].idealWidth, data.distinctMaterialTextures[i].idealHeight));
                    paddings.Add(new AtlasPadding(data._atlasPadding));
                }
                MB2_TexturePacker tp = CreateTexturePacker(data._packingAlgorithm);
                tp.atlasMustBePowerOfTwo = data._meshBakerTexturePackerForcePowerOfTwo;
                packerRects = tp.GetRects(imageSizes, paddings, data._maxAtlasSize, data._maxAtlasSize, true);
                //Debug.Assert(packerRects.Length != 0);
            }
            return packerRects;
        }
        */

        internal static MB2_TexturePacker CreateTexturePacker(MB2_PackingAlgorithmEnum _packingAlgorithm)
        {
            if (_packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker)
            {
                return new MB2_TexturePackerRegular();
            }
            else if (_packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
            {
                return new MB2_TexturePackerRegular();
            }
            else if (_packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal)
            {
                MB2_TexturePackerHorizontalVert tp = new MB2_TexturePackerHorizontalVert();
                tp.packingOrientation = MB2_TexturePackerHorizontalVert.TexturePackingOrientation.horizontal;
                return tp;
            }
            else if (_packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical)
            {
                MB2_TexturePackerHorizontalVert tp = new MB2_TexturePackerHorizontalVert();
                tp.packingOrientation = MB2_TexturePackerHorizontalVert.TexturePackingOrientation.vertical;
                return tp;
            }
            else
            {
                Debug.LogError("packing algorithm must be one of the MeshBaker options to create a Texture Packer");
            }
            return null;
        }

        internal static Vector2 GetAdjustedForScaleAndOffset2Dimensions(MeshBakerMaterialTexture source, Vector2 obUVoffset, Vector2 obUVscale, TexturePipelineData data, MB2_LogLevel LOG_LEVEL)
        {
            if (source.matTilingRect.x == 0f && source.matTilingRect.y == 0f && source.matTilingRect.width == 1f && source.matTilingRect.height == 1f)
            {
                if (data._fixOutOfBoundsUVs)
                {
                    if (obUVoffset.x == 0f && obUVoffset.y == 0f && obUVscale.x == 1f && obUVscale.y == 1f)
                    {
                        return new Vector2(source.width, source.height); //no adjustment necessary
                    }
                }
                else
                {
                    return new Vector2(source.width, source.height); //no adjustment necessary
                }
            }

            if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("GetAdjustedForScaleAndOffset2Dimensions: " + source.GetTexName() + " " + obUVoffset + " " + obUVscale);
            Rect encapsulatingSamplingRect = source.GetEncapsulatingSamplingRect().GetRect();
            float newWidth = encapsulatingSamplingRect.width * source.width;
            float newHeight = encapsulatingSamplingRect.height * source.height;

            if (newWidth > data._maxTilingBakeSize) newWidth = data._maxTilingBakeSize;
            if (newHeight > data._maxTilingBakeSize) newHeight = data._maxTilingBakeSize;
            if (newWidth < 1f) newWidth = 1f;
            if (newHeight < 1f) newHeight = 1f;
            return new Vector2(newWidth, newHeight);
        }

        /* 
        Unity uses a non-standard format for storing normals for some platforms. Imagine the standard format is English, Unity's is French
        When the normal-map checkbox is ticked on the asset importer the normal map is translated into french. When we build the normal atlas
        we are reading the french. When we save and click the normal map tickbox we are translating french -> french. A double transladion that
        breaks the normal map. To fix this we need to "unconvert" the normal map to english when saving the atlas as a texture so that unity importer
        can do its thing properly. 
        */
        internal static Color32 ConvertNormalFormatFromUnity_ToStandard(Color32 c)
        {
            Vector3 n = Vector3.zero;
            n.x = c.a * 2f - 1f;
            n.y = c.g * 2f - 1f;
            n.z = Mathf.Sqrt(1 - n.x * n.x - n.y * n.y);
            //now repack in the regular format
            Color32 cc = new Color32();
            cc.a = 1;
            cc.r = (byte)((n.x + 1f) * .5f);
            cc.g = (byte)((n.y + 1f) * .5f);
            cc.b = (byte)((n.z + 1f) * .5f);
            return cc;
        }

        internal static float GetSubmeshArea(Mesh m, int submeshIdx)
        {
            if (submeshIdx >= m.subMeshCount || submeshIdx < 0)
            {
                return 0f;
            }
            Vector3[] vs = m.vertices;
            int[] tris = m.GetIndices(submeshIdx);
            float area = 0f;
            for (int i = 0; i < tris.Length; i += 3)
            {
                Vector3 v0 = vs[tris[i]];
                Vector3 v1 = vs[tris[i + 1]];
                Vector3 v2 = vs[tris[i + 2]];
                Vector3 cross = Vector3.Cross(v1 - v0, v2 - v0);
                area += cross.magnitude / 2f;
            }
            return area;
        }

        internal static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

    }
}

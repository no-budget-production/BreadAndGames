using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine.Serialization;


/// <summary>
/// Used internally during the material baking process
/// </summary>
[Serializable]
public class MB_AtlasesAndRects{
	public Texture2D[] atlases;
    [NonSerialized]
	public List<MB_MaterialAndUVRect> mat2rect_map;
	public string[] texPropertyNames;
}

[System.Serializable]
public class MB_MultiMaterial{
	public Material combinedMaterial;
    public bool considerMeshUVs;
	public List<Material> sourceMaterials = new List<Material>();
}

[System.Serializable]
public class MB_MaterialAndUVRect
{
    /// <summary>
    /// The source material that was baked into the atlas.
    /// </summary>
    public Material material;
    
    /// <summary>
    /// The rectangle in the atlas where the texture (including all tiling) was copied to.
    /// </summary>
    public Rect atlasRect;

    /// <summary>
    /// For debugging. The name of the first srcObj that uses this MaterialAndUVRect.
    /// </summary>
    public string srcObjName;

    public bool allPropsUseSameTiling = true;

    /// <summary>
    /// Only valid if allPropsUseSameTiling = true. Else should be 0,0,0,0
    /// The material tiling on the source material
    /// </summary>
    [FormerlySerializedAs("sourceMaterialTiling")]
    public Rect allPropsUseSameTiling_sourceMaterialTiling;

    /// <summary>
    /// Only valid if allPropsUseSameTiling = true. Else should be 0,0,0,0
    /// The encapsulating sampling rect that was used to sample for the atlas. Note that the case
    /// of dont-considerMeshUVs is the same as do-considerMeshUVs where the uv rect is 0,0,1,1 
    /// </summary>
    [FormerlySerializedAs("samplingEncapsulatinRect")]
    public Rect allPropsUseSameTiling_samplingEncapsulatinRect;

    /// <summary>
    /// Only valid if allPropsUseSameTiling = false.
    /// The UVrect of the source mesh that was baked. We are using a trick here.
    /// Instead of storing the material tiling for each
    /// texture property here, we instead bake all those tilings into the atlases and here we pretend
    /// that all those tilings were 0,0,1,1. Then all we need is to store is the 
    /// srcUVsamplingRect
    /// </summary>
    public Rect propsUseDifferntTiling_srcUVsamplingRect;

    /// <summary>
    /// The tilling type for this rectangle in the atlas.
    /// </summary>
    public MB_TextureTilingTreatment tilingTreatment = MB_TextureTilingTreatment.unknown;

    /// <param name="mat">The Material</param>
    /// <param name="destRect">The rect in the atlas this material maps to</param>
    /// <param name="allPropsUseSameTiling">If true then use sourceMaterialTiling and samplingEncapsulatingRect.
    /// if false then use srcUVsamplingRect. None used values should be 0,0,0,0.</param>
    /// <param name="sourceMaterialTiling">allPropsUseSameTiling_sourceMaterialTiling</param>
    /// <param name="samplingEncapsulatingRect">allPropsUseSameTiling_samplingEncapsulatinRect</param>
    /// <param name="srcUVsamplingRect">propsUseDifferntTiling_srcUVsamplingRect</param>
    public MB_MaterialAndUVRect(Material mat, 
        Rect destRect, 
        bool allPropsUseSameTiling,
        Rect sourceMaterialTiling, 
        Rect samplingEncapsulatingRect,
        Rect srcUVsamplingRect,
        MB_TextureTilingTreatment treatment, 
        string objName)
    {
        if (allPropsUseSameTiling)
        {
            Debug.Assert(srcUVsamplingRect == new Rect(0, 0, 0, 0));
        }

        if (!allPropsUseSameTiling) {
            Debug.Assert(samplingEncapsulatingRect == new Rect(0, 0, 0, 0));
            Debug.Assert(sourceMaterialTiling == new Rect(0, 0, 0, 0));
        }

        material = mat;
        atlasRect = destRect;
        tilingTreatment = treatment;
        this.allPropsUseSameTiling = allPropsUseSameTiling;
        allPropsUseSameTiling_sourceMaterialTiling = sourceMaterialTiling;
        allPropsUseSameTiling_samplingEncapsulatinRect = samplingEncapsulatingRect;
        propsUseDifferntTiling_srcUVsamplingRect = srcUVsamplingRect;
        srcObjName = objName;
    }

    public override int GetHashCode()
    {
        return material.GetInstanceID() ^ allPropsUseSameTiling_samplingEncapsulatinRect.GetHashCode() ^ propsUseDifferntTiling_srcUVsamplingRect.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is MB_MaterialAndUVRect)) return false;
        MB_MaterialAndUVRect b = (MB_MaterialAndUVRect)obj;
        return material == b.material && 
            allPropsUseSameTiling_samplingEncapsulatinRect == b.allPropsUseSameTiling_samplingEncapsulatinRect &&
            allPropsUseSameTiling_sourceMaterialTiling == b.allPropsUseSameTiling_sourceMaterialTiling &&
            allPropsUseSameTiling == b.allPropsUseSameTiling &&
            propsUseDifferntTiling_srcUVsamplingRect == b.propsUseDifferntTiling_srcUVsamplingRect;
    }

    public Rect GetEncapsulatingRect()
    {
        if (allPropsUseSameTiling)
        {
            return allPropsUseSameTiling_samplingEncapsulatinRect;
        }
        else
        {
            return propsUseDifferntTiling_srcUVsamplingRect;
        }
    }

    public Rect GetMaterialTilingRect()
    {
        if (allPropsUseSameTiling)
        {
            return allPropsUseSameTiling_sourceMaterialTiling;
        }
        else
        {
            return new Rect(0, 0, 1, 1);
        }
    }
}

/// <summary>
/// This class stores the results from an MB2_TextureBaker when materials are combined into atlases. It stores
/// a list of materials and the corresponding UV rectangles in the atlases. It also stores the configuration
/// options that were used to generate the combined material.
/// 
/// It can be saved as an asset in the project so that textures can be baked in one scene and used in another.
/// </summary>
public class MB2_TextureBakeResults : ScriptableObject {

    public static int VERSION { get{ return 3252; }}
    public int version;
    public MB_MaterialAndUVRect[] materialsAndUVRects;
    public MB_MultiMaterial[] resultMaterials;
    public bool doMultiMaterial;

    public MB2_TextureBakeResults()
    {
        version = VERSION;
    }

    private void OnEnable()
    {
        // backward compatibility copy depricated fixOutOfBounds values to resultMaterials
        if (version < 3251)
        {
            for (int i = 0; i < materialsAndUVRects.Length; i++)
            {
                materialsAndUVRects[i].allPropsUseSameTiling = true;
            }
        }

        version = VERSION;
    }

    /// <summary>
    /// Creates for materials on renderer.
    /// </summary>
    /// <returns>Generates an MB2_TextureBakeResult that can be used if all objects to be combined use the same material.
    /// Returns a MB2_TextureBakeResults that will map all materials used by renderer r to
    /// the rectangle 0,0..1,1 in the atlas.</returns>
    /// <param name="r">The red component.</param>
    public static MB2_TextureBakeResults CreateForMaterialsOnRenderer(GameObject[] gos, List<Material> matsOnTargetRenderer)
    {

        HashSet<Material> fullMaterialList = new HashSet<Material>(matsOnTargetRenderer);
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i] == null)
            {
                Debug.LogError(string.Format("Game object {0} in list of objects to add was null", i));
                return null;
            }
            Material[] oMats = MB_Utility.GetGOMaterials(gos[i]);
            if (oMats.Length == 0)
            {
                Debug.LogError(string.Format("Game object {0} in list of objects to add no renderer", i));
                return null;
            }
            for (int j = 0; j < oMats.Length; j++)
            {
                if (!fullMaterialList.Contains(oMats[j])) { fullMaterialList.Add(oMats[j]); }
            }
        }
        Material[] rms = new Material[fullMaterialList.Count];
        fullMaterialList.CopyTo(rms);

        MB2_TextureBakeResults tbr = (MB2_TextureBakeResults) ScriptableObject.CreateInstance( typeof(MB2_TextureBakeResults) );
        List<MB_MaterialAndUVRect> mss = new List<MB_MaterialAndUVRect>();
        for (int i = 0; i < rms.Length; i++)
        {
            if (rms[i] != null)
            {
                MB_MaterialAndUVRect matAndUVRect = new MB_MaterialAndUVRect(rms[i], new Rect(0f, 0f, 1f, 1f), true, new Rect(0f,0f,1f,1f), new Rect(0f,0f,1f,1f), new Rect(0,0,0,0), MB_TextureTilingTreatment.none, "");
                if (!mss.Contains(matAndUVRect))
                {
                    mss.Add(matAndUVRect);
                }
            }
        }

        tbr.resultMaterials = new MB_MultiMaterial[mss.Count];
        for (int i = 0; i < mss.Count; i++){
			tbr.resultMaterials[i] = new MB_MultiMaterial();
			List<Material> sourceMats = new List<Material>();
			sourceMats.Add (mss[i].material);
			tbr.resultMaterials[i].sourceMaterials = sourceMats;
			tbr.resultMaterials[i].combinedMaterial = mss[i].material;
            tbr.resultMaterials[i].considerMeshUVs = false;
		}
        if (rms.Length == 1)
        {
            tbr.doMultiMaterial = false;
        } else
        {
            tbr.doMultiMaterial = true;
        }

        tbr.materialsAndUVRects = mss.ToArray();
        return tbr;
	}
	
    public bool DoAnyResultMatsUseConsiderMeshUVs()
    {
        if (resultMaterials == null) return false;
        for (int i = 0; i < resultMaterials.Length; i++)
        {
            if (resultMaterials[i].considerMeshUVs) return true;
        }
        return false;
    }

    public bool ContainsMaterial(Material m)
    {
        for (int i = 0; i < materialsAndUVRects.Length; i++)
        {
            if (materialsAndUVRects[i].material == m){
                return true;
            }
        }
        return false;
    }


	public string GetDescription(){
		StringBuilder sb = new StringBuilder();
		sb.Append("Shaders:\n");
		HashSet<Shader> shaders = new HashSet<Shader>();
		if (materialsAndUVRects != null){
			for (int i = 0; i < materialsAndUVRects.Length; i++){
                if (materialsAndUVRects[i].material != null)
                {
                    shaders.Add(materialsAndUVRects[i].material.shader);
                }	
			}
		}
		
		foreach(Shader m in shaders){
			sb.Append("  ").Append(m.name).AppendLine();
		}
		sb.Append("Materials:\n");
		if (materialsAndUVRects != null){
			for (int i = 0; i < materialsAndUVRects.Length; i++){
                if (materialsAndUVRects[i].material != null)
                {
                    sb.Append("  ").Append(materialsAndUVRects[i].material.name).AppendLine();
                }
			}
		}
		return sb.ToString();
	}

    public class Material2AtlasRectangleMapper
    {
        MB2_TextureBakeResults tbr;
        int[] numTimesMatAppearsInAtlas;
        MB_MaterialAndUVRect[] matsAndSrcUVRect;

        public Material2AtlasRectangleMapper(MB2_TextureBakeResults res)
        {
            tbr = res;
            matsAndSrcUVRect = res.materialsAndUVRects;

            //count the number of times a material appears in the atlas. used for fast lookup
            numTimesMatAppearsInAtlas = new int[matsAndSrcUVRect.Length];
            for (int i = 0; i < matsAndSrcUVRect.Length; i++)
            {
                if (numTimesMatAppearsInAtlas[i] > 1)
                {
                    continue;
                }
                int count = 1;
                for (int j = i + 1; j < matsAndSrcUVRect.Length; j++)
                {
                    if (matsAndSrcUVRect[i].material == matsAndSrcUVRect[j].material)
                    {
                        count++;
                    }
                }
                numTimesMatAppearsInAtlas[i] = count;
                if (count > 1)
                {
                    //allMatsAreUnique = false;
                    for (int j = i + 1; j < matsAndSrcUVRect.Length; j++)
                    {
                        if (matsAndSrcUVRect[i].material == matsAndSrcUVRect[j].material)
                        {
                            numTimesMatAppearsInAtlas[j] = count;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A material can appear more than once in an atlas if using fixOutOfBoundsUVs.
        /// in this case you need to use the UV rect of the mesh to find the correct rectangle.
        /// If the all properties on the mat use the same tiling then 
        /// encapsulatingRect can be larger and will include baked UV and material tiling
        /// If mat uses different tiling for different maps then encapsulatingRect is the uvs of
        /// source mesh used to bake atlas and sourceMaterialTilingOut is 0,0,1,1. This works because
        /// material tiling was baked into the atlas.
        /// </summary>
        public bool TryMapMaterialToUVRect(Material mat, Mesh m, int submeshIdx, int idxInResultMats, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelCache, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisCache,
                                             out MB_TextureTilingTreatment tilingTreatment, 
                                             out Rect rectInAtlas,
                                             out Rect encapsulatingRectOut,
                                             out Rect sourceMaterialTilingOut,
                                             ref String errorMsg,
                                             MB2_LogLevel logLevel)
        {
            if (tbr.version < VERSION)
            {
                UpgradeToCurrentVersion(tbr);
            }
            tilingTreatment = MB_TextureTilingTreatment.unknown;
            if (tbr.materialsAndUVRects.Length == 0)
            {
                errorMsg = "The 'Texture Bake Result' needs to be re-baked to be compatible with this version of Mesh Baker. Please re-bake using the MB3_TextureBaker.";
                rectInAtlas = new Rect();
                encapsulatingRectOut = new Rect();
                sourceMaterialTilingOut = new Rect();
                return false;
            }
            if (mat == null)
            {
                rectInAtlas = new Rect();
                encapsulatingRectOut = new Rect();
                sourceMaterialTilingOut = new Rect();
                errorMsg = String.Format("Mesh {0} Had no material on submesh {1} cannot map to a material in the atlas", m.name, submeshIdx);
                return false;
            }
            if (submeshIdx >= m.subMeshCount)
            {
                errorMsg = "Submesh index is greater than the number of submeshes";
                rectInAtlas = new Rect();
                encapsulatingRectOut = new Rect();
                sourceMaterialTilingOut = new Rect();
                return false;
            }

            //find the first index of this material
            int idx = -1;
            for (int i = 0; i < matsAndSrcUVRect.Length; i++)
            {
                if (mat == matsAndSrcUVRect[i].material)
                {
                    idx = i;
                    break;
                }
            }
            // if couldn't find material
            if (idx == -1)
            {
                rectInAtlas = new Rect();
                encapsulatingRectOut = new Rect();
                sourceMaterialTilingOut = new Rect();
                errorMsg = String.Format("Material {0} could not be found in the Texture Bake Result", mat.name);
                return false;
            }

            if (!tbr.resultMaterials[idxInResultMats].considerMeshUVs)
            {
                if (numTimesMatAppearsInAtlas[idx] != 1)
                {
                    Debug.LogError("There is a problem with this TextureBakeResults. FixOutOfBoundsUVs is false and a material appears more than once.");
                }
                MB_MaterialAndUVRect mr = matsAndSrcUVRect[idx];
                rectInAtlas = mr.atlasRect;
                tilingTreatment = mr.tilingTreatment;
                encapsulatingRectOut = mr.GetEncapsulatingRect();
                sourceMaterialTilingOut = mr.GetMaterialTilingRect();
                return true;
            }
            else
            {
                //todo what if no UVs
                //Find UV rect in source mesh
                MB_Utility.MeshAnalysisResult[] mar;
                if (!meshAnalysisCache.TryGetValue(m.GetInstanceID(), out mar))
                {
                    mar = new MB_Utility.MeshAnalysisResult[m.subMeshCount];
                    for (int j = 0; j < m.subMeshCount; j++)
                    {
                        Vector2[] uvss = meshChannelCache.GetUv0Raw(m);
                        MB_Utility.hasOutOfBoundsUVs(uvss, m, ref mar[j], j);
                    }
                    meshAnalysisCache.Add(m.GetInstanceID(), mar);
                }

                //this could be a mesh that was not used in the texture baking that has huge UV tiling too big for the rect that was baked
                //find a record that has an atlas uvRect capable of containing this
                bool found = false;
                Rect encapsulatingRect = new Rect(0,0,0,0);
                Rect sourceMaterialTiling = new Rect(0,0,0,0);
                if (logLevel >= MB2_LogLevel.trace)
                {
                    Debug.Log(String.Format("Trying to find a rectangle in atlas capable of holding tiled sampling rect for mesh {0} using material {1} meshUVrect={2}", m, mat, mar[submeshIdx].uvRect.ToString("f5")));
                }
                for (int i = idx; i < matsAndSrcUVRect.Length; i++)
                {
                    MB_MaterialAndUVRect matAndUVrect = matsAndSrcUVRect[i];
                    if (matAndUVrect.material == mat)
                    {
                        if (matAndUVrect.allPropsUseSameTiling)
                        {
                            encapsulatingRect = matAndUVrect.allPropsUseSameTiling_samplingEncapsulatinRect;
                            sourceMaterialTiling = matAndUVrect.allPropsUseSameTiling_sourceMaterialTiling;
                        }
                        else
                        {
                            encapsulatingRect = matAndUVrect.propsUseDifferntTiling_srcUVsamplingRect;
                            sourceMaterialTiling = new Rect(0, 0, 1, 1);
                        }

                        if (IsMeshAndMaterialRectEnclosedByAtlasRect(
                                matAndUVrect.tilingTreatment,
                                mar[submeshIdx].uvRect,
                                sourceMaterialTiling,
                                encapsulatingRect,
                                logLevel))
                        {
                            if (logLevel >= MB2_LogLevel.trace)
                            {
                                Debug.Log("Found rect in atlas capable of containing tiled sampling rect for mesh " + m + " at idx=" + i);
                            }
                            idx = i;
                            found = true;
                            break;
                        }
                    }
                }
                if (found)
                {
                    MB_MaterialAndUVRect mr = matsAndSrcUVRect[idx];
                    rectInAtlas = mr.atlasRect;
                    tilingTreatment = mr.tilingTreatment;
                    encapsulatingRectOut = mr.GetEncapsulatingRect();
                    sourceMaterialTilingOut = mr.GetMaterialTilingRect();
                    return true;
                }
                else
                {
                    rectInAtlas = new Rect();
                    encapsulatingRectOut = new Rect();
                    sourceMaterialTilingOut = new Rect();
                    errorMsg = String.Format("Could not find a tiled rectangle in the atlas capable of containing the uv and material tiling on mesh {0} for material {1}. Was this mesh included when atlases were baked?", m.name, mat);
                    return false;
                }
            }
        }

        private void UpgradeToCurrentVersion(MB2_TextureBakeResults tbr)
        {
            if (tbr.version < 3252)
            {
                for (int i = 0; i < tbr.materialsAndUVRects.Length; i++)
                {
                    tbr.materialsAndUVRects[i].allPropsUseSameTiling = true;
                }
            }
        }
    }

    public static bool IsMeshAndMaterialRectEnclosedByAtlasRect(MB_TextureTilingTreatment tilingTreatment, Rect uvR, Rect sourceMaterialTiling, Rect samplingEncapsulatinRect, MB2_LogLevel logLevel)
    {
        Rect potentialRect = new Rect();
        // test to see if this would fit in what was baked in the atlas

        potentialRect = MB3_UVTransformUtility.CombineTransforms(ref uvR, ref sourceMaterialTiling);
        if (logLevel >= MB2_LogLevel.trace)
        {
            if (logLevel >= MB2_LogLevel.trace) Debug.Log("IsMeshAndMaterialRectEnclosedByAtlasRect Rect in atlas uvR=" + uvR.ToString("f5") + " sourceMaterialTiling=" + sourceMaterialTiling.ToString("f5") + "Potential Rect (must fit in encapsulating) " + potentialRect.ToString("f5") + " encapsulating=" + samplingEncapsulatinRect.ToString("f5") + " tilingTreatment=" + tilingTreatment);
        }

        if (tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeX)
        {
            if (MB3_UVTransformUtility.LineSegmentContainsShifted(samplingEncapsulatinRect.y, samplingEncapsulatinRect.height, potentialRect.y, potentialRect.height))
            {
                return true;
            }
        }
        else if (tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeY)
        {
            if (MB3_UVTransformUtility.LineSegmentContainsShifted(samplingEncapsulatinRect.x, samplingEncapsulatinRect.width, potentialRect.x, potentialRect.width))
            {
                return true;
            }
        }
        else if (tilingTreatment == MB_TextureTilingTreatment.edgeToEdgeXY)
        {
            //only one rect in atlas and is edge to edge in both X and Y directions.
            return true;
        }
        else
        {
            if (MB3_UVTransformUtility.RectContainsShifted(ref samplingEncapsulatinRect, ref potentialRect))
            {
                return true;
            }
        }
        return false;
    }
}
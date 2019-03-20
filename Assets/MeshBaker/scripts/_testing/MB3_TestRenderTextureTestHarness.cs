using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
#if !UNITY_WEBPLAYER
using System.IO;
#endif
public class MB3_TestRenderTextureTestHarness : MonoBehaviour {

    public Texture2D input;
    public bool doColor;
    public Color32 color;

    public Texture2D Create3x3Tex() {
        Texture2D t = new Texture2D(3, 3, TextureFormat.ARGB32, false);
        Color32[] cs = new Color32[t.width * t.height];
        for (int i =0; i < cs.Length; i++) {
            cs[i] = color;
        }
        t.SetPixels32(cs);
        t.Apply();
        return t;
    }

    public Texture2D Create3x3Clone() {
        Texture2D t = new Texture2D(3, 3, TextureFormat.ARGB32, false);
        Color32[] cs = new Color32[] {
            new Color32(54, 54, 201, 255), new Color32(128, 37, 218, 255), new Color32(201, 54, 201, 255),
new Color32(37, 128, 218, 255), new Color32(128, 128, 255, 255), new Color32(218, 128, 218, 255),
new Color32(54, 201, 201, 255), new Color32(128, 218, 218, 255), new Color32(201, 201, 201, 255),
        };

        t.SetPixels32(cs);
        t.Apply();
        return t;
    }

#if UNITY_EDITOR
	// Use this for initialization
	void Start () {
        Texture2D t = input;
        if (doColor) {
            t = Create3x3Clone();
        }
        TestRender(t, null);	    
	}
#endif

	public static void TestRender(Texture2D input, Texture2D output) {
		int numAtlases = 1;
		ShaderTextureProperty[] texPropertyNames = new ShaderTextureProperty[] {
			new ShaderTextureProperty("_BumpMap",false)
		};
		int atlasSizeX = input.width;
		int atlasSizeY = input.height;
		int _padding = 0;
		Rect[] uvRects = new Rect[] { new Rect(0f, 0f, 1f, 1f) };
		List<MB_TexSet> distinctMaterialTextures = new List<MB_TexSet>();
		
		MeshBakerMaterialTexture[] dmts = new MeshBakerMaterialTexture[] {
			new MeshBakerMaterialTexture(input)
		};
		MB_TexSet texSet = new MB_TexSet(dmts,Vector2.zero,Vector2.one, MB_TextureTilingTreatment.considerUVs);
		distinctMaterialTextures.Add(texSet);
		
		GameObject renderAtlasesGO = null;
		
		renderAtlasesGO = new GameObject("MBrenderAtlasesGO");
		MB3_AtlasPackerRenderTexture atlasRenderTexture = renderAtlasesGO.AddComponent<MB3_AtlasPackerRenderTexture>();
		renderAtlasesGO.AddComponent<Camera>();
		for (int i = 0; i < numAtlases; i++) {
			Texture2D atlas = null;
			Debug.Log("About to render " + texPropertyNames[i].name + " isNormal=" + texPropertyNames[i].isNormalMap);
			atlasRenderTexture.LOG_LEVEL = MB2_LogLevel.trace;
			atlasRenderTexture.width = atlasSizeX;
			atlasRenderTexture.height = atlasSizeY;
			atlasRenderTexture.padding = _padding;
			atlasRenderTexture.rects = uvRects;
			atlasRenderTexture.textureSets = distinctMaterialTextures;
			atlasRenderTexture.indexOfTexSetToRender = i;
			atlasRenderTexture.isNormalMap = texPropertyNames[i].isNormalMap;
			// call render on it
			atlas = atlasRenderTexture.OnRenderAtlas(null);
			
			Debug.Log("Created atlas " + texPropertyNames[i].name + " w=" + atlas.width + " h=" + atlas.height + " id=" + atlas.GetInstanceID());
			Debug.Log("Color " + atlas.GetPixel(5,5) + " " + Color.red);
#if !UNITY_WEBPLAYER
			byte[] bytes = atlas.EncodeToPNG();
			File.WriteAllBytes(Application.dataPath + "/_Experiment/red.png", bytes);
#endif
		}
	}

}

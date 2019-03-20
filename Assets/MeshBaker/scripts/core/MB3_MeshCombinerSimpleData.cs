using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;

namespace DigitalOpus.MB.Core
{
    /// <summary>
    /// Manages a single combined mesh.This class is the core of the mesh combining API.
    /// 
    /// It is not a component so it can be can be instantiated and used like a normal c sharp class.
    /// </summary>
    public partial class MB3_MeshCombinerSingle : MB3_MeshCombiner
    {

        //2D arrays are not serializable but arrays  of arrays are.
        [System.Serializable]
        public class SerializableIntArray
        {
            public int[] data;

            public SerializableIntArray() { }

            public SerializableIntArray(int len)
            {
                data = new int[len];
            }
        }

        /*
		 Stores information about one source game object that has been added to
		 the combined mesh.  
		*/
        [System.Serializable]
        public class MB_DynamicGameObject : IComparable<MB_DynamicGameObject>
        {
            public int instanceID;
            public string name;
            public int vertIdx;
            public int blendShapeIdx;
            public int numVerts;
            public int numBlendShapes;

            //distinct list of bones in the bones array
            public int[] indexesOfBonesUsed = new int[0];

            //public Transform[] _originalBones;    //used only for integrity checking
            //public Matrix4x4[] _originalBindPoses; //used only for integrity checking

            public int lightmapIndex = -1;
            public Vector4 lightmapTilingOffset = new Vector4(1f, 1f, 0f, 0f);

            public Vector3 meshSize = Vector3.one; // in world coordinates

            public bool show = true;

            public bool invertTriangles = false;

            /// <summary>
            /// combined mesh will have one submesh per result material
            /// source meshes can have any number of submeshes.They are mapped to a result submesh based on their material
            /// if two different submeshes have the same material they are merged in the same result submesh
            /// </summary>
            // These are result mesh submeshCount comine these into a class.
            public int[] submeshTriIdxs;
            public int[] submeshNumTris;

            /// <summary>
            /// These are source go mesh submeshCount todo combined these into a class.
            /// Maps each submesh in source mesh to a submesh in combined mesh.
            /// </summary>
            public int[] targetSubmeshIdxs;

            /// <summary>
            /// The UVRects in the combinedMaterial atlas.
            /// </summary>
            public Rect[] uvRects;

            /// <summary>
            /// If AllPropsUseSameMatTiling is the rect that was used for sampling the atlas texture from the source texture including both mesh uvTiling and material tiling.
            /// else is the source mesh obUVrect. We don't need to care which.
            /// </summary>
            public Rect[] encapsulatingRect;

            /// <summary>
            /// If AllPropsUseSameMatTiling is the source texture material tiling.
            /// else is 0,0,1,1.  We don't need to care which.
            /// </summary>
            public Rect[] sourceMaterialTiling;

            /// <summary>
            /// The obUVRect for each submesh;
            /// </summary>
            public Rect[] obUVRects;
            

            public bool _beingDeleted = false;
            public int _triangleIdxAdjustment = 0;

            //used so we don't have to call GetBones and GetBindposes twice
            [NonSerialized]
            public SerializableIntArray[] _tmpSubmeshTris;
            [NonSerialized]
            public Transform[] _tmpCachedBones;
            [NonSerialized]
            public Matrix4x4[] _tmpCachedBindposes;
            [NonSerialized]
            public BoneWeight[] _tmpCachedBoneWeights;
            [NonSerialized]
            public int[] _tmpIndexesOfSourceBonesUsed;

            public int CompareTo(MB_DynamicGameObject b)
            {
                return this.vertIdx - b.vertIdx;
            }
        }

        //if baking many instances of the same sharedMesh, want to cache these results rather than grab them multiple times from the mesh 
        public class MeshChannels
        {
            public Vector3[] vertices;
            public Vector3[] normals;
            public Vector4[] tangents;
            public Vector2[] uv0raw;
            public Vector2[] uv0modified;
            public Vector2[] uv2;
            public Vector2[] uv3;
            public Vector2[] uv4;
            public Color[] colors;
            public BoneWeight[] boneWeights;
            public Matrix4x4[] bindPoses;
            public int[] triangles;
            public MBBlendShape[] blendShapes;
        }

        [Serializable]
        public class MBBlendShapeFrame
        {
            public float frameWeight;
            public Vector3[] vertices;
            public Vector3[] normals;
            public Vector3[] tangents;
        }

        [Serializable]
        public class MBBlendShape
        {
            public int gameObjectID;
            public string name;
            public int indexInSource;
            public MBBlendShapeFrame[] frames;
        }

        public class MeshChannelsCache
        {
            MB3_MeshCombinerSingle mc;
            protected Dictionary<int, MeshChannels> meshID2MeshChannels = new Dictionary<int, MeshChannels>();

            public MeshChannelsCache(MB3_MeshCombinerSingle mcs)
            {
                mc = mcs;
            }

            internal Vector3[] GetVertices(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.vertices == null)
                {
                    mc.vertices = m.vertices;
                }
                return mc.vertices;
            }
            internal Vector3[] GetNormals(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.normals == null)
                {
                    mc.normals = _getMeshNormals(m);
                }
                return mc.normals;
            }
            internal Vector4[] GetTangents(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.tangents == null)
                {
                    mc.tangents = _getMeshTangents(m);
                }
                return mc.tangents;
            }

            internal Vector2[] GetUv0Raw(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.uv0raw == null)
                {
                    mc.uv0raw = _getMeshUVs(m);
                }
                return mc.uv0raw;
            }
            internal Vector2[] GetUv0Modified(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.uv0modified == null)
                {
                    //todo
                    mc.uv0modified = null;
                }
                return mc.uv0modified;
            }
            internal Vector2[] GetUv2(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.uv2 == null)
                {
                    mc.uv2 = _getMeshUV2s(m);
                }
                return mc.uv2;
            }
            internal Vector2[] GetUv3(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.uv3 == null)
                {
                    mc.uv3 = MBVersion.GetMeshUV3orUV4(m, true, this.mc.LOG_LEVEL);
                }
                return mc.uv3;
            }
            internal Vector2[] GetUv4(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.uv4 == null)
                {
                    mc.uv4 = MBVersion.GetMeshUV3orUV4(m, false, this.mc.LOG_LEVEL);
                }
                return mc.uv4;
            }
            internal Color[] GetColors(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.colors == null)
                {
                    mc.colors = _getMeshColors(m);
                }
                return mc.colors;
            }
            internal Matrix4x4[] GetBindposes(Renderer r)
            {
                MeshChannels mc;
                Mesh m = MB_Utility.GetMesh(r.gameObject);
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.bindPoses == null)
                {
                    mc.bindPoses = _getBindPoses(r);
                }
                return mc.bindPoses;
            }

            internal BoneWeight[] GetBoneWeights(Renderer r, int numVertsInMeshBeingAdded)
            {
                MeshChannels mc;
                Mesh m = MB_Utility.GetMesh(r.gameObject);
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.boneWeights == null)
                {
                    mc.boneWeights = _getBoneWeights(r, numVertsInMeshBeingAdded);
                }
                return mc.boneWeights;
            }

            internal int[] GetTriangles(Mesh m)
            {
                MeshChannels mc;
                if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                {
                    mc = new MeshChannels();
                    meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                }
                if (mc.triangles == null)
                {
                    mc.triangles = m.triangles;
                }
                return mc.triangles;
            }

            internal MBBlendShape[] GetBlendShapes(Mesh m, int gameObjectID)
            {
                if (MBVersion.GetMajorVersion() > 5 ||
                ( MBVersion.GetMajorVersion() == 5 && MBVersion.GetMinorVersion() >= 3))
                {

                    MeshChannels mc;
                    if (!meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out mc))
                    {
                        mc = new MeshChannels();
                        meshID2MeshChannels.Add(m.GetInstanceID(), mc);
                    }
                    if (mc.blendShapes == null)
                    {
                        MBBlendShape[] shapes = new MBBlendShape[m.blendShapeCount];
                        int arrayLen = m.vertexCount;
                        for (int i = 0; i < shapes.Length; i++)
                        {
                            MBBlendShape shape = shapes[i] = new MBBlendShape();
                            shape.frames = new MBBlendShapeFrame[MBVersion.GetBlendShapeFrameCount(m, i)];
                            shape.name = m.GetBlendShapeName(i);
                            shape.indexInSource = i;
                            shape.gameObjectID = gameObjectID;
                            for (int j = 0; j < shape.frames.Length; j++)
                            {
                                MBBlendShapeFrame frame = shape.frames[j] = new MBBlendShapeFrame();
                                frame.frameWeight = MBVersion.GetBlendShapeFrameWeight(m, i, j);
                                frame.vertices = new Vector3[arrayLen];
                                frame.normals = new Vector3[arrayLen];
                                frame.tangents = new Vector3[arrayLen];
                                MBVersion.GetBlendShapeFrameVertices(m, i, j, frame.vertices, frame.normals, frame.tangents);
                            }
                        }
                        mc.blendShapes = shapes;
                        return mc.blendShapes;
                    }
                    else
                    { //copy cached blend shapes assigning a different gameObjectID
                        MBBlendShape[] shapes = new MBBlendShape[mc.blendShapes.Length];
                        for (int i = 0; i < shapes.Length; i++)
                        {
                            shapes[i] = new MBBlendShape();
                            shapes[i].name = mc.blendShapes[i].name;
                            shapes[i].indexInSource = mc.blendShapes[i].indexInSource;
                            shapes[i].frames = mc.blendShapes[i].frames;
                            shapes[i].gameObjectID = gameObjectID;
                        }
                        return shapes;
                    }
                } else {
                    return new MBBlendShape[0];
                }
            }

            Color[] _getMeshColors(Mesh m)
            {
                Color[] cs = m.colors;
                if (cs.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no colors. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have colors. Generating an array of white colors");
                    cs = new Color[m.vertexCount];
                    for (int i = 0; i < cs.Length; i++) { cs[i] = Color.white; }
                }
                return cs;
            }

            Vector3[] _getMeshNormals(Mesh m)
            {
                Vector3[] ns = m.normals;
                if (ns.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no normals. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have normals. Generating normals.");
                    Mesh tempMesh = (Mesh)GameObject.Instantiate(m);
                    tempMesh.RecalculateNormals();
                    ns = tempMesh.normals;
                    MB_Utility.Destroy(tempMesh);
                }
                return ns;
            }

            Vector4[] _getMeshTangents(Mesh m)
            {
                Vector4[] ts = m.tangents;
                if (ts.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no tangents. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have tangents. Generating tangents.");
                    Vector3[] verts = m.vertices;
                    Vector2[] uvs = GetUv0Raw(m);
                    Vector3[] norms = _getMeshNormals(m);
                    ts = new Vector4[m.vertexCount];
                    for (int i = 0; i < m.subMeshCount; i++)
                    {
                        int[] tris = m.GetTriangles(i);
                        _generateTangents(tris, verts, uvs, norms, ts);
                    }
                }
                return ts;
            }

            Vector2 _HALF_UV = new Vector2(.5f, .5f);
            Vector2[] _getMeshUVs(Mesh m)
            {
                Vector2[] uv = m.uv;
                if (uv.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no uvs. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have uvs. Generating uvs.");
                    uv = new Vector2[m.vertexCount];
                    for (int i = 0; i < uv.Length; i++) { uv[i] = _HALF_UV; }
                }
                return uv;
            }

            Vector2[] _getMeshUV2s(Mesh m)
            {
                Vector2[] uv = m.uv2;
                if (uv.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no uv2s. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have uv2s. Generating uv2s.");
                    if (this.mc._lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged_to_separate_rects) Debug.LogError("Mesh " + m + " did not have a UV2 channel. Nothing to copy when trying to copy UV2 to separate rects. The combined mesh will not lightmap properly. Try using generate new uv2 layout.");
                    uv = new Vector2[m.vertexCount];
                    for (int i = 0; i < uv.Length; i++) { uv[i] = _HALF_UV; }
                }
                return uv;
            }

            public static Matrix4x4[] _getBindPoses(Renderer r)
            {
                if (r is SkinnedMeshRenderer)
                {
                    return ((SkinnedMeshRenderer)r).sharedMesh.bindposes;
                }
                else if (r is MeshRenderer)
                {
                    Matrix4x4 bindPose = Matrix4x4.identity;
                    Matrix4x4[] poses = new Matrix4x4[1];
                    poses[0] = bindPose;
                    return poses;
                }
                else {
                    Debug.LogError("Could not _getBindPoses. Object does not have a renderer");
                    return null;
                }
            }

            public static BoneWeight[] _getBoneWeights(Renderer r, int numVertsInMeshBeingAdded)
            {
                if (r is SkinnedMeshRenderer)
                {
                    return ((SkinnedMeshRenderer)r).sharedMesh.boneWeights;
                }
                else if (r is MeshRenderer)
                {
                    BoneWeight bw = new BoneWeight();
                    bw.boneIndex0 = bw.boneIndex1 = bw.boneIndex2 = bw.boneIndex3 = 0;
                    bw.weight0 = 1f;
                    bw.weight1 = bw.weight2 = bw.weight3 = 0f;
                    BoneWeight[] bws = new BoneWeight[numVertsInMeshBeingAdded];
                    for (int i = 0; i < bws.Length; i++) bws[i] = bw;
                    return bws;
                }
                else {
                    Debug.LogError("Could not _getBoneWeights. Object does not have a renderer");
                    return null;
                }
            }


            void _generateTangents(int[] triangles, Vector3[] verts, Vector2[] uvs, Vector3[] normals, Vector4[] outTangents)
            {
                int triangleCount = triangles.Length;
                int vertexCount = verts.Length;

                Vector3[] tan1 = new Vector3[vertexCount];
                Vector3[] tan2 = new Vector3[vertexCount];

                for (int a = 0; a < triangleCount; a += 3)
                {
                    int i1 = triangles[a + 0];
                    int i2 = triangles[a + 1];
                    int i3 = triangles[a + 2];

                    Vector3 v1 = verts[i1];
                    Vector3 v2 = verts[i2];
                    Vector3 v3 = verts[i3];

                    Vector2 w1 = uvs[i1];
                    Vector2 w2 = uvs[i2];
                    Vector2 w3 = uvs[i3];

                    float x1 = v2.x - v1.x;
                    float x2 = v3.x - v1.x;
                    float y1 = v2.y - v1.y;
                    float y2 = v3.y - v1.y;
                    float z1 = v2.z - v1.z;
                    float z2 = v3.z - v1.z;

                    float s1 = w2.x - w1.x;
                    float s2 = w3.x - w1.x;
                    float t1 = w2.y - w1.y;
                    float t2 = w3.y - w1.y;

                    float rBot = (s1 * t2 - s2 * t1);
                    if (rBot == 0f)
                    {
                        Debug.LogError("Could not compute tangents. All UVs need to form a valid triangles in UV space. If any UV triangles are collapsed, tangents cannot be generated.");
                        return;
                    }
                    float r = 1.0f / rBot;

                    Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                    Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                    tan1[i1] += sdir;
                    tan1[i2] += sdir;
                    tan1[i3] += sdir;

                    tan2[i1] += tdir;
                    tan2[i2] += tdir;
                    tan2[i3] += tdir;
                }


                for (int a = 0; a < vertexCount; ++a)
                {
                    Vector3 n = normals[a];
                    Vector3 t = tan1[a];

                    Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
                    outTangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
                    outTangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
                }
            }
        }

        //Used for comparing if skinned meshes use the same bone and bindpose.
        //Skinned meshes must be bound with the same TRS to share a bone.
        public struct BoneAndBindpose
        {
            public Transform bone;
            public Matrix4x4 bindPose;

            public BoneAndBindpose(Transform t, Matrix4x4 bp)
            {
                bone = t;
                bindPose = bp;
            }

            public override bool Equals(object obj)
            {
                if (obj is BoneAndBindpose)
                {
                    if (bone == ((BoneAndBindpose)obj).bone && bindPose == ((BoneAndBindpose)obj).bindPose)
                    {
                        return true;
                    }
                }
                return false;
            }

            public override int GetHashCode()
            {
                //OK if don't check bindPose well because bp should be the same
                return (bone.GetInstanceID() % 2147483647) ^ (int)bindPose[0, 0];
            }
        }
    }
}
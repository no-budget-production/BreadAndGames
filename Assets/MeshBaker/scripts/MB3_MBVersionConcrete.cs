/**
 *	\brief Hax!  DLLs cannot interpret preprocessor directives, so this class acts as a "bridge"
 */
using System;
using UnityEngine;
using System.Collections;

namespace DigitalOpus.MB.Core{

	public class MBVersionConcrete:MBVersionInterface{
		public string version(){
			return "3.26.2";	 
		}
		
		public int GetMajorVersion(){
            /*
            #if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
                        return 3;
            #elif UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
                        return 4;
            #else
                        return 5;
            #endif
            */
            string v = Application.unityVersion;
            String[] vs = v.Split(new char[] { '.' });
            return Int32.Parse(vs[0]);
        }

        public int GetMinorVersion(){
            /*
			#if UNITY_3_0 || UNITY_3_0_0 
			return 0;
			#elif UNITY_3_1 
			return 1;
			#elif UNITY_3_2 
			return 2;
			#elif UNITY_3_3 
			return 3;
			#elif UNITY_3_4 
			return 4;
			#elif UNITY_3_5
			return 5;
			#elif UNITY_4_0 || UNITY_4_0_1
			return 0;
			#elif UNITY_4_1
			return 1;
			#elif UNITY_4_2
			return 2;
			#elif UNITY_4_3
			return 3;
			#elif UNITY_4_4
			return 4;
			#elif UNITY_4_5
			return 5;
			#else
			return 0;
			#endif
            */
            string v = Application.unityVersion;
            String[] vs = v.Split(new char[] { '.' });
            return Int32.Parse(vs[1]);
        }
		
		public bool GetActive(GameObject go){
			#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
			return go.active;
			#else
			return go.activeInHierarchy;
			#endif			
		}
		
		public void SetActive(GameObject go, bool isActive){
			#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
			go.active = isActive;
			#else
			go.SetActive(isActive);
			#endif
		}
		
		public void SetActiveRecursively(GameObject go, bool isActive){
			#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
			go.SetActiveRecursively(isActive);
			#else
			go.SetActive(isActive);
			#endif
		}
		
		public UnityEngine.Object[] FindSceneObjectsOfType(Type t){
			#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
			return GameObject.FindSceneObjectsOfType(t);
			#else
			return GameObject.FindObjectsOfType(t);
			#endif				
		}
		
        public void OptimizeMesh(Mesh m)
        {
#if UNITY_EDITOR
#if UNITY_5_5_OR_NEWER
    UnityEditor.MeshUtility.Optimize(m);  
#else
            m.Optimize();
#endif
#endif
        }


        public bool IsRunningAndMeshNotReadWriteable(Mesh m){
			if (Application.isPlaying){
				#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
				return false;
				#else
				return !m.isReadable;
				#endif
			} else {
				return false;
			}
		}

		Vector2 _HALF_UV = new Vector2(.5f, .5f);
		public Vector2[] GetMeshUV1s(Mesh m, MB2_LogLevel LOG_LEVEL)
		{
			Vector2[] uv;
			#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5)
			uv = m.uv1;

			#else
			if (LOG_LEVEL >= MB2_LogLevel.warn) MB2_Log.LogDebug("UV1 does not exist in Unity 5+");
			uv = m.uv;
			#endif
			if (uv.Length == 0){
				if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no uv1s. Generating");
				if (LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have uv1s. Generating uv1s.");			
				uv = new Vector2[m.vertexCount];
				for (int i = 0; i < uv.Length; i++){uv[i] = _HALF_UV;}
			}		
			return uv;
		}

        public Vector2[] GetMeshUV3orUV4(Mesh m, bool get3, MB2_LogLevel LOG_LEVEL) {
            Vector2[] uvs;
#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5)
			if (LOG_LEVEL >= MB2_LogLevel.warn) MB2_Log.LogDebug("UV3 and UV4 do not exist in Unity 4");
            uvs = m.uv;
#else
            if (get3) uvs = m.uv3;
            else uvs = m.uv4;
#endif
            if (uvs.Length == 0) {
                if (LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no uv" + (get3 ? "3" : "4") + ". Generating");
                uvs = new Vector2[m.vertexCount];
                for (int i = 0; i < uvs.Length; i++) { uvs[i] = _HALF_UV; }
            }
            return uvs;
        }

        public void MeshClear(Mesh m, bool t){
#if UNITY_3_5
				m.Clear();
#else
				m.Clear(t);
#endif
		}

		public void MeshAssignUV3(Mesh m, Vector2[] uv3s){
#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5)
			Debug.LogWarning("UV3 was checked but UV3 does not exist in Unity 4");
#else
            m.uv3 = uv3s;
#endif
        }

        public void MeshAssignUV4(Mesh m, Vector2[] uv4s) {
#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5)
			Debug.LogWarning("UV4 was checked but UV4 does not exist in Unity 4");
#else
            m.uv4 = uv4s;
#endif
        }

        public Vector4 GetLightmapTilingOffset(Renderer r){
#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5)
			  return r.lightmapTilingOffset ;
#else
			return r.lightmapScaleOffset; //r.lightmapScaleOffset ;
#endif
		}
#if UNITY_5_OR_NEWER
        public Transform[] GetBones(Renderer r)
        {
            if (r is SkinnedMeshRenderer)
            {
                Transform[] bone;
                //check if I need to deoptimize
                Animator anim = r.GetComponentInParent<Animator>();

                if (anim != null)
                {
                    if (anim.hasTransformHierarchy)
                    {
                        //nothing to do
                    } else if (anim.isOptimizable)
                    {
                        //Deoptimize
                        AnimatorUtility.DeoptimizeTransformHierarchy(anim.gameObject);
                        
                    }
                    else
                    {
                        Debug.LogError("Could not getBones. Bones optimized but could not create TransformHierarchy.");
                        return null;
                    }
                    bone = ((SkinnedMeshRenderer)r).bones;
                    //can't deoptimize here because the transforms need to exist for the combined  mesh
                } else
                {
                    //no Animator component but check to see if bones were optimized on import
                    bone = ((SkinnedMeshRenderer)r).bones;
#if UNITY_EDITOR
                    if (bone.Length == 0)
                    {
					Mesh m = ((SkinnedMeshRenderer)r).sharedMesh;
					if (m.bindposes.Length != bone.Length) Debug.LogError("SkinnedMesh (" + r.gameObject + ") in the list of objects to combine has no bones. Check that 'optimize game object' is not checked in the 'Rig' tab of the asset importer. Mesh Baker cannot combine optimized skinned meshes because the bones are not available.");
				}
#endif
                }
				return bone;	
            }
            else if (r is MeshRenderer)
            {
				Transform[] bone = new Transform[1];
				bone[0] = r.transform;
				return bone;
            }
            else {
                Debug.LogError("Could not getBones. Object is not a Renderer.");
				return null;
			}
		}
#else
        public Transform[] GetBones(Renderer r){
			if (r is SkinnedMeshRenderer){
				Transform[] bone = ((SkinnedMeshRenderer)r).bones;
				#if UNITY_EDITOR
				if (bone.Length == 0){
					Mesh m = ((SkinnedMeshRenderer)r).sharedMesh;
					if (m.bindposes.Length != bone.Length) Debug.LogError("SkinnedMesh (" + r.gameObject + ") in the list of objects to combine has no bones. Check that 'optimize game object' is not checked in the 'Rig' tab of the asset importer. Mesh Baker cannot combine optimized skinned meshes because the bones are not available.");
				}
				#endif
				return bone;	
			} else if (r is MeshRenderer){
				Transform[] bone = new Transform[1];
				bone[0] = r.transform;
				return bone;
			} else {
				Debug.LogError("Could not getBones. Object does not have a renderer");
				return null;
			}
		}
#endif

        public int GetBlendShapeFrameCount(Mesh m, int shapeIndex)
        {
#if UNITY_5_3_OR_NEWER
            return m.GetBlendShapeFrameCount(shapeIndex);
#else
            return 0;
#endif
        }

        public float GetBlendShapeFrameWeight(Mesh m, int shapeIndex, int frameIndex)
        {
#if UNITY_5_3_OR_NEWER
            return m.GetBlendShapeFrameWeight(shapeIndex, frameIndex);
#else
            return 0;
#endif
        }

        public void GetBlendShapeFrameVertices(Mesh m, int shapeIndex, int frameIndex, Vector3[] vs, Vector3[] ns, Vector3[] ts)
        {
#if UNITY_5_3_OR_NEWER
            m.GetBlendShapeFrameVertices(shapeIndex, frameIndex, vs, ns, ts);
#endif
        }

        public void ClearBlendShapes(Mesh m)
        {
#if UNITY_5_3_OR_NEWER
            m.ClearBlendShapes();
#endif
        }

        public void AddBlendShapeFrame(Mesh m, string nm, float wt, Vector3[] vs, Vector3[] ns, Vector3[] ts)
        {
#if UNITY_5_3_OR_NEWER
            m.AddBlendShapeFrame(nm, wt, vs, ns, ts);
#endif
        }

        public int MaxMeshVertexCount()
        {
#if UNITY_2017_3_OR_NEWER
            return 2147483646;
#else
            return 65534;
#endif
        }

        public void SetMeshIndexFormatAndClearMesh(Mesh m, int numVerts, bool vertices, bool justClearTriangles)
        {
#if UNITY_2017_3_OR_NEWER
            if (vertices && numVerts > 65534 && m.indexFormat == UnityEngine.Rendering.IndexFormat.UInt16)
            {
                MBVersion.MeshClear(m, false);
                m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                return;
            }
            else if (vertices && numVerts <= 65534 && m.indexFormat == UnityEngine.Rendering.IndexFormat.UInt32)
            {
                MBVersion.MeshClear(m, false);
                m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
                return;
            }
#endif
            if (justClearTriangles)
            {
                MBVersion.MeshClear(m, true); //clear just triangles 
            }
            else
            {//clear all the data and start with a blank mesh
                MBVersion.MeshClear(m, false);
            }
        }
    }

}
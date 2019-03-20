/**
 *	\brief Hax!  DLLs cannot interpret preprocessor directives, so this class acts as a "bridge"
 */
using System;
using UnityEngine;
using System.Collections;

namespace DigitalOpus.MB.Core{

	public interface MBVersionInterface{
		string version();
		int GetMajorVersion();
		int GetMinorVersion();
		bool GetActive(GameObject go);
		void SetActive(GameObject go, bool isActive);
		void SetActiveRecursively(GameObject go, bool isActive);
		UnityEngine.Object[] FindSceneObjectsOfType(Type t);
		bool IsRunningAndMeshNotReadWriteable(Mesh m);
        Vector2[] GetMeshUV3orUV4(Mesh m, bool get3, MB2_LogLevel LOG_LEVEL);
        void MeshClear(Mesh m, bool t);
		void MeshAssignUV3(Mesh m, Vector2[] uv3s);
        void MeshAssignUV4(Mesh m, Vector2[] uv4s);
        Vector4 GetLightmapTilingOffset(Renderer r);
		Transform[] GetBones(Renderer r);
        void OptimizeMesh(Mesh m);
        int GetBlendShapeFrameCount(Mesh m, int shapeIndex);
        float GetBlendShapeFrameWeight(Mesh m, int shapeIndex, int frameIndex);
        void GetBlendShapeFrameVertices(Mesh m, int shapeIndex, int frameIndex, Vector3[] vs, Vector3[] ns, Vector3[] ts);
        void ClearBlendShapes(Mesh m);
        void AddBlendShapeFrame(Mesh m, string nm, float wt, Vector3[] vs, Vector3[] ns, Vector3[] ts);
        int MaxMeshVertexCount();
        void SetMeshIndexFormatAndClearMesh(Mesh m, int numVerts, bool vertices, bool justClearTriangles);
    }

	public class MBVersion
	{
		private static MBVersionInterface _MBVersion;

		private static MBVersionInterface _CreateMBVersionConcrete(){
			Type vit = null;
#if EVAL_VERSION
			vit = Type.GetType("DigitalOpus.MB.Core.MBVersionConcrete,Assembly-CSharp");
#else
			vit = typeof(MBVersionConcrete);
#endif
			return (MBVersionInterface) Activator.CreateInstance(vit);
		}

		public static string version(){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			return _MBVersion.version();
		}
		
		public static int GetMajorVersion(){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			return _MBVersion.GetMajorVersion();	
		}

		public static int GetMinorVersion(){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			return _MBVersion.GetMinorVersion();
		}

		public static bool GetActive(GameObject go){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			return _MBVersion.GetActive(go);
		}
	
		public static void SetActive(GameObject go, bool isActive){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			_MBVersion.SetActive(go,isActive);
		}
		
		public static void SetActiveRecursively(GameObject go, bool isActive){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			_MBVersion.SetActiveRecursively(go,isActive);
		}

		public static UnityEngine.Object[] FindSceneObjectsOfType(Type t){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			return _MBVersion.FindSceneObjectsOfType(t);				
		}

		public static bool IsRunningAndMeshNotReadWriteable(Mesh m){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			return _MBVersion.IsRunningAndMeshNotReadWriteable(m);
		}

        public static Vector2[] GetMeshUV3orUV4(Mesh m, bool get3, MB2_LogLevel LOG_LEVEL) {
            if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
            return _MBVersion.GetMeshUV3orUV4(m,get3,LOG_LEVEL);
        }

        public static void MeshClear(Mesh m, bool t){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			_MBVersion.MeshClear(m,t);
		}

		public static void MeshAssignUV3(Mesh m, Vector2[] uv3s){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			_MBVersion.MeshAssignUV3(m,uv3s);
		}

        public static void MeshAssignUV4(Mesh m, Vector2[] uv4s) {
            if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
            _MBVersion.MeshAssignUV4(m, uv4s);
        }

        public static Vector4 GetLightmapTilingOffset(Renderer r){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			return _MBVersion.GetLightmapTilingOffset(r);
		}

		public static Transform[] GetBones(Renderer r){
			if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
			return _MBVersion.GetBones(r);
		}

        public static void OptimizeMesh(Mesh m)
        {
            if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
            _MBVersion.OptimizeMesh(m);
        }

        public static int GetBlendShapeFrameCount(Mesh m, int shapeIndex)
        {
            if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
            return _MBVersion.GetBlendShapeFrameCount(m, shapeIndex);
        }

        public static float GetBlendShapeFrameWeight(Mesh m, int shapeIndex, int frameIndex)
        {
            if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
            return _MBVersion.GetBlendShapeFrameWeight(m, shapeIndex, frameIndex);
        }

        public static void GetBlendShapeFrameVertices(Mesh m, int shapeIndex, int frameIndex, Vector3[] vs, Vector3[] ns, Vector3[] ts)
        {
            if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
            _MBVersion.GetBlendShapeFrameVertices(m, shapeIndex, frameIndex, vs, ns, ts);
        }

        public static void ClearBlendShapes(Mesh m)
        {
            if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
            _MBVersion.ClearBlendShapes(m);
        }

        public static void AddBlendShapeFrame(Mesh m, string nm, float wt, Vector3[] vs, Vector3[] ns, Vector3[] ts)
        {
            if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
            _MBVersion.AddBlendShapeFrame(m, nm, wt, vs, ns, ts);
        }

        public static int MaxMeshVertexCount()
        {
            if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
            return _MBVersion.MaxMeshVertexCount();
        }

        public static void SetMeshIndexFormatAndClearMesh(Mesh m, int numVerts, bool vertices, bool justClearTriangles)
        {
            if (_MBVersion == null) _MBVersion = _CreateMBVersionConcrete();
            _MBVersion.SetMeshIndexFormatAndClearMesh(m, numVerts, vertices, justClearTriangles);
        }
    }
}
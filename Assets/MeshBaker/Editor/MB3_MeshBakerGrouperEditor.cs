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


[CustomEditor(typeof(MB3_MeshBakerGrouper))]
public class MB3_MeshBakerGrouperEditor : Editor
{

    long lastBoundsCheckRefreshTime = 0;

    static GUIContent gc_ClusterType = new GUIContent("Cluster Type", "The scene will be divided cells. Meshes in each cell will be grouped into a single mesh baker");
    static GUIContent gc_GridOrigin = new GUIContent("Origin", "The scene will be divided into of cells. Meshes in each cell will be grouped into a single baker. This sets the origin for the clustering.");
    static GUIContent gc_CellSize = new GUIContent("Cell Size", "The scene will be divided into a grid of cells. Meshes in each cell will be grouped into a single baker. This sets the size of the cells.");
    static GUIContent gc_ClusterOnLMIndex = new GUIContent("Group By Lightmap Index", "Meshes sharing a lightmap index will be grouped together.");
    static GUIContent gc_NumSegements = new GUIContent("Num Pie Segments", "Number of segments/slices in the pie.");
    static GUIContent gc_PieAxis = new GUIContent("Pie Axis", "Scene will be divided into segments about this axis.");
    static GUIContent gc_ClusterByLODLevel = new GUIContent("Cluster By LOD Level", "A baker will be created for each LOD level.");
    static GUIContent gc_ClusterDistance = new GUIContent("Max Distance", "Source meshes closer than this value will be grouped into clusters.");
    static GUIContent gc_IncludeCellsWithOnlyOneRenderer = new GUIContent("Include Cells With Only One Renderer", "There is no benefit in combining meshes with only one mesh except to adjust UVs to share an atlas.");


    private SerializedObject grouper;
    private SerializedProperty clusterType, gridOrigin, cellSize, clusterOnLMIndex, numSegments, pieAxis, clusterByLODLevel, clusterDistance, includeCellsWithOnlyOneRenderer;

    public void OnEnable()
    {
        lastBoundsCheckRefreshTime = 0;
        grouper = new SerializedObject(target);
       
        SerializedProperty d = grouper.FindProperty("data");
        
        clusterType = grouper.FindProperty("clusterType");
        includeCellsWithOnlyOneRenderer = d.FindPropertyRelative("includeCellsWithOnlyOneRenderer");
        gridOrigin = d.FindPropertyRelative("origin");
        cellSize = d.FindPropertyRelative("cellSize");
        clusterOnLMIndex = d.FindPropertyRelative("clusterOnLMIndex");
        clusterByLODLevel = d.FindPropertyRelative("clusterByLODLevel");
        numSegments = d.FindPropertyRelative("pieNumSegments");
        pieAxis = d.FindPropertyRelative("pieAxis");
       // clusterTreeHeight = d.FindPropertyRelative("height");
        clusterDistance = d.FindPropertyRelative("maxDistBetweenClusters");
       
    }

    public override void OnInspectorGUI()
    {
        MB3_MeshBakerGrouper tbg = (MB3_MeshBakerGrouper)target;
        MB3_TextureBaker tb = ((MB3_MeshBakerGrouper)target).GetComponent<MB3_TextureBaker>();
        grouper.Update();
        DrawGrouperInspector();
        EditorGUILayout.PropertyField(includeCellsWithOnlyOneRenderer, gc_IncludeCellsWithOnlyOneRenderer);
        if (GUILayout.Button("Generate Mesh Bakers"))
        {
            if (tb == null)
            {
                Debug.LogError("There must be an MB3_TextureBaker attached to this game object.");
                return;
            }
            if (tb.GetObjectsToCombine().Count == 0)
            {
                Debug.LogError("The MB3_MeshBakerGrouper creates clusters based on the objects to combine in the MB3_TextureBaker component. There were no objects in this list.");
                return;
            }
            //check if any of the objes that will be added to bakers already exist in child bakers
            List<GameObject> objsWeAreGrouping = tb.GetObjectsToCombine();
            MB3_MeshBakerCommon[] alreadyExistBakers = tbg.GetComponentsInChildren<MB3_MeshBakerCommon>();
            bool foundChildBakersWithObjsToCombine = false;
            for (int i = 0; i < alreadyExistBakers.Length; i++)
            {
                List<GameObject> childOjs2Combine = alreadyExistBakers[i].GetObjectsToCombine();
                for (int j = 0; j < childOjs2Combine.Count; j++)
                {
                    if (childOjs2Combine[j] != null && objsWeAreGrouping.Contains(childOjs2Combine[j]))
                    {
                        Debug.LogError("Could not Generate Mesh Bakers. This MeshBakerGrouper already has child MeshBaker's that contain some of the objects to be grouped. Remove these child bakers before trying to Generate Mesh Bakers.");
                        foundChildBakersWithObjsToCombine = true;
                        break;
                    }
                }
                Debug.LogWarning("This MB3_TextureBaker had some existing child objects. You may want to delete these before 'Generating Mesh Bakers' since your source objects may be included in the List Of Objects To Combine of multiple MeshBaker objects.");
            }
            if (!foundChildBakersWithObjsToCombine)
            {
                ((MB3_MeshBakerGrouper)target).grouper.DoClustering(tb,tbg);
            }
        }
        if (GUILayout.Button("Bake All Child MeshBakers"))
        {
            try
            {
                MB3_MeshBakerCommon[] mBakers = tbg.GetComponentsInChildren<MB3_MeshBakerCommon>();
                for (int i = 0; i < mBakers.Length; i++)
                {
                    bool createdDummyMaterialBakeResult;
                    MB3_MeshBakerEditorFunctions.BakeIntoCombined(mBakers[i], out createdDummyMaterialBakeResult);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        string buttonTextEnableRenderers = "Disable Renderers On All Child MeshBaker Source Objs";
        bool enableRenderers = false;
        MB3_MeshBakerCommon bc = tbg.GetComponentInChildren<MB3_MeshBakerCommon>();
        if (bc != null && bc.GetObjectsToCombine().Count > 0)
        {
            GameObject go = bc.GetObjectsToCombine()[0];
            if (go != null && go.GetComponent<Renderer>() != null && go.GetComponent<Renderer>().enabled == false)
            {
                buttonTextEnableRenderers = "Enable Renderers On All Child MeshBaker Source Objs";
                enableRenderers = true;
            }
        }
        if (GUILayout.Button(buttonTextEnableRenderers))
        {
            try
            {
                MB3_MeshBakerCommon[] mBakers = tbg.GetComponentsInChildren<MB3_MeshBakerCommon>();
                for (int i = 0; i < mBakers.Length; i++)
                {
                    mBakers[i].EnableDisableSourceObjectRenderers(enableRenderers);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        if (DateTime.UtcNow.Ticks - lastBoundsCheckRefreshTime > 10000000 && tb != null)
        {
            List<GameObject> gos = tb.GetObjectsToCombine();
            Bounds b = new Bounds(Vector3.zero, Vector3.one);
            if (gos.Count > 0 && gos[0] != null && gos[0].GetComponent<Renderer>() != null)
            {
                b = gos[0].GetComponent<Renderer>().bounds;
            }
            for (int i = 0; i < gos.Count; i++)
            {
                if (gos[i] != null && gos[i].GetComponent<Renderer>() != null)
                {
                    b.Encapsulate(gos[i].GetComponent<Renderer>().bounds);
                }
            }
            tbg.sourceObjectBounds = b;
            lastBoundsCheckRefreshTime = DateTime.UtcNow.Ticks;
        }
        grouper.ApplyModifiedProperties();
    }

    public void DrawGrouperInspector()
    {
        EditorGUILayout.HelpBox("This component helps you group meshes that are close together so they can be combined together." +
                            " It generates multiple MB3_MeshBaker objects from the List Of Objects to be combined in the MB3_TextureBaker component." +
                            " Objects that are close together will be grouped together and added to a new child MB3_MeshBaker object.\n\n" +
                            " TIP: Try the new agglomerative cluster type. It's awsome!", MessageType.Info);
        MB3_MeshBakerGrouper grouper = (MB3_MeshBakerGrouper)target;
        
        EditorGUILayout.PropertyField(clusterType, gc_ClusterType);
        MB3_MeshBakerGrouper.ClusterType gg = (MB3_MeshBakerGrouper.ClusterType)clusterType.enumValueIndex;
        if ((gg == MB3_MeshBakerGrouper.ClusterType.none && !(grouper.grouper is MB3_MeshBakerGrouperNone)) ||
            (gg == MB3_MeshBakerGrouper.ClusterType.grid && !(grouper.grouper is MB3_MeshBakerGrouperGrid)) ||
            (gg == MB3_MeshBakerGrouper.ClusterType.pie && !(grouper.grouper is MB3_MeshBakerGrouperPie)) ||
            (gg == MB3_MeshBakerGrouper.ClusterType.agglomerative && !(grouper.grouper is MB3_MeshBakerGrouperCluster))
            )
        {
            grouper.CreateGrouper(gg, grouper.data);
            grouper.clusterType = gg;
        }
        if (clusterType.enumValueIndex == (int) MB3_MeshBakerGrouper.ClusterType.grid)
        {
            EditorGUILayout.PropertyField(gridOrigin, gc_GridOrigin);
            EditorGUILayout.PropertyField(cellSize, gc_CellSize);
        }
        else if (clusterType.enumValueIndex == (int) MB3_MeshBakerGrouper.ClusterType.pie)
        {
            EditorGUILayout.PropertyField(gridOrigin, gc_GridOrigin);
            EditorGUILayout.PropertyField(numSegments, gc_NumSegements);
            EditorGUILayout.PropertyField(pieAxis, gc_PieAxis);
        }
        else if (clusterType.enumValueIndex == (int) MB3_MeshBakerGrouper.ClusterType.agglomerative)
        {
            float dist = clusterDistance.floatValue;
            float maxDist = 100f;
            float minDist = .000001f;
            MB3_MeshBakerGrouperCluster cl = null;
            if (grouper.grouper is MB3_MeshBakerGrouperCluster)
            {
                cl = (MB3_MeshBakerGrouperCluster)grouper.grouper;
                maxDist = cl._ObjsExtents;
                minDist = cl._minDistBetweenClusters;
                if (dist < minDist)
                {
                    dist = Mathf.Lerp(minDist, maxDist, .11f);
                }
            }
            
            dist = EditorGUILayout.Slider(gc_ClusterDistance, dist, minDist, maxDist);
            clusterDistance.floatValue = dist;
       
            string btnName = "Refresh Clusters";
            if (cl.cluster == null || cl.cluster.clusters == null || cl.cluster.clusters.Length == 0)
            {
                btnName = "Click To Build Clusters";
            }
            if (GUILayout.Button(btnName))
            {
                if (grouper.grouper is MB3_MeshBakerGrouperCluster)
                {
                    MB3_MeshBakerGrouperCluster cg = (MB3_MeshBakerGrouperCluster)grouper.grouper;
                    MB3_TextureBaker tb = grouper.GetComponent<MB3_TextureBaker>();
                    if (tb != null)
                    {
                        //MB3_EditorMethods em = new MB3_EditorMethods();
                        cg.BuildClusters(tb.GetObjectsToCombine(), updateProgressBar);
                        EditorUtility.ClearProgressBar();
                        Repaint();
                    }
                }
            }
        }
        EditorGUILayout.PropertyField(clusterOnLMIndex, gc_ClusterOnLMIndex);
        EditorGUILayout.PropertyField(clusterByLODLevel, gc_ClusterByLODLevel);
    }

    public bool updateProgressBar(string msg, float progress)
    {
        //EditorUtility.DisplayProgressBar("Creating Clusters", msg, progress);
        return EditorUtility.DisplayCancelableProgressBar("Creating Clusters", msg, progress);
    }
}

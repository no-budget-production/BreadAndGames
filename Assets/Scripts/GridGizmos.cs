using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGizmos : MonoBehaviour
{
    ////////Variables////////

    [SerializeField]
    private float size = 0.5f;

    public GameObject grid_Visual;

    

    ////////Functions////////

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        float posX = grid_Visual.transform.position.x;
        //float posY = grid_Visual.transform.position.y;
        float posZ = grid_Visual.transform.position.z;

        float xCount = ((posX + 0.5f) - 5);
        float yCount = position.y;
        float zCount = ((posZ + 0.5f) - 5);

        Vector3 result = new Vector3(
            (float)xCount + (position.x / 1f),
            (float)yCount,
            (float)zCount + (position.z / 1f));

        

        return result;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < 10; i++)
        {
            for (int k = 0; k < 10; k++)
            {
                var point = GetNearestPointOnGrid(new Vector3(i, 0f, k));
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }

}

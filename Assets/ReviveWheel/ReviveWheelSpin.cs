using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ReviveWheelSpin : MonoBehaviour {

    public float spinSpeed;
    public int spawnBlock = 4;
    public RectTransform block_Prefab;

    private void OnEnable()
    {
        float[] rotations = new float[spawnBlock];
        /*
        for (var i = 0; i < spawnBlock; i++)
        {
            var repeat = true;
            if (i == 0) repeat = false;
            do
            {
                rotations[i] = Random.Range(0f, 360f);
                for (var j = 1; j <= i; j++)
                {
                    if (rotations[i - j] - 25 > rotations[i] && rotations[i] > rotations[i - j] + 25 || rotations[i - j] - 25 < rotations[i] && rotations[i] < rotations[i - j] + 25) repeat = false;
                }
            } while (repeat);
        }
        */
        for (var i = 0; i < spawnBlock; i++)
        {
            var spawnedBlocks = Instantiate(block_Prefab, Vector3.zero, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
            spawnedBlocks.SetParent(transform, false);
        }
    }
    
    void Update ()
    {
        transform.rotation *= Quaternion.Euler(0, 0, (spinSpeed * Time.deltaTime));
	}
}

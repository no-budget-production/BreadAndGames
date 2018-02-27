using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ReviveWheelSpin : MonoBehaviour {

    public float spinSpeed;
    public int spawnBlock = 4;
    public float size;
    public RectTransform block_Prefab;
    private int direction = 1;
    private float speedMultiplier = 1;

    private void OnEnable()
    {
        float[] rotations = new float[spawnBlock];
        for (var i = 0; i < spawnBlock; i++)
        {
            var repeat = true;
            if (i == 0) repeat = false;
            do
            {
                rotations[i] = Random.Range(0, 359);
                var check = 0;
                for (var j = 1; j <= i; j++)
                {
                    if (!((rotations[i - j] - size) < rotations[i] && rotations[i] < (rotations[i - j] + size))) check++;
                }
                if (check == i) repeat = false;
            } while (repeat);
        }

        for (var i = 0; i < spawnBlock; i++)
        {
            var spawnedBlocks = Instantiate(block_Prefab, Vector3.zero, Quaternion.Euler(0f, 0f, rotations[i]));
            spawnedBlocks.SetParent(transform, false);
        }
    }
    
    void Update ()
    {
        transform.rotation *= Quaternion.Euler(0, 0, ((spinSpeed * speedMultiplier) * Time.deltaTime) * direction);
	}

    public void changeDirection ()
    {
        if (direction == 1)
        {
            direction = -1;
            return;
        }
        if (direction == -1)
        {
            direction = 1;
            return;
        }
    }

    public void increaseSpeed()
    {
        speedMultiplier += 1f;
    }

    public void resetSpeed()
    {
        speedMultiplier = 1f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDestroy : MonoBehaviour
{

    private void Awake()
    {
        Destroy(this.gameObject);
    }
}

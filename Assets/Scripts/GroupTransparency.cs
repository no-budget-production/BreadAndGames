using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupTransparency : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float aValue = 1f;
    private CanvasGroup trans;
    void Start()
    {
        trans = GetComponent<CanvasGroup>();
        trans.alpha = aValue;
    }
}

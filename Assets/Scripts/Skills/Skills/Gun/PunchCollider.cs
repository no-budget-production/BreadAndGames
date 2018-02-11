using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollider : MonoBehaviour
{
    public List<Character> enemies;

    public List<int> ReturnSelectedElements()
    {
        List<int> selectedElements = new List<int>();
        for (int i = 0; i < System.Enum.GetValues(typeof(UnitTypes)).Length; i++)
        {
            int layer = 1 << i;
            if (((int)ThisUnityTypeFlags & layer) != 0)
            {
                selectedElements.Add(i);
            }
        }
        return selectedElements;
    }

    [EnumFlagsAttribute]
    public UnitTypes ThisUnityTypeFlags;

    void OnTriggerEnter(Collider other)
    {
        var temp = other.GetComponent<Character>();
        if (temp == null)
            return;
        if ((temp.ThisUnityTypeFlags & ThisUnityTypeFlags) != 0)
        {
            enemies.Add(temp);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var temp = other.GetComponent<Character>();
        if (temp == null)
            return;
        if ((temp.ThisUnityTypeFlags & ThisUnityTypeFlags) != 0)
        {
            enemies.Add(temp);
        }
        enemies.Remove(temp);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PunchCollider : MonoBehaviour
{
    public List<Entity> Enemies;

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
    public UnitTypesFlags ThisUnityTypeFlags;

    void OnTriggerEnter(Collider other)
    {
        var temp = other.GetComponent<Entity>();
        if (temp == null)
            return;
        if (FlagsHelper.HasUnitTypes(temp.ThisUnityTypeFlags, ThisUnityTypeFlags))
        {
            Enemies.Add(temp);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var temp = other.GetComponent<Entity>();
        if (temp == null)
            return;
        if (FlagsHelper.HasUnitTypes(temp.ThisUnityTypeFlags, ThisUnityTypeFlags))
        {
            Enemies.Remove(temp);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreEnergie : Skill {

    public Transform[] energyRestorationPoints;
    public float drainRate;
    public float range;

    public override void Shoot()
    {
        var nearest_erp = GetClosestRestorationPoint(energyRestorationPoints);
        var distance = (nearest_erp.position - transform.position).magnitude;
        if (distance > range) return;

        var used_erp = nearest_erp.GetComponent<EnergyRestorationPoint>();
        if (used_erp.EnergyMaximum < (drainRate * Time.deltaTime)) return;

        var used = Character.RestoreActionPoints(drainRate);
        used_erp.EnergyMaximum -= used;
    }

    Transform GetClosestRestorationPoint(Transform[] erp)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in erp)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

}

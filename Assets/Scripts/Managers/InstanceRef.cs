using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstanceRef : MonoBehaviour
{
    public List<Transform> PlayerSpawns;
    //public Transform[] SpawnTriggerSpawns;
    public GameObject[] SphereTriggers;
    public Transform[] ReinforcementPoints;
    public Transform EnemyHolder;
    public Transform SpawnHolder;
    public Transform ClusterHolder;
    public Transform _DynamicHolder;
    public Transform ProjectileHolder;
    public Transform VisualsHolder;

    public Slider[] HUDHealthBarSlider;
    public Slider[] HUDActionPointsBar;
}

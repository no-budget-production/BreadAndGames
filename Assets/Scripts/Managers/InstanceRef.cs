using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstanceRef : MonoBehaviour
{
    public List<Transform> PlayerSpawns;
    public GameObject[] SphereTriggers;
    //public Transform[] ReinforcementPoints;
    public Transform EnemyHolder;
    public Transform SpawnHolder;
    public Transform ClusterHolder;
    public Transform _DynamicHolder;
    public Transform ProjectileHolder;
    public Transform VisualsHolder;

    public Slider[] HUDHealthBarSlider;
    public Slider[] HUDActionPointsBar;
    public Slider[] HUDOverChargeBar;
    public Slider[] HUDReloadBar;
    public Slider[] HUDRealReloadBar;

    public Image[] HUDHealthBarDelay;
    public Image[] HUDActionPointsBarDelay;
    public Image[] HUDOverChargeBarDelay;
    public Image[] HUDReloadBarDelay;
    public Image[] HUDRealReloadBarDelay;

    public UIScript UIScript;

    public ActivateWheel_Melee ReviveWheel_Melee;
    public ActivateWheel_Shooter ReviveWheel_Shooter;

    public GameObject[] HUDCanvas;
    public Image[] DamageImage;
}

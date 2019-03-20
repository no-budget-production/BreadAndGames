using UnityEngine;

public class LightSwitch : Cheat
{
    public string DirectionalLight = "Directional Light";
    public Vector3 NightLight = new Vector3(-10, -25, -180);
    public Vector3 DayLight = new Vector3(-35.58f, 0, 0);
    public bool isLightOn;

    public float DaylightIntensity;
    public float NightlightIntensity;

    private void Awake()
    {
        isLightOn = false;
        var LightObject = GameObject.Find("Directional Light");
        if (LightObject != null)
        {
            NightLight = LightObject.transform.rotation.eulerAngles;
            var Light = LightObject.GetComponent<Light>();
            if (Light != null)
            {
                Light.intensity = NightlightIntensity;
            }
        }
    }

    public override void Shoot()
    {
        if (isLightOn)
        {
            isLightOn = false;
            var LightObject = GameObject.Find("Directional Light");
            if (LightObject != null)
            {
                LightObject.transform.rotation = Quaternion.Euler(NightLight);
                var Light = LightObject.GetComponent<Light>();
                if (Light != null)
                {
                    Light.intensity = NightlightIntensity;
                    //Light.lightmapBakeType = LightmapBakeType.Mixed;
                }
            }
        }
        else
        {
            isLightOn = true;
            var LightObject = GameObject.Find("Directional Light");
            if (LightObject != null)
            {
                LightObject.transform.rotation = Quaternion.Euler(DayLight);
                var Light = LightObject.GetComponent<Light>();
                if (Light != null)
                {
                    Light.intensity = DaylightIntensity;
                    //Light.lightmapBakeType = LightmapBakeType.Realtime;
                }
            }
        }
    }
}
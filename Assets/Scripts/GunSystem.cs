using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    public Transform weaponHold;
    public Gun startingGun;
    Gun equippedGun;

	void Start ()
    {
	    if (startingGun != null)
        {
            EquipGun(startingGun);
        }
	}
	
	// Update is called once per frame
	public void EquipGun (Gun gunToEquip)
    {
	    if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equippedGun.transform.parent = weaponHold;
	}

    public void Shoot()
    {
        if (equippedGun != null)
            equippedGun.Shoot();
    }
}
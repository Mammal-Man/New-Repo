using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunData : MonoBehaviour
{
    public PlayerControl player;
    public float CurrentAmmo;
    public float CurrentMag;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.holdingWeapon)
        {
            CurrentAmmo = player.CurrentAmmo;
            CurrentMag = player.CurrentMag;
        }

        if(player.weaponID > -1)
        {

        }
    }
}

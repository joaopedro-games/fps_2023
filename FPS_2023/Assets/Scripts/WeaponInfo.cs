using UnityEngine;

[System.Serializable]
public class WeaponInfo
{
    public string strName;  //weapon name equipped
    public float shootTime;    //interval between shots
    public bool autoShot;    //whether or not the weapon is automatic

    //ammunition stuff
    public int currentAmmo;
    public int maxAmmo;
    public int extraAmmo;  //can only reload if you have any extra ammo on magazine left
    public int maxExtraAmmo;
    public bool canReload;  //y'know, in case you can't reload.
    public AudioClip sfxReload;

    //controls weapon recoil
    public Transform trfWeapon; //gets weapon transform
    public Vector3 wpnRotate; //offsets aim
    public Vector3 wpnRetreat;
    public Transform posShot;   //possy. I mean position where shots are fired
    public float delayNextShot; //amount for next shot's delay.

    //damage values
    public float damage;   //(ringo voice) BIG damage!
    public Vector3 posCrosshair;    //crosshair position

    //controls sfx
    public GameObject objShot;
    public GameObject soundShot;
}

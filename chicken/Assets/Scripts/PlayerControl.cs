using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Rigidbody myRB;
    Camera playerCam;
    Transform cameraHolder;
    public GameObject playerSpawn;
    Vector2 camRot;
    public Transform weaponSlot;

    [Header("Player Stats")]
    public int MaxHealth = 100;
    public int CurrentHealth = 100;
    public int HealVal = 20;
    public int AmmoRefill = 20;
    public bool inAir = false;
    public float groundDetecDist = 1.1f;
    
    [Header("WeaponStats")]
    public GameObject shot;
    public float shotSpeed = 10000;
    public bool canFire = true;
    public int weaponID = -1;
    public float fireRate = 0;
    public float MaxAmmo = 0;
    public float CurrentAmmo = 0;
    public int fireMode = 0;
    public float reloadAmount = 0;
    public float clipSize = 0;
    public float CurrentClip = 0;
    public float bulletLifespan = 0;

    [Header("Movement Settings")]
    public float MoveSpeed = 10;
    public float sprintMulti = 2.5f;
    public float JumpHeight = 5;
    public bool sprinting = false;

    [Header("User Settings")]
    public bool sprintTogOpt = false;
    public float mouseSens = 2;
    public float xSens = 2;
    public float ySens = 2;
    public float camRotLim = 90;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize components
        myRB = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        cameraHolder = transform.GetChild(0);

        camRot = Vector2.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Respawn
        if (CurrentHealth <= 0)
        {
            transform.position = playerSpawn.transform.position;
            CurrentHealth = 100;
        }


        //Do the Hokey Pokey and turn yourself around 
        camRot.x += Input.GetAxisRaw("Mouse X") * mouseSens;
        camRot.y += Input.GetAxisRaw("Mouse Y") * mouseSens;

        playerCam.transform.rotation = Quaternion.Euler(-camRot.y, camRot.x, 0);
        transform.localRotation = Quaternion.AngleAxis(camRot.x, Vector3.up);

        // FIRE!
        if(fireMode > 1)
        {// Automatics
            if (Input.GetMouseButton(0) && canFire && CurrentClip > 0 && weaponID > -1)
            {
                GameObject s = Instantiate(shot, weaponSlot.position, weaponSlot.rotation);
                s.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * shotSpeed);
                Destroy(s, bulletLifespan);

                canFire = false;
                CurrentClip--;
                StartCoroutine("cooldownFire");
            }
        }
        else
        {// Semi-Autos
            if (Input.GetMouseButtonDown(0) && canFire && CurrentClip > 0 && weaponID > -1)
            {
                GameObject s = Instantiate(shot, weaponSlot.position, weaponSlot.rotation);
                s.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * shotSpeed);
                Destroy(s, bulletLifespan);

                canFire = false;
                CurrentClip--;
                StartCoroutine("cooldownFire");
            }
        }
        
        //Reload
        if(Input.GetKeyDown(KeyCode.R))
        { reloadClip(); }

        //Neck Brace
        camRot.y = Mathf.Clamp(camRot.y, -camRotLim, camRotLim);
        playerCam.transform.position = cameraHolder.position;

        //Movement variables
        Vector3 temp = myRB.velocity;
        float vertMove = Input.GetAxisRaw("Vertical");
        float horizMove = Input.GetAxisRaw("Horizontal");

        //Run for your life
        if (!sprintTogOpt)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            { sprinting = true; }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            { sprinting = false; }
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift) && vertMove > 0)
            { sprinting = true; }
            if (vertMove <= 0)
            { sprinting = false; }
        }

        if (sprinting)
        { temp.x = vertMove * MoveSpeed * sprintMulti; }
        else
        { temp.x = vertMove * MoveSpeed; }

        temp.z = horizMove * MoveSpeed;

        //Jump for Joy
        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, groundDetecDist))
        { temp.y = JumpHeight;
            inAir = true;
        }

        // Use the Force Luke
        myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //You're grounded!
        if (collision.gameObject.tag == "Ground")
        { inAir = false; }

        //I'm Hit!
        if (collision.gameObject.tag == "Basic Enemy")
        { CurrentHealth -= 20; }

        // I need a medic bag
        if (CurrentHealth < MaxHealth && collision.gameObject.tag == "HealPickup")
        {
            CurrentHealth += HealVal;

            if (CurrentHealth > MaxHealth)
            { CurrentHealth = MaxHealth; }

            Destroy(collision.gameObject);
        }

        //Ammo refill
        if (CurrentAmmo < MaxAmmo && collision.gameObject.tag == "AmmoPickup")
        {
            CurrentAmmo += AmmoRefill;

            if (CurrentAmmo > MaxAmmo)
            { CurrentAmmo = MaxAmmo; }

            Destroy(collision.gameObject);
        }

        // I need more booletts
        if (CurrentAmmo < MaxAmmo && collision.gameObject.tag == "AmmoPickup")
        {
            CurrentAmmo += reloadAmount;

            if (CurrentAmmo > MaxAmmo)
            { CurrentAmmo = MaxAmmo; }

            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Weapon")
        {
            collision.gameObject.transform.SetPositionAndRotation(weaponSlot.position, weaponSlot.rotation);
            collision.gameObject.transform.SetParent(weaponSlot);
        }
    }

    // Wibbly wobbly timey wimey
    private void cooldown(bool condition, float timeLim)
    {
        float timer = 0;

        if (timer < timeLim)
            timer += Time.deltaTime;
        else
            condition = true;
    }

    // Put the ammo in the gun
    public void reloadClip()
    {
        if(CurrentClip >= clipSize)
        {
            return;
        }
        else
        {
            float reloadCount = clipSize - CurrentClip;

            if(CurrentAmmo < reloadCount)
            {
                CurrentClip += CurrentAmmo;
                CurrentAmmo = 0;
                return;
            }

            else
            {
                CurrentClip += reloadCount;
                CurrentAmmo -= reloadCount;
                return;
            }
        }
    }

    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Ive been shot?
        if (collision.gameObject.tag == "Enemy Shot")
        {
            Destroy(collision.gameObject);
            CurrentHealth--;
        }
        
        // Arm Yourself
        if (collision.gameObject.tag == "Weapon")
        {
            collision.gameObject.transform.SetPositionAndRotation(weaponSlot.position, weaponSlot.rotation);
            collision.gameObject.transform.SetParent(weaponSlot);

            switch (collision.gameObject.name)
            {
                case "Pistol":

                    weaponID = 0;
                    fireMode = 0;
                    fireRate = 0.25f;
                    CurrentClip = 15;
                    clipSize = 15;
                    MaxAmmo = 75;
                    CurrentAmmo = 75;
                    reloadAmount = 15;
                    bulletLifespan = 1;
                    break;

                case "Assault Rifle":

                    weaponID = 1;
                    fireMode = 2;
                    fireRate = 0.1f;
                    CurrentClip = 30;
                    clipSize = 30;
                    MaxAmmo = 150;
                    CurrentAmmo = 150;
                    reloadAmount = 30;
                    bulletLifespan = 1;
                    break;

                case "Grappling Hook":

                    weaponID = 2;
                    fireRate = 1;
                    fireMode = 1;
                    bulletLifespan = 1;
                    break;

                default:
                    break;
            }
        }
    }
}

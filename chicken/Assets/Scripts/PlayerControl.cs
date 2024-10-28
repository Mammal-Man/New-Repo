using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Objects & Bools")]
    public GameManager GM;
    Rigidbody myRB;
    Camera playerCam;
    Transform cameraHolder;
    public GameObject playerSpawn;
    Vector2 camRot;
    public Transform weaponSlot;
    public Transform gun;
    public GameObject pistol;
    public GameObject assaultRifle;
    public GameObject shot;
    public GameObject conditionText;
    public bool camFirstPerson = true;
    public bool recoilApplied = false;
    public bool canFire = true;
    public bool pistolCollected;
    public bool ARCollected;

    [Header("Player Stats")]
    public float MaxHealth = 5;
    public float CurrentHealth = 5;
    public int HealVal = 1;
    public float groundDetecDist = 1.1f;
    public bool holdingWeapon = false;
    public float pickupCooldown = 0.1f;
    public int deathCount;

    [Header("Weapon Stats")]
    public float shotSpeed = 100;
    public int weaponID = -1;
    public float fireRate = 0;
    public float MaxAmmo;
    public float pistolMaxAmmo = 0;
    public float ARMaxAmmo = 0;
    public int fireMode = 0;
    public float reloadAmount = 0;
    public float magSize = 0;
    public float bulletLifespan = 1;
    public float recoil = 0;
    public float recoilAmnt = 0;
    public AudioSource wpnSpeaker;

    [Header("Ammo Counts")]
    public float CurrentAmmo = 0;
    public float ARCurrentAmmo = 0;
    public float pistolCurrentAmmo = 0;
    public float CurrentMag = 0;
    public float ARCurrentMag = 0;
    public float pistolCurrentMag = 0;

    [Header("Movement Stats")]
    public float MoveSpeed = 10;
    public float sprintMulti = 2.5f;
    public float JumpHeight = 5;
    public bool sprinting = false;
    public bool inAir = true;

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
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

        camRot = Vector2.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GM.isPaused)
        {
            //Respawn
            if (CurrentHealth <= 0)
            {
                GM.playerDied();
            }

            //Drop your weapon!
            if (holdingWeapon && Input.GetKeyDown(KeyCode.Q))
            {
                gun = weaponSlot.GetChild(0);
                gun.GetComponent<CapsuleCollider>().enabled = true;
                gun.transform.SetParent(null);
                weaponID = -1;
                StartCoroutine("PickupCooldown");
            }

            //What gun am I using?
            if (weaponID == 0)
            {
                CurrentMag = pistolCurrentMag;
                CurrentAmmo = pistolCurrentAmmo;
                MaxAmmo = pistolMaxAmmo;
            }
            if (weaponID == 1)
            {
                CurrentMag = ARCurrentMag;
                CurrentAmmo = ARCurrentAmmo;
                MaxAmmo = ARMaxAmmo;
            }

            //Do the Hokey Pokey and turn yourself around 
            camRot.x += Input.GetAxisRaw("Mouse X") * mouseSens * Time.timeScale;
            camRot.y += Input.GetAxisRaw("Mouse Y") * mouseSens * Time.timeScale;

            playerCam.transform.rotation = Quaternion.Euler(-camRot.y, camRot.x, 0);
            transform.localRotation = Quaternion.AngleAxis(camRot.x, Vector3.up);
            cameraHolder.transform.rotation = playerCam.transform.rotation;

            // FIRE!
            if (fireMode > 0)
            {
                // Automatics
                if (Input.GetMouseButton(0) && canFire && CurrentMag > 0 && weaponID > -1)
                {
                    wpnSpeaker.Play();
                    GameObject s = Instantiate(shot, weaponSlot.position, weaponSlot.rotation);
                    s.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * shotSpeed);
                    Destroy(s, bulletLifespan);
                    recoil = recoilAmnt;
                    recoilApplied = true;

                    canFire = false;
                    if (weaponID == 1)
                    { ARCurrentMag--; }
                    StartCoroutine("cooldownFire");
                }
            }
            else if (fireMode < 0)
            {
                // Semi-Autos
                if (Input.GetMouseButtonDown(0) && canFire && CurrentMag > 0 && weaponID > -1)
                {
                    wpnSpeaker.Play();
                    GameObject s = Instantiate(shot, weaponSlot.position, weaponSlot.rotation);
                    s.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * shotSpeed);
                    Destroy(s, bulletLifespan);
                    recoil = recoilAmnt;
                    recoilApplied = true;

                    canFire = false;
                    if (weaponID == 0)
                    { pistolCurrentMag--; }
                    StartCoroutine("cooldownFire");
                }
            }
            
            //Reload
            if (Input.GetKeyDown(KeyCode.R))
            { reloadClip(); }

            //Neck Brace
            camRot.y = Mathf.Clamp(camRot.y + recoil, -camRotLim, camRotLim);
            playerCam.transform.position = cameraHolder.position;

            //do not recoil
            if (recoilApplied)
            {
                recoil = 0;
                recoilApplied = false;
            }

            //Movement variables
            Vector3 temp = myRB.velocity;
            float vertMove = Input.GetAxisRaw("Vertical");
            float horizMove = Input.GetAxisRaw("Horizontal");

            //Run for your life
            if (!sprintTogOpt)
            {
                if (Input.GetKey(KeyCode.LeftShift) && !inAir)
                { sprinting = true; }
                if (Input.GetKeyUp(KeyCode.LeftShift))
                { sprinting = false; }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift) && !inAir && vertMove > 0)
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
            if (Input.GetKeyDown(KeyCode.Space) && !inAir)
            {
                temp.y = JumpHeight;
            }

            if (Physics.Raycast(transform.position, -transform.up, groundDetecDist))
            {
                inAir = false;
            }
            else
            {
                inAir = true;
            }

            // Use the Force Luke
            myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Arm Yourself
        if (collision.gameObject.tag == "Weapon" && !holdingWeapon)
        {
            wpnSpeaker = collision.gameObject.GetComponent<AudioSource>();
            collision.gameObject.transform.SetPositionAndRotation(weaponSlot.position, weaponSlot.rotation);
            collision.gameObject.transform.SetParent(weaponSlot);

            switch (collision.gameObject.name)
            {
                case "Pistol":

                    if (!pistolCollected)
                    {
                        pistolCurrentMag = 15;
                        pistolCurrentAmmo = 75;
                    }
                    weaponID = 0;
                    fireMode = -1;
                    fireRate = 0.25f;
                    magSize = 15;
                    pistolMaxAmmo = 75;
                    reloadAmount = 15;
                    recoilAmnt = 2;
                    pistol.GetComponent<CapsuleCollider>().enabled = false;
                    holdingWeapon = true;
                    pistolCollected = true;
                    break;

                case "Assault Rifle":

                    if (!ARCollected)
                    {
                        ARCurrentMag = 30;
                        ARCurrentAmmo = 150;
                    }
                    weaponID = 1;
                    fireMode = 1;
                    fireRate = 0.1f;
                    magSize = 30;
                    ARMaxAmmo = 150;
                    reloadAmount = 30;
                    recoilAmnt = 1;
                    assaultRifle.GetComponent<CapsuleCollider>().enabled = false;
                    holdingWeapon = true;
                    ARCollected = true;
                    break;

                default:
                    break;
            }
        }
        
        //You're grounded!
        if (collision.gameObject.tag == "Ground")
        { inAir = false; }

        //I'm Hit!
        if (collision.gameObject.tag == "Basic Enemy")
        {
            CurrentHealth --;
        }

        //That's next level thinking
        if (collision.gameObject.tag == "Teleporter")
        {
            if (holdingWeapon)
            {
                switch (collision.gameObject.name)
                {

                    case "Level1":
                        GM.LoadLevel(1);
                        break;

                    case "Level2":
                        GM.LoadLevel(2);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                conditionText.SetActive(true);
            }
        }

            // I need a medic bag
            if (CurrentHealth < MaxHealth && collision.gameObject.tag == "HealPickup")
        {
            CurrentHealth += HealVal;
            Destroy(collision.gameObject);
        }
        
        // I need more booletts
        if (CurrentAmmo < MaxAmmo && collision.gameObject.tag == "AmmoPickup")
        {
            if (ARCurrentAmmo + reloadAmount < ARMaxAmmo)
            { ARCurrentAmmo += reloadAmount; }
            else { ARCurrentAmmo = ARMaxAmmo; }

            if (pistolCurrentAmmo + reloadAmount < pistolMaxAmmo)
            { pistolCurrentAmmo += reloadAmount; }
            else { pistolCurrentAmmo = pistolMaxAmmo; }

            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Teleporter" && holdingWeapon)
        {
            if(holdingWeapon)
            {
               switch (collision.gameObject.name)
                {
                    case "level1":
                        GM.LoadLevel(1);
                        break;

                    case "Level2":
                        Gme.LoadLevel(2);
                        break;

                    default:
                        break;
                }
            }

            else
            {
                conditionText.SetActive(true);
            }
        }
    }

    // Gimme a second
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
        if(CurrentMag >= magSize)
        {
            return;
        }
        else
        {
            float reloadCount = magSize - CurrentMag;

            if(CurrentAmmo < reloadCount)
            {
                if (weaponID == 0)
                {
                    pistolCurrentMag += pistolCurrentAmmo;
                    pistolCurrentAmmo = 0;
                }
                if (weaponID == 1)
                {
                    ARCurrentMag += ARCurrentAmmo;
                    ARCurrentAmmo = 0;
                }
                return;
            }

            else
            {
                if (weaponID == 0)
                {
                    pistolCurrentMag += reloadCount;
                    pistolCurrentAmmo -= reloadCount;
                }
                if (weaponID == 1)
                {
                    ARCurrentMag += reloadCount;
                    ARCurrentAmmo -= reloadCount;
                }
                return;
            }
        }
    }

    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    IEnumerator PickupCooldown()
    {
        yield return new WaitForSeconds(pickupCooldown);
        holdingWeapon = false;
    }
}

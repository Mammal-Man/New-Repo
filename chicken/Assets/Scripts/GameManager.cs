using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool isPaused = false;
    public bool PlayerDied = false;

    public GameObject pauseMenu;
    public GameObject HUD;
    public GameObject deathScreen;
    public PlayerControl player;

    public Image healthBar;
    public TextMeshProUGUI magCounter;
    public TextMeshProUGUI reserveCounter;
    public TextMeshProUGUI deathCount;
    public GameObject munitionsDisplay;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        { player = GameObject.Find("Player").GetComponent<PlayerControl>(); }
    }

    // Update is called once per frame
    void Update()
    {
        //Main menu
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            //healthy player
            healthBar.fillAmount = Mathf.Clamp((float)player.CurrentHealth / (float)player.MaxHealth, 0, 1);

            if (player.deathCount > 0)
            {
                deathCount.gameObject.SetActive(true);
                deathCount.text = "Deaths: " + player.deathCount;
            }
            else
                deathCount.gameObject.SetActive(false);

            //He's got a weapon
            if (player.weaponID < 0)
            {
                munitionsDisplay.gameObject.SetActive(false);
            }
            else
            {
                munitionsDisplay.gameObject.SetActive(true);

                if (player.weaponID == 0)
                { magCounter.text = "Pistol: " + player.CurrentMag + "/" + player.magSize; }
                if (player.weaponID == 1)
                { magCounter.text = "Assault Rifle: " + player.CurrentMag + "/" + player.magSize; }
                reserveCounter.text = "Ammo: " + player.CurrentAmmo;
            }

            //Pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!PlayerDied)
                {
                    if (!isPaused)
                    {
                        Pause();
                    }

                    else
                    { Resume(); }
                }
            }
        }
    }

    //Resume
    public void Resume()
    {
        pauseMenu.SetActive(false);
        HUD.SetActive(true);

        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        isPaused = false;
    }

    //Quit while you're ahead
    public void quitGame()
    {
        Application.Quit();
    }

    //pull up
    public void LoadLevel(int sceneID)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneID);
    }

    //Try that again
    public void restartLevel()
    {
        Time.timeScale = 1;
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void playerDied()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        HUD.SetActive(false);
        PlayerDied = true;
    }

    public void respawn()
    {
        if (player.weaponID > -1)
        {
            player.gun = player.weaponSlot.GetChild(0);
            player.gun.GetComponent<CapsuleCollider>().enabled = true;
            player.gun.transform.SetParent(null);
            player.weaponID = -1;
            StartCoroutine("PickupCooldown");
        }

        player.transform.position = player.playerSpawn.transform.position;
        player.CurrentHealth = player.MaxHealth;

        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        HUD.SetActive(true);
        PlayerDied = false;
        deathScreen.SetActive(false);
    }


    public void Pause()
    { 
        pauseMenu.SetActive(true);
        HUD.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;

        isPaused = true;
    }
    
}

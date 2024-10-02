using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool isPaused = false;

    public GameObject pauseMenu;
    public PlayerControl player;

    public Image healthBar;
    public TextMeshProUGUI clipCounter;
    public TextMeshProUGUI reserveCounter;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        //Main menu
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            //healthy player
            healthBar.fillAmount = Mathf.Clamp((float)player.CurrentHealth / (float)player.MaxHealth, 0, 1);

            //He's got a weapon
            if (player.weaponID < 0)
            {
                clipCounter.gameObject.SetActive(false);
                reserveCounter.gameObject.SetActive(false);
            }
            else
            {
                clipCounter.gameObject.SetActive(true);
                reserveCounter.gameObject.SetActive(true);

                clipCounter.text = "Clip: " + player.CurrentClip + "/" + player.clipSize;
                reserveCounter.text = "Ammo: " + player.CurrentAmmo;
            }

            //Pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                {
                    pauseMenu.SetActive(true);

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    Time.timeScale = 0;

                    isPaused = true;
                }

                else
                { Resume(); }
            }
        }
    }

    //Resume
    public void Resume()
    {
        pauseMenu.SetActive(false);

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

    public void LoadLevel(int sceneID)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneID);
    }

    public void restartLevel()
    {
        Time.timeScale = 1;
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }
}

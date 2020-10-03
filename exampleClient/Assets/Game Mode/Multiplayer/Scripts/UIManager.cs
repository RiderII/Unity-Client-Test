using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject pauseMenu;
    public GameObject startMenu;
    public InputField usernameField;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        //startMenu.SetActive(false);
        //usernameField.interactable = false;
        //Client.instance.userName = DataBridge.instance.userProfile.username;
        //Client.instance.ConnectToServer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.activeInHierarchy)
            {
                pauseMenu.SetActive(true);
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
                pauseMenu.SetActive(false);
            }
        }
    }

    public void GoMenu()
    {
        for (int i = 1; i < GameManager.players.Count; i++)
        {
            if (GameManager.players.ContainsKey(i))
            {
                Destroy(GameManager.players[i].gameObject);
                
            }
        }

        GameManager.players.Clear();

        Client.instance.Disconnect();

        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void Continue()
    {
        Cursor.visible = false;
        pauseMenu.SetActive(false);
    }
}

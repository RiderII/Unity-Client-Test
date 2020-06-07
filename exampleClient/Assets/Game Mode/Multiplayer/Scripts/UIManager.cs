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
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.userName = usernameField.text;
        Client.instance.ConnectToServer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
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

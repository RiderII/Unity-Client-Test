using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LvlManger : MonoBehaviour
{
    public GameObject challengeMenu;
    public GameObject mainMenu;
    public GameObject challengePrefab;
    public GameObject backbtnPrefab;

    //public DataBridge dataBridge;
    
    public void loadLevel(string pNombreNivel){
        Time.timeScale = 1;
        SceneManager.LoadScene(pNombreNivel); 
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public async void LoadChallenges()
    {
        mainMenu.SetActive(false);
        challengeMenu.SetActive(true);
        var lista = await DataBridge.instance.LoadDataChallenges();
        
        foreach (var t in lista)
        {
            var challenge = Instantiate(challengePrefab, challengeMenu.transform);
            challenge.GetComponentInChildren<TextMeshProUGUI>().text = t.Descripcion;
        }
        var backbtn = Instantiate(backbtnPrefab, challengeMenu.transform);
        Button btn = backbtn.GetComponent<Button>();
        btn.onClick.AddListener(delegate() {
            mainMenu.SetActive(true);
            foreach (Transform child in challengeMenu.transform)
                Destroy(child.gameObject);

            challengeMenu.SetActive(false);
        });

        print("loaded");

    }


}

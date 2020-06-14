﻿using System.Collections;
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
    public GameObject title;
    public GameObject profileMenu;
    public Button profileBackBtn;

    public GameObject challengePrefab;
    public GameObject backbtnPrefab;
    public GameObject medalOffPrefab;
    public GameObject medalOnPrefab;
    public GameObject modeErrorBoxPrefab;

    private MedalCollection medalCollection;

    private void Start()
    {
        medalCollection = new MedalCollection();
    }


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
        if(DataBridge.instance.GetMode() == null)
        {
            var box = Instantiate(modeErrorBoxPrefab, challengeMenu.transform);
            var backbtn = Instantiate(backbtnPrefab, challengeMenu.transform);
            Button btn = backbtn.GetComponent<Button>();
            btn.onClick.AddListener(delegate () {
                mainMenu.SetActive(true);
                foreach (Transform child in challengeMenu.transform)
                    Destroy(child.gameObject);
                challengeMenu.SetActive(false);
            });
        }
        if(DataBridge.instance.GetMode() != null)
        {
            var lista = await DataBridge.instance.LoadDataChallenges();

            foreach (var t in lista)
            {
                var challenge = Instantiate(challengePrefab, challengeMenu.transform);
                challenge.GetComponentInChildren<TextMeshProUGUI>().text = t.Descripcion;
            }
            var backbtn = Instantiate(backbtnPrefab, challengeMenu.transform);
            Button btn = backbtn.GetComponent<Button>();
            btn.onClick.AddListener(delegate () {
                mainMenu.SetActive(true);
                foreach (Transform child in challengeMenu.transform)
                    Destroy(child.gameObject);

                challengeMenu.SetActive(false);
            });
            print("loaded");
        }
    }

    public async void LoadProfile()
    {
        mainMenu.SetActive(false);
        title.SetActive(false);
        profileMenu.SetActive(true);

        GameObject board = profileMenu.transform.GetChild(0).gameObject;
       

        var lista = await DataBridge.instance.LoadUserMedals();

        foreach (KeyValuePair<string, MedalSprites> entry in medalCollection.sprites)
        {
            if(lista.Contains(entry.Key)){
                GameObject medal = Instantiate(medalOnPrefab, board.transform);
                medal.GetComponent<Image>().sprite = entry.Value.on;
            }
            else
            {
                GameObject medal = Instantiate(medalOffPrefab, board.transform);
                medal.GetComponent<Image>().sprite = entry.Value.off;
            }
        }
        profileBackBtn.onClick.AddListener(delegate () {
            mainMenu.SetActive(true);
            foreach (Transform child in board.transform)
                Destroy(child.gameObject);

            mainMenu.SetActive(true);
            title.SetActive(true);
            profileMenu.SetActive(false);
        });

    }

}

public class MedalCollection
{

    private const string path = "Design/UI/medallas/";
    public Dictionary<string, MedalSprites> sprites;

    public MedalCollection()
    {
        sprites = new Dictionary<string, MedalSprites>();
        sprites.Add("one", new MedalSprites
        {
            on = Resources.Load<Sprite>(path + "cup_gold"),
            off = Resources.Load<Sprite>(path + "cup") });
        sprites.Add("two", new MedalSprites
        {
            on = Resources.Load<Sprite>(path + "medal_gold"),
            off = Resources.Load<Sprite>(path + "medal") });
        sprites.Add("three", new MedalSprites
        {
            on = Resources.Load<Sprite>(path + "reward_gold"),
            off = Resources.Load<Sprite>(path + "reward")
        });
        sprites.Add("four", new MedalSprites
        {
            on = Resources.Load<Sprite>(path + "trophy_gold"),
            off = Resources.Load<Sprite>(path + "trophy")
        });
    }

   
}
public class MedalSprites
{
    public Sprite on { get; set; }
    public Sprite off { get; set; }
}

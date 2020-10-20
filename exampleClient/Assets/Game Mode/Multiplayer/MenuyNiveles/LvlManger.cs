using System;
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
    public GameObject title;
    public GameObject profileMenu;

    public GameObject challengePrefab;
    public GameObject backbtnPrefab;
    public GameObject modeErrorBoxPrefab;

    public void loadLevel(string pNombreNivel){
        Time.timeScale = 1;
        SceneManager.LoadScene(pNombreNivel); 
    }

    public void playerDetail(int playerId)
    {

        PlayerManager player = null;
        foreach (PlayerManager p in GameManager.players.Values)
        {
            if (p.placement == playerId)
            {
                player = p;
            }
        }

        if (player.seeDetail)
        {
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(3).gameObject.SetActive(false);
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(3).gameObject.SetActive(true);
            foreach (PlayerManager p in GameManager.players.Values)
            {
                if (p.finishedGame && p.id != player.id)
                {
                    p.seeDetail = false;
                }
            }
        }
        else
        {

            GameManager.players[Client.instance.myId].playerLayers[0] = GameManager.players[Client.instance.myId].raceResults.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;

            switch (player.placement)
            {
                case 1: GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(2).GetComponent<Image>().sprite = GameManager.players[Client.instance.myId].one; break;
                case 2: GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(2).GetComponent<Image>().sprite = GameManager.players[Client.instance.myId].two; break;
                case 3: GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(2).GetComponent<Image>().sprite = GameManager.players[Client.instance.myId].three; break;
                case 4: GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(2).GetComponent<Image>().sprite = GameManager.players[Client.instance.myId].four; break;
            }

            foreach (PlayerManager p in GameManager.players.Values)
            {
                if (p.finishedGame && p.id != player.id)
                {
                    p.seeDetail = true;
                }
            }
            if (GameManager.players[Client.instance.myId].playerLayers[1] != null) GameManager.players[Client.instance.myId].playerLayers[1].gameObject.SetActive(false);
            if (GameManager.players[Client.instance.myId].playerLayers[2] != null) GameManager.players[Client.instance.myId].playerLayers[2].gameObject.SetActive(false);
            if (GameManager.players[Client.instance.myId].playerLayers[3] != null) GameManager.players[Client.instance.myId].playerLayers[3].gameObject.SetActive(false);
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(3).gameObject.SetActive(false);
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.username;
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "HORA DE FINALIZACIÓN " + player.finishGameTime;
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "TIEMPO: " + Mathf.FloorToInt(player.finalTime) + " s";
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(true);
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(3).gameObject.SetActive(true);
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "DISTANCIA RECORRIDA: " + System.Math.Round(player.traveled_meters, 2) + " m";
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "CALORIAS: " + System.Math.Round(player.burned_calories, 2) + " Kcal";
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "COLISIONES: " + player.collisions;
            GameManager.players[Client.instance.myId].playerLayers[0].transform.GetChild(1).transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "PUNTAJE: " + player.points;
        }

        player.seeDetail = !player.seeDetail;
    }

    public void loadGameLevel(string pNombreNivel) {
        Time.timeScale = 1;
        Client.instance.levelSelected = pNombreNivel;
        Debug.Log($"Player level selected: {Client.instance.levelSelected}");
        //if (Client.instance.gameModeSelected == "Multiplayer" || SystemInfo.supportsGyroscope || true)
        if (Client.instance.gameModeSelected == "Multiplayer" || SystemInfo.supportsGyroscope)
        {
            StartCoroutine(LoadAsynchronously("Lobby"));
        }
        else if (Client.instance.gameModeSelected == "Singleplayer" && Client.instance.levelSelected == "Vaquita")
        {
            StartCoroutine(LoadAsynchronously("VaquitaS"));
        }
        else
        {
            StartCoroutine(LoadAsynchronously(pNombreNivel));
        }
    }

    public void setDescriptionPanel(GameObject panelContainer)
    {
        panelContainer.SetActive(!panelContainer.activeSelf);
    }
        
    public void setGameModeSelected(string gameMode)
    {
        Client.instance.gameModeSelected = gameMode;
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public async void LoadChallenges(string mode)
    {
        var opciones = challengeMenu.transform.GetChild(1).gameObject;
        var desafios = challengeMenu.transform.GetChild(2).gameObject;

        var panel = desafios.transform.GetChild(0);
        var backbtn = desafios.transform.GetChild(1);

        var lista = await DataBridge.instance.LoadDataChallenges(mode);

        foreach (var t in lista)
        {
            var challenge = Instantiate(challengePrefab, panel.transform);
            challenge.GetComponentInChildren<TextMeshProUGUI>().text = t.Descripcion;
        }
        Button btn = backbtn.GetComponent<Button>();
        btn.onClick.AddListener(delegate () {
            foreach (Transform child in panel.transform)
                Destroy(child.gameObject);

            desafios.SetActive(false);
            opciones.SetActive(true);
        });
        
    }

    public async void LoadProfile()
    {
        mainMenu.SetActive(false);
        profileMenu.SetActive(true);

        GameObject Username = profileMenu.transform.GetChild(1).gameObject;
        GameObject Weight = profileMenu.transform.GetChild(2).gameObject;
        TMP_InputField usernameInput = Username.transform.GetChild(0).GetComponent<TMP_InputField>();
        TMP_InputField weightInput = Weight.transform.GetChild(0).GetComponent<TMP_InputField>();

        
        User user = DataBridge.instance.userProfile;
        usernameInput.text = user.username;
        weightInput.text = Convert.ToString(user.weight);


        //RECORDS
        //passing all records to profilemanager
        var response = await DataBridge.instance.LoadUserRecords(user.ID);
        ProfileManager.instance.records = response.records;
        string[] words = response.totals.Split(',');
        //filling totals
        TMP_InputField time = profileMenu.transform.Find("records").Find("TiempoTotal").Find("TimeInput").GetComponent<TMP_InputField>();
        TMP_InputField calory = profileMenu.transform.Find("records").Find("CaloriasTotal").Find("CaloriaInput").GetComponent<TMP_InputField>();
        TMP_InputField distance = profileMenu.transform.Find("records").Find("DistanciaTotal").Find("DistanceInput").GetComponent<TMP_InputField>();
        calory.text = words[0];
        time.text = words[1];
        distance.text = words[2];

        var profileBackBtn = profileMenu.transform.Find("backBtn").GetComponent<Button>();
        profileBackBtn.onClick.AddListener(delegate () {
            mainMenu.SetActive(true);
            profileMenu.SetActive(false);
            profileMenu.transform.Find("FunMode").GetChild(0).gameObject.SetActive(false);
            profileMenu.transform.Find("FitnessMode").GetChild(0).gameObject.SetActive(false);
        });

        if(DataBridge.instance.GetMode() == "Fun")
        {
            var activated = profileMenu.transform.Find("FunMode").GetChild(0).gameObject;
            activated.SetActive(true);
        }
        if (DataBridge.instance.GetMode() == "Fitness")
        {
            var activated = profileMenu.transform.Find("FitnessMode").GetChild(0).gameObject;
            activated.SetActive(true);
        }

    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            Debug.Log($"LOADING {progress}");

            yield return null; // wait until next frame
        }
    }
}

public static class MedalCollection
{

    private const string path = "Design/UI/medallas/";
    public static Dictionary<string, MedalSprites> sprites;

    public static Dictionary<string, MedalSprites> Sprites()
    {
        sprites = null;
        sprites = new Dictionary<string, MedalSprites>();
        sprites.Add("one", new MedalSprites
        {
            on = Resources.Load<Sprite>(path + "cup_gold"),
            off = Resources.Load<Sprite>(path + "cup"),
            onDescriptionFun = "Nivel 200m: Cumplidor!\nTerminaste la carrera sin colisionar con ningún obstáculo",
            offDescriptionFun = "Nivel 200m: Cumplidor!\nDebes terminar la carrera sin colisionar con ningún obstáculo",
            onDescriptionFit = "Nivel 200m: Cumplidor!\nTerminaste la carrera en menos de 22 segundos",
            offDescriptionFit = "Nivel 200m: Cumplidor!\nDebes terminar la carrera en menos de 22 segundos",
        });
        sprites.Add("two", new MedalSprites
        {
            on = Resources.Load<Sprite>(path + "medal_gold"),
            off = Resources.Load<Sprite>(path + "medal"),
            onDescriptionFun = "Nivel 600m: Experto!\nTerminaste la carrera sin colisiones y en menos de 85 segundos",
            offDescriptionFun = "Nivel 600m: Experto!\nDebes terminar la carrera sin colisiones y en menos de 85 segundos",
            onDescriptionFit = "Nivel 600m: Experto!\nTerminaste la carrera en menos de 75 segundos",
            offDescriptionFit = "Nivel 600m: Experto!\nDebes terminar la carrera en menos de 75 segundos",
        });
        sprites.Add("three", new MedalSprites
        {
            on = Resources.Load<Sprite>(path + "reward_gold"),
            off = Resources.Load<Sprite>(path + "reward"),
            onDescriptionFun = "Nivel 1000m: Maestro!\nTerminaste la carrera sin colisiones en menos de 120 segundos",
            offDescriptionFun = "Nivel 1000m: Maestro!\nDebes terminar la carrera sin colisiones en menos de 120 segundos",
            onDescriptionFit = "Nivel 1000m: Maestro!\nTerminaste la carrera en menos de 110 segundos",
            offDescriptionFit = "Nivel 1000m: Maestro!\nDebes terminar la carrera en menos de 110 segundos",
        });
        sprites.Add("four", new MedalSprites
        {
            on = Resources.Load<Sprite>(path + "trophy_gold"),
            off = Resources.Load<Sprite>(path + "trophy"),
            onDescriptionFun = "Nivel 4600m: Crack!\nTerminaste la carrera en menos de 500 segundos",
            offDescriptionFun = "Nivel 4600m: Crack!\nDebes terminar la carrera en menos de 500 segundos",
            onDescriptionFit = "Nivel 4600m: Crack!\nTerminaste la carrera en menos de 420 segundos",
            offDescriptionFit = "Nivel 4600m: Crack!\nDebes terminar la carrera en menos de 420 segundos",
        });

        return sprites;
    }
}
public class MedalSprites
{
    public Sprite on { get; set; }
    public Sprite off { get; set; }
    public string onDescriptionFun { get; set; }
    public string offDescriptionFun { get; set; }
    public string onDescriptionFit { get; set; }
    public string offDescriptionFit { get; set; }
}

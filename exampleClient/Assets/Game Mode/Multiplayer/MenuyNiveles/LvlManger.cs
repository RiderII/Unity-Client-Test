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
        GameObject Diameter = profileMenu.transform.GetChild(3).gameObject;
        TMP_InputField usernameInput = Username.transform.GetChild(0).GetComponent<TMP_InputField>();
        TMP_InputField weightInput = Weight.transform.GetChild(0).GetComponent<TMP_InputField>();
        TMP_InputField diameterInput = Diameter.transform.GetChild(0).GetComponent<TMP_InputField>();

        
        User user = DataBridge.instance.userProfile;
        usernameInput.text = user.username;
        weightInput.text = Convert.ToString(user.weight);
        diameterInput.text = Convert.ToString(user.bikeWheelDiameter);


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
            off = Resources.Load<Sprite>(path + "cup")
        });
        sprites.Add("two", new MedalSprites
        {
            on = Resources.Load<Sprite>(path + "medal_gold"),
            off = Resources.Load<Sprite>(path + "medal")
        });
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

        return sprites;
    }


}
public class MedalSprites
{
    public Sprite on { get; set; }
    public Sprite off { get; set; }
}

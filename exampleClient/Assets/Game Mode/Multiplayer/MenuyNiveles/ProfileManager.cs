using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager instance;
    public GameObject container;
    public GameObject confirmationPanel;
    public TMP_InputField usernameInput;
    public TMP_InputField weightInput;
    public TMP_InputField diameterInput;
    public Button editBtn;
    public Button checkBtn;
    public Button cancelBtn;
    public List<MapReport> records;
    public GameObject medalsPanel;
    public GameObject medalOffPrefab;
    public GameObject medalOnPrefab;

    public GameObject recordPrefab;

    private string username;
    private float weight;
    private float diameter;

    private Button yesBtn;
    private Button noBtn;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AssigmentClick(string mode)
    {
        if (DataBridge.instance.GetMode() != mode) //ver playerrefs
        {
            DataBridge.instance.SaveUserPreferences(mode);
            confirmationPanel.SetActive(true);
            this.gameObject.transform.Find("FunMode").GetChild(0).gameObject.SetActive(false);
            this.gameObject.transform.Find("FitnessMode").GetChild(0).gameObject.SetActive(false);
            if (mode == "Fun")
            {
                var activated = this.gameObject.transform.Find("FunMode").GetChild(0).gameObject;
                activated.SetActive(true);
            }
            if (mode == "Fitness")
            {
                var activated = this.gameObject.transform.Find("FitnessMode").GetChild(0).gameObject;
                activated.SetActive(true);
            }
        }
    }


    public void EditBtnClick()
    {
        editBtn.gameObject.SetActive(false);
        checkBtn.gameObject.SetActive(true);
        cancelBtn.gameObject.SetActive(true);

        username = usernameInput.text;
        weight = float.Parse(weightInput.text);
        diameter = float.Parse(diameterInput.text);
        usernameInput.interactable = true;
        weightInput.interactable = true;
        diameterInput.interactable = true;
    }

    public void CancelBtnClick()
    {
        editBtn.gameObject.SetActive(true);
        checkBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);

        usernameInput.text = username;
        weightInput.text = weight.ToString(); ;
        diameterInput.text = diameter.ToString();
        usernameInput.interactable = false;
        weightInput.interactable = false;
        diameterInput.interactable = false;
    }

    public void CheckBtnClick()
    {
        username = usernameInput.textComponent.text;
        weight = float.Parse(weightInput.text);
        diameter = float.Parse(diameterInput.text); 
        User user = DataBridge.instance.userProfile;
        user.username = username;
        user.weight = weight;
        user.bikeWheelDiameter = diameter;
        DataBridge.instance.SaveNewUser(user);

        editBtn.gameObject.SetActive(true);
        checkBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);

        usernameInput.interactable = false;
        weightInput.interactable = false;
        diameterInput.interactable = false;
    }

    public void LoadRecords()
    {
        foreach (var r in records)
        {
            var row = Instantiate(recordPrefab, container.transform);

            Text title = row.transform.GetChild(0).GetComponent<Text>();
            TextMeshProUGUI calorias = row.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI time = row.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI distance = row.transform.GetChild(3).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

            title.text = r.datetime;
            calorias.text = r.burned_calories.ToString("f2");
            time.text = r.totalGameTime.ToString("f2") + " s";
            distance.text = r.traveled_kilometers.ToString("f2") +" m";

        }
    }

    public async void LoadMedals()
    {
        var boardFitness = medalsPanel.transform.GetChild(0);
        var boardFun = medalsPanel.transform.GetChild(1);
        var listaFit = await DataBridge.instance.LoadUserMedalsFitness();
        foreach (KeyValuePair<string, MedalSprites> entry in MedalCollection.Sprites())
        {
            if (listaFit.Contains(entry.Key))
            {
                GameObject medal = Instantiate(medalOnPrefab, boardFitness.transform);
                medal.GetComponent<Image>().sprite = entry.Value.on;
            }
            else
            {
                GameObject medal = Instantiate(medalOffPrefab, boardFitness.transform);
                medal.GetComponent<Image>().sprite = entry.Value.off;
            }
        }
        var listaFun = await DataBridge.instance.LoadUserMedalsFun();
        foreach (KeyValuePair<string, MedalSprites> entry in MedalCollection.Sprites())
        {
            if (listaFun.Contains(entry.Key))
            {
                GameObject medal = Instantiate(medalOnPrefab, boardFun.transform);
                medal.GetComponent<Image>().sprite = entry.Value.on;
            }
            else
            {
                GameObject medal = Instantiate(medalOffPrefab, boardFun.transform);
                medal.GetComponent<Image>().sprite = entry.Value.off;
            }
        }

        var profileBackBtn = medalsPanel.transform.Find("backBtn").GetComponent<Button>();
        profileBackBtn.onClick.AddListener(delegate () {
            medalsPanel.SetActive(false);
            foreach (Transform child in boardFun.transform)
                Destroy(child.gameObject);
            foreach (Transform child in boardFitness.transform)
                Destroy(child.gameObject);

            this.gameObject.SetActive(true);
        });
    }

    public void HistoricBackClick()
    {
        foreach (Transform child in container.transform)
            Destroy(child.gameObject);
    }

}

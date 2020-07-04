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
    public Button editBtn;
    public Button checkBtn;
    public Button cancelBtn;
    public List<MapReport> records;

    public GameObject recordPrefab;

    private string username;

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
            confirmationPanel.SetActive(true);
            yesBtn = confirmationPanel.transform.GetChild(1).gameObject.GetComponent<Button>();
            noBtn = confirmationPanel.transform.GetChild(2).gameObject.GetComponent<Button>();

            noBtn.onClick.RemoveAllListeners();
            yesBtn.onClick.RemoveAllListeners();
            noBtn.onClick.AddListener(delegate ()
            {
                confirmationPanel.SetActive(false);
            });
            yesBtn.onClick.AddListener(delegate ()
            {
                ChangeAssigment(mode);
            });
        }
    }

    public void ChangeAssigment(string mode)
    {
        DataBridge.instance.SaveUserPreferences(mode);
        confirmationPanel.SetActive(false);
    }

    public void EditBtnClick()
    {
        editBtn.gameObject.SetActive(false);
        checkBtn.gameObject.SetActive(true);
        cancelBtn.gameObject.SetActive(true);

        username = usernameInput.text;
        usernameInput.interactable = true;
    }

    public void CancelBtnClick()
    {
        editBtn.gameObject.SetActive(true);
        checkBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);

        usernameInput.text = username;
        usernameInput.interactable = false;
    }

    public void CheckBtnClick()
    {
        username = usernameInput.textComponent.text;
        User user = DataBridge.instance.userProfile;
        user.username = username;
        DataBridge.instance.SaveNewUser(user);

        editBtn.gameObject.SetActive(true);
        checkBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);

        usernameInput.interactable = false;
    }

    public void LoadRecords()
    {
        foreach (var r in records)
        {
            var row = Instantiate(recordPrefab, container.transform);

            Text title = row.transform.GetChild(0).GetComponent<Text>();
            TextMeshProUGUI calorias = row.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI time = row.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI distance = row.transform.GetChild(6).GetComponent<TextMeshProUGUI>();

            title.text = r.datetime;
            calorias.text = $"{r.burned_calories}";
            time.text = $"{r.totalGameTime} s";
            distance.text = $"{r.traveled_kilometers} m";

        }
    }

    public void HistoricBackClick()
    {
        foreach (Transform child in container.transform)
            Destroy(child.gameObject);
    }

}

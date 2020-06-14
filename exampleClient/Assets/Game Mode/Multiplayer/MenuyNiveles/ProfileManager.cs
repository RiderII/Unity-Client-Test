using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    public GameObject confirmationPanel;
    private Button yesBtn;
    private Button noBtn;
    

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

    
}

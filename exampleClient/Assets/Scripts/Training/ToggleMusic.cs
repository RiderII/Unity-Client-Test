using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMusic : MonoBehaviour
{
    // Start is called before the first frame update
    //public Toggle m_Toggle;
    AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        Debug.Log(audioSource);
        //m_Toggle = GetComponent<Toggle>();
        //Debug.Log(m_Toggle);

    }

   public void toggleOnclick(Toggle m_Toggle)
   {
        if (m_Toggle.isOn)
        {
            Debug.Log("is on");

            audioSource.Pause();
        }
        else
        {
            Debug.Log("is off");

            audioSource.Play(0);
            
        }
   }
}

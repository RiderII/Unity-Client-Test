using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class swipe_selection : MonoBehaviour
{
    public GameObject scrollbar;
    float scroll_pos = 0;
    float[] pos;
    bool run;
    bool buttonClicked;
    float previous_pos = 1;
    float current_pos = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                buttonClicked = true;
            });
        }

        if (Input.GetMouseButton(0))
        {
            run = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            previous_pos = scrollbar.GetComponent<Scrollbar>().value;

            Debug.Log($"PREV { previous_pos}, {scrollbar.GetComponent<Scrollbar>().value}");
        }
        if (Input.GetMouseButtonUp(0))
        {
            current_pos = scrollbar.GetComponent<Scrollbar>().value;

            Debug.Log($"Current { current_pos}, {scrollbar.GetComponent<Scrollbar>().value}");

            if (current_pos == previous_pos && buttonClicked)
            {
                Debug.Log("Button was click");
                switch (current_pos)
                {
                    case var _ when current_pos > 0.9f: current_pos = 2f; break;
                    case var _ when current_pos >= 0.4f: current_pos = 1f; break;
                    case var _ when current_pos < 0.1: current_pos = 0f; break;
                }

                string levelName = transform.GetChild(Mathf.RoundToInt(current_pos)).GetComponent<Button>().transform.GetChild(0).GetComponent<Text>().text;
                Client.instance.levelSelected = levelName;
                Debug.Log($"Player level selected: {Client.instance.levelSelected}");
                if (Client.instance.gameModeSelected == "Multiplayer")
                {
                    StartCoroutine(LoadAsynchronously("Lobby"));
                }
                else if (Client.instance.gameModeSelected == "Singleplayer" && Client.instance.levelSelected == "Vaquita")
                {
                    StartCoroutine(LoadAsynchronously("VaquitaS"));
                }
                else
                {
                    StartCoroutine(LoadAsynchronously(levelName));
                }
            }

        }

        if (run)
        {
            pos = new float[transform.childCount];
            float distance = 1f / (pos.Length - 1f);
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = distance * i;
            }
            if (Input.GetMouseButton(0))
            {
                scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
            }
            else
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                    {
                        scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                    }
                }
            }
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                    for (int a = 0; a < pos.Length; a++)
                    {
                        if (a != i)
                        {
                            transform.GetChild(a).localScale = Vector2.Lerp(transform.GetChild(a).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                        }
                    }
                }
            }
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

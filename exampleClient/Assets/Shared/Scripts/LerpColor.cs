using UnityEngine;
using UnityEngine.UI;

public class LerpColor : MonoBehaviour
{
    public float speed = 1.0f;
    public Color startColor;
    public Color endColor;
    public bool repeatable = true;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!repeatable)
        {
            float t = (Time.time - startTime) * speed;
            GetComponent<Image>().material.color = Color.Lerp(startColor, endColor, t);
        }
        else
        {
            float t = (Mathf.Sin(Time.time - startTime) * speed);
            GetComponent<Image>().material.color = Color.Lerp(startColor, endColor, t);
        }
    }
}

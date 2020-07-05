using UnityEngine;
using UnityEngine.UI;

public class LerpColor : MonoBehaviour
{
    //public float speed = 1.0f;
    //public Color startColor;
    //public Color endColor;
    //public bool repeatable = false;
    //float startTime;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    startTime = Time.time;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (!repeatable)
    //    {
    //        float t = (Time.time - startTime) * speed;
    //        GetComponent<Image>().color = Color.Lerp(startColor, endColor, t);
    //    }
    //    else
    //    {
    //        float t = (Mathf.Sin(Time.time - startTime) * speed);
    //        GetComponent<Image>().color = Color.Lerp(startColor, endColor, t);
    //    }
    //}

    [SerializeField] [Range(0f, 20f)] float lerpTime;

    [SerializeField] Color[] myColors;

    int colorIndex = 0;

    float t = 0f;

    int len;

    // Start is called before the first frame update
    void Start()
    {
        len = myColors.Length;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color, myColors[colorIndex], lerpTime * Time.deltaTime);

        t = Mathf.Lerp(t, 1f, lerpTime * Time.deltaTime);
        if (t > .9f)
        {
            t = 0f;
            colorIndex++;
            colorIndex = (colorIndex >= len) ? 0 : colorIndex;
        }
    }
}

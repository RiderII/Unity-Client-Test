using UnityEngine;
using UnityEngine.UI;

public class LerpColor : MonoBehaviour
{
    Image colorPanel;
    [SerializeField] [Range(0f, 1f)] float lerpTime;

    [SerializeField] Color myColor;

    // Start is called before the first frame update
    void Start()
    {
        colorPanel = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        colorPanel.material.color = Color.Lerp(colorPanel.material.color, myColor, lerpTime);
    }
}

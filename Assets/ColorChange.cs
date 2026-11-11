using UnityEngine;
using UnityEngine.UI;

public class ColorChange : MonoBehaviour
{
    public float speed;
    public Text text;
    public float hue;
    void Start()
    {
        text = GetComponent<Text>();
    }
    void Update()
    {
        hue += Time.deltaTime * speed;
        if (hue > 1f) hue = 0f;

        text.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}

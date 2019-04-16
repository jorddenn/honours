using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class paint : MonoBehaviour
{
    RawImage image;
    public Texture2D orig;
    Texture2D tex;
    Slider H, S, V, a, bs;
    Button cb, ab, rb;
    Color rgb = Color.white;
    public IcoSphere reColour;

    void Start()
    {
        H = GameObject.Find("H").GetComponent<Slider>();
        S = GameObject.Find("S").GetComponent<Slider>();
        V = GameObject.Find("V").GetComponent<Slider>();
        a = GameObject.Find("a").GetComponent<Slider>();
        bs = GameObject.Find("Brush Size").GetComponent<Slider>();
        cb = GameObject.Find("colour").GetComponent<Button>();
        ab = GameObject.Find("ApplyButton").GetComponent<Button>();
        rb = GameObject.Find("ResetButton").GetComponent<Button>();

        image = gameObject.GetComponent<RawImage>();

        tex = Instantiate(orig);
        //tex.hideFlags = HideFlags.HideAndDontSave;


        for (int i = 0; i < 100; i++)
        {
            for (int r = 0; r < 100; r++)
            {
                tex.SetPixel(i, r, Color.black);
            }
        }

        tex.Apply();

        image.texture = tex;

    }


    void Paint(PointerEventData ped)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), ped.position, ped.pressEventCamera, out localCursor))
            return;

        if (localCursor.x > 0 && localCursor.x < 500 && localCursor.y > 0 && localCursor.y < 250)
        {
            float minX = localCursor.x - bs.value - 1 > 0 ? minX = localCursor.x - bs.value - 1 : minX = 0;
            float maxX = localCursor.x + bs.value + 1 < 500 ? maxX = localCursor.x + bs.value + 1 : maxX = 500;
            float minY = localCursor.y - bs.value - 1 > 0 ? minY = localCursor.y - bs.value - 1 : minY = 0;
            float maxY = localCursor.y + bs.value + 1 < 250 ? maxY = localCursor.y + bs.value + 1 : maxY = 250;

            minX = map(minX, 0, 500, 0, 2048);
            maxX = map(maxX, 0, 500, 0, 2048);
            minY = map(minY, 0, 250, 0, 1024);
            maxY = map(maxY, 0, 250, 0, 1024);

            for (float x = minX; x < maxX; x++)
            {
                for (float y = minY; y < maxY; y++)
                {
                    if (Mathf.Pow((x - map(localCursor.x, 0, 500, 0, 2048)), 2) + Mathf.Pow((y - map(localCursor.y, 0, 250, 0, 1024)), 2) <= Mathf.Pow(bs.value, 2))
                    {
                        tex.SetPixel((int)x, (int)y, calculateMix((int)x, (int)y));
                    }
                }
            }
            tex.Apply();

            image.texture = tex;

        }
    }

    public void Apply()
    {
        Debug.Log("height" + tex.height);
        reColour.reColour(tex);
    }

    public void Rset()
    {
        tex = Instantiate(orig);
        image.texture = tex;
        reColour.reColour(tex);
    }


    // Update is called once per frame
    void Update()
    {
        rgb = Color.HSVToRGB(H.value, S.value, V.value);
        rgb = new Color(rgb.r, rgb.g, rgb.b, a.value);
        cb.GetComponent<Image>().color = rgb;

        if (Input.GetMouseButton(0))
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            Paint(eventData);
        }

    }

    Color calculateMix(int x, int y)
    {
        Color cur = tex.GetPixel(x, y);
        Color mix = new Color();

        mix.a = 1 - (1 - rgb.a) * (1 - cur.a);
        mix.r = (rgb.r * rgb.a / mix.a) + (cur.r * cur.a * (1 - rgb.a) / mix.a);
        mix.g = (rgb.g * rgb.a / mix.a) + (cur.g * cur.a * (1 - rgb.a) / mix.a);
        mix.b = (rgb.b * rgb.a / mix.a) + (cur.b * cur.a * (1 - rgb.a) / mix.a);

        return mix;
    }



    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }


}

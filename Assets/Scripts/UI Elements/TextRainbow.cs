using UnityEngine.UI;
using UnityEngine;

public class TextRainbow : MonoBehaviour
{
    Text text;
    byte r = 0;
    byte g = 0;
    byte b = 255;
    byte a = 255;

    float changeRate;
    float nextChange;

    byte colorChangeRate = 17;

    float time = 0.0f;


    void Start()
    {
        text = GetComponent<Text>();
        nextChange = 0.0f;
        changeRate = 0.004f;
    }

    void Update()
    {
        time += Time.unscaledDeltaTime;
        if(CheckTime2())
        {
            ColorAdjustTick();
            SetColor();
        }
    }

    bool CheckTime()
    {
        if(Time.unscaledTime > nextChange)
        {
            nextChange = Time.unscaledTime + changeRate;
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckTime2()
    {
        if(time > changeRate)
        {
            time = 0.0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    void ColorAdjustTick()
    {
        if(r < 255 && g == 0 && b == 255)
        {
            //Debug.Log("r++;");

            r += colorChangeRate;
            // increaseRed()
        }
        else if(r == 255 && g == 0 && b > 0)
        {
            //Debug.Log("b--;");

            b -= colorChangeRate;
            // decreaseBlue()
        }
        else if(r == 255 && g < 255 && b == 0)
        {
            //Debug.Log("g++;");

            g += colorChangeRate;
            // increaseGreen()
        }
        else if(r > 0 && g == 255 && b == 0)
        {
            //Debug.Log("r--;");

            r -= colorChangeRate;
            // decreaseRed()
        }
        else if(r == 0 && g == 255 && b < 255)
        {
            //Debug.Log("b++;");

            b += colorChangeRate;
            // increaseBlue()
        }
        else if(r == 0 && g > 0 && b == 255)
        {
            //Debug.Log("g--;");

            g -= colorChangeRate;
            // decreaseGreen()
        }
        else
        {
            Debug.Log("Something is wrong");
        }
    }

    void SetColor()
    {
        text.color = new Color32(r, g, b, a);
    }

    void RandomColor()
    {
        /*
        r = (byte) Random.Range(0, 256);
        g = (byte) Random.Range(0, 256);
        b = (byte) Random.Range(0, 256);
        text.color = new Color32(r, g, b, (byte) 255);
        */
    }
    
}

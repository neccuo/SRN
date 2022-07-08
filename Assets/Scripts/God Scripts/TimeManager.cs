using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Month
{
    January, February, March, April, May, June, July, August, September, October, November, December
}

public class Date
{
    static public int dayOfMonth;

    static public Month month;
    static public int year;

    static Date()
    {
        LoadDate();
    }
    
    public static void PassDay()
    {
        dayOfMonth++;
        switch(month)
        {
            case Month.January:
                _CheckMonthEnd(31);
                break;
            case Month.February:
                if(_CheckLeapYear())
                    _CheckMonthEnd(29);
                else
                    _CheckMonthEnd(28);
                break;
            case Month.March:
                _CheckMonthEnd(31);
                break;
            case Month.April:
                _CheckMonthEnd(30);
                break;
            case Month.May:
                _CheckMonthEnd(31);
                break;
            case Month.June:
                _CheckMonthEnd(30);
                break;
            case Month.July:
                _CheckMonthEnd(31);
                break;
            case Month.August:
                _CheckMonthEnd(31);
                break;
            case Month.September:
                _CheckMonthEnd(30);
                break;
            case Month.October:
                _CheckMonthEnd(31);
                break;
            case Month.November:
                _CheckMonthEnd(30);
                break;
            case Month.December:
                _CheckMonthEnd(31);
                break;
        }
    }

    private static void _CheckMonthEnd(int dayLim)
    {
        if(dayOfMonth > dayLim)
        {
            PassMonth();
        }
    }

    public static void PassMonth()
    {
        int monthAsNum = (int) month;
        month = (Month) ((monthAsNum+1) % 12); // yarrak
        dayOfMonth = 1;
        if(month == Month.January)
        {
            PassYear();
        }
    }

    public static void PassYear()
    {
        year++;
    }

    private static bool _CheckLeapYear()
    {
        if(year%4 != 0)
            return false;
        if(year%100 != 0)
            return true;
        if(year%400 == 0)
            return false;
        else
            return true;
    }

    public static void LoadDate()
    {
        dayOfMonth = PlayerPrefs.GetInt("day", 1);
        month = (Month) PlayerPrefs.GetInt("month", 0);
        year = PlayerPrefs.GetInt("year", 3000);;
    }

    public static void SaveDate()
    {
        PlayerPrefs.SetInt("day", dayOfMonth);
        PlayerPrefs.SetInt("month", (int) month);
        PlayerPrefs.SetInt("year", year);
    }
}

public class TimeManager : MonoBehaviour
{
    // day = nco/spd

    [SerializeField] private SystemDB _systemDB;

    private Player _player;

    private float _playerSpeed;

    public Text timeIndicator;

    public Text secondsPastIndicator;


    float secondsPast;

    void Start()
    {
        _player = Controller.ControllerGod.player;
        _playerSpeed = _player.transform.GetChild(0).GetComponent<Spaceship>().speed;
        timeIndicator.text = DisplayDate();

    }

    private void OnPassDay()
    {
        StartCoroutine(GameManager.Instance.saveLoad.UpdatePrices());
        timeIndicator.text = DisplayDate();
        // If you happen to add a functionality that updates planet locations per day, put it under here
        _systemDB.SavePlanets();
        // save it to playerprefs
        Date.SaveDate();


    }

    void Update()
    {
        if(secondsPast >= 1.0f)
        {
            secondsPast = 0;
            Date.PassDay();
            // ^DEFINITELY USE UNITY EVENTS AT SOME POINT FOR THIS
            OnPassDay();
        }
        secondsPast += Time.deltaTime;
        secondsPastIndicator.text = "Day: " + (secondsPast)*100 + "%";


        if(Input.GetMouseButtonDown(0) && GameManager.Instance.GetCurrentState() == GameState.PlanMovement)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dist = mousePos - (Vector2) _player.transform.position;
            Debug.Log("Difference is: " + dist.magnitude);
            float day = dist.magnitude/_playerSpeed;
            Debug.LogFormat("{0} Days will pass to reach the destination", day);
        }
        
    }

    string DisplayDate()
    {
        return "" + Date.dayOfMonth + " / " + Date.month + " / " + Date.year;
    }

}

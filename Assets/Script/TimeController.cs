using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [SerializeField] private MenuButton MenuButton;
    [SerializeField] private GameFinish gameFinish; 
    [HideInInspector] public double MinuteUse = 0;
    [HideInInspector] public double SecUse;
    [HideInInspector] public double TimeRemaining = 0;
    [HideInInspector] public bool StartTime = false;
    [HideInInspector] public double SecTotalDefault = 0;
    public Text TimeText;
    public double SecTotal = 0;
    private double Sec = 0;
    private string SecText;
    
    private void Start()
    {
        SecTotalDefault = SecTotal;
        Sec = SecTotal % 60;
        ShowDefaultTime();
    }

    void FixedUpdate ()
    {
        if (MenuButton.isPlay) StartTime = true;

        if (MenuButton.isStop == false && StartTime == true)
        {
            if(SecTotal > 0)
            {
                SecTotal -= Time.deltaTime;
                TimeRemaining = Math.Floor(SecTotal);

                if (Sec > 0) Sec -= Time.deltaTime;
                else Sec = 60f;

                if (Sec < 10) SecText = "0" + (Math.Floor(Sec)).ToString();
                //else if(Sec >= 59.5f) SecText = "00";
                else SecText = (Math.Floor(Sec)).ToString();

                if(SecTotal >= 0) TimeText.text = "0" + (Math.Floor(SecTotal / 60f)).ToString() + " : " + SecText;
                else if(SecTotal < 0 && Sec > 0) TimeText.text = "00 : " + SecText;
                else TimeText.text = "00 : 00";
            }
            else
            {
                CalTimeSpent();
                MenuButton.isPlay = false;
                SecTotal = SecTotalDefault;
                Sec = SecTotal % 60;
                gameFinish.LevelFinish("Lose", "Time out");
            }
        }
        else if(MenuButton.isStop)
        {
            SecTotal = SecTotalDefault;
            Sec = SecTotal % 60;
            SecText = "0" + (Math.Round(Sec)).ToString();
            ShowDefaultTime();
            StartTime = false;
        }
	}

    public void CalTimeSpent()
    {
        var timeuse = SecTotalDefault - TimeRemaining;

        if (timeuse != 0 && timeuse < SecTotalDefault)
        {
            MinuteUse = (int)(timeuse / 60f);
            SecUse = Math.Abs(Math.Round(timeuse - (60 * MinuteUse)));
        }
        else
        {
            MinuteUse = (int)(SecTotalDefault / 60);
            SecUse = SecTotalDefault % 60;
        }
    }

    public void ShowDefaultTime()
    {
        if (SecTotal >= 60)
        {
            string secText;

            if (SecTotal % 60 >= 10) secText = (SecTotal % 60).ToString();
            else secText = "0"+(SecTotal % 60).ToString();

            TimeText.text = "0" + (Math.Floor(SecTotal / 60f)).ToString() + " : " + secText;
        }
        else if (SecTotal >= 10) TimeText.text = "00" + " : " + SecTotal;
        else TimeText.text = "00" + " : 0" + SecTotal;
    }
}

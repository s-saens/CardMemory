// TimerBar(UI) ' s component

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Timer : MonoBehaviour
{
    private int limitTime = 10;
    private float time;

    private Image img;
    private Text timeString;

    private IEnumerator timerCoroutine = null;

    private Action endCallback = null;

    public void StartTimer(int limitTime, Action callback)
    {
        this.endCallback = callback;
        img = this.GetComponent<Image>();
        timeString = this.transform.GetChild(0).GetComponent<Text>();
        // 이전에 진행되던 타이머코루틴이 있다면 멈춰주기
        if(timerCoroutine != null)
        {
            StopTimer();
        }

        this.limitTime = limitTime;

        this.endCallback = callback;

        timerCoroutine = TimerCoroutine();
        StartCoroutine(timerCoroutine);

    }

    // Timer를 종료하고 time 값을 반환
    public float StopTimer()
    {
        // 타이머코루틴이 없다면 -1 반환
        if(timerCoroutine == null)
        {
            return -1;
        }

        StopCoroutine(timerCoroutine);
        return time;
    }

    IEnumerator TimerCoroutine()
    {
        time = limitTime;
        UpdateUI();

        while(time > 0) {
            time-= Time.deltaTime;
            UpdateUI();
            yield return 0;
        }
        this.endCallback();
    }

    private void UpdateUI()
    {
        timeString.text = FloatToTime(time);
        img.fillAmount = time / limitTime;
    }

    private string FloatToTime(float t)
    {
        return $"{Mathf.Ceil(t)}";
    }
}
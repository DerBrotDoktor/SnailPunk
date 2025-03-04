using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float secondsInFullDay = 120f;
    [SerializeField, Range(0, 1)] private float currentTimeOfDay = 0.45f;
    
    [Header("Sun Light Settings")]
    [SerializeField] private Light sun;
    [SerializeField] private Gradient sunColorGradient;
    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float sunThreshold = 0.05f;

    [Header("Ambient Light Settings")]
    [SerializeField] private float maxAmbient = 1f;
    [SerializeField] private float minAmbient = 0f;
    [SerializeField] private float ambientThreshold = -0.2f;

    [Header("Sunrise and Sunset Settings")]
    [SerializeField, Range(0, 1)] private float sunriseStart = 0.2f;
    [SerializeField, Range(0, 1)] private float sunriseEnd = 0.3f;
    [SerializeField, Range(0, 1)] private float sunsetStart = 0.7f;
    [SerializeField, Range(0, 1)] private float sunsetEnd = 0.8f;

    [Header("Time Display")]
    public int hours;
    public int minutes; 
    public bool IsDay => currentTimeOfDay > sunriseEnd && currentTimeOfDay < sunsetStart;

    public static Action NewDay;
    public static Action NightStarted;

    private bool IsDayLastFrame;

    private void Start()
    {
        UpdateRotation();
        UpdateLight();
    }

    private void Update()
    {
        if (Game.TimeScale <= 0) return;
        
        UpdateTime();
        UpdateRotation();
        UpdateLight();
    }
    
    private void UpdateTime()
    {
        
        if (currentTimeOfDay >= 1)
        {
            currentTimeOfDay = 0;
        }

        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * Game.TimeScale;
        
        float timeInHours = currentTimeOfDay * 24;
        hours = Mathf.FloorToInt(timeInHours);
        minutes = Mathf.FloorToInt((timeInHours - hours) * 60);

        if (!IsDay && IsDayLastFrame)
        {
            IsDayLastFrame = false;
            NightStarted?.Invoke();
        }
        else if (IsDay && !IsDayLastFrame)
        {
            IsDayLastFrame = true;
            NewDay?.Invoke();
        }
    }
    
    private void UpdateRotation()
    {
        sun.transform.localRotation = Quaternion.Euler((-currentTimeOfDay * 360f) - 90, 170, 0);
    }
    
    private void UpdateLight()
    {
        float sunIntensity = CalculateSunIntensity();
        sun.intensity = sunIntensity;
        sun.color = sunColorGradient.Evaluate(sunIntensity);


        float ambientIntensity = CalculateAmbientIntensity();
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.ambientLight = sunColorGradient.Evaluate(ambientIntensity);
    }


    private float CalculateSunIntensity()
    {
        if (currentTimeOfDay >= sunriseStart && currentTimeOfDay <= sunriseEnd)
        {
            float t = (currentTimeOfDay - sunriseStart) / (sunriseEnd - sunriseStart);
            return Mathf.Lerp(minIntensity, maxIntensity, t);
        }
        else if (currentTimeOfDay >= sunsetStart && currentTimeOfDay <= sunsetEnd)
        {
            float t = (currentTimeOfDay - sunsetStart) / (sunsetEnd - sunsetStart);
            return Mathf.Lerp(maxIntensity, minIntensity, t);
        }
        else if (currentTimeOfDay > sunriseEnd && currentTimeOfDay < sunsetStart)
        {
            return maxIntensity;
        }
        else
        {
            return minIntensity;
        }
    }
    
    private float CalculateAmbientIntensity()
    {
        if (currentTimeOfDay >= sunriseStart && currentTimeOfDay <= sunriseEnd)
        {
            float t = (currentTimeOfDay - sunriseStart) / (sunriseEnd - sunriseStart);
            return Mathf.Lerp(minAmbient, maxAmbient, t);
        }
        else if (currentTimeOfDay >= sunsetStart && currentTimeOfDay <= sunsetEnd)
        {
            float t = (currentTimeOfDay - sunsetStart) / (sunsetEnd - sunsetStart);
            return Mathf.Lerp(maxAmbient, minAmbient, t);
        }
        else if (currentTimeOfDay > sunriseEnd && currentTimeOfDay < sunsetStart)
        {
            return maxAmbient;
        }
        else
        {
            return minAmbient;
        }
    }
}

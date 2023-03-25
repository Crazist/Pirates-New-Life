using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayChange : MonoBehaviour
{
    [SerializeField] private Gradient AmbientColor;
    [SerializeField] private Gradient DirectinalColor;
    [SerializeField] private Gradient FogColor;
    [SerializeField] private Light DirectionalLight;
   
    [SerializeField, Range(0, 24)] private float TimeOfday;

    private float cycleDuration = 600f; // 10 minutes in seconds
    
    private Action _dayChangeAction;

    private float cycleProgress = 6f / 24f;

    private const int StartDay = 6;

    private const int StartNight = 22;
    private void OnValidate()
    {
        if(DirectionalLight != null)
        {
            return;
        }


        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();

            foreach (var light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }

    }
  
    public void SetDayChangeAction(Action dayChangeAction)
    {
        _dayChangeAction = dayChangeAction;
    }
    private void On24HourCycle()
    {
        Debug.Log("24 hour cycle has elapsed");
        _dayChangeAction.Invoke();
        // Put your code here that you want to execute every 24 hours
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            cycleProgress += Time.deltaTime / cycleDuration;
            cycleProgress %= 1f;
            TimeOfday = cycleProgress * 24f;
            if ((TimeOfday >= StartDay && TimeOfday < StartDay + Time.deltaTime) || (TimeOfday >= StartNight && TimeOfday < StartNight + Time.deltaTime))
            {
                On24HourCycle();
            }
            UpdateLighting(cycleProgress);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = FogColor.Evaluate(timePercent);

        if (DirectionalLight != null)
        {
            DirectionalLight.color = DirectinalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0f));
        }
    }
    public Gradient GetAmbientColor()
    {
        return AmbientColor;
    }
    public Gradient GetDirectinalColor()
    {
        return DirectinalColor;
    }
    public Gradient GetFogColor()
    {
        return FogColor;
    }
    public Light GetDirectionalLight()
    {
        return DirectionalLight;
    }
}

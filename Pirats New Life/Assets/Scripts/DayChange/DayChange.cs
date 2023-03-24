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

    private float cycleProgress = 6f / 24f;
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
   

    private void Update()
    {
        if (Application.isPlaying)
        {
            cycleProgress += Time.deltaTime / cycleDuration;
            cycleProgress %= 1f;
            TimeOfday = cycleProgress * 24f;
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

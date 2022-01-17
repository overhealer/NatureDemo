using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightningManager : MonoBehaviour
{
    [SerializeField] private Light _directionalLight;
    [SerializeField] private LightningPreset _preset;

    [SerializeField, Range(0, 24)] private float _timeOfDay;


    private void Update()
    {
        if (_preset == null)
            return;

        if (Application.isPlaying)
        {
            //(Replace with a reference to the game time)
            _timeOfDay += Time.deltaTime;
            _timeOfDay %= 24; //Modulus to ensure always between 0-24
            UpdateLighting(_timeOfDay / 24f);
        }
        else
        {
            UpdateLighting(_timeOfDay / 24f);
        }
    }


    private void UpdateLighting(float timePercent)
    {
        //Set ambient and fog
        RenderSettings.ambientLight = _preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = _preset.FogColor.Evaluate(timePercent);

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (_directionalLight != null)
        {
            _directionalLight.color = _preset.DirectionalColor.Evaluate(timePercent);

            _directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

    }

    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (_directionalLight != null)
            return;

        //Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            _directionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    _directionalLight = light;
                    return;
                }
            }
        }
    }
}

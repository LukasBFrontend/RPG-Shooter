using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightLevels : MonoBehaviour
{
    [System.Serializable]
    public struct Lights
    {
        public Light2D z0;
        public Light2D z1;
        public Light2D z2;
    }
    [SerializeField] Renderer sortingLayer;
    [SerializeField] Lights lights;
    [Min(0f)]
    [SerializeField] float baseIntensity;
    [Tooltip("The percentage rate at which light intensity is reduced per layer")]
    [Range(0, 100)]
    [SerializeField] int perLayerFalloff = 15;
    static readonly Dictionary<string, int> _layerIndexLookup = new()
    {
        ["A"] = 0,
        ["B"] = 1,
        ["C"] = 2,
        ["CharacterA"] = 0,
        ["CharacterB"] = 1,
        ["CharacterC"] = 2,
        ["Overlay A"] = 0,
        ["Overlay B"] = 1,
        ["Overlay C"] = 2,
    };

    void SetLightIntensities()
    {
        Light2D[] _lightArray = {
            lights.z0,
            lights.z1,
            lights.z2,
        };

        if (!sortingLayer || _lightArray.Contains(null))
        {
            return;
        }

        int _startIndex = _layerIndexLookup[sortingLayer.sortingLayerName];

        for (int i = 0; i < _lightArray.Length; i++)
        {
            int _difference = Mathf.Abs(_startIndex - i);
            float _multiplier = Mathf.Pow((100f - perLayerFalloff) / 100f, _difference);

            _lightArray[i].intensity = baseIntensity * _multiplier;
        }
    }

    void OnValidate()
    {
        SetLightIntensities();
    }
}

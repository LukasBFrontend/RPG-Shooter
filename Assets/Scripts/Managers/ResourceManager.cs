using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ResourceManager : Singleton<ResourceManager>
{
    [Header("Materials")]
    public Material[] materials;
    public ColorVariableEntry[] Colors;
    [System.Serializable]
    public class ColorVariableEntry
    {
        public string Name;
        public Color color;
    }

    public Material GetMaterial(string name)
    {
        return materials.First(mat => mat.name == name);
    }
}

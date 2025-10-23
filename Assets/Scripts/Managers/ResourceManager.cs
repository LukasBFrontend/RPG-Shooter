using System.Linq;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    [Header("Materials")]
    public Material[] materials;

    public Material GetMaterial(string name)
    {
        return materials.First(mat => mat.name == name);
    }
}

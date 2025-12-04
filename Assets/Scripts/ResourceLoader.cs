using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;


public class ResourceLoader : Singleton<ResourceLoader>
{
    [Header("Materials")]
    public Material[] materials;
    public ColorVariableEntry[] Colors;
    public AudioMixer audioMixer;
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

    void Start()
    {
        RoomManager.CacheRooms();

        Delegate();
    }

    void Delegate()
    {
        SoundMixer.AudioMixer = audioMixer;
    }
}

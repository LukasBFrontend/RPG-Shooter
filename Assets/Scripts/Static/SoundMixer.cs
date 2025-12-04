using UnityEngine;
using UnityEngine.Audio;

public static class SoundMixer
{
    public static AudioMixer AudioMixer;

    public static void SetMasterVolume(float level)
    {
        AudioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }

    public static void SetSoundFXVolume(float level)
    {
        AudioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
    }

    public static void SetMusicVolume(float level)
    {
        AudioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
    }
}

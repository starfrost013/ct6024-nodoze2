using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio manager class...
/// </summary>
internal static class AudioManager
{
    // change this when we have assetbundles
    internal const string AUDIO_PATH = "Audio/";

    /// <summary>
    /// The audio clips
    /// </summary>
    internal static AudioClip[] clips { get; private set; }
    
    private static bool initialised;

    /// <summary>
    /// Loads all audio clips.
    /// </summary>
    internal static void Init()
    {
        Debug.Log("Loading audio...");
        clips = AssetManager.LoadAssetsInFolder<AudioClip>(AUDIO_PATH);
        initialised = (clips != null);
    }

    /// <summary>
    /// get an audio clip by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns>The audio clip object with the name corresponding to the name parameter if one is found, otherwise NULL</returns>
    internal static AudioClip GetAudioByName(string name)
    {
        if (!initialised)
            return null; 

        foreach (AudioClip clip in clips)
        {
            if (clip.name == name) 
                return clip; 
        }

        Debug.LogError("The audio by " + name + " doesn't exist. Please put it in Assets\\Resources\\Audio");
        return null;
    }

    /// <summary>
    /// Play an audio at the main camera position
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    internal static void PlayAudioAtCameraPosition(AudioClip clip, float volume = 1.0f)
    {
        if (!clip)
        {
            Debug.LogWarning("Called AudioManager::PlayAudioAtCameraPosition with NULL clip!");
            return; 
        }

        // this will explode if we ever have multiple cameras
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }

    /// <summary>
    /// Play an audio at the current main camera position
    /// </summary>
    /// <param name="name">The name of the audio clip to play</param>
    internal static void PlayAudioAtCameraPosition(string name, float volume = 1.0f)
    {
        PlayAudioAtCameraPosition(GetAudioByName(name));
    }

    /// <summary>
    /// Play an audio source.
    /// </summary>
    /// <param name="source">The audio source object</param>
    /// <param name="sound">The sound to play</param>
    /// <param name="position">The position the sound emanates fom</param>
    /// <param name="volume">The volume</param>
    /// <param name="loop">If the sound should loop</param>
    /// <returns></returns>
    internal static AudioSource PlayAudioSource(AudioSource source, AudioClip sound, Vector3 position, float volume = 1.0f, bool loop = false)
    {
        if (!source)
        {
            Debug.LogWarning("Called AudioManager::PlayAudioSource with NULL source!");
            return null;
        }
        else if (!sound)
        {
            Debug.LogWarning("Called AudioManager::PlayAudioSource with NULL clip!");
            return null;
        }

        source.transform.position = position;
        source.volume = volume;
        source.clip = sound;
        source.loop = loop;

        source.Play();
        return source;
    }

    /// <summary>
    /// Play an audio source with user-supplied volume, loop and audio clip.
    /// </summary>
    /// <param name="source">The Unity Audio Source component to play.</param>
    /// <returns>The Audio Source after it has played.</returns>
    internal static AudioSource PlayAudioSource(AudioSource source)
    {
        return PlayAudioSource(source);
    }

    /// <summary>
    /// Create an audio source and play it at a certain point.
    /// </summary>
    /// <param name="sound">Sound to play</param>
    /// <param name="position">Positiont o play the sound at</param>
    /// <param name="volume">Volume of the sound</param>
    /// <param name="loop">If the sounds hould loop or not.</param>
    /// <returns></returns>
    internal static AudioSource PlayAudioAtPoint(AudioClip sound, Vector3 position, float volume = 1.0f, bool loop = false)
    {
        // implicitly create an audio sourc
        AudioSource source = new GameObject("SoundPlayer").AddComponent<AudioSource>();
        return PlayAudioSource(source, sound, position, volume, loop);
        
    }

    /// <summary>
    /// Create an audio source and play it at a certain point.
    /// </summary>
    /// <param name="sound">Sound to play</param>
    /// <param name="position">Positiont o play the sound at</param>
    /// <param name="volume">Volume of the sound</param>
    /// <param name="loop">If the sounds hould loop or not.</param>
    /// <returns></returns>
    internal static AudioSource PlayAudioAtPoint(string name, Vector3 position, float volume = 1.0f, bool loop = false)
    {
        // implicitly create an audio sourc
        AudioSource source = new GameObject("SoundPlayer").AddComponent<AudioSource>();
        return PlayAudioSource(source, GetAudioByName(name), position, volume, loop);

    }
}

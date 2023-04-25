using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Sound effects for the character when they perform various tasks like walking, running, jumping, attacking, etc.
/// Sounds are grunts, footsteps, etc.
/// This could be used for enemies as well
/// </summary>
public class SoundEffects : MonoBehaviour
{
    private string surfaceType; //what surface the character is walking on
    private AudioSource audioSource;

    private Dictionary<string, List<AudioClip>> soundEffectLists = new Dictionary<string, List<AudioClip>>();
    
    //sounds for player
    [SerializeField] private List<AudioClip> StepSounds;
    [SerializeField] private List<AudioClip> GruntSounds;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        AddSoundEffectList("Steps", StepSounds);
        AddSoundEffectList("Grunts", GruntSounds);
    }

    public void AddSoundEffectList(string soundType, List<AudioClip> soundEffects)
    {
        //if we dont already have the sound effect type in the dictionary
        if (!soundEffectLists.ContainsKey(soundType))
        {
            soundEffectLists.Add(soundType, soundEffects);
        }
        else
        {
            Debug.LogWarning("Effect line list for: " + soundType + " already exists!");
        }
    }
    
    //Play a specific sound effect for the character
    public void PlaySoundEffect(string soundType, string soundEffectName)
    {
        if (soundEffectLists.ContainsKey(soundType))
        {
            List<AudioClip> soundEffects = soundEffectLists[soundType];
            //check if we have the sound effect we're looking for
            AudioClip soundEffect = soundEffects.Find(sound => sound.name == soundEffectName);

            if (soundEffect != null)
            {
                audioSource.PlayOneShot(soundEffect);
                audioSource.pitch = UnityEngine.Random.Range(1f, 1.2f);
            }
            else
            {
                Debug.LogWarning("Sound effect: " + soundEffectName + " not found for: " + soundType + "!");
            }
        }
        else
        {
            Debug.LogWarning("No sound effect list found for: " + soundType + "!");
        }
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        //save tag of object the player collided with (stepped on)
        surfaceType = col.gameObject.tag;
    }

    private void Footsteps()
    {
        Debug.Log(surfaceType);
        switch (surfaceType)
        {
            case "Grass":
                PlaySoundEffect("Steps", "GrassFootsteps");
                break;
            case "Wood":
                PlaySoundEffect("Steps", "WoodFootsteps");
                break;
            case "Dirt":
                break;
            default: 
                PlaySoundEffect("Steps", "GrassFootsteps");
                break;
        }
    }
}

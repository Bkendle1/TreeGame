using System;
using UnityEngine;
using System.Collections.Generic;

public class VoiceLineManager : MonoBehaviour
{
    public static VoiceLineManager Instance;

    // Dictionary of voice line lists for each character
    public Dictionary<string, List<AudioClip>> voiceLineLists = new Dictionary<string, List<AudioClip>>();

    // Audio source to play the voice lines
    private AudioSource audioSource;

    //Voice Lines for characters
    [SerializeField] private List<AudioClip> MotherNatureVoiceLines;
    [SerializeField] private List<AudioClip> PlayerVoiceLines;
    [SerializeField] private List<AudioClip> BirdVoiceLines;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Get the audio source component
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        AddVoiceLineList("MotherNature", MotherNatureVoiceLines);
        AddVoiceLineList("Player", PlayerVoiceLines);
        AddVoiceLineList("Bird", BirdVoiceLines);
    }

    // Add a new voice line list for a character
    public void AddVoiceLineList(string characterName, List<AudioClip> voiceLines)
    {
        if (!voiceLineLists.ContainsKey(characterName))
        {
            voiceLineLists.Add(characterName, voiceLines);
        }
        else
        {
            Debug.LogWarning("Voice line list for " + characterName + " already exists!");
        }
    }

    // Play a specific voice line for a character
    public void PlayVoiceLine(string characterName, string voiceLineName)
    {
        if (voiceLineLists.ContainsKey(characterName))
        {
            List<AudioClip> voiceLines = voiceLineLists[characterName];
            AudioClip voiceLine = voiceLines.Find(clip => clip.name == voiceLineName);

            if (voiceLine != null)
            {
                audioSource.clip = voiceLine;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("Voice line " + voiceLineName + " not found for " + characterName + "!");
            }
        }
        else
        {
            Debug.LogWarning("No voice line list found for " + characterName + "!");
        }
    }
    //Stop playing voice line
    public void StopVoiceLine()
    {
        audioSource.Stop();
    }
}
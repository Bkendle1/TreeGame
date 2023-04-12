using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.SearchService;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    public bool dialogueIsPlaying { get; private set; }
    private Story currentStory;
    
    private const string SPEAKER_TAG = "speaker";

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
        
        //get all of the choices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            //store the text for each choice in the array of choices
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        //return if dialogue isn't playing
        if (!dialogueIsPlaying)
        {
            return;
        }
        
        //handle continuing to the next line in the dialogue when submit is pressed
        if (currentStory.currentChoices.Count == 0 
            && Movement.Instance.GetSubmitPressed()) 
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        //reset dialogue assets
        displayNameText.text = "Name";
        
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            //set tesxt for the current dialogue line
            dialogueText.text = currentStory.Continue();
            //display choices, if any for this dialogue line
            DisplayChoices();
            //handle tags
            HandleTags(currentStory.currentTags);
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        //loop through each tag and handle them accordingly
        foreach (string tag in currentTags)
        {
            //parse the tag
            //we're splitting the tags into their key:value pairs
            string[] splitTag = tag.Split(":");
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag couldn't be appropriately parsed: " + tag);
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();
            
            //handle the tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    Debug.Log("speaker=" + tagValue);
                    break;
                 default:
                     Debug.LogWarning("Tag game in but isn't currently being handled: " + tag);
                     break;
            }
        }
    }
    
    private void DisplayChoices()
    {
        //storing the choices in a list
        List<Choice> currentChoices = currentStory.currentChoices;
    
        //defensive check to make sure our UI can support the number of choices coming in
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;
        //enable and initialize the choices up to the amount of choices for this line of dialogue
        foreach (Choice choice in currentChoices)
        {
            //turn on choice option
            choices[index].gameObject.SetActive(true);
            //set the textmeshpro's text to be the text written in the choice
            choicesText[index].text = choice.text;
            index++;
        }
        //go through the remaining choices the UI supports and make sure they're hidden
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        //Event System requires we clear it first, then wait
        //for at least one frame before we set the current selected object.
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0]);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);    
        Movement.Instance.RegisterSubmitPressed();
        ContinueStory();
    }
}

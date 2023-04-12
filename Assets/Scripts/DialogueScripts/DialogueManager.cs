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
    [SerializeField] private float typingSpeed = .4f; //the smaller the value, the faster the speed
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private AudioClip _audioClip;
    
    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    public bool dialogueIsPlaying { get; private set; }
    private Story currentStory;
    private Coroutine displayLineCoroutine;
    private bool canContinueToNextLine = false;
    private AudioSource _audioSource;
    private bool voicePlaying = true;
    
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
        _audioSource = GetComponent<AudioSource>();
        
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
        if (canContinueToNextLine
            && currentStory.currentChoices.Count == 0 
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
        
        //listen for calls to this function
        currentStory.BindExternalFunction("playVoiceLine", (string speaker, string voiceLine) =>
        {
            Debug.Log("speaker: " + speaker + " voice line: " + voiceLine); 
            VoiceLineManager.Instance.PlayVoiceLine(speaker, voiceLine);
        });
        
        //reset dialogue assets
        displayNameText.text = "Name";
        
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        currentStory.UnbindExternalFunction("playVoiceLine");
            
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            //set text for the current dialogue line
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));
            
            //handle tags
            HandleTags(currentStory.currentTags);

        }
        else
        {
            ExitDialogueMode();
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        //empty the dialogue text
        dialogueText.text = "";
        
        //hide items while text is typing
        //continueIcon.SetActive(false);
        HideChoices();
        
        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;
        
        //index needs to be 1 so there's not that loud typing sfx at the beginning of the dialogue when there's rich text
        int index = 1;
        //display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            //if the submit button is pressed, finish displaying
            //the light right away

            if (Movement.Instance.GetSubmitPressed())
            {
                dialogueText.text = line;
                break;
            }
            
            //check for rich text tag, if found, add it without waiting
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                dialogueText.text += letter;
                
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            //if not rich text, add the next letter and wait a small time
            else
            {
                dialogueText.text += letter;
                index++;
                yield return new WaitForSeconds(typingSpeed);
            }
            //play typing sfx for every other character
            if (index % 2 == 0)
            {
                 _audioSource.PlayOneShot(_audioClip);
            }
        }
        //continueIcon.SetActive(true);
        
        //display choices, if any for this dialogue line
        //after the entire line is done typing
        DisplayChoices();
        canContinueToNextLine = true;
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
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
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);    
            Movement.Instance.RegisterSubmitPressed();
            ContinueStory();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonSceneSwap : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private AudioSource audioSource;
    [SerializeField] private AudioClip sfx;
    [Tooltip("How long to wait until button action executes.")]
    [SerializeField] private float buttonActionDelay = .5f;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartSwapScene()
    {
        audioSource.PlayOneShot(sfx);
        Time.timeScale = 1f; // if the game is paused, unpause so invoke can play
        Invoke("SwapScene", buttonActionDelay);
    }

    public void StartResumeGame()
    {
        Time.timeScale = 1f; // if the game is paused, unpause so invoke can play
        audioSource.PlayOneShot(sfx);
        Invoke("ResumeGame", buttonActionDelay);
    }

    private void ResumeGame()
    {
        gameObject.GetComponentInParent<PauseMenu>().ResumeGame();
    }
    
    //swap scene on button press
    private void SwapScene()
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void StartQuitGame()
    {
        Time.timeScale = 1f; // if the game is paused, unpause so invoke can play
        audioSource.PlayOneShot(sfx);
        Invoke("QuitGame", buttonActionDelay);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}

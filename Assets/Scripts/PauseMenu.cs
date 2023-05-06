using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private GameObject pauseMenu;
    public bool isPaused = false;

    public static PauseMenu Instance;
    
    private void Awake()
    {
        //make sure time is at its normal speed 
        Time.timeScale = 1f;
        pauseMenu = GameObject.Find("PauseMenu");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        //freeze time
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        //unfreeze time
        Time.timeScale = 1f;
        isPaused = false;
    }
}

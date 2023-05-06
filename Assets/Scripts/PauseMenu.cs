using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    private RectTransform[] children;
    
    private void Awake()
    {
        
        //get reference to objects in the pause menu
        children = GetComponentsInChildren<RectTransform>();
    }

    private void Start()
    {
        //make sure time is at its normal speed 
        ResumeGame();
        //make sure all children are deactivated at start
        foreach (RectTransform child in children)
        {
            child.gameObject.SetActive(false);
        }  
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
        //activate children
        foreach (RectTransform child in children)
        {
            child.gameObject.SetActive(true);
        }        
        //freeze time
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        //deactivate children
        foreach (RectTransform child in children)
        {
            child.gameObject.SetActive(false);
        }
        //unfreeze time
        Time.timeScale = 1f;
        isPaused = false;
    }
}

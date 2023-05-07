using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineManager : MonoBehaviour
{
    public static CinemachineManager Instance;
    
    //check whether or not we're using the normalCam
    private bool normalCam = true;

    [Tooltip("Normal cinemachine that follows the player through out most of the game.")]
    [SerializeField] private CinemachineVirtualCamera cmNormal;
    [Tooltip("Normal cinemachine for the boss fight.")]
    [SerializeField] private CinemachineVirtualCamera cmBossCam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SwitchPriority()
    {
        if (normalCam)
        {
            cmNormal.Priority = 0;
            cmBossCam.Priority = 1;
        }
        else
        {
            cmNormal.Priority = 1;
            cmBossCam.Priority = 0;
        }

        normalCam = !normalCam;
    }
}

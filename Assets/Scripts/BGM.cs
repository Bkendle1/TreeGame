using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{

    [SerializeField] private AudioClip normalBGM;
    [SerializeField] private AudioClip bossBGM;
    private AudioSource _audioSource;
    private BossBattleTrigger bossTrigger;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        bossTrigger = FindObjectOfType<BossBattleTrigger>();
        _audioSource.clip = normalBGM;
        _audioSource.Play();
    }

    void Update()
    {
        if (!bossTrigger.activateBossFight)
        {
            if (_audioSource.clip == bossBGM)
            {
                _audioSource.clip = normalBGM;
                _audioSource.Play();
            }
        }
        else
        {

            if (_audioSource.clip == normalBGM)
            {
                _audioSource.clip = bossBGM;
                _audioSource.Play();
            }
        }
    }
}

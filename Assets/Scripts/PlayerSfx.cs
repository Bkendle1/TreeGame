using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSfx : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip footSfx;
    [SerializeField] private AudioClip collectExp;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Footsteps()
    {
        audioSource.PlayOneShot(footSfx);
        audioSource.pitch = UnityEngine.Random.Range(1f, 1.2f);
    }

    public void CollectExp()
    {
        audioSource.PlayOneShot(collectExp);
        audioSource.pitch = UnityEngine.Random.Range(1f, 1.2f);
    }

}

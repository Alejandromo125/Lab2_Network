using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioDelayed : MonoBehaviour
{
    public AudioClip track;
    private AudioSource audioSource;

    public float delay = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayDelayed(delay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

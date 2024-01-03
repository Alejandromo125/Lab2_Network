using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] musicTracks; // List of music tracks.

    private AudioSource audioSource;
    private bool isPlaying = false;
    private float loopDuration;
    private float nextTrackTime;
    private int lastTrackIndex = -1;

    public float minMusicDuration = 65f;
    public float maxMusicDuration = 100f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomTrack();
    }

    void Update()
    {
        if (isPlaying && Time.time >= nextTrackTime)
        {
            isPlaying = false;
        }

        if (!isPlaying)
        {
            PlayRandomTrack();
        }
    }

    void PlayRandomTrack()
    {
        if (musicTracks.Length == 0)
        {
            Debug.LogWarning("No music tracks found!");
            return;
        }

        int randomIndex = GetRandomTrackIndex();
        while (randomIndex == lastTrackIndex) // Ensure no repetition of the same track.
        {
            randomIndex = GetRandomTrackIndex();
        }

        lastTrackIndex = randomIndex;
        audioSource.clip = musicTracks[randomIndex];

        loopDuration = Random.Range(minMusicDuration, maxMusicDuration); // Random duration between 90 to 180 seconds.
        audioSource.Play();
        isPlaying = true;
        nextTrackTime = Time.time + loopDuration;
    }

    int GetRandomTrackIndex()
    {
        if (lastTrackIndex == musicTracks.Length - 1)
        {
            ShuffleTracks();
            lastTrackIndex = -1;
        }

        int randomIndex = Random.Range(lastTrackIndex + 1, musicTracks.Length);
        return randomIndex;
    }

    void ShuffleTracks()
    {
        for (int i = 0; i < musicTracks.Length; i++)
        {
            AudioClip temp = musicTracks[i];
            int randomIndex = Random.Range(i, musicTracks.Length);
            musicTracks[i] = musicTracks[randomIndex];
            musicTracks[randomIndex] = temp;
        }
    }
}

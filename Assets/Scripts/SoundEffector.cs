using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffector : MonoBehaviour {
    [SerializeField]
    private AudioSource audioPlayer;
    [SerializeField]
    private AudioClip[] clips;

    public void Play() {
        int randomClipIndex = Random.Range(0, clips.Length);

        if (audioPlayer.isPlaying)
            return;

        audioPlayer.clip = clips[randomClipIndex];
        audioPlayer.Play();
	}
}

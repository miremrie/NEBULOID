using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmAudioController : MonoBehaviour
{
    public float minPitch, maxPitch;

    public AudioSource audioSourceL, audioSourceR;

    public void PlayLeftArm()
    {
        audioSourceL.pitch = Random.Range(minPitch, maxPitch);
        audioSourceL.Play();
    }

    public void PlayRightArm()
    {
        audioSourceR.pitch = Random.Range(minPitch, maxPitch);
        audioSourceR.Play();
    }
}

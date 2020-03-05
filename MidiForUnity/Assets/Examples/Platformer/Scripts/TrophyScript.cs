using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TrophyScript : MonoBehaviour
{
    private AudioSource soundSource;

    void Start()
    {
        soundSource = GetComponent<AudioSource>();
    }

    public void reactToPlayerCollision()
    {
        soundSource.Play();
        Destroy(transform.gameObject);
    }
}

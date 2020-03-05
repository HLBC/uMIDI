using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FruitScript : MonoBehaviour
{
    private AudioSource soundSource;

    void Start()
    {
        soundSource = GetComponent<AudioSource>();
    }

    public void reactToPlayerCollision()
    {
        Debug.Log("Player Collision!");
        soundSource.Play();
        Destroy(transform.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSound : MonoBehaviour
{
    [SerializeField] private GameObject sound;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !other.isTrigger)
            {
            sound.SetActive(false);
           
            sound.SetActive(true);

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

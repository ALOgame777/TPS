using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CineTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            CinemaManager.instance.StartCineMachine();
            gameObject.SetActive(false);
        }
    }
}

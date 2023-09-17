using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("Hit floor");
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventDispatcher : MonoBehaviour
{
    public void ActivateHitBox()
    {
        Debug.Log(gameObject.name + "AnimationEventDispatcher : ActivateHitBox() : Activate hit box");
    }

    public void DeactivateHitBox() 
    {
        Debug.Log(gameObject.name + "AnimationEventDispatcher : DeactivateHitBox() : Deactivate hit box");
    }
}

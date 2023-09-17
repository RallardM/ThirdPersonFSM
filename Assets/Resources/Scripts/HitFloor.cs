using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HitFloor : MonoBehaviour
{
    private Coroutine m_damageCoroutine;
    private const int LAVA_DAMAGE = 10;
    private const float TIME_UNTIL_DAMAGE = 1.0f;
    private bool m_isCoroutineRunning = false;

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("Enter floor : " + other.name);
        if (other.name != "MainCharacter")
        {
            return;
        }

        // Delay the damage to the player so that it doesn't take damage every frame
        if (!m_isCoroutineRunning)
        {
            //Debug.Log("Start coroutine.");
            //Source : https://youtu.be/ACDZ3W-stCE
            m_damageCoroutine = StartCoroutine(DamageAfterDelay(other.GetComponentInChildren<CharacterControllerStateMachine>()));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name != "MainCharacter")
        {
            return;
        }

        //Debug.Log("Exit floor : " + other.name);
        if (m_isCoroutineRunning)
        {
            // Source : https://discussions.unity.com/t/how-to-stop-a-co-routine-in-c-instantly/49118/4
            StopCoroutine(m_damageCoroutine);
            m_isCoroutineRunning = false;
        }
    }

    private IEnumerator DamageAfterDelay(CharacterControllerStateMachine characterControllerStateMachine)
    {
        //Debug.Log("DamageAfterDelay()");
        m_isCoroutineRunning = true;
        yield return new WaitForSeconds(TIME_UNTIL_DAMAGE);
        characterControllerStateMachine.TakeDamage(LAVA_DAMAGE);
        m_isCoroutineRunning = false;
    }
}

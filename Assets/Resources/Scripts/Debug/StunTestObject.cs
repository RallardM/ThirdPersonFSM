using System.Collections;
using UnityEngine;

public class StunTestObject : MonoBehaviour
{
    private Coroutine m_stunCoroutine;
    private const float TIME_UNTIL_DAMAGE = 1.0f;
    private bool m_isCoroutineRunning = false;

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("Enter floor : " + other.name);
        if (other.name != "MainCharacter")
        {
            return;
        }

        Debug.Log("Enter floor : " + other.name);
        // Delay the damage to the player so that it doesn't take damage every frame
        if (!m_isCoroutineRunning)
        {
            //Debug.Log("Start coroutine.");
            //Source : https://youtu.be/ACDZ3W-stCE
            m_stunCoroutine = StartCoroutine(DamageAfterDelay(other.GetComponentInChildren<CharacterControllerStateMachine>()));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enter floor : " + other.name);
        if (other.name != "MainCharacter")
        {
            return;
        }

        Debug.Log("Enter floor : " + other.name);
        // Delay the damage to the player so that it doesn't take damage every frame
        if (!m_isCoroutineRunning)
        {
            //Debug.Log("Start coroutine.");
            //Source : https://youtu.be/ACDZ3W-stCE
            m_stunCoroutine = StartCoroutine(DamageAfterDelay(other.GetComponentInChildren<CharacterControllerStateMachine>()));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name != "MainCharacter")
        {
            return;
        }

        Debug.Log("Exit floor : " + other.name);
        if (m_isCoroutineRunning)
        {
            // Source : https://discussions.unity.com/t/how-to-stop-a-co-routine-in-c-instantly/49118/4
            StopCoroutine(m_stunCoroutine);
            m_isCoroutineRunning = false;
        }
    }

    private IEnumerator DamageAfterDelay(CharacterControllerStateMachine characterControllerStateMachine)
    {
        //Debug.Log("DamageAfterDelay()");
        m_isCoroutineRunning = true;
        yield return new WaitForSeconds(TIME_UNTIL_DAMAGE);
        if (characterControllerStateMachine.IsStunned == false)
        {
            characterControllerStateMachine.GetStun();
        }
        
        m_isCoroutineRunning = false;
    }
}

using UnityEngine;

public class PlayerDetectionAI : MonoBehaviour
{
    [SerializeField]
    private Animator m_enemyAnimator;

    [SerializeField]
    private Transform m_playerTransform;

    private bool m_isAttacking = false;

    private void Update()
    {
        if (m_isAttacking == false)
        {
            return;
        }

        transform.LookAt(m_playerTransform.position);
        m_enemyAnimator.SetTrigger("Attack");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detects : "+ other.name);
        // Early return if not layer 6 (Player)
        if (other.gameObject.layer != 6)
        {
            Debug.Log("Not player");
            return;
        }

        Debug.Log("Is player! Attack!");
        m_isAttacking = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 6)
        {
            Debug.Log("Not player");
            return;
        }

        m_isAttacking = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != 6)
        {
            Debug.Log("Not player");
            return;
        }

        m_isAttacking = false;
    }
}

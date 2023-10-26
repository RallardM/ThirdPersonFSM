using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventDispatcher : MonoBehaviour
{
    private EnemyController m_enemyController;

    private void Awake()
    {
        m_enemyController = GetComponent<EnemyController>();
    }

    public void ActivateHitBox()
    {
        m_enemyController.OnEnableAttackHitBox(true);
    }

    public void DeactivateHitBox() 
    {
        m_enemyController.OnEnableAttackHitBox(false);
    }
}

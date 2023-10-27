using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventDispatcher : MonoBehaviour
{
    private EnemyController m_enemyController;

    [SerializeField]
    private CharacterAudioController m_audioController;

    private void Awake()
    {
        m_enemyController = GetComponent<EnemyController>();
    }

    public void ActivateHitBox()
    {
        m_enemyController.OnEnableAttackHitBox(true);
        m_audioController.PlaySound(ESoundType.Grunt);
    }

    public void DeactivateHitBox() 
    {
        m_enemyController.OnEnableAttackHitBox(false);
    }
}

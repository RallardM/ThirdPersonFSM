using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    protected GameObject m_hitBox;

    public void OnEnableAttackHitBox(bool isEnable = true)
    {
        //Debug.Log(gameObject.name + "GameManagerSM : OnEnableAttack() : HitBox is enabled : " + isEnable);
        m_hitBox.SetActive(isEnable);
    }
}

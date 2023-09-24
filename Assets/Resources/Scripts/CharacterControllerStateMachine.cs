using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerStateMachine : MonoBehaviour
{
    public Camera Camera { get; private set; }
    [field: SerializeField]
    public Rigidbody RB { get; private set; }
    [field: SerializeField]
    private Animator Animator { get; set; }

    [field: SerializeField]
    public float AccelerationValue { get; private set; }
    [field: SerializeField]
    public float MaxVelocity { get; private set; }
    [field: SerializeField]
    public float SideVelocity { get; private set; }
    [field: SerializeField]
    public float JumpIntensity { get; private set; } = 1000.0f;
    [field: SerializeField]
    private CharacterFloorTrigger m_floorTrigger;

    private CharacterState m_currentState;
    private List<CharacterState> m_possibleStates;

    public int Health { get; private set; } = 100;
    public int PreviousHealth { get; private set; } = 100;

    private void Awake()
    {
        m_possibleStates = new List<CharacterState>();
        m_possibleStates.Add(new FreeState());
        m_possibleStates.Add(new JumpState());
        m_possibleStates.Add(new GettingHitState());
        m_possibleStates.Add(new AttackState());
        m_possibleStates.Add(new FallingState());

    }

    // Start is called before the first frame update
    void Start()
    {
        Camera = Camera.main;

        foreach (CharacterState state in m_possibleStates)
        {
            state.OnStart(this);
        }
        m_currentState = m_possibleStates[0];
        m_currentState.OnEnter();
    }

    private void Update()
    {
        m_currentState.OnUpdate();
        TryStateTransition();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_currentState.OnFixedUpdate();
    }

    private void TryStateTransition()
    {
        if (!m_currentState.CanExit())
        {
            return;
        }

        foreach (var state in m_possibleStates)
        {
            if (m_currentState == state)
            {
                continue;
            }

            if (state.CanEnter(state))
            {
                // Quit the current state
                m_currentState.OnExit();
                m_currentState = state;
                // Enter the new current state
                m_currentState.OnEnter();
                return;
            }
        }
    }

    public bool IsInContactWithFloor()
    {
        return m_floorTrigger.IsOnFloor;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void UpdateHeatlh()
    {
        PreviousHealth = Health;
    }

    private void Die()
    { 
        // TODO: Implement death animation
    }

    public void UpdateAnimatorMovements(Vector3 movementValue)
    {
        // Convert the movement vector to local space relative to the character's transform
        // Source : https://stackoverflow.com/questions/71580880/unintended-player-movement-from-transform-inversetransformdirection
        movementValue = RB.transform.InverseTransformDirection(movementValue);

        // Project the movement vector onto the horizontal plane and normalize it
        Vector3 vectOnFloor = Vector3.ProjectOnPlane(movementValue, Vector3.up);
        vectOnFloor.Normalize();

        // Set the y component to zero to ignore vertical movement
        movementValue = new Vector3(vectOnFloor.x, 0, vectOnFloor.z);

        // Set the animator values
        Animator.SetFloat("MoveX", movementValue.x);
        Animator.SetFloat("MoveY", movementValue.z);
    }

    public void UpdateAnimation(CharacterState state)
    {
        // Source : https://discussions.unity.com/t/how-can-i-check-if-an-animation-is-playing-or-has-finished-using-animator-c/57888/5
        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);

        CharacterState hitState = state as GettingHitState;
        if (hitState != null)
        {
            if (stateInfo.IsName("GetHit") && stateInfo.normalizedTime < 1.0f)
            {
                // If the animation is already playing, restart it
                Animator.Play("GetHit", 0, 0f);
            }
            else if (!stateInfo.IsName("GetHit"))
            {
                // Start the animation if it's not already playing
                Animator.SetTrigger("GetHit");
            }
        }

        CharacterState attackState = state as AttackState;
        if (attackState != null)
        {
            bool isAttackAnim = stateInfo.IsName("AttackState");
            bool isPlaying = stateInfo.normalizedTime < 1.0f;
            //Debug.Log("Is attack anim : " + isAttackAnim + " Is playing : " + isPlaying);

            if (isAttackAnim && isPlaying)
            {
                //Debug.Log("Restarting attack animation");

                // If the animation is already playing, restart it
                Animator.Play("AttackState", 0, 0f);
                Animator.SetTrigger("Attack");
            }
            else
            {
                //Debug.Log("Start attack animation");

                // Start the animation if it's not already playing
                Animator.SetTrigger("Attack");
            }
        }

        CharacterState fallState = state as FallingState;
        if (fallState != null)
        {
            bool isFallAnim = stateInfo.IsName("InAir");
            bool isPlaying = stateInfo.normalizedTime < 1.0f;

            if (isFallAnim && isPlaying)
            {
                Debug.Log("Restarting fall animation");

                // If the animation is already playing, stop it
                Animator.Play("InAir", 0, 0f);

            }
            else
            {
                Debug.Log("Start fall animation");

                // Start the animation if it's not already playing
                Animator.SetTrigger("Fall");
            }
        }

        CharacterState jumpState = state as JumpState;
        if (jumpState != null)
        {
            // If in one of the two jumping states
            if (Animator.GetBool("IsJumping"))
            {
                Debug.Log("Inform that the jump anim ended");

                // Inform the animator that the player is not jumping anymore
                Animator.SetBool("IsJumping", false);

            }
            else
            {
                Debug.Log("Start jump animation");

                // Start the animation if Jump state animation not already playing
                Animator.SetTrigger("Jump");

                // Inform the animator that the player is jumping
                Animator.SetBool("IsJumping", true);
            }
        }
    }
}
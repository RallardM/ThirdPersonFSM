using System.Collections.Generic;
using System.Xml;
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
    public float JumpIntensity { get; private set; } = 1000.0f;
    [field: SerializeField]
    private CharacterFloorTrigger m_floorTrigger;

    private CharacterState m_currentState;
    private List<CharacterState> m_possibleStates;
    public int Health { get; private set; } = 100;
    public int PreviousHealth { get; private set; } = 100;
    public bool IsStunned { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsKeyPressed { get; private set; }

    private void Awake()
    {
        m_possibleStates = new List<CharacterState>();
        m_possibleStates.Add(new FreeState());
        m_possibleStates.Add(new JumpState());
        m_possibleStates.Add(new GettingHitState());
        m_possibleStates.Add(new AttackState());
        m_possibleStates.Add(new FallingState());
        m_possibleStates.Add(new StunnedState());
        m_possibleStates.Add(new DeadState());
        m_possibleStates.Add(new HitInAir());
        //m_possibleStates.Add(new StunnedInAir()); // TODO

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
        CheckIfKeyPresed();
    }

    private void CheckIfKeyPresed()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Space))
        {
            //Debug.Log("A key is pressed");
            IsKeyPressed = true;
        }
        else
        {
            IsKeyPressed = false;
        }
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

        if (damage > 10)
        {
            Debug.Log("Player is stunned.");
            IsStunned = true;
        }
    }

    public void GetStun()
    {
        Debug.Log("Player got stunned");
        IsStunned = true;
    }

    public void UpdateHeatlh()
    {
        PreviousHealth = Health;
    }

    // Animator access :

    private void Die()
    {
        IsDead = true;
    }

    public void SetTouchGround(bool isOnGround)
    {
        Animator.SetBool("IsTouchingGround", isOnGround);
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
        CharacterState hitInAirState = state as HitInAir;
        if (hitState != null || hitInAirState != null)
        {
            if (stateInfo.IsName("GettingHit") && stateInfo.normalizedTime < 1.0f)
            {
                // If the animation is already playing, restart it
                Animator.Play("GettingHit", 0, 0f);
            }
            else if (!stateInfo.IsName("GettingHit"))
            {
                // Start the animation if it's not already playing
                Animator.SetTrigger("GettingHit");
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
            bool isFalling = Animator.GetBool("IsFalling");
            bool isPlaying = stateInfo.normalizedTime < 1.0f;

            Animator.SetFloat("MoveX", 0.0f);
            Animator.SetFloat("MoveY", 0.0f);

            //if (isFallAnim && isPlaying && isFalling)
            if (isFallAnim && isPlaying && isFalling)
            {
                Debug.Log("Restarting fall animation");

                // If the animation is already playing, restart it
                Animator.Play("InAir", 0, 0f);
                Animator.SetBool("IsFalling", true);

            }
            else if (isFalling == false)
            {
                //Debug.Log("Start fall animation");
                Debug.Log("Start fall animation");
                Animator.SetBool("IsFalling", true);
                // Start the animation if it's not already playing
                //Animator.SetTrigger("Fall");
            }
            else if (isFallAnim == false && isPlaying == false && IsInContactWithFloor() && isFalling)
            {
                Debug.Log("Inform animator fall eneded");
                Animator.SetBool("IsFalling", false);
            }
        }

        CharacterState jumpState = state as JumpState;
        if (jumpState != null)
        {
            Debug.Log("Start jump animation");
            // If not jumping
            // Inform the animator that the player is jumping
            //Animator.SetBool("IsJumping", true);
            Animator.SetTrigger("Jump");
        }

        CharacterState stunnedState = state as StunnedState;
        if (stunnedState != null)
        {
            // If in one of the two jumping states
            if (Animator.GetBool("IsStunned"))
            {
                Debug.Log("Inform that the stunned anim ended");

                // Inform the animator that the player is not stunned anymore
                Animator.SetBool("IsStunned", false);
                IsStunned = false;

            }
            else
            {
                Debug.Log("Start stunned animation");

                // Inform the animator that the player is stunned
                Animator.SetBool("IsStunned", true);

                // Reset Getting hit so that it does not start the hit state after the stun
                Animator.ResetTrigger("GettingHit");
            }
        }

        CharacterState deadState = state as DeadState;
        if (deadState != null)
        {
            Debug.Log("Player died");
            Animator.SetBool("IsStunned", true);
        }
    }
}
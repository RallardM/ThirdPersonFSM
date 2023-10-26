using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class CharacterControllerStateMachine : BaseStateMachine<CharacterState>, IDamageable
{
    public Camera Camera { get; private set; }

    [field: SerializeField]
    public Rigidbody RB { get; private set; }

    [field: SerializeField]
    public Animator Animator { get; set; }

    [field: SerializeField]
    public CharacterAudioController AudioController { get; private set; }

    [field: SerializeField]
    public float AccelerationValue { get; private set; }

    [field: SerializeField]
    public float MaxVelocity { get; private set; }

    [field: SerializeField]
    public float JumpIntensity { get; private set; } = 1000.0f;

    [SerializeField]
    private CharacterFloorTrigger m_floorTrigger;

    [SerializeField]
    protected GameObject m_hitBox;

    public int Health { get; private set; } = 1000;
    public int PreviousHealth { get; private set; } = 1000;
    public bool IsStunned { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsKeyPressed { get; private set; }
    public bool OnHitStimuliReceived { get; set; } = false;
    public bool OnStunStimuliReceived { get; set; } = false;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        foreach (CharacterState state in m_possibleStates)
        {
            state.OnStart(this);
        }

        m_currentState = m_possibleStates[0];
        m_currentState.OnEnter();

        Camera = Camera.main;
    }

    protected override void Update()
    {
        base.Update();
        CheckIfKeyPresed();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void CreatePossibleStates()
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

    public IState GetCurrentState()
    {
        return m_currentState;
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

    public bool IsInContactWithFloor()
    {
        return m_floorTrigger.IsOnFloor;
    }

    public void TakeDamage(int damage) // TODO : Merge with ReceiveDamage
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

    public void ReceiveDamage(EDamageType damageType)
    {
        if (damageType == EDamageType.Normal)
        {
            OnHitStimuliReceived = true;
        }
        if (damageType == EDamageType.Stunning)
        {
            OnStunStimuliReceived = true;
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

    public void UpdateAnimation(IState state)
    {
        // Source : https://discussions.unity.com/t/how-can-i-check-if-an-animation-is-playing-or-has-finished-using-animator-c/57888/5
        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);

        IState hitState = state as GettingHitState;
        IState hitInAirState = state as HitInAir;
        if (hitState != null || hitInAirState != null)
        {
            if (stateInfo.IsName("GettingHitState") && stateInfo.normalizedTime < 1.0f) // TODO rewritte using clipInfo[0].clip.name
            {
                // If the animation is already playing, restart it
                Animator.Play("GettingHitState", 0, 0f);
            }
            else if (!stateInfo.IsName("GettingHitState")) // TODO rewritte using clipInfo[0].clip.name
            {
                // Start the animation if it's not already playing
                Animator.SetTrigger("GettingHitState");
            }
        }

        IState attackState = state as AttackState;
        if (attackState != null)
        {
            int attackId = Animator.StringToHash("AttackState");
            if (attackId == stateInfo.fullPathHash)
            {
                Debug.Log("Attack anim is playing");
            }

            AnimatorClipInfo[] clipInfo;
            int layerIndex = 0;
            clipInfo = Animator.GetCurrentAnimatorClipInfo(layerIndex);
            bool isAttackAnim = clipInfo[0].clip.name == "Attack01";
            bool isPlaying = stateInfo.normalizedTime < 1.0f;

            if (isAttackAnim && isPlaying)
            {
                Debug.Log("Restarting attack animation");

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

        IState fallState = state as FallingState;
        if (fallState != null)
        {
            bool isFallAnim = stateInfo.IsName("InAir"); // TODO rewritte using clipInfo[0].clip.name
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

        IState jumpState = state as JumpState;
        if (jumpState != null)
        {
            Debug.Log("Start jump animation");
            // If not jumping
            // Inform the animator that the player is jumping
            //Animator.SetBool("IsJumping", true);
            Animator.SetTrigger("Jump");
        }

        IState stunnedState = state as StunnedState;
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

        IState deadState = state as DeadState;
        if (deadState != null)
        {
            Debug.Log("Player died");
            Animator.SetBool("IsStunned", true);
        }
    }

    public void OnEnableAttackHitBox(bool isEnable = true)
    {
        Debug.Log(gameObject.name + "GameManagerSM : OnEnableAttack() : HitBox is enabled : " + isEnable);
        m_hitBox.SetActive(isEnable);
    }
}
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Cinemachine;
using System;

public class GameManagerSM : BaseStateMachine<IState>
{
    [field:SerializeField]
    public CharacterControllerStateMachine CharacterControllerStateMachine { get; private set; }

    [field: SerializeField]
    public GameObject MainCharacter { get; set; }

    [field: SerializeField]
    public PlayableDirector IntroTimeline { get; set; }

    [field: SerializeField]
    public GameObject IntroDollyTracks { get; set; }

    [field: SerializeField]
    public GameObject GameplayVirtualCamera { get; set; }

    [SerializeField]
    protected Camera m_mainCamera;

    //Exposed slider 0 to 1 m_timeSlownValue
    [SerializeField]
    [Range(0.0f, 1.0f)] private float m_timeSlownValue = 0.5f;
    private float m_fixedDeltaTime = 0.02f;
    //private float m_maxFixedDeltaTime = 1.0f;


    [SerializeField]
    private AudioSource m_musicTrack;

    private const float BULLET_TIME_DURATION = 1.0f;
    private float m_bulletTimeTimer = 0.0f;
    private bool m_isBulletTimeActive = false;

    public IState DesiredState { get; set; } = null;
    //public bool CanPlayerMove { get; set; } = true;

    private static GameManagerSM s_instance;

    public static GameManagerSM GetInstance()
    {
        if (s_instance == null)
        {
            return new GameManagerSM();
        }

        return s_instance;
    }

    protected override void Awake()
    {
        base.Awake();

        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Debug.LogWarning("GameManagerSM : Awake() - Attempted to create a second instance of the GameManagerSM singleton!");
            Destroy(this);
        }
    }

    // TODO: Try to integrate this into the state machine
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        DontDestroyOnLoad(this);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        GameplayInputs();

        // If no cinemachine virtual camera is active, activate the gameplay virtual camera
        CinemachineBrain brain = CinemachineCore.Instance.GetActiveBrain(0);
        if (brain != null)
        {
            if (brain.ActiveVirtualCamera == null)
            {
                GameplayVirtualCamera.SetActive(true);
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        FixedSlownTime();
    }

    // Source : https://youtu.be/9-5kBGlpwhA
    // Source : https://youtu.be/0VGosgaoTsw
    private void FixedSlownTime()
    {
        if (m_isBulletTimeActive == false)
        {
            Time.timeScale += (1f / BULLET_TIME_DURATION) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            return;
        }

        m_bulletTimeTimer -= Time.fixedDeltaTime;
        if (m_bulletTimeTimer <= 0.0f)
        {
            m_isBulletTimeActive = false;
            Debug.Log("Bullet time deactivated");
        }

        Time.timeScale = m_timeSlownValue;
        Time.fixedDeltaTime = Time.timeScale * m_fixedDeltaTime;
    }

    private void GameplayInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            //UnityEditor.EditorApplication.isPlaying = false;
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            // Teleport player to the beginning of the next level
            MainCharacter.transform.position = new Vector3(0, 0, 0);

            // Cheat code to skip to next level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            CharacterControllerStateMachine.OnGameManagerStateChange(false);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            // Cheat code to reset level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            // Cheat code to pause the music
            if (m_musicTrack.isPlaying)
            {
                m_musicTrack.Pause();
            }
            else
            {
                m_musicTrack.Play();
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // Set Explosion
            VFXManager.GetInstance().SetExplosion();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            // Cancel intro cinematic timeline
            if (IntroTimeline != null)
            {
                IntroTimeline.Stop();
                IntroDollyTracks.SetActive(false);
                GameplayVirtualCamera.SetActive(true);
                DesiredState = m_possibleStates[1];
                CharacterControllerStateMachine.OnGameManagerStateChange(false);
                return;
            }

            // If there is no cinematic intro switch the gameplay state
            CharacterControllerStateMachine.OnGameManagerStateChange(!CharacterControllerStateMachine.InNonGameplayState);
        }
    }

    protected override void CreatePossibleStates()
    {
        Debug.Log("GameManagerSM : CreatePossibleStates()");
        m_possibleStates = new List<IState>();
        m_possibleStates.Add(new CinematicState(m_mainCamera));
        m_possibleStates.Add(new GameplayState(m_mainCamera));
        m_possibleStates.Add(new SceneTransitionState());
    }

    public void ActivateCinematicState()
    {
        Debug.Log("GameManagerSM : ActivateCinematicState()");
        // Set the desired state to the cinematic state
        DesiredState = m_possibleStates[0];
    }

    public void ActivateGameplayState()
    {
        Debug.Log("GameManagerSM : ActivateGameplayState()");
        // Set the desired state to the gameplay state
        DesiredState = m_possibleStates[1];
    }

    public void ActivateBulletTime()
    {
        if (m_isBulletTimeActive == false)
        {
            m_isBulletTimeActive = true;
            m_bulletTimeTimer = BULLET_TIME_DURATION;
        }
    }
}
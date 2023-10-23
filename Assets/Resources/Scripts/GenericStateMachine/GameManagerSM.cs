using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManagerSM : BaseStateMachine<IState>
{
    [SerializeField]
    protected Camera m_gameplayCamera;
    [SerializeField]
    protected Camera m_cinematicCamera;

    private static GameManagerSM s_instance;

    public bool CanPlayerMove { get; set; } = true;

    public static GameManagerSM GetInstance()
    {
        if (s_instance == null)
        {
            return new GameManagerSM();
        }

        return s_instance;
    }

    public GameManagerSM()
    {
        s_instance = this;
    }

    // TODO: Try to integrate this into the state machine
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            //UnityEditor.EditorApplication.isPlaying = false;
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            // Cheat code to skip to next level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    protected override void CreatePossibleStates()
    {
        m_possibleStates = new List<IState>();
        m_possibleStates.Add(new GameplayState(m_gameplayCamera));
        m_possibleStates.Add(new CinematicState(m_cinematicCamera));
        m_possibleStates.Add(new SceneTransitionState());
    }
}
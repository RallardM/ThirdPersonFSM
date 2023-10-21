using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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

    protected override void CreatePossibleStates()
    {
        m_possibleStates = new List<IState>();
        m_possibleStates.Add(new GameplayState(m_gameplayCamera));
        m_possibleStates.Add(new CinematicState(m_cinematicCamera));
    }
}
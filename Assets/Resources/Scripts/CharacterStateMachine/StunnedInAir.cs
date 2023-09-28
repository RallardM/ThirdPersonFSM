using UnityEngine;

public class StunnedInAir : CharacterState
{
    // TODO
    public override void OnEnter()
    {
        //Debug.Log("Enter State: Stunned in air state");
    }

    public override void OnExit()
    {
        //Debug.Log("Exit state: stunned in air state");
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {

    }

    public override bool CanEnter(CharacterState currentState)
    {
        return false;
    }

    public override bool CanExit()
    {
        return false;
    }
}

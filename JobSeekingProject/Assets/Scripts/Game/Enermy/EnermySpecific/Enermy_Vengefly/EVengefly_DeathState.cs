﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EVengefly_DeathState : DeathState
{
    private Enermy_Vengefly enermy;

    public EVengefly_DeathState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeathState stateData, Enermy_Vengefly enermy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enermy = enermy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        enermy.rb.gravityScale = 1;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

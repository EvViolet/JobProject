﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHuskSentry_DeathState : DeathState
{
    private Enermy_HuskSentry enermy;

    public EHuskSentry_DeathState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeathState stateData, Enermy_HuskSentry enermy) : base(entity, stateMachine, animBoolName, stateData)
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

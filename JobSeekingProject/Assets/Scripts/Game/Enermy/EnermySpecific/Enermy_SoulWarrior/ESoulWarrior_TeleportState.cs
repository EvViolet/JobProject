﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESoulWarrior_TeleportState : TeleportOutState
{
    private Enermy_SoulWarrior enermy;

    public ESoulWarrior_TeleportState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_TeleportOutState stateData, Enermy_SoulWarrior enermy) : base(entity, stateMachine, animBoolName, stateData)
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

        if (isTeleportOver)
        {
            stateMachine.ChangeState(enermy.stompState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

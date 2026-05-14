using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class BaseAction
{
    protected ActionContext action_context;
    protected Action<BaseAction> on_action_complete;  

    public ActionState actionState {get; private set;} 
    public BaseAction Parent;
    public readonly List<BaseAction> ChainedActions = new List<BaseAction>();

    public virtual void Execute(Action<BaseAction> OnActionComplete){}

    protected void CompleteAction()
    {
        on_action_complete(this);
    }

    public void SetActionState(ActionState actionState) => this.actionState = actionState;

    public enum ActionState
    {
        Waiting,
        Running,
        Completed,
        Canceled
    }
}

public abstract class BaseAction<Tparameters> : BaseAction
{
    public Tparameters parameters {get; private set;}

    public BaseAction(Tparameters parameters) : base()
    {
        this.parameters = parameters;
    }
}

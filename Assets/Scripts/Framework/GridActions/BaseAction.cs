using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction
{
    protected ActionContext action_context;
    protected Action<BaseAction> on_action_complete;  

    public ActionState actionState {get; private set;} 
    public BaseAction parent;

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
    protected Tparameters parameters {get; private set;}

    public BaseAction(Tparameters parameters, ActionContext context)
    {
        this.parameters = parameters;
        action_context = context;
    }

    public virtual void Execute(Action<BaseAction> OnActionComplete){}
}

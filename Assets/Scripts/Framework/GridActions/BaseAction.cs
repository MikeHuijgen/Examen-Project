using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class BaseAction
{
    protected ActionContext action_context;
    protected Action<BaseAction> on_action_complete;

    public ActionState State { get; private set; }
    public BaseAction Parent;
    public readonly List<BaseAction> ChainedActions = new List<BaseAction>();
    public bool IsCanceled => State == ActionState.Canceled;

    public BaseAction Root
    {
        get
        {
            var current = this;

            while (current.Parent != null)
                current = current.Parent;

            return current;
        }
    }

    public virtual void Execute(Action<BaseAction> OnActionComplete) { }

    public virtual void Cancel()
    {
        if (State == ActionState.Canceled || State == ActionState.Completed) return;

        State = ActionState.Canceled;

        foreach (var child in ChainedActions)
        {
            child.Cancel();
        }
    }

    protected void CompleteAction()
    {
        if (IsCanceled) return;
        State = ActionState.Completed;
        on_action_complete?.Invoke(this);
    }

    public void SetActionState(ActionState actionState) => this.State = actionState;

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
    public Tparameters parameters { get; private set; }

    public BaseAction(Tparameters parameters) : base() => this.parameters = parameters;
}

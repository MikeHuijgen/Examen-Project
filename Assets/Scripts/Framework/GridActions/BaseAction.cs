using System;
using System.Threading.Tasks;

public abstract class BaseAction
{
    protected ActionContext action_context;
    protected Action on_action_complete;   

    protected void CompleteAction()
    {
        on_action_complete();
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

    public void Execute(GridActionProcessor processor)
    {
        processor.ProcessAction(this);
    }

    public virtual void Execute(Action OnActionComplete){}
}

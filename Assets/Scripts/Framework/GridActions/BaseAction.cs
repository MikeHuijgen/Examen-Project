using System.Threading.Tasks;
public abstract class BaseAction<Tparameters>
{
    protected Tparameters parameters {get; private set;}
    public BaseAction(Tparameters parameters)
    {
        this.parameters = parameters;
    }

    public virtual async Task Execute(){}
}

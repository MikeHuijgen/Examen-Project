using System.Threading.Tasks;
using DG.Tweening;

public class SwapAction : BaseAction<SwapActionParameters>
{
    public SwapAction(SwapActionParameters parameters) : base(parameters){}

    public override async Task Execute()
    {
        var from = parameters.from;
        var to = parameters.to;
        var GetWorldPositionCallback = parameters.GetWorldPositionCallback;

        var tweenSwapSpeed = parameters.tweenSwapSpeed;

        parameters.dataSwapCallback(from, to);
        await parameters.visualSwapCallback(from, to, GetWorldPositionCallback, tweenSwapSpeed, Ease.InOutQuad);
    }
}

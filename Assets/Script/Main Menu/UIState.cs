using System.Collections;
using UnityEngine;

public class UIState : IStateUI
{
    private Animator stateAnimator;
    private string enterTrigger;
    private string enterAnimationName;
    private string exitTrigger;
    private string exitAnimationName;
    public UIState(Animator stateAnimator, string enterTrigger, string enterAnimationName, string exitTrigger, string exitAnimationName)
    {
        this.stateAnimator = stateAnimator;
        this.enterTrigger = enterTrigger;
        this.enterAnimationName = enterAnimationName;
        this.exitTrigger = exitTrigger;
        this.exitAnimationName = exitAnimationName;
    }
    public IEnumerator PlayEnterAnimation()
    {
        yield return CanvasAnimator.WaitForAnimation(stateAnimator, enterAnimationName, enterTrigger);
    }

    public IEnumerator PlayExitAnimation()
    {
        yield return CanvasAnimator.WaitForAnimation(stateAnimator, exitAnimationName, exitTrigger);
    }
}

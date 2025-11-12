using System.Collections;
using UnityEngine;

public static class CanvasAnimator
{
    public static IEnumerator WaitForAnimation(Animator animator, string animationName, string animationTrigger)
    {
        animator.SetTrigger(animationTrigger);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }
}

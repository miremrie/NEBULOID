using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmAnimationController : MonoBehaviour
{
    public Animator LeftArmAnimator, RightArmAnimator;
    private const string moveAnimation = "ArmMove";

    public void AnimateLeftArm()
    {
        LeftArmAnimator.SetTrigger(moveAnimation);
    }

    public void AnimateRightArm()
    {
        RightArmAnimator.SetTrigger(moveAnimation);
    }
}

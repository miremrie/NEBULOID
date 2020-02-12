using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionControl : MonoBehaviour
{
    public InputControl actionControl;

    public virtual void DoAction(InputController controller)
    {
    }

    public virtual void OnExitAction(InputController controller)
    {
    }
}

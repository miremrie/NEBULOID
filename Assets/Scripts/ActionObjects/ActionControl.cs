using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionControl : MonoBehaviour
{
    public ButtonInputControl actionControl;

    public virtual void DoAction(Char2Controller controller)
    {
    }

    public virtual void OnExitAction(Char2Controller controller)
    {
    }
}

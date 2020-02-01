﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionObject : MonoBehaviour
{
    public InputControl actionControl;
    private bool actionObjectReady = true;

    public bool IsActionObjectReady()
    {
        return actionObjectReady;
    }
    public virtual void DoAction(InputController controller)
    {

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler
{

    private string horMovementKey;
    private string verMovementKey;
    private string actionKey;
    private string subActionKey;
    private string escapeKey;
    //private float deadZone = 0.1f;
    public InputHandler()
    {

    }
    public InputHandler(string horMovement, string verMovement, string action, string subAction)
    {
        horMovementKey = horMovement;
        verMovementKey = verMovement;
        actionKey = action;
        subActionKey = subAction;
    }

    public virtual int GetHorizontal()
    {
        int movement = (int)Input.GetAxisRaw(horMovementKey);
        return movement;
    }

    public virtual int GetVertical()
    {
        int movement = (int)Input.GetAxisRaw(verMovementKey);
        return movement;
    }

    public bool GetControlPressed(InputControl actionButton)
    {

        switch (actionButton)
        {
            case (InputControl.Action):
                return GetActionPressed();
            case (InputControl.Down):
                return GetDownActionPressed();
            case (InputControl.Up):
                return GetUpActionPressed();
            case (InputControl.SubAction):
                return GetSubActionPressed();
            default:
                return false;
        }
    }

    public bool GetUpActionPressed()
    {
        return Input.GetAxisRaw(verMovementKey) == 1;
    }
    public bool GetDownActionPressed()
    {
        return Input.GetAxisRaw(verMovementKey) == -1;
    }
    public bool GetActionPressed()
    {
        return Input.GetButtonDown(actionKey);
    }
    public bool GetSubActionPressed()
    {
        return Input.GetButtonDown(subActionKey);
    }
    public bool GetEscapeKeyPressed()
    {
        return Input.GetButtonDown(escapeKey);
    }


    public static InputHandler RegisterController(int gamepadNumber)
    {
        InputHandler handler = new InputHandler();
        handler.actionKey = "Action" + gamepadNumber.ToString();
        handler.subActionKey = "SubAction" + gamepadNumber.ToString();
        handler.horMovementKey = "Hor" + gamepadNumber.ToString();
        handler.verMovementKey = "Ver" + gamepadNumber.ToString();
        handler.escapeKey = "EscAction" + gamepadNumber.ToString();
        return handler;
    }
}

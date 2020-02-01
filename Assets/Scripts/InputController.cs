using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputControl
{
    Up, Down, Left, Right, Action
}

public class InputController : MonoBehaviour
{
    public int playerNumber;
    public bool playerActive;
    public int gamepadNumber;
    public float speed;
    private InputHandler handler;
    private bool nearActionObject = false;
    private ActionObject actionObject;
    private Vector3 movementDestination;
    private bool movingTowardsDestination;
    public float destinationReachedDistance = 0.01f;
    public float destMoveSpeed = 5f;
    private bool lockedForActions = false;

    private void Awake()
    {
        ActivatePlayer();
    }

    public void ActivatePlayer()
    {
        playerActive = true;
        RegisterController();
    }

    private void Update()
    {
        if (playerActive)
        {
            if (!lockedForActions)
            {
                if (!movingTowardsDestination)
                {
                    MoveHor();
                }
                ExecuteAction();
            }
            MoveTowardsDestination();
        }
    }

    private void RegisterController()
    {
        handler = new InputHandler("Hor" + gamepadNumber.ToString(), "Ver" + gamepadNumber.ToString(), "Action" + gamepadNumber.ToString());
    }

    private void MoveHor()
    {
        transform.Translate(Vector3.right * handler.GetMovement() * speed * Time.deltaTime);
    }

    private void ExecuteAction()
    {
        if (nearActionObject && actionObject.IsActionObjectReady() && handler.GetControlPressed(actionObject.actionControl))
        {
            actionObject.DoAction(this);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "ActionObject")
        {
            nearActionObject = true;
            actionObject = col.gameObject.GetComponent<ActionObject>();
        }
    } 

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "ActionObject")
        {

            ActionObject curActionObject = col.gameObject.GetComponent<ActionObject>();
            if (curActionObject == actionObject)
            {
                nearActionObject = false;
                actionObject = null;
            }
        }
    }

    public void MoveUpTo(Transform dest)
    {
        //anim stuff
        lockedForActions = true;
        movingTowardsDestination = true;
        movementDestination = dest.position;
    }

    private void MoveTowardsDestination()
    {
        if (movingTowardsDestination)
        {
            if (Vector3.Distance(movementDestination, transform.position) <= destinationReachedDistance)
            {
                transform.position = movementDestination;
                movingTowardsDestination = false;
                lockedForActions = false;
            } else
            {
                transform.Translate((movementDestination - transform.position).normalized * destMoveSpeed * Time.deltaTime);
            }

        }

    }

}

public class InputHandler
{

    private string horMovementKey;
    private string verMovementKey;
    private string actionKey;
    //private float deadZone = 0.1f;

    public InputHandler(string horMovement, string verMovement, string action)
    {
        horMovementKey = horMovement;
        verMovementKey = verMovement;
        actionKey = action;
    }

    public virtual int GetMovement()
    {

        return (int)Input.GetAxisRaw(horMovementKey);
        /*int movement = 0;
        float tmpMove = Input.GetAxis(horMovementKey);

        if (horMovementKey == "Hor1")
        {
            Debug.Log(tmpMove);
        }
        if (tmpMove > deadZone)
        {
            movement++;
        }
        if (tmpMove < -deadZone)
        {
            movement--;
        }
        return movement;*/
    }

    public bool GetControlPressed(InputControl actionButton)
    {
        switch (actionButton)
        {
            case (InputControl.Action):
                return GetActionPressed();
            case (InputControl.Down):
                return GetMoveDownActionPressed();
            case (InputControl.Up):
                return GetMoveUpActionPressed();
            default:
                return false;
        }
    }

    public bool GetMoveUpActionPressed()
    {
        return Input.GetAxisRaw(verMovementKey) == 1;
    }
    public bool GetMoveDownActionPressed()
    {
        return Input.GetAxisRaw(verMovementKey) == -1;
    }
    public bool GetActionPressed()
    {
        return Input.GetButtonDown(actionKey);
    }
}

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
    private ShipActionControl actionObject;
    private Transform movementDestination;
    private bool movingTowardsDestination;
    public float destinationReachedDistance = 0.01f;
    public float destMoveSpeed = 5f;
    private bool lockedForActions = false;
    public Animator animator;
    public SpriteRenderer charSpriteRenderer;
    private const string moveAnimName = "Move";
    private const string moveVerAnimName = "MoveVer";
    private int curMovementDirection = 0;
    public int altInput = -1;

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
        curMovementDirection = 0;
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
        AnimateCharacter();
        CheckForSecondInput();
    }

    private void RegisterController()
    {
        handler = new InputHandler("Hor" + gamepadNumber.ToString(), "Ver" + gamepadNumber.ToString(), "Action" + gamepadNumber.ToString());
    }

    private void MoveHor()
    {
        curMovementDirection = handler.GetMovement();
        transform.Translate(Vector3.right * curMovementDirection * speed * Time.deltaTime);
    }

    private void ExecuteAction()
    {
        if (nearActionObject 
            && actionObject != null 
            && actionObject.IsActionObjectReady() 
            && handler.GetControlPressed(actionObject.actionControl))
        {
            actionObject.DoAction(this);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == Tags.ACTION_OBJECT)
        {
            nearActionObject = true;
            actionObject = col.gameObject.GetComponent<ShipActionControl>();
        }
    }

    //void OnTriggerStay2D(Collider2D col)
    //{
    //    if (col.tag == Tags.ACTION_OBJECT)
    //    {
    //        Debug.Log($"Staying in {col.name}");
    //    }
    //}

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == Tags.ACTION_OBJECT)
        {

            ShipActionControl curActionObject = col.gameObject.GetComponent<ShipActionControl>();
            if (curActionObject == actionObject)
            {
                curActionObject.OnExitAction();
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
        movementDestination = dest;
    }

    private void MoveTowardsDestination()
    {
        if (movingTowardsDestination)
        {
            if (Vector3.Distance(movementDestination.position, transform.position) <= destinationReachedDistance)
            {
                transform.position = movementDestination.position;
                movingTowardsDestination = false;
                lockedForActions = false;
            } else
            {
                transform.Translate((movementDestination.position - transform.position).normalized * destMoveSpeed * Time.deltaTime);
            }

        }

    }

    private void AnimateCharacter()
    {
        animator.SetBool(moveAnimName, curMovementDirection != 0);
        if (curMovementDirection != 0)
        {
            charSpriteRenderer.flipX = curMovementDirection > 0;
        }
        animator.SetBool(moveVerAnimName, movingTowardsDestination);
    }

    private void CheckForSecondInput()
    {
        if (altInput != -1 && Input.GetButtonDown("Action" + altInput.ToString())) {
            int tmp = altInput;
            altInput = gamepadNumber;
            gamepadNumber = tmp;
            RegisterController();
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
        int movement = (int)Input.GetAxisRaw(horMovementKey);
        return movement;
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

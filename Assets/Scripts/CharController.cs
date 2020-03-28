using CustomInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonInputControl
{
    Up, Down, Left, Right, Action, SubAction
}

public class Char2Controller : MonoBehaviour
{
    public Game game;
    public CharacterInput charInput;
    public int playerNumber;
    [Header("Input")]
    public int gamepadNumber;
    public int altInput = -1;
    private InputHandler handler;

    [Header("Movement")]
    public float speed;
    private Transform fixedMoveDst;
    private bool isFixedMoving;
    public float fixedMoveDstMinOffset = 0.01f;
    public float fixedMoveSpeed = 5f;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer charSpriteRenderer;
    private const string moveAnimName = "Move";
    private const string moveVerAnimName = "MoveVer";
    private int curMovementDirection = 0;

    private bool lockedForActions = false;
    private Dictionary<ButtonInputControl, ActionControl> currentActions = new Dictionary<ButtonInputControl, ActionControl>();

    private void Awake()
    {
        ActivatePlayer();
    }

    public void ActivatePlayer()
    {
        RegisterController();
    }

    private void Update()
    {
        if (handler.GetEscapeKeyPressed())
        {
            game.GoBackToMenu();
        }
        curMovementDirection = 0;
        if (!lockedForActions)
        {
            if (!isFixedMoving)
            {
                MoveHor();
            }
            ExecuteAction();
        }
        MoveTowardsDestination();
        AnimateMovement();
        CheckForSecondInput();
    }

    private void MoveHor()
    {
        curMovementDirection = handler.GetHorizontal();
        transform.Translate(Vector3.right * curMovementDirection * speed * Time.deltaTime);
    }

    private void ExecuteAction()
    {
        foreach (KeyValuePair<ButtonInputControl, ActionControl> action in currentActions)
        {
            if (action.Value != null && handler.GetControlPressed(action.Key))
            {
                action.Value.DoAction(this);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == Tags.ACTION_OBJECT)
        {

            ActionControl newControl = col.gameObject.GetComponent<ActionControl>();

            if (currentActions.ContainsKey(newControl.actionControl))
            {
                currentActions[newControl.actionControl] = newControl;
            } else
            {
                currentActions.Add(newControl.actionControl, newControl);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == Tags.ACTION_OBJECT)
        {
            ActionControl leavingActionControl = col.gameObject.GetComponent<ActionControl>();
            if (currentActions.ContainsKey(leavingActionControl.actionControl))
            {
                leavingActionControl.OnExitAction(this);
                currentActions.Remove(leavingActionControl.actionControl);
            }
        }
    }

    public void FixedMoveTo(Transform dest)
    {
        lockedForActions = true;
        isFixedMoving = true;
        fixedMoveDst = dest;
    }

    private void MoveTowardsDestination()
    {
        if (isFixedMoving)
        {
            if (Vector3.Distance(fixedMoveDst.position, transform.position) <= fixedMoveDstMinOffset)
            {
                transform.position = fixedMoveDst.position;
                isFixedMoving = false;
                lockedForActions = false;
            } else
            {
                transform.Translate((fixedMoveDst.position - transform.position).normalized * fixedMoveSpeed * Time.deltaTime);
            }

        }

    }

    private void AnimateMovement()
    {
        animator.SetBool(moveAnimName, curMovementDirection != 0);
        if (curMovementDirection != 0)
        {
            charSpriteRenderer.flipX = curMovementDirection > 0;
        }
        animator.SetBool(moveVerAnimName, isFixedMoving);
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

    private void RegisterController()
    {
        handler = InputHandler.RegisterController(gamepadNumber);
    }

}
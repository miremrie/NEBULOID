using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NBLD.Input
{


    public class InputRecorder : MonoBehaviour
    {
        public bool record = false;
        public int playerIndex;
        public InputRecordingSession recordingSession;
        private PlayerGameplayInputManager gameplayInput;
        private bool initialized = false;
        private bool subscribed = false;
        private uint updateCounter = 0;
        private uint fixedUpdateCounter = 0;
        #region Action history

        #endregion
        private void Start()
        {
            if (InputManager.Instance != null && InputManager.Instance.Initialized)
            {
                Initialize();
            }
        }
        private void Initialize()
        {
            initialized = true;
            if (InputManager.Instance.GetPlayerCount() > playerIndex)
            {
                var psdata = InputManager.Instance.GetPlayerSessionData(playerIndex);
                gameplayInput = psdata.gameplayInputManager;
                Subscribe();
            }
            else
            {
                InputManager.Instance.OnPlayerRegistered += OnPlayerRegistered;
            }


        }

        private void OnPlayerRegistered(PlayerSessionData playerSessionData)
        {
            if (playerSessionData.playerIndex == playerIndex)
            {
                var psdata = InputManager.Instance.GetPlayerSessionData(playerIndex);
                gameplayInput = psdata.gameplayInputManager;
                InputManager.Instance.OnPlayerRegistered -= OnPlayerRegistered;
                Subscribe();
            }
        }

        private void Subscribe()
        {
            if (initialized && record && gameplayInput != null && !subscribed)
            {
                subscribed = true;
                gameplayInput.OnMove += OnMove;
                gameplayInput.OnAction += OnAction;
                gameplayInput.OnSubAction += OnSubAction;
                gameplayInput.OnMoveAssistPerformed += OnMoveAssistPerformed;
                gameplayInput.OnMoveAssistStarted += OnMoveAssistStarted;
                gameplayInput.OnMoveHorizontal += OnMoveHorizontal;
                gameplayInput.OnMoveVertical += OnMoveVertical;
                gameplayInput.OnUp += OnUp;
                gameplayInput.OnDown += OnDown;
                gameplayInput.OnTalk += OnTalk;
            }
        }



        private void Unsubscribe()
        {
            if (initialized && subscribed)
            {
                subscribed = false;
                gameplayInput.OnMove -= OnMove;
                gameplayInput.OnAction -= OnAction;
                gameplayInput.OnSubAction -= OnSubAction;
                gameplayInput.OnMoveAssistPerformed -= OnMoveAssistPerformed;
                gameplayInput.OnMoveAssistStarted -= OnMoveAssistStarted;
                gameplayInput.OnMoveHorizontal -= OnMoveHorizontal;
                gameplayInput.OnMoveVertical -= OnMoveVertical;
                gameplayInput.OnUp -= OnUp;
                gameplayInput.OnDown -= OnDown;
                gameplayInput.OnTalk -= OnTalk;
            }
        }
        private void OnDisable()
        {
            Unsubscribe();
        }
        private void Update()
        {
            updateCounter++;
        }
        private void FixedUpdate()
        {
            fixedUpdateCounter++;
        }

        #region Input Events
        private void OnMove(Vector2 nav)
        {
            recordingSession.moveHistory.Add(new NavigationHistoryEntry(nav, GetTime(), GetFrameNumber()));
        }
        private void OnTalk()
        {
            recordingSession.talkButtonHistory.Add(new ButtonHistoryEntry(true, GetTime(), GetFrameNumber()));
        }

        private void OnDown()
        {
            recordingSession.downButtonHistory.Add(new ButtonHistoryEntry(true, GetTime(), GetFrameNumber()));
        }

        private void OnUp()
        {
            recordingSession.upButtonHistory.Add(new ButtonHistoryEntry(true, GetTime(), GetFrameNumber()));
        }

        private void OnMoveVertical(float value)
        {
            recordingSession.verticalMoveHistory.Add(new FloatHistoryEntry(value, GetTime(), GetFrameNumber()));
        }

        private void OnMoveHorizontal(float value)
        {
            recordingSession.horizontalMoveHistory.Add(new FloatHistoryEntry(value, GetTime(), GetFrameNumber()));
        }

        private void OnMoveAssistStarted()
        {
            recordingSession.moveAssistStartedHistory.Add(new ButtonHistoryEntry(true, GetTime(), GetFrameNumber()));
        }

        private void OnMoveAssistPerformed()
        {
            recordingSession.moveAssistPerformedHistory.Add(new ButtonHistoryEntry(true, GetTime(), GetFrameNumber()));
        }

        private void OnSubAction()
        {
            recordingSession.subActionButtonHistory.Add(new ButtonHistoryEntry(true, GetTime(), GetFrameNumber()));
        }

        private void OnAction()
        {
            recordingSession.actionButtonHistory.Add(new ButtonHistoryEntry(true, GetTime(), GetFrameNumber()));
        }
        #endregion

        private float GetTime()
        {
            return InputSystem.settings.updateMode == InputSettings.UpdateMode.ProcessEventsInDynamicUpdate ? Time.time : Time.fixedTime;
        }
        private uint GetFrameNumber()
        {
            return InputSystem.settings.updateMode == InputSettings.UpdateMode.ProcessEventsInDynamicUpdate ? updateCounter : fixedUpdateCounter;

        }
    }
}


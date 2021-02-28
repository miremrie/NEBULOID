using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NBLD.Input
{
    public class RecordedInputPlayer : MonoBehaviour
    {
        public InputRecordingSession session;
        public GameplayInputEvents inputEvents = new GameplayInputEvents();
        public bool playByFrame = true;
        private bool initialized = true;
        private uint updateCounter = 0;
        private uint fixedUpdateCounter = 0;
        private int nextMoveEventIndex, nextMoveHorEventIndex, nextMoveVerEventIndex;
        private int nextActionEventIndex, nextSubActionEventIndex, nextUpEventIndex, nextDownEventIndex;
        private int nextMoveAssistStartedEventIndex, nextMoveAssistPerformedEventIndex, nextTalkEventIndex;
        private void Awake()
        {
            nextMoveEventIndex = 0;
            nextMoveHorEventIndex = 0;
            nextMoveVerEventIndex = 0;
            nextActionEventIndex = 0;
            nextSubActionEventIndex = 0;
            nextUpEventIndex = 0;
            nextDownEventIndex = 0;
            nextMoveAssistStartedEventIndex = 0;
            nextMoveAssistPerformedEventIndex = 0;
            nextTalkEventIndex = 0;
        }
        private void Update()
        {
            if (session.updateMode == InputSettings.UpdateMode.ProcessEventsInDynamicUpdate)
            {
                UpdateInput(Time.time, updateCounter);
            }
            updateCounter++;
        }
        private void FixedUpdate()
        {
            if (session.updateMode == InputSettings.UpdateMode.ProcessEventsInFixedUpdate)
            {
                UpdateInput(Time.fixedTime, fixedUpdateCounter);
            }
            fixedUpdateCounter++;
        }
        private void UpdateInput(float time, uint frame)
        {
            if (session.moveHistory.Count > nextMoveEventIndex && ShouldEventBePlayed(session.moveHistory[nextMoveEventIndex], time, frame))
            {
                inputEvents.RaiseMove(session.moveHistory[nextMoveEventIndex].value);
                nextMoveEventIndex++;
            }
            if (session.verticalMoveHistory.Count > nextMoveVerEventIndex && ShouldEventBePlayed(session.verticalMoveHistory[nextMoveVerEventIndex], time, frame))
            {
                inputEvents.RaiseMoveVertical(session.verticalMoveHistory[nextMoveVerEventIndex].value);
                nextMoveVerEventIndex++;
            }
            if (session.horizontalMoveHistory.Count > nextMoveHorEventIndex && ShouldEventBePlayed(session.horizontalMoveHistory[nextMoveHorEventIndex], time, frame))
            {
                inputEvents.RaiseMoveHorizontal(session.horizontalMoveHistory[nextMoveHorEventIndex].value);
                nextMoveHorEventIndex++;
            }
            if (session.actionButtonHistory.Count > nextActionEventIndex && ShouldEventBePlayed(session.actionButtonHistory[nextActionEventIndex], time, frame))
            {
                inputEvents.RaiseAction();
                nextActionEventIndex++;
            }
            if (session.subActionButtonHistory.Count > nextSubActionEventIndex && ShouldEventBePlayed(session.subActionButtonHistory[nextSubActionEventIndex], time, frame))
            {
                inputEvents.RaiseSubAction();
                nextSubActionEventIndex++;
            }
            if (session.upButtonHistory.Count > nextUpEventIndex && ShouldEventBePlayed(session.upButtonHistory[nextUpEventIndex], time, frame))
            {
                inputEvents.RaiseUp();
                nextUpEventIndex++;
            }
            if (session.downButtonHistory.Count > nextDownEventIndex && ShouldEventBePlayed(session.downButtonHistory[nextDownEventIndex], time, frame))
            {
                inputEvents.RaiseDown();
                nextDownEventIndex++;
            }
            if (session.moveAssistStartedHistory.Count > nextMoveAssistStartedEventIndex && ShouldEventBePlayed(session.moveAssistStartedHistory[nextMoveAssistStartedEventIndex], time, frame))
            {
                inputEvents.RaiseMoveAssistStarted();
                nextMoveAssistStartedEventIndex++;
            }
            if (session.moveAssistPerformedHistory.Count > nextMoveAssistPerformedEventIndex && ShouldEventBePlayed(session.moveAssistPerformedHistory[nextMoveAssistPerformedEventIndex], time, frame))
            {
                inputEvents.RaiseMoveAssistPerformed();
                nextMoveAssistPerformedEventIndex++;
            }
            if (session.talkButtonHistory.Count > nextTalkEventIndex && ShouldEventBePlayed(session.talkButtonHistory[nextTalkEventIndex], time, frame))
            {
                inputEvents.RaiseTalk();
                nextTalkEventIndex++;
            }
        }

        private bool ShouldEventBePlayed(ActionHistoryEntry entry, float time, uint frame)
        {
            return (playByFrame && entry.frameNumber <= frame) || (!playByFrame && entry.time <= time);
        }
    }
}


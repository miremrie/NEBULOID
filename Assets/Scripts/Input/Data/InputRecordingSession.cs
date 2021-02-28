using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NBLD.Input
{
    [CreateAssetMenu(fileName = "Input Recording Session", menuName = "NBLD/Input Recording Session", order = 3)]

    public class InputRecordingSession : ScriptableObject
    {
        public InputSettings.UpdateMode updateMode;
        public List<ButtonHistoryEntry> actionButtonHistory = new List<ButtonHistoryEntry>();
        public List<ButtonHistoryEntry> subActionButtonHistory = new List<ButtonHistoryEntry>();
        public List<ButtonHistoryEntry> upButtonHistory = new List<ButtonHistoryEntry>();
        public List<ButtonHistoryEntry> downButtonHistory = new List<ButtonHistoryEntry>();
        public List<ButtonHistoryEntry> talkButtonHistory = new List<ButtonHistoryEntry>();
        public List<ButtonHistoryEntry> moveAssistStartedHistory = new List<ButtonHistoryEntry>();
        public List<ButtonHistoryEntry> moveAssistPerformedHistory = new List<ButtonHistoryEntry>();
        public List<NavigationHistoryEntry> moveHistory = new List<NavigationHistoryEntry>();
        public List<FloatHistoryEntry> verticalMoveHistory = new List<FloatHistoryEntry>();
        public List<FloatHistoryEntry> horizontalMoveHistory = new List<FloatHistoryEntry>();
        [ContextMenu("Reset History")]
        public void Reset()
        {
            actionButtonHistory = new List<ButtonHistoryEntry>();
            subActionButtonHistory = new List<ButtonHistoryEntry>();
            upButtonHistory = new List<ButtonHistoryEntry>();
            downButtonHistory = new List<ButtonHistoryEntry>();
            talkButtonHistory = new List<ButtonHistoryEntry>();
            moveAssistStartedHistory = new List<ButtonHistoryEntry>();
            moveAssistPerformedHistory = new List<ButtonHistoryEntry>();
            moveHistory = new List<NavigationHistoryEntry>();
            verticalMoveHistory = new List<FloatHistoryEntry>();
            horizontalMoveHistory = new List<FloatHistoryEntry>();
        }

    }

    public class ActionHistoryEntry
    {
        public float time;
        public uint frameNumber;

        public ActionHistoryEntry(float time, uint frameNumber)
        {
            this.time = time;
            this.frameNumber = frameNumber;
        }
    }
    [System.Serializable]
    public class ButtonHistoryEntry : ActionHistoryEntry
    {
        public bool pressed;

        public ButtonHistoryEntry(bool pressed, float time, uint frameNumber) : base(time, frameNumber)
        {
            this.pressed = pressed;
        }
    }
    [System.Serializable]
    public class NavigationHistoryEntry : ActionHistoryEntry
    {
        public Vector2 value;

        public NavigationHistoryEntry(Vector2 value, float time, uint frameNumber) : base(time, frameNumber)
        {
            this.value = value;
        }
    }
    [System.Serializable]
    public class FloatHistoryEntry : ActionHistoryEntry
    {
        public float value;

        public FloatHistoryEntry(float value, float time, uint frameNumber) : base(time, frameNumber)
        {
            this.value = value;
        }
    }
    [System.Serializable]
    public class IntNavigationHistoryEntry : ActionHistoryEntry
    {
        public Vector2Int value;

        public IntNavigationHistoryEntry(Vector2Int value, float time, uint frameNumber) : base(time, frameNumber)
        {
            this.value = value;
        }
    }
}


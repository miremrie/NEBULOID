using NBLD.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public abstract class UseAction : MonoBehaviour
    {
        public UseActionButton actionButton;
        [Header("Priority")]
        public bool overridePriority = false;
        public int overridenPriority = 0;
        public int Priority => (overridePriority) ? overridenPriority : DefaultActionPriority;
        public virtual int DefaultActionPriority { get; }

        public abstract bool AvailableForChar(CharController charController);

        public abstract void DoAction(CharController user);

        public virtual void OnExitAction(CharController userLeaving)
        {
        }
    }
}


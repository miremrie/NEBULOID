using NBLD.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public abstract class UseAction : MonoBehaviour
    {
        public UseActionButton actionButton;
        public abstract bool AvailableForCharState(CharState charState);

        public abstract void DoAction(CharController user);

        public virtual void OnExitAction(CharController userLeaving)
        {
        }
    }
}


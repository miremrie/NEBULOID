using NBLD.Character;
using NBLD.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public class InsideUseAction : UseAction
    {
        public override CharState AvailableForCharState()
        {
            return CharState.Inside;
        }

        public virtual void DoAction(InsideCharBehaviour controller)
        {
        }

        public virtual void OnExitAction(InsideCharBehaviour controller)
        {
        }
    }
}


using NBLD.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public class OutsideUseAction : UseAction
    {
        public override CharacterState AvailableForCharacterState()
        {
            return CharacterState.Outside;
        }

        public virtual void DoAction(OutsideCharBehaviour behaviour)
        {
        }

        public virtual void OnExitAction(OutsideCharBehaviour behaviour)
        {
        }
    }
}


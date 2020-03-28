using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class OutsideCharController : CharController
    {
        public override void OnMoveAssistPerformed()
        {
            base.OnMoveAssistPerformed();
            charInputManager.ChangeState(Input.CharacterState.Inside);
        }
    }

}

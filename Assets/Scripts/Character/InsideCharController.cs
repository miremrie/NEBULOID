using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class InsideCharController : CharController
    {
        public override void OnTalk()
        {
            base.OnTalk();
            charInputManager.ChangeState(Input.CharacterState.Outside);
        }
    }
}
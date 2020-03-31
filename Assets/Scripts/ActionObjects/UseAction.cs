using NBLD.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UseActions
{
    public abstract class UseAction : MonoBehaviour
    {
        public UseActionButton actionControl;

        public abstract CharacterState AvailableForCharacterState();
    }
}


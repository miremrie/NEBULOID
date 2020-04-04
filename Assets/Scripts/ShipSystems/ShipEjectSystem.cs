using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using UnityEngine;

namespace NBLD.ShipSystems
{
    public class ShipEjectSystem : ShipSystem
    {
        public Transform outsidePipeEnd, insidePipeEnd;
        private CharController charController;
        private bool occupied = false;

        public override void DoAction(CharBehaviour charBehaviour)
        {
            base.DoAction(charBehaviour);
            if (!occupied)
            {
                EjectCharacter(charBehaviour);
            }
        }

        private void EjectCharacter(CharBehaviour charBehaviour)
        {
            occupied = true;
            charController = charBehaviour.charController;
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class CharTool : MonoBehaviour
    {
        protected CharController wielder;
        protected bool equiped;
        protected bool initialized = false;
        public virtual void Initialize()
        {
            initialized = true;
        }
        public virtual void EquipToCharacter(CharController charController)
        {
            wielder = charController;
            equiped = true;
        }
        public virtual void Unequip()
        {
            wielder = null;
            equiped = false;
        }
        public virtual void Activate()
        {

        }

        protected virtual void Update()
        {

        }
        protected virtual void FixedUpdate()
        {

        }
        public virtual bool IsAvailable()
        {
            return initialized;
        }
    }
}
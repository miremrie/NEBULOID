using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using NBLD.ExteriorObjects;
using UnityEngine;

namespace NBLD.UseActions
{
    public class GarageButtonUseAction : UseAction
    {
        public GarageArea garageArea;
        public Animator buttonAnimator;
        private const string activeButtonAnimKey = "ButtonEnabled";
        private const string clickedButtonAnimKey = "ButtonClicked";
        private bool subscribed = false;
        private bool garageAvailable = false;
        public override int DefaultActionPriority => 60;
        public override bool AvailableForChar(CharController charController)
        {
            return charController.GetState() == CharState.Outside;
        }

        private void Subscribe()
        {
            if (!subscribed)
            {
                garageArea.OnGarageAreaStateChanged += OnGarageAreaStateChanged;
                subscribed = true;
            }
        }
        private void Unsubscribe()
        {
            if (subscribed)
            {
                garageArea.OnGarageAreaStateChanged -= OnGarageAreaStateChanged;
                subscribed = false;
            }
        }
        private void OnEnable()
        {
            Subscribe();
        }
        private void OnDisable()
        {
            Unsubscribe();
        }
        public override void DoAction(CharController user)
        {
            if (garageAvailable)
            {
                garageArea.StartToEnterGarage();
            }
        }

        private void OnGarageAreaStateChanged()
        {
            bool prevGarageAvailable = garageAvailable;
            garageAvailable = garageArea.CanGarageBeEntered();
            if (!prevGarageAvailable && garageAvailable)
            {
                buttonAnimator.SetBool(activeButtonAnimKey, true);
            }
            else if (prevGarageAvailable && !garageAvailable)
            {
                buttonAnimator.SetBool(activeButtonAnimKey, false);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using NBLD.Character;
using NBLD.ExteriorObjects;
using UnityEngine;

namespace NBLD.UseActions
{
    public class GarageButtonUseAction : OutsideUseAction
    {
        public GarageArea garageArea;
        public Animator buttonAnimator;
        private const string activeButtonAnimKey = "ButtonEnabled";
        private const string clickedButtonAnimKey = "ButtonClicked";
        private bool subscribed = false;
        private bool garageAvailable = false;

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
        public override CharState AvailableForCharState()
        {
            return CharState.Outside;
        }
        public override void DoAction(OutsideCharBehaviour behaviour)
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
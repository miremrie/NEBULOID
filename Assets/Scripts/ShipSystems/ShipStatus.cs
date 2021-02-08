using System.Collections;
using System.Collections.Generic;
using NBLD.UI;
using UnityEngine;

namespace NBLD.Ship
{
    [System.Serializable]
    public class ShipFuel
    {
        public float min, max;
        public float burnRate;
        [SerializeField]
        private float current;
        public float Current
        {
            get => current;
            set => current = Mathf.Clamp(value, min, max);
        }
        public void UpdateFuel(float dt)
        {
            Current -= dt * burnRate;
        }

        public bool IsEmpty() => Mathf.Approximately(Current, min);
        public void FillUp()
        {
            Current = max;
        }

        public float GetPercent() => (current - min) / (max - min);
    }
    public class ShipStatus : MonoBehaviour
    {
        public ShipMovement shipMovement;
        public ShipFuel shipFuel;

        public ShipAudioController shipAudioController;
        [Header("External components")]
        private Game game;
        public MaskedSlider fuelUI;
        private bool initialized = false;
        public void Initialize(Game game)
        {
            this.game = game;
            shipFuel.FillUp();
            fuelUI.Initalize(shipFuel.min, shipFuel.max);
            initialized = true;
        }
        private void Update()
        {
            if (initialized)
            {
                if (!game.IsGameOver())
                {
                    UpdateFuel();
                }
            }

        }
        #region Fuel
        private void UpdateFuel()
        {
            shipFuel.UpdateFuel(Time.deltaTime);
            shipAudioController.SetFuelFX(shipFuel.GetPercent());
            fuelUI.UpdateValue(shipFuel.Current);
            if (shipFuel.IsEmpty())
            {
                GameOver();
            }
        }

        public void FuelCollected(FuelTank fuel)
        {
            shipAudioController.StartFuelRefill();
            shipFuel.FillUp();
        }
        #endregion
        #region Repairs
        #endregion

        private void GameOver()
        {
            game.GameOver();
        }
    }
}
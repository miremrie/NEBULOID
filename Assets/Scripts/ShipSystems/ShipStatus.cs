using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public Repairable[] repairables;
        public ShipAudioController audioController;
        [Header("Prefabs")]
        public GameObject explosionFX;
        [Header("External Scene components")]
        private Game game;
        public MaskedSlider fuelUI;
        public ScreenShake screenShake;
        private bool initialized = false;
        public void Initialize(Game game)
        {
            this.game = game;
            shipFuel.FillUp();
            fuelUI.Initalize(shipFuel.min, shipFuel.max);
            initialized = true;
            for (int i = 0; i < repairables.Length; i++)
            {
                repairables[i].Initialize(this);
            }
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
            audioController.SetFuelFX(shipFuel.GetPercent());
            fuelUI.UpdateValue(shipFuel.Current);
            if (shipFuel.IsEmpty())
            {
                GameOver();
            }
        }

        public void FuelCollected(FuelTank fuel)
        {
            audioController.StartFuelRefill();
            shipFuel.FillUp();
        }

        #endregion
        #region Repairs And Hits
        public void ObstacleHit(Obstacle obs)
        {
            screenShake.TriggerShake(0.5f);
            var rand = new System.Random();
            var dmgIndex = rand.Next(0, repairables.Length);
            var rp = repairables[dmgIndex];
            rp.TakeDamage(obs.Damage);
            audioController.PlayShipHit();
            audioController.PlayAlarm();
            Instantiate(explosionFX, obs.transform.position, Quaternion.identity);
        }
        public void RoomRepaired(Repairable repairable)
        {
            if (repairables.Any(rp => !rp.IsRepaired())) return;
            audioController.StopAlarm();
        }
        #endregion

        private void GameOver()
        {
            game.GameOver();
        }
    }
}
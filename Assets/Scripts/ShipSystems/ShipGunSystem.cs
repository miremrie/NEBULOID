using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Ship
{
    public class ShipGunSystem : ShipSystem
    {
        public Bullet bullet;
        public Transform bulletShoothole;
        public bool shootBuletsNotBombs = true;
        public ShipAudioController audioController;
        public ScreenShake shake;

        public override void DoAction()
        {
            base.DoAction();
            shake.TriggerShake(0.5f);
            Bullet tmpBullet = Instantiate(bullet);
            tmpBullet.transform.position = bulletShoothole.position;
            tmpBullet.Shoot(bulletShoothole.up, shootBuletsNotBombs);
            audioController.PlayShootClip();
        }
    }
}


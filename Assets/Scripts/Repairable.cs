using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repairable : MonoBehaviour
{

    public float RepairedAmount { get; private set; }
    bool repairing;
    static float repairSpeed = 0.1f;
    public ParticleSystem smoke;

    void Start()
    {
        RepairedAmount = 1f;
    }


    public void StartRepairing() {
        repairing = true;
    }

    public void StopRepairing() {
        repairing = false;
    }

    public bool IsRepaired() {
        return RepairedAmount >= 1f;
    }

    void Update() {

        if (!IsRepaired() && repairing) {

            RepairedAmount += repairSpeed * Time.deltaTime;

            if (IsRepaired()) {
                // on repaired do once
                StopRepairing();
                if (smoke != null) smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    public void TakeDamage()
    {
        RepairedAmount = 0;
        if (smoke != null) smoke.Play();
    }
}

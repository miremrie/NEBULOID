using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repairable : MonoBehaviour
{

    public float RepairedAmount { get; private set; }
    bool repairing;
    static float repairSpeed = 0.8f;
    public ParticleSystem smoke;
    public Game game;
    public GameObject alarmIndicator;

    void Start()
    {
        alarmIndicator.SetActive(false);
        RepairedAmount = 1f;
    }


    public void StartRepairing() {
        repairing = true;
    }

    public void StopRepairing() {
        alarmIndicator.SetActive(false);
        repairing = false;
        game.Repaired(this);
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
        alarmIndicator.SetActive(true);
        RepairedAmount = 0;
        if (smoke != null) smoke.Play();
    }
}

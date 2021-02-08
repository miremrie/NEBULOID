using NBLD.Ship;
using NBLD.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repairable : MonoBehaviour
{

    public float RepairedAmount { get; private set; }
    bool repairing;
    static float repairSpeed = 0.25f;
    public ParticleSystem smoke;
    public ShipStatus shipStatus;
    public GameObject alarmIndicator;
    public MaskedSlider maskedSlider;
    private bool initialized = false;
    public void Initialize(ShipStatus shipStatus)
    {
        this.shipStatus = shipStatus;
        repairing = false;
        alarmIndicator.SetActive(false);
        RepairedAmount = 1f;
        maskedSlider.Initalize(0, 1);
        initialized = true;
    }


    public void StartRepairing()
    {
        repairing = true;
    }

    public void StopRepairing()
    {
        repairing = false;
    }

    public bool IsRepaired()
    {
        return RepairedAmount >= 1f;
    }

    void Update()
    {
        if (initialized)
        {
            if (!IsRepaired() && repairing)
            {
                RepairedAmount += repairSpeed * Time.deltaTime;
                maskedSlider.Show();
                maskedSlider.UpdateValue(1 - RepairedAmount);
                if (IsRepaired())
                {
                    maskedSlider.Hide();
                    // on repaired do once
                    StopRepairing();
                    alarmIndicator.SetActive(false);
                    shipStatus.RoomRepaired(this);
                    if (smoke != null) smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

    }

    public void TakeDamage(float percent)
    {
        maskedSlider.Show();

        alarmIndicator.SetActive(true);
        RepairedAmount = Mathf.Max(RepairedAmount - percent, 0);
        maskedSlider.UpdateValue(1 - RepairedAmount);
        if (smoke != null) smoke.Play();
    }

    [ContextMenu("TakeFullDamage")]
    public void TakeFullDamage()
    {
        TakeDamage(1);
    }
}

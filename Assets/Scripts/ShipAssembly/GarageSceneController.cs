using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageSceneController : MonoBehaviour
{
    public ShipCreator shipCreator;

    private void Start()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        shipCreator.onCreationStageChanged += OnCreationStageChanged;
    }

    private void OnCreationStageChanged(CreationStage creationStage)
    {

    }
}

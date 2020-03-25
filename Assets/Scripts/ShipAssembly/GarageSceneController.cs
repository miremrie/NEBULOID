using NBLD.ShipCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GarageSceneController : MonoBehaviour
{
    public ShipCreator shipCreator;

    private void Awake()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        shipCreator.onCreationStageChanged += OnCreationStageChanged;
    }

    private void OnCreationStageChanged(CreationStage creationStage, CreationStage previousStage)
    {
        if (creationStage == CreationStage.Canceled || creationStage == CreationStage.Confirmed)
        {
            AkSoundEngine.StopAll();
            SceneManager.LoadScene(0);
        }
    }
}

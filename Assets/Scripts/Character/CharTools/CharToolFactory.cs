using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public class CharToolFactory : MonoBehaviour
    {
        [Header("Mine Planter Tool")]
        public MinePlanterTool minePlanterToolPrefab;
        public Transform minePlacementRoot;
        [Header("Flashlight Tool")]
        public FlashilightTool flashLightToolPrefab;
        [Header("Wrench Tool")]
        public WrenchTool wrenchToolPrefab;
        public CharTool CreateCharTool(CharToolType type)
        {
            CharTool tool;
            if (type == CharToolType.MinePlanter)
            {
                MinePlanterTool mpTool = GameObject.Instantiate(minePlanterToolPrefab);
                mpTool.minePlacementRoot = minePlacementRoot;
                tool = mpTool;
            }
            else if (type == CharToolType.Flashlight)
            {
                FlashilightTool fTool = GameObject.Instantiate(flashLightToolPrefab);
                tool = fTool;
            }
            else if (type == CharToolType.Wrench)
            {
                WrenchTool wTool = GameObject.Instantiate(wrenchToolPrefab);
                tool = wTool;
            }
            else
            {
                return null;
            }
            tool.transform.localPosition = Vector3.zero;
            tool.Initialize();
            return tool;
        }
    }
}


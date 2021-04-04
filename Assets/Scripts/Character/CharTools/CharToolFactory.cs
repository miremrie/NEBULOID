using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    public enum CharToolType { MinePlanter, Flashlight, Empty };
    public class CharToolFactory : MonoBehaviour
    {
        [Header("Mine Planter Tool")]
        public MinePlanterTool minePlanterToolPrefab;
        public Transform minePlacementRoot;
        [Header("Flashlight Tool")]
        public FlashilightTool flashLightToolPrefab;
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


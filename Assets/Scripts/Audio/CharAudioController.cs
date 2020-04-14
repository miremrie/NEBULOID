using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Audio
{
    public class CharAudioController : MonoBehaviour
    {
        private const string FootstepsKey = "Play_Footsteps_Main";
        //public GameObject footstepsPlayer;
        public float[] auxValues = new float[3];
        private const string LadderAscend = "Play_Footsteps_Ladder_Asc";
        private const string LadderDescend = "Play_Footsteps_Dsc";
        //public GameObject ladderPlayer;
        private static int auxCount = 3;
        private static string[] floorAux = new string[]
        {
            "LowerRoom",
            "MainRoom",
            "UpperRoom"
        };
        private static int[] floorAuxMap = new int[4] { 0, 1, 1, 2 };

        public void PlayFootstep()
        {
            AkSoundEngine.PostEvent(FootstepsKey, gameObject);
        }

        public void PlayLadderAsc()
        {
            AkSoundEngine.PostEvent(LadderAscend, gameObject);
        }
        public void PlayerLadderDsc()
        {
            AkSoundEngine.PostEvent(LadderDescend, gameObject);
        }

        public void SetEnvironmentBasedOnFloor(int floor)
        {
            AkAuxSendArray aEnvs = new AkAuxSendArray();
            int floorAuxIndex = floorAuxMap[floor];
            for (int i = 0; i < auxCount; i++)
            {
                float value = (floorAuxIndex == i) ? 1.0f : 0.0f;
                auxValues[i] = value;
                aEnvs.Add(AkSoundEngine.GetIDFromString(floorAux[i]), value);
            }
            
            AkSoundEngine.SetGameObjectAuxSendValues(gameObject, aEnvs, (uint)auxCount);
        }

    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Audio
{
    public class CharAudioController : MonoBehaviour
    {
        public int playerIndex;
        private const string footstepsEv = "Play_Footsteps_Main";
        //public GameObject footstepsPlayer;
        public float[] auxValues = new float[3];
        private const string ladderAscendEv = "Play_Footsteps_Ladder_Asc";
        private const string ladderDescendEv = "Play_Footsteps_Dsc";
        //public GameObject ladderPlayer;
        private static int auxCount = 3;
        private static string[] floorAux = new string[]
        {
            "LowerRoom",
            "MainRoom",
            "UpperRoom"
        };
        private static int[] floorAuxMap = new int[4] { 0, 1, 1, 2 };
        private static string[] idleBehaviourEvents = new string[2] {
            "Play_Player_1_Barbble",
            "Play_Player_2_Barbble"
        };
        private static string[] hurtEvents = new string[2] {
            "Play_Player_1_Hurt",
            "Play_Player_2_Hurt"
        };

        public void PlayFootstep()
        {
            AkSoundEngine.PostEvent(footstepsEv, gameObject);
        }

        public void PlayLadderAsc()
        {
            AkSoundEngine.PostEvent(ladderAscendEv, gameObject);
        }
        public void PlayLadderDsc()
        {
            AkSoundEngine.PostEvent(ladderDescendEv, gameObject);
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
        public void PlayIdleBehaviour()
        {
            if (playerIndex < idleBehaviourEvents.Length)
            {
                AkSoundEngine.PostEvent(idleBehaviourEvents[playerIndex], gameObject);
            }
            else
            {
                AkSoundEngine.PostEvent(idleBehaviourEvents[0], gameObject);
            }
        }
        public void PlayHurt()
        {
            if (playerIndex < hurtEvents.Length)
            {
                AkSoundEngine.PostEvent(hurtEvents[playerIndex], gameObject);
            }
            else
            {
                AkSoundEngine.PostEvent(hurtEvents[0], gameObject);
            }
        }
    }
}


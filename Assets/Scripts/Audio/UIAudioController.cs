using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Audio {
    public class UIAudioController : MonoBehaviour
    {
        private const string clickEv = "Play_Click";
        private const string hoverEv = "Play_Menu_Hover";
        private const string enterEv = "Play_MenuEnter";
        private const string exitEv = "Play_MenuExit";

        public void PlayClick() {
            AkSoundEngine.PostEvent(clickEv, gameObject);
        }
        public void PlayHover() {
            AkSoundEngine.PostEvent(hoverEv, gameObject);
        }
        public void PlayEnter() {
            AkSoundEngine.PostEvent(enterEv, gameObject);
        }
        public void PlayExit() {
            AkSoundEngine.PostEvent(exitEv, gameObject);
        }
    }
}


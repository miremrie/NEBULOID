using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.MainMenu
{
    public class CharacterSelectSinglePanel : MonoBehaviour
    {
        public GameObject inactivePanel;
        public GameObject activePanel;
        public void Activate()
        {
            inactivePanel.SetActive(false);
            activePanel.SetActive(true);
        }

        public void Deactivate()
        {
            inactivePanel.SetActive(true);
            activePanel.SetActive(false);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.UI
{
    public class SingleOptionSelectText : SingleOptionSelect
    {
        public TMPro.TextMeshProUGUI currentlySelected;

        public void UpdateText(string text)
        {
            currentlySelected.text = text;
        }
    }
}


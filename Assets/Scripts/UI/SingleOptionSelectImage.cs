using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NBLD.UI
{
    public class SingleOptionSelectImage : SingleOptionSelect
    {
        public Image currentlySelected;

        public void UpdateImage(Sprite sprite)
        {
            currentlySelected.sprite = sprite;
        }
    }
}


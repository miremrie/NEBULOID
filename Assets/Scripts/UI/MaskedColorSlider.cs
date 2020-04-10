using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NBLD.UI
{
    public class MaskedColorSlider : MaskedSlider
    {
        public Color minColor;
        public Color maxColor;
        public Image fillImage;

        public override void UpdateValue(float value)
        {
            base.UpdateValue(value);

            fillImage.color = Color.Lerp(minColor, maxColor, GetPercentValue(value));
        }
    }
}

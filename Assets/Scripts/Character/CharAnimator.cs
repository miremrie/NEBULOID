using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Character
{
    [ExecuteInEditMode]
    public class CharAnimator : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public int currentSpriteIndex;
        public Sprite[] sprites;

        private void Update()
        {
            CheckIfSpriteChanged();
        }


        /*private void OnValidate()
        {
            //CheckIfSpriteChanged();
        }*/
        private void OnAnimatorMove()
        {
            //CheckIfSpriteChanged();
        }
        private void CheckIfSpriteChanged()
        {
            /*if (spriteRenderer.sprite != sprites[currentSpriteIndex])
            {*/
            if (sprites != null && currentSpriteIndex.IsBetween(-1, sprites.Length))
            {
                //Debug.Log($"{currentSpriteIndex}/{sprites.Length}");
                spriteRenderer.sprite = sprites[currentSpriteIndex];
            }
            //}
        }
    }
}


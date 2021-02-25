using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace NBLD.Data
{
    [System.Serializable]
    public class CharacterSkinData
    {
        public string name;
        public Sprite defaultImage;
        public AnimatorController animatorController;
        public List<Sprite> sprites;

    }
    [CreateAssetMenu(fileName = "Character Skins", menuName = "NBLD/Character Skins", order = 2)]
    public class CharacterSkins : ScriptableObject
    {
        [SerializeField]
        public List<CharacterSkinData> skinDatas;
        private bool generatedArrays = false;
        public int GetSkinsCount()
        {
            return skinDatas.Count;
        }
        public CharacterSkinData GetSkinData(int index)
        {
            return skinDatas[index];
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NBLD.Data
{
    [CreateAssetMenu(fileName = "Character Names", menuName = "NBLD/Character Names", order = 1)]
    public class CharacterNames : ScriptableObject
    {
        [SerializeField, TextArea]
        private string devotionNames;
        [SerializeField, TextArea]
        private string spiritNames;

        private string[] devotionNamesArray;
        private string[] spiritNamesArray;

        private bool generatedArrays = false;
        public int GetSpiritNamesCount()
        {
            if (!generatedArrays)
            {
                GenerateArrays();
            }
            return spiritNamesArray.Length;
        }
        public int GetDevotionNamesCount()
        {
            if (!generatedArrays)
            {
                GenerateArrays();
            }
            return devotionNamesArray.Length;
        }
        public string GetDevotionName(int index)
        {
            if (!generatedArrays)
            {
                GenerateArrays();
            }
            return devotionNamesArray[index];
        }
        public string GetSpiritName(int index)
        {
            if (!generatedArrays)
            {
                GenerateArrays();
            }
            return spiritNamesArray[index];
        }
        private void GenerateArrays()
        {
            devotionNamesArray = devotionNames.Split(',');
            spiritNamesArray = spiritNames.Split(',');
            generatedArrays = true;
        }
    }
}


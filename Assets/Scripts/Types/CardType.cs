using UnityEngine;

namespace CardFramework
{
    [System.Serializable]
    public class CardType
    {

        public string Name { get; set; }

        public string TextureName { get; set; }

        public Texture2D Texture { get; set; }

        public int CountStars { get; set; }
    }
}
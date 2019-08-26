using UnityEngine.UI;

namespace CardFramework
{
    [System.Serializable]
    public struct CardComponents
    {

        public Image Art { get; set; }

        public Text Title { get; set; }

        public Image TitleBackground { get; set; }

        public Image[] Backgrounds { get; set; }

        public Image[] Stars { get; set; }
    }
}
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CrossInput;

namespace CardFramework
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Card : MonoBehaviour
    {
        public bool lockClick = true;
        public CardRevealParams revealSettings;
        public Texture2D titleTexture;
        public Texture2D starTexture;

        protected Image[] allImages;

        protected CardComponents structureComponents;
        protected Image cardArt;

        protected BoxCollider2D boxCollider;
        public BoxCollider2D Collider
        {
            get
            {
                return boxCollider;
            }
            set
            {
                boxCollider = value;
            }
        }

        protected Texture2D randomTexture;

        protected CardType randomCard;

        protected Texture2D randomBackground;

        protected CardPack parentPack;

        void Awake()
        {
            var starsGroup = GetComponentInChildren<HorizontalLayoutGroup>();
            allImages = GetComponentsInChildren<Image>();
            boxCollider = GetComponentInChildren<BoxCollider2D>(true);
            parentPack = GetComponentInParent<CardPack>();

            if (allImages.Length > 0)
            {
                structureComponents.Art = allImages.SingleOrDefault(img =>
                {
                    return img.name.Equals("card-art") || img.name.ToLower().EndsWith("art");
                });

                structureComponents.TitleBackground = allImages.SingleOrDefault(img =>
                {
                    return img.name.Equals("name-bg") || img.name.ToLower().EndsWith("-bg");
                });

                structureComponents.Backgrounds = allImages.Where(img =>
                {
                    return img.name.StartsWith("left") || img.name.ToLower().StartsWith("right");
                }).ToArray();

                if (structureComponents.TitleBackground != null)
                {
                    var title = structureComponents.TitleBackground.GetComponentInChildren<Text>();
                    if (title != null)
                    {
                        structureComponents.Title = title;
                    }
                }
            }

            if (starsGroup != null)
            {
                structureComponents.Stars = starsGroup.GetComponentsInChildren<Image>();
            }

            if (parentPack != null)
            {
                randomCard = parentPack.RandomizeCard();
                if (parentPack.texturesBackground.Length > 0)
                {
                    randomBackground = parentPack.RandomizeBg();
                }
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            if (boxCollider != null)
            {
                if (boxCollider.size.x <= 0 || boxCollider.size.y <= 0)
                {
                    string msg = "A {0} a SIZE is required to wrap a Card. Is necessary to interact with the Card (touch, click...)";
                    Debug.LogErrorFormat(msg, boxCollider.name);
                }
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!lockClick)
            {
                InputManager.OnClick("Left", gameObject, (position) =>
                {
                    Reveal();
                });
            }
        }

        /// <summary>
        /// Randomize a <see cref="CardType"/> calling <see cref="CardPack.RandomizeCard"/> 
        /// method from parent CardPack instance 
        /// </summary>
        /// <returns>A <see cref="CardType"/> instance</returns>
        public virtual CardType RandomizeType()
        {

            if (parentPack != null)
            {
                return parentPack.RandomizeCard();
            }

            return null;
        }

        /// <summary>
        /// Create a new sprite from a <see cref="Texture2D"/> image
        /// </summary>
        /// <remarks>
        /// This method is generic, and can be moved to a utility class
        /// </remarks>
        /// <param name="texture"> A <see cref="Texture2D"/> of an image to change</param>
        /// <returns> A new <see cref="Sprite"/> with the texture passed by <paramref name="texture"/> param</returns>
        public virtual Sprite ChangeTexture(Texture2D texture)
        {
            Rect size = new Rect(0.0f, 0.0f, texture.width, texture.height);
            Vector2 pivotPoint = new Vector2(0.5f, 0.5f);

            return Sprite.Create(
                texture,
                size,
                pivotPoint
            );
        }

        /// <summary>
        /// Change texture image stars
        /// </summary>
        public virtual void ChangeStars()
        {
            if (starTexture != null)
            {
                for (int i = 0; i < randomCard.CountStars; i++)
                {
                    structureComponents.Stars[i].sprite = ChangeTexture(starTexture);
                }
            }
        }

        /// <summary>
        /// Change the background texture image
        /// </summary>
        public virtual void ChangeBackground()
        {

            if (randomBackground != null)
            {
                foreach (var bg in structureComponents.Backgrounds)
                {
                    bg.sprite = ChangeTexture(randomBackground);
                }
            }
        }

        /// <summary>
        /// Change the text and texture image of the title of this card
        /// </summary>
        public virtual void ChangeTitle()
        {

            if (titleTexture != null)
            {
                structureComponents.TitleBackground.sprite = ChangeTexture(titleTexture);
                structureComponents.Title.text = randomCard.Name;
            }
        }

        /// <summary>
        /// Reveal this card with <see cref="Tween"/> flip animation
        /// </summary>
        /// <param name="revealParams"></param>
        /// <returns> A <see cref="Tween"/> from the rotate/flip animation </returns>
        public virtual Tween Reveal(CardRevealParams revealParams = null)
        {
            if (revealParams == null)
            {
                revealParams = revealSettings;
            }

            return transform.DORotate(new Vector3(0, revealParams.degreesY, 0), revealSettings.duration).OnComplete(() =>
                {
                    if (structureComponents.Art != null)
                    {
                        if (randomCard.Texture != null)
                        {
                            structureComponents.Art.sprite = ChangeTexture(randomCard.Texture);
                        }
                        ChangeTitle();
                        ChangeStars();
                        ChangeBackground();

                        int rotateBack = revealParams.degreesY == 90 ? 0 : 90;
                        transform.DORotate(new Vector3(0, rotateBack, 0), revealParams.duration);
                    }
                });
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CrossInput;

namespace CardFramework
{
    public class CardPack : MonoBehaviour
    {
        public Canvas canvas;
        public Image handIcon;

        public ShowUI indicator;

        public Texture2D cursorTexture;

        public CursorMode cursorMode = CursorMode.Auto;
        protected Card[] cards;

        protected Object[] cardsTextures;

        public Texture2D[] texturesBackground;

        protected CardType[] availableCards = new[] {
            new CardType() {
                Name = "Candles",
                TextureName = "candles2.png",
                CountStars = 2
            },
            new CardType() {
                Name = "Flowers Orange",
                TextureName = "flowers_orange.png",
                CountStars = 5
            },
            new CardType() {
                Name = "White Guitar",
                TextureName = "guitar_white.png",
                CountStars = 4
            },
            new CardType() {
                Name = "Maracas Skull",
                TextureName = "maracas_skull.png",
                CountStars = 1
            },
            new CardType() {
                Name = "Margarita",
                TextureName = "margarita.png",
                CountStars = 3
            },
            new CardType() {
                Name = "Musician",
                TextureName = "musician.png",
                CountStars = 5
            },
            new CardType() {
                Name = "Scarf",
                TextureName = "scarf.png",
                CountStars = 2
            },
            new CardType() {
                Name = "Skeleton Dancer",
                TextureName = "skeleton_dancer.png",
                CountStars = 5
            },
            new CardType() {
                Name = "Sombrero",
                TextureName = "sombrero.png",
                CountStars = 3
            },
            new CardType() {
                Name = "Taco",
                TextureName = "taco.png",
                CountStars = 4
            }
        };

        protected Dictionary<Card, Vector3> originPositions = new Dictionary<Card, Vector3>();

        private bool revealed = false;

        void Awake()
        {
            cards = GetComponentsInChildren<Card>();
            cardsTextures = Resources.LoadAll("Cards", typeof(Texture2D));

            if (indicator == null)
            {
                indicator = GetComponentInChildren<ShowUI>(true);
            }

            if (cardsTextures.Length == 0)
            {
                Debug.LogError("A directory Assets/Resources/Cards with Card art images is required!");
            }
            else
            {
                foreach (var texture in cardsTextures)
                {
                    Texture2D castedTexture = texture as Texture2D;
                    CardType cardType = availableCards.SingleOrDefault(card => card.TextureName.ToLower().Contains(castedTexture.name));
                    if (cardType != null)
                    {
                        cardType.Texture = castedTexture;
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (!revealed)
            {
                InputManager.OnClick("Left", gameObject, (position) =>
                {
                    if (indicator != null)
                    {
                        indicator.gameObject.SetActive(false);
                    }

                    revealed = true;

                    StartCoroutine(Reveal());
                });
            }
        }

        void OnMouseEnter()
        {
            ChangeCursor();
        }

        void OnMouseExit()
        {
            ResetCursor();
        }

        /// <summary>
        /// Change the mouse cursor.
        /// Only if the Card pack is revelead
        /// </summary>
        public virtual void ChangeCursor()
        {
            if (cursorTexture != null && !revealed)
            {
                Cursor.SetCursor(cursorTexture, Vector2.zero, cursorMode);
            }
        }

        /// <summary>
        /// Restore the mouse cursor to default
        /// </summary>
        public virtual void ResetCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }

        /// <summary>
        ///  Get a random <see cref="CardType"/> from <see cref="availableCards"/> array
        /// </summary>
        /// <returns>A ramdomized <see cref="CardType"/> instance</returns>
        public virtual CardType RandomizeCard()
        {
            return availableCards[Random.Range(0, availableCards.Length)];
        }

        /// <summary>
        /// Get a random background <see cref="Texture2D"/> image,
        /// from <see cref="CardPack.texturesBackground"/> array
        /// </summary>
        /// <returns>A <see cref="Texture2D"/> random image</returns>
        public virtual Texture2D RandomizeBg()
        {
            return texturesBackground[Random.Range(0, texturesBackground.Length)];
        }

        /// <summary>
        /// Reveal all cards, with chain Coroutines animations
        /// </summary>
        /// <see cref="https://riptutorial.com/unity3d/example/12479/chaining-coroutines">Chaining coroutines </see>
        /// <returns>A enumerator to execute with <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/></returns>
        public virtual IEnumerator Reveal()
        {
            yield return StartCoroutine(Organize());
            yield return StartCoroutine(MoveToHand(this));
        }

        /// <summary>
        /// Move cards to the "player hand"
        /// </summary>
        /// <param name="pack">A object to move. Can be a <see cref="CardPack"/> or a specific <see cref="Card"/> object</param>
        /// <returns>A enumerator of the sequence move animations to use with <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/></returns>
        public virtual IEnumerator MoveToHand(CardPack pack)
        {
            yield return HandMoveAndScale(pack);

            if (handIcon != null)
            {
                handIcon.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Move the cards relative to the center of <see cref="canvas"/>
        /// </summary>
        /// <param name="card">A <see cref="Card"/> object to move</param>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual YieldInstruction Move(Card card, Vector3 position)
        {
            Sequence sequence = DOTween.Sequence();
            return sequence.Append(card.transform.DOLocalMove(position, .5f))
                           .Join(card.Reveal())
                           .WaitForCompletion();
        }

        /// <summary>
        /// Organize the cards from a pack, moving to the center of <see cref="canvas"/>
        /// and revealing each card.
        /// </summary>
        /// <returns></returns>
        /// <see cref="https://answers.unity.com/questions/1359168/wait-for-seconds-inside-for-loop.html">Reference: Coroutine wait inside a loop</see>
        public virtual IEnumerator Organize()
        {
            Vector3 lastPosition = Vector3.zero;

            foreach (var card in cards)
            {
                Vector3 newPosition = Vector3.zero;
                originPositions.Add(card, (card.transform as RectTransform).anchoredPosition);

                if (lastPosition == Vector3.zero)
                {
                    newPosition.x = -(canvas.pixelRect.center.x + 100);
                }
                else
                {
                    newPosition.x = lastPosition.x + 300;
                }

                yield return Move(card, newPosition);

                lastPosition.x = (card.transform as RectTransform).anchoredPosition.x;
                
                // Enable the collider(s) to allow Card interactions(click, drag...)
                if (card.Collider != null && !card.Collider.isActiveAndEnabled)
                {
                    card.Collider.enabled = true;
                }
            }
        }
        
        /// <summary>
        /// Sequence card move and scale animations
        /// </summary>
        /// <param name="pack">A <see cref="CardPack"/> instance to animate</param>
        /// <returns> A <see cref="YieldInstruction"/> to execute inside of enumerators methods to execute in <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/> </returns>
        public virtual YieldInstruction HandMoveAndScale(CardPack pack)
        {
            Sequence sequence = DOTween.Sequence();
            return sequence
                .Append(pack.transform.DOLocalMoveY(-100, .7f))
                .Join(pack.transform.DOScale(new Vector2(.7f, .7f), 1))
                .WaitForCompletion();
        }
    }
}

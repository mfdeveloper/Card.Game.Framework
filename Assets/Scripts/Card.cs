using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CrossInput;


[RequireComponent(typeof(BoxCollider2D))]
public class Card : MonoBehaviour
{
    [System.Serializable]
    public class RevealParams
    {

        public float duration = 1;

        public float degreesY = 90;
    }

    [System.Serializable]
    public class CardType
    {

        public string Name { get; set; }

        public string TextureName { get; set; }

        public Texture2D Texture { get; set; }

        public int Stars { get; set; }
    }

    [System.Serializable]
    public struct CardComponents
    {

        public Image Art { get; set; }

        public Text Title { get; set; }

        public Image TitleBackground { get; set; }

        public Image[] Backgrounds { get; set; }

        public Image[] Stars { get; set; }
    }

    public RevealParams revealSettings;
    public Texture2D titleTexture;
    public Texture2D starTexture;

    public Texture2D[] texturesBackground;

    protected Image[] allImages;

    protected CardComponents structureComponents;
    protected Image cardArt;

    protected BoxCollider2D boxCollider;

    protected Object[] cardsTextures;

    protected CardType[] availableCards = new[] {
        new CardType() {
            Name = "Musician",
            TextureName = "musician.png",
            Stars = 5
        },
        new CardType() {
            Name = "Margarita",
            TextureName = "margarita.png",
            Stars = 3
        },
        new CardType() {
            Name = "Maracas Skull",
            TextureName = "maracas_skull.png",
            Stars = 1
        }
    };

    protected Texture2D randomTexture;

    protected CardType randomCard;

    protected Texture2D randomBackground;

    void Awake()
    {
        var starsGroup = GetComponentInChildren<HorizontalLayoutGroup>();
        allImages = GetComponentsInChildren<Image>();
        boxCollider = GetComponentInChildren<BoxCollider2D>(true);
        cardsTextures = Resources.LoadAll("Cards", typeof(Texture2D));

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

        if (cardsTextures.Length == 0)
        {
            Debug.LogError("A directory Assets/Resources/Cards with Card art images is required!");
        } else
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
        
        randomCard = availableCards[Random.Range(0, availableCards.Length)];

        if (texturesBackground.Length > 0)
        {
            randomBackground = texturesBackground[Random.Range(0, texturesBackground.Length)];
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (boxCollider != null)
        {
            if (!boxCollider.isActiveAndEnabled)
            {
                Debug.LogErrorFormat("Enable the {0} to interact with Cards", boxCollider.name);
            }

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
        InputManager.OnClick("Left", gameObject, (position) =>
        {
            Reveal();
        });
    }

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

    public virtual void ChangeStars() {

        if (starTexture != null)
        {    
            for (int i = 0; i < randomCard.Stars; i++)
            {
                structureComponents.Stars[i].sprite = ChangeTexture(starTexture);
            }
        }
    }

    public virtual void ChangeBackground() {

        if (randomBackground != null)
        {
            foreach (var bg in structureComponents.Backgrounds)
            {
                bg.sprite = ChangeTexture(randomBackground);
            }
        }
    }

    public virtual void ChangeTitle() {

        if (titleTexture != null)
        {
            structureComponents.TitleBackground.sprite = ChangeTexture(titleTexture);
            structureComponents.Title.text = randomCard.Name;
            
        }
    }

    public virtual void Reveal(RevealParams revealParams = null)
    {
        if (revealParams == null)
        {
            revealParams = revealSettings;
        }

        transform.DORotate(new Vector3(0, revealParams.degreesY, 0), revealSettings.duration).OnComplete(() =>
            {
                if (structureComponents.Art != null)
                {
                    // cardArt.sprite = ChangeArt(randomCard.Texture);
                    structureComponents.Art.sprite = ChangeTexture(randomCard.Texture);
                    ChangeTitle();
                    ChangeStars();
                    ChangeBackground();

                    int rotateBack = revealParams.degreesY == 90 ? 0 : 90;
                    transform.DORotate(new Vector3(0, rotateBack, 0), revealParams.duration);
                }
            });
    }
}

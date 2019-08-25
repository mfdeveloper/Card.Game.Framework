using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class CardPack : MonoBehaviour
{

    public Canvas canvas;

    protected Card[] cards;


    void Awake()
    {
        cards = GetComponentsInChildren<Card>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cards.Length > 0)
        {
            StartCoroutine(Organize());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Reveal()
    {

    }

    /// <summary>
    /// Reference: https://answers.unity.com/questions/1359168/wait-for-seconds-inside-for-loop.html
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Organize()
    {

        Dictionary<string, Vector3> originPositions = new Dictionary<string, Vector3>();
        float lastPosition = 0;
        // var canvasPosition = Camera.main.ScreenToWorldPoint(canvas.transform.position);

        foreach (var card in cards)
        {
            float newX = 0;
            originPositions.Add(card.name, card.transform.position);

            if (lastPosition == 0)
            {
                newX = -(canvas.pixelRect.center.x + 100);
            }
            else
            {
                newX = lastPosition + 300;
            }
            
            /// <todo> Refactor this to group move/reveal into a single Action
            yield return Move(card.gameObject, newX);
            card.Reveal();

            lastPosition = (card.transform as RectTransform).anchoredPosition.x;
            
        }
    }

    YieldInstruction Move(GameObject obj, float xAxis)
    {
        Tween myTween = obj.transform.DOLocalMoveX(xAxis, 1.5f);
        return myTween.WaitForCompletion();
    }
}

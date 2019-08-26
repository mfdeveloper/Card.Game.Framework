using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

namespace CardFramework
{
    public class ShowUI : MonoBehaviour
    {
        public enum AnimationType
        {
            INDICATE,
            TOP_TO_DOWN
        }

        public float duration = 1;

        public float xAxis = 120;

        public AnimationType animationType = AnimationType.TOP_TO_DOWN;

        protected Dictionary<AnimationType, Func<Tween>> animations = new Dictionary<AnimationType, Func<Tween>>();

        void Awake()
        {
            animations.Add(AnimationType.INDICATE, Indicate);
            animations.Add(AnimationType.TOP_TO_DOWN, TopToDown);
        }

        // Start is called before the first frame update
        void Start()
        {
            Func<Tween> fn;
            if (animations.TryGetValue(animationType, out fn))
            {
                fn();
            }
        }

        public virtual Sequence Indicate()
        {
            Sequence sequence = DOTween.Sequence();

            // Runs this sequence animations, Infinitely
            return sequence.Append(
                transform.DOBlendableLocalMoveBy(new Vector2(xAxis, 0), duration)
            )
            .Append(
                transform.DOBlendableLocalMoveBy(new Vector2(-xAxis, 0), duration / 2)
            )
            .SetLoops(-1);
        }

        public virtual Tween TopToDown()
        {
            return transform.DOBlendableLocalMoveBy(new Vector2(0, -xAxis), duration);
        }
    }
}


using UnityEngine;
using DG.Tweening;
using System;

public class DOTweenSlideAnimator : ISlideAnimator
{
    private const float k_SlideDistance = 600f;

    public void SlideIn(RectTransform target, float duration, Action onComplete = null)
    {
        Vector2 startPos = target.anchoredPosition;
        Vector2 offScreenPos = startPos + Vector2.down * k_SlideDistance;

        // 初期位置を画面外に設定
        target.anchoredPosition = offScreenPos;

        // DOTweenでスライドイン
        target
            .DOAnchorPos(startPos, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void SlideOut(RectTransform target, float duration, Action onComplete = null)
    {
        Vector2 currentPos = target.anchoredPosition;
        Vector2 offScreenPos = currentPos + Vector2.down * k_SlideDistance;

        // DOTweenでスライドアウト
        target
            .DOAnchorPos(offScreenPos, duration)
            .SetEase(Ease.InCubic)
            .OnComplete(() => onComplete?.Invoke());
    }
}

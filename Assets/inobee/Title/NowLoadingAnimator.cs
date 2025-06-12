using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class NowLoadingAnimator : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _loadingText;

    [SerializeField]
    private RectTransform _chibiIcon;

    [SerializeField]
    private FloatingDecoSpawner _decoSpawner;

    private void Start()
    {
        AnimateText();
        AnimateChibiIcon();
        InvokeRepeating(nameof(SpawnDeco), 0f, 0.5f);
    }

    private void AnimateText()
    {
        // テキストをバウンスさせる
        _loadingText.transform
            .DOScale(Vector3.one * 1.1f, 0.4f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    private void AnimateChibiIcon()
    {
        // キャラをぷるぷるさせる
        _chibiIcon
            .DOAnchorPosX(_chibiIcon.anchoredPosition.x + 20f, 0.4f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void SpawnDeco()
    {
        _decoSpawner.SpawnFloatingDeco();
    }
}

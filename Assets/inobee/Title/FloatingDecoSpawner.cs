using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FloatingDecoSpawner : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _floatingIcons; // 星やハートのスプライト

    [SerializeField]
    private RectTransform _spawnArea; // スポーン範囲

    [SerializeField]
    private GameObject _decoPrefab; // 飾り用Imageプレハブ

    public void SpawnFloatingDeco()
    {
        var icon = Instantiate(_decoPrefab, transform);
        var image = icon.GetComponent<Image>();
        image.sprite = _floatingIcons[Random.Range(0, _floatingIcons.Length)];

        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(
            Random.Range(-_spawnArea.rect.width / 2f, _spawnArea.rect.width / 2f),
            0f
        );
        rt.localScale = Vector3.one * Random.Range(0.5f, 1.2f);

        float floatHeight = Random.Range(100f, 200f);
        float duration = Random.Range(2f, 3f);

        rt.DOAnchorPosY(rt.anchoredPosition.y + floatHeight, duration).SetEase(Ease.OutSine);
        image.DOFade(0f, duration).OnComplete(() => Destroy(icon));
    }
}

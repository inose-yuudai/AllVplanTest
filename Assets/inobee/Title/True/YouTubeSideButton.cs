using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class YouTubeSideButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField]
    private Button _button;

    [SerializeField]
    private TextMeshProUGUI _buttonText;

    [SerializeField]
    private Image _backgroundImage;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Header("Visual Settings")]
    [SerializeField]
    private Color _normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

    [SerializeField]
    private Color _selectedColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [SerializeField]
    private float _animationDuration = 0.2f;

    private MenuItemData _menuData;
    private IMenuCommand _command;
    private int _buttonIndex;
    private bool _isSelected = false;
    private bool _isStickyMode = false;

    public event Action<int> OnButtonClicked;
    public event Action<int> OnButtonHovered;

    public void Initialize(MenuItemData menuData, IMenuCommand command, int index)
    {
        _menuData = menuData;
        _command = command;
        _buttonIndex = index;

        SetupUI();
        SetupEvents();
    }

    private void SetupUI()
    {
        _buttonText.text = _menuData.DisplayText;
        _backgroundImage.color = _normalColor;
        _button.interactable = _menuData.IsEnabled;

        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void SetupEvents()
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => OnButtonClicked?.Invoke(_buttonIndex));

        // ホバー検出
        var eventTrigger = gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((_) => OnButtonHovered?.Invoke(_buttonIndex));
        eventTrigger.triggers.Add(pointerEnter);
    }

    public void SetSelected(bool selected)
    {
        if (_isSelected == selected)
            return;

        _isSelected = selected;

        Color targetColor = selected ? _selectedColor : _normalColor;
        float targetScale = selected ? 1.05f : 1f;

        _backgroundImage.DOColor(targetColor, _animationDuration);
        transform.DOScale(Vector3.one * targetScale, _animationDuration).SetEase(Ease.OutBack);
    }

    public void UpdateStickyState(bool shouldBeSticky, float scrollProgress)
    {
        _isStickyMode = shouldBeSticky;

        // スティッキー状態での透明度制御
        float targetAlpha = shouldBeSticky ? 1f : Mathf.Lerp(0.3f, 1f, 1f - scrollProgress);
        _canvasGroup.DOFade(targetAlpha, 0.2f);
    }

    public void ExecuteCommand()
    {
        if (!_menuData.IsEnabled)
            return;

        // 実行時のフィードバック
        transform
            .DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f)
            .OnComplete(() => _command?.Execute());
    }
}

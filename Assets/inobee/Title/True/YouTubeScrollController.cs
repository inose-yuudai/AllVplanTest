using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class YouTubeScrollController : MonoBehaviour
{
    [Header("Main Areas")]
    [SerializeField]
    private RectTransform _mainContainer; // 全体のスクロールコンテナ

    [SerializeField]
    private RectTransform _videoArea; // 動画エリア

    [SerializeField]
    private RectTransform _commentArea; // コメントエリア

    [SerializeField]
    private ScrollRect _mainScrollRect; // メインスクロール

    [Header("Side Menu")]
    [SerializeField]
    private RectTransform _sideButtonContainer; // 右側ボタンコンテナ

    [SerializeField]
    private GameObject _menuButtonPrefab; // ボタンプレハブ

    [SerializeField]
    private MenuItemData[] _menuItems; // メニューデータ

    [Header("Scroll Settings")]
    [SerializeField]
    private float _scrollSpeed = 500f; // スクロール速度

    [SerializeField]
    private float _pageScrollAmount = 0.3f; // 1ページ分のスクロール量

    [SerializeField]
    private float _smoothScrollDuration = 0.5f; // スムーススクロール時間

    [Header("Sticky Button Settings")]
    [SerializeField]
    private float _stickyStartPosition = 0.2f; // スティッキー開始位置

    [SerializeField]
    private float _stickyEndPosition = 0.8f; // スティッキー終了位置

    [Header("Comment System")]
    [SerializeField]
    private CommentManager _commentManager; // コメント管理

    private readonly List<YouTubeSideButton> _sideButtons = new();
    private int _selectedButtonIndex = 0;
    private bool _isScrolling = false;
    private float _lastInputTime = 0f;
    private const float k_InputCooldown = 0.1f;

    // スクロール状態の監視用
    private float _currentScrollPosition = 0f; // 0: 最上部, 1: 最下部

    private void Awake()
    {
        InitializeComponents();
        CreateSideButtons();
        SetupScrollBehavior();
    }

    private void InitializeComponents()
    {
        // ScrollRectの設定
        _mainScrollRect.horizontal = false;
        _mainScrollRect.vertical = true;
        _mainScrollRect.scrollSensitivity = _scrollSpeed;

        // スクロール監視
        _mainScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void CreateSideButtons()
    {
        for (int i = 0; i < _menuItems.Length; i++)
        {
            CreateSideButton(_menuItems[i], i);
        }

        // 初期選択設定
        if (_sideButtons.Count > 0)
        {
            SetSelectedButton(0);
        }
    }

    private void CreateSideButton(MenuItemData menuData, int index)
    {
        GameObject buttonObj = Instantiate(_menuButtonPrefab, _sideButtonContainer);
        YouTubeSideButton sideButton = buttonObj.GetComponent<YouTubeSideButton>();

        // ボタン初期化
        IMenuCommand command = MenuCommandFactory.CreateCommand(menuData.Type);
        sideButton.Initialize(menuData, command, index);

        // イベント購読
        sideButton.OnButtonClicked += OnSideButtonClicked;
        sideButton.OnButtonHovered += OnSideButtonHovered;

        _sideButtons.Add(sideButton);

        // 生成アニメーション
        AnimateSideButtonSpawn(buttonObj, index);
    }

    private void AnimateSideButtonSpawn(GameObject buttonObj, int index)
    {
        float delay = index * 0.1f;
        buttonObj.transform.localScale = Vector3.zero;

        buttonObj.transform.DOScale(Vector3.one, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
    }

    private void Update()
    {
        HandleInput();
        UpdateStickyButtons();
    }

    private void HandleInput()
    {
        if (Time.time - _lastInputTime < k_InputCooldown || _isScrolling)
            return;

        // キーボード入力
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            ScrollPage(1); // 下にスクロール
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ScrollPage(-1); // 上にスクロール
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            NavigateSideMenu(-1); // 左（上のボタン）
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NavigateSideMenu(1); // 右（下のボタン）
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteSelectedButton();
        }

        // マウスホイール入力
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            HandleMouseScroll(scroll);
        }
    }

    private void ScrollPage(int direction)
    {
        _lastInputTime = Time.time;

        float targetScrollValue =
            _mainScrollRect.verticalNormalizedPosition - (direction * _pageScrollAmount);
        targetScrollValue = Mathf.Clamp01(targetScrollValue);

        SmoothScrollTo(targetScrollValue);
    }

    private void HandleMouseScroll(float scrollDelta)
    {
        // 通常のScrollRectの挙動を活用
        // 特別な処理は不要、ScrollRectが自動的に処理
    }

    private void SmoothScrollTo(float targetValue)
    {
        if (_isScrolling)
            return;

        _isScrolling = true;

        DOTween
            .To(
                () => _mainScrollRect.verticalNormalizedPosition,
                value => _mainScrollRect.verticalNormalizedPosition = value,
                targetValue,
                _smoothScrollDuration
            )
            .SetEase(Ease.OutCubic)
            .OnComplete(() => _isScrolling = false);
    }

    private void OnScrollValueChanged(Vector2 scrollPosition)
    {
        _currentScrollPosition = 1f - scrollPosition.y; // 0: 最上部, 1: 最下部

        // コメントの無限スクロールチェック
        if (_currentScrollPosition > 0.9f) // 90%スクロールしたら
        {
            // スクロール位置を保存してからコメント読み込み
            StartCoroutine(LoadMoreCommentsWithPositionFix());
        }
    }

    private IEnumerator LoadMoreCommentsWithPositionFix()
    {
        // 現在のスクロール位置を記録
        float savedScrollPosition = _mainScrollRect.verticalNormalizedPosition;

        // コメント読み込み
        _commentManager?.LoadMoreComments();

        // 1フレーム待機してレイアウト更新を待つ
        yield return null;
        yield return null; // 念のため2フレーム待機

        // スクロール位置を復元
        _mainScrollRect.verticalNormalizedPosition = savedScrollPosition;
    }

    private void UpdateStickyButtons()
    {
        // スクロール位置に応じてボタンの表示状態を更新
        bool shouldShowSticky =
            _currentScrollPosition >= _stickyStartPosition
            && _currentScrollPosition <= _stickyEndPosition;

        // スティッキー表示の制御
        foreach (var button in _sideButtons)
        {
            button.UpdateStickyState(shouldShowSticky, _currentScrollPosition);
        }
    }

    private void NavigateSideMenu(int direction)
    {
        _lastInputTime = Time.time;

        int newIndex = _selectedButtonIndex + direction;

        // ループ処理
        if (newIndex >= _sideButtons.Count)
            newIndex = 0;
        else if (newIndex < 0)
            newIndex = _sideButtons.Count - 1;

        SetSelectedButton(newIndex);
    }

    private void SetSelectedButton(int index)
    {
        if (index == _selectedButtonIndex)
            return;

        // 前の選択を解除
        if (_selectedButtonIndex < _sideButtons.Count)
        {
            _sideButtons[_selectedButtonIndex].SetSelected(false);
        }

        // 新しい選択を設定
        _selectedButtonIndex = Mathf.Clamp(index, 0, _sideButtons.Count - 1);
        _sideButtons[_selectedButtonIndex].SetSelected(true);
    }

    private void ExecuteSelectedButton()
    {
        if (_selectedButtonIndex < _sideButtons.Count)
        {
            _sideButtons[_selectedButtonIndex].ExecuteCommand();
        }
    }

    private void OnSideButtonClicked(int buttonIndex)
    {
        SetSelectedButton(buttonIndex);
        ExecuteSelectedButton();
    }

    private void OnSideButtonHovered(int buttonIndex)
    {
        SetSelectedButton(buttonIndex);
    }

    private void SetupScrollBehavior()
    {
        // Content Sizze Fitterを設定して動的なサイズ調整
        ContentSizeFitter contentSizeFitter = _mainContainer.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter == null)
        {
            contentSizeFitter = _mainContainer.gameObject.AddComponent<ContentSizeFitter>();
        }

        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // VerticalLayoutGroupで要素を縦に並べる
        VerticalLayoutGroup layoutGroup = _mainContainer.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = _mainContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;
    }
}

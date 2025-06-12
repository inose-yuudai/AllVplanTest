using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class CommentManager : MonoBehaviour
{
    [Header("Comment Settings")]
    [SerializeField]
    private Transform _commentContainer;

    [SerializeField]
    private GameObject _commentPrefab;

    [SerializeField]
    private int _initialCommentCount = 10;

    [SerializeField]
    private int _loadMoreCount = 5;

    [Header("Loading")]
    [SerializeField]
    private GameObject _loadingIndicator;

    private readonly List<CommentData> _allComments = new();
    private readonly List<GameObject> _commentObjects = new();
    private int _currentLoadedCount = 0;
    private bool _isLoading = false;

    [System.Serializable]
    public class CommentData
    {
        public string userName;
        public string commentText;
        public string timeAgo;
        public int likeCount;
        public Sprite userAvatar;
    }

    private void Start()
    {
        GenerateSampleComments();
        LoadInitialComments();
    }

    private void GenerateSampleComments()
    {
        // サンプルコメントデータを生成
        string[] sampleUsers = { "ユーザー1", "ゲーマー太郎", "配信好き", "コメンター", "視聴者A" };
        string[] sampleComments =
        {
            "面白い動画ですね！",
            "続きが気になります",
            "音質がとても良いです",
            "BGMが素敵ですね",
            "また見に来ます！"
        };

        for (int i = 0; i < 50; i++) // 50個のサンプルコメント
        {
            var comment = new CommentData
            {
                userName = sampleUsers[i % sampleUsers.Length],
                commentText = sampleComments[i % sampleComments.Length],
                timeAgo = $"{Random.Range(1, 60)}分前",
                likeCount = Random.Range(0, 100)
            };
            _allComments.Add(comment);
        }
    }

    private void LoadInitialComments()
    {
        LoadComments(_initialCommentCount);
    }

    public void LoadMoreComments()
    {
        if (_isLoading || _currentLoadedCount >= _allComments.Count)
            return;

        StartCoroutine(LoadCommentsCoroutine());
    }

    private IEnumerator LoadCommentsCoroutine()
    {
        _isLoading = true;

        if (_loadingIndicator != null)
            _loadingIndicator.SetActive(true);

        // ローディング演出のため少し待機
        yield return new WaitForSeconds(0.5f);

        LoadComments(_loadMoreCount);

        if (_loadingIndicator != null)
            _loadingIndicator.SetActive(false);

        _isLoading = false;
    }

    private void LoadComments(int count)
    {
        int endIndex = Mathf.Min(_currentLoadedCount + count, _allComments.Count);

        for (int i = _currentLoadedCount; i < endIndex; i++)
        {
            CreateCommentUI(_allComments[i]);
        }

        _currentLoadedCount = endIndex;
    }

    private void CreateCommentUI(CommentData commentData)
    {
        GameObject commentObj = Instantiate(_commentPrefab, _commentContainer);

        // コメントUIの設定
        var commentUI = commentObj.GetComponent<CommentUI>();
        if (commentUI != null)
        {
            commentUI.SetupComment(commentData);
        }

        _commentObjects.Add(commentObj);

        // 生成アニメーション
        commentObj.transform.localScale = Vector3.zero;
        commentObj.transform.DOScale(Vector3.one, 0.3f).SetEase(DG.Tweening.Ease.OutBack);
    }
}

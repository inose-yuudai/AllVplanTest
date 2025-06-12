using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CommentUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField]
    private Image _avatarImage;

    [SerializeField]
    private TMP_Text _userNameText;

    [SerializeField]
    private TMP_Text _commentText;

    [SerializeField]
    private TMP_Text _timeAgoText;

    [SerializeField]
    private TMP_Text _likeCountText;

    [SerializeField]
    private Button _likeButton;

    public void SetupComment(CommentManager.CommentData commentData)
    {
        _userNameText.text = commentData.userName;
        _commentText.text = commentData.commentText;
        _timeAgoText.text = commentData.timeAgo;
        _likeCountText.text = commentData.likeCount.ToString();

        if (commentData.userAvatar != null && _avatarImage != null)
        {
            _avatarImage.sprite = commentData.userAvatar;
        }

        // いいねボタンの設定
        if (_likeButton != null)
        {
            _likeButton.onClick.AddListener(() => OnLikeClicked(commentData));
        }
    }

    private void OnLikeClicked(CommentManager.CommentData commentData)
    {
        // いいね機能の実装
        commentData.likeCount++;
        _likeCountText.text = commentData.likeCount.ToString();

        // いいねアニメーション
        _likeButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 1, 0.5f);

    }
}

using UnityEngine;
using System;

public interface IMenuCommand
{
    void Execute();
    bool CanExecute();
}

public interface ISlideAnimator
{
    void SlideIn(RectTransform target, float duration, Action onComplete = null);
    void SlideOut(RectTransform target, float duration, Action onComplete = null);
}

public interface IFocusable
{
    void SetFocus(bool focused);
    bool CanFocus { get; }
}

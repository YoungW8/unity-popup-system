using System;

/// <summary>
/// Minimal event bus stub required by PopupController.
/// If your project already has its own EventBus with OnUIPause/OnUIResume,
/// delete this file and use the project one instead.
/// </summary>
public static class EventBus
{
    public static Action OnUIPause;
    public static void RaiseOnUIPause() => OnUIPause?.Invoke();
    public static void SubscribeOnUIPause(Action action) => OnUIPause += action;
    public static void UnsubscribeOnUIPause(Action action) => OnUIPause -= action;

    public static Action OnUIResume;
    public static void RaiseOnUIResume() => OnUIResume?.Invoke();
    public static void SubscribeOnUIResume(Action action) => OnUIResume += action;
    public static void UnsubscribeOnUIResume(Action action) => OnUIResume -= action;
}

using UnityEngine;

/// <summary>
/// Close-animation stub referenced by Popup.
/// In the original project this is a "bubble" scale animation played before
/// the popup is deactivated. Popup only calls StartAnimation() when this
/// component is present on the popup object.
/// </summary>
public class UIA_Bubble : MonoBehaviour
{
    public Coroutine StartAnimation() => null;
}

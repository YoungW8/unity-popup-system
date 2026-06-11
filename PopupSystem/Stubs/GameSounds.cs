using UnityEngine;

/// <summary>
/// Minimal sound system stub required by Popup (open/close/additional button sounds).
/// Replace the Play() implementation with your project's audio system,
/// or delete this file if your project already has a GameSounds class.
/// </summary>
public class GameSounds : MonoBehaviour
{
    public static GameSounds instance;

    private void Awake()
    {
        instance = this;
    }

    public void Play(string soundName, float volume = 1f)
    {
        // Hook your audio system here.
    }
}

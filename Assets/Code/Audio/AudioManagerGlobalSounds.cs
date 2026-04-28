
/// <summary>
/// Utility methods to play certain sounds (for maintainability) that we need everywhere (mainly to avoid duplicating a string 1000000 times)
/// </summary>
internal static class AudioManagerGlobalSounds
{
    private const string UI_CLICK_PATH = "UI_Click";
    private const string UI_WRONG_PATH = "UI_Wrong";

    internal static void PlayUIClickSound(float volume = 1.0f)
    {
        AudioManager.PlayAudioAtCameraPosition(UI_CLICK_PATH, volume);
    }

    internal static void PlayUIWrongSound(float volume = 1.0f)
    {
        AudioManager.PlayAudioAtCameraPosition(UI_WRONG_PATH, volume);
    }
}
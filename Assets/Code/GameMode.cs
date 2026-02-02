using UnityEngine; 

// This is the base class for game modes.
internal abstract class GameMode
{
    // todo: figure out some kind of fixed tick update system

    internal abstract void OnEnter();
    internal abstract void OnFrame();
    internal abstract void OnFixedUpdate();
    internal abstract void OnLeave();

    // Legacy GUI (good for debug/temp uis)
    internal virtual void OnLegacyGUI() { } // not needed
}
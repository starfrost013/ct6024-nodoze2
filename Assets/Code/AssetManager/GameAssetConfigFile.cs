using UnityEngine; 

// This is the same as a textasset but wrapped in our tracking system
// I would go my own way and inherit directly from unityengine.object and put postload in a base class etc but then i couldn't inherit frrom any unity behaviours
// Also, the unity TextAsset class, is partially written in c++
internal class GameAssetConfigFile : TextAsset 
{
    
}
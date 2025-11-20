using UnityEngine;

// This thingy is a connector between unity and our fancy stuff

public class GameManagerObject : MonoBehaviour
{
    GameManager manager; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = new GameManager();

        manager.SetGameState(GameManager.GameState.Init);
    }

    // Update is called once per frame
    void Update()
    {
        manager.OnFrame();        
    }
}

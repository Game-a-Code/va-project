using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void Respawn()
    {
        SavingSystem.i.Load("saveSlot1");
        GameController.Instance.Respawned();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}

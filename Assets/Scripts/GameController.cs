using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Menu, Paused }

public class GameController : MonoBehaviour
{
  [SerializeField] private PlayerController playerController;
  [SerializeField] private Camera worldCamera;
  
  private GameState state;
  private GameState stateBeforePause;

  public SceneDetails CurrentScene { get; private set;  }
  public SceneDetails PrevScene { get; private set;  }

  public static GameController Instance { get; private set; }
  private void Awake()
  {
    Instance = this;
  }
  
  public void PauseGame(bool pause)
  {
    if (pause)
    {
      stateBeforePause = state;
      state = GameState.Paused;
    }
    else
    {
      state = stateBeforePause;
    }
  }
  
  public void SetCurrentScene(SceneDetails currScene)
  {
    PrevScene = CurrentScene;
    CurrentScene = currScene;
  }
}
using System;
using UnityEngine;
using UnityEngine.UI;

// TODO: Auto deactivate BattleSystem etc. before game starts

public enum GameState { FreeRoam, Battle, Dialog, Menu, Cutscene, Paused }

public class GameController : MonoBehaviour
{
  [SerializeField] private PlayerController playerController;
  [SerializeField] private BattleSystem battleSystem;
  [SerializeField] private Camera worldCamera;
  [SerializeField] private Text areaText;

  private GameState state;
  private GameState stateBeforePause;

  public SceneDetails CurrentScene { get; private set;  }
  public SceneDetails PrevScene { get; private set;  }

  public static GameController Instance { get; private set; }
  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    battleSystem.OnBattleOver += EndBattle;
    
    DialogManager.Instance.OnShowDialog += () =>
    {
      state = GameState.Dialog;
    };

    DialogManager.Instance.OnCloseDialog += () =>
    {
      if (state == GameState.Dialog)
        state = GameState.FreeRoam;
    };
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
  private EnemyController trainer;
  public void StartBattle(EnemyController enemyCon)
  {
    state = GameState.Battle;
    battleSystem.gameObject.SetActive(true);
    worldCamera.gameObject.SetActive(false);
    
    trainer = enemyCon;
    
    battleSystem.StartBattle(enemyCon);
  }
  
  public void OnEnterTrainersView(EnemyController enemyCon)
  {
    state = GameState.Cutscene;
    StartCoroutine(enemyCon.TriggerTrainerBattle(playerController));
  }

  private void EndBattle(bool won)
  {
    if (won /* == true */)
    {
      trainer.BattleLost();
    }
    
    state = GameState.FreeRoam;
    battleSystem.gameObject.SetActive(false);
    worldCamera.gameObject.SetActive(true);
  }
  
  private void Update()
  {
    if (state == GameState.FreeRoam)
    {
      playerController.HandleUpdate();
    }
    else if (state == GameState.Battle)
    {
      // battleSystem.HandleUpdate();
    }
    else if (state == GameState.Dialog)
    {
      DialogManager.Instance.HandleUpdate();
    }
  }
  
  public void SetCurrentScene(SceneDetails currScene)
  {
    PrevScene = CurrentScene;
    CurrentScene = currScene;
  }
  
  public void SetAreaText(string areaName)
  {
    areaText.text = areaName;
  }
  
}
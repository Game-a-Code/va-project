
using UnityEngine;
using UnityEngine.UI;

// TODO: Auto deactivate BattleSystem etc. before game starts

public enum GameState { FreeRoam, Battle, GameOver, Dialog, Menu, Cutscene, StartingCut, Paused, Credits }

public class GameController : MonoBehaviour
{
  [SerializeField] private PlayerController playerController;
  [SerializeField] private BattleSystem battleSystem;
  [SerializeField] private Camera worldCamera;
  [SerializeField] private Text areaText;
  [SerializeField] private GameObject credits;
  [SerializeField] private GameObject gameOver;

  private GameState state;
  private GameState stateBeforePause;

  public SceneDetails CurrentScene { get; private set; }
  public SceneDetails PrevScene { get; private set; }

  private MenuController menuController;
  private CutsceneController cutsceneController;

  public static GameController Instance { get; private set; }
  private void Awake()
  {
    Instance = this;

    menuController = GetComponent<MenuController>();
    cutsceneController = GetComponent<CutsceneController>();

    /* if (PlayerController.FirstGamePlay)
    {
      state = GameState.StartingCut;
    }*/
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

    menuController.onBack += () =>
    {
      state = GameState.FreeRoam;
    };

    menuController.onMenuSelected += OnMenuSelected;
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
      playerController.LevelUp(trainer.EnemyPrefab.GetComponent<BattleUnit>().dificulty);
    }
    else
    {
      state = GameState.GameOver;
      // SavingSystem.i.Load("saveSlot1");
    }
    
    if (state == GameState.Battle)
      state = GameState.FreeRoam;
    battleSystem.gameObject.SetActive(false);
    worldCamera.gameObject.SetActive(true);
  }
  
  private void Update()
  {
    if (state == GameState.FreeRoam)
    {
      playerController.HandleUpdate();

      if (Input.GetKeyDown(Settings.I.Return))
      {
        menuController.OpenMenu();
        state = GameState.Menu;
      }
    }
    else if (state == GameState.Battle)
    {
      // battleSystem.HandleUpdate();
    }
    else if (state == GameState.GameOver)
    {
      GameOver();
    }
    else if (state == GameState.Dialog)
    {
      DialogManager.Instance.HandleUpdate();
    }
    else if (state == GameState.Menu)
    {
      menuController.HandleUpdate();
    }
    else if (state == GameState.StartingCut)
    {
      cutsceneController.HandleUpdate();
    }
    else if (state == GameState.Credits)
    {
      // Yay Credits
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
  
  public void ActivateCredits()
  {
    state = GameState.Credits;
    credits.GetComponent<Credits>().SetText();
    credits.SetActive(true);
  }
  
  public void GameOver()
  {
    state = GameState.GameOver;
    gameOver.SetActive(true);
  }
  
  public void Respawned()
  {
    state = GameState.FreeRoam;
    gameOver.SetActive(false);
  }

  void OnMenuSelected(int selectedItem)
  {
    if (selectedItem == 0)
    {
      // Save
      SavingSystem.i.Save("saveSlot1");
    }
    else if (selectedItem == 1)
    {
      // Load
      SavingSystem.i.Load("saveSlot1");
    }
    else if (selectedItem == 2)
    {
      // Exit + Save
      SavingSystem.i.Save("saveSlot1");
      Application.Quit();
    }

    state = GameState.FreeRoam;
  }
  
}
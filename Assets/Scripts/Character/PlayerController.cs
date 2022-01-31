using System;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable once Unity.PreferNonAllocApi

public class PlayerController : MonoBehaviour, ISavable
{
  [SerializeField] private new string name;
  [SerializeField] private Sprite sprite;
  public static int BossesBeaten;
  public static bool FirstGamePlay = true;
  public static int EasterEggsFound;

  public GameObject playerPrefab;

  private Vector2 input;

  private Character character;
  
  private void Awake()
  {
    character = GetComponent<Character>();
  }

  public void HandleUpdate()
  {
    if (!character.IsMoving)
    {
      input.x = Input.GetAxisRaw("Horizontal");
      input.y = Input.GetAxisRaw("Vertical");

      // remove diagonal movement
      if (input.x != 0) input.y = 0;

      if (input != Vector2.zero)
      {
        StartCoroutine(character.Move(input, OnMoveOver));
      }
      
      character.HandleUpdate();
      
      if (Input.GetKeyDown(KeyCode.Z))
        Interact();
    }
  }

  private void Interact()
  {
    var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
    var interactPos = transform.position + facingDir;
    
    // Draws a Line from your Position to the block your facing
    // Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

    var overlapCircle = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.I.InteractableLayer);
    if (overlapCircle != null)
    {
      overlapCircle.GetComponent<Interactable>()?.Interact(transform);
    }
  }
  
  private void OnMoveOver()
  {
    var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.I.TriggerableLayers);

    foreach (var collider2D1 in colliders)
    {
      var triggerable = collider2D1.GetComponent<IPlayerTriggerable>();
      if (triggerable != null)
      {
        triggerable.OnPlayerTriggered(this);
        break;
      }
    }
  }
  
  public string Name => name;
  public Sprite Sprite => sprite;
  public Character Character => character;
  
  public object CaptureState()
  {
    var saveData = new PlayerSaveData()
    {
      // Used to be new Float[] (but Rider recommended it)
      position = new[] {transform.position.x, transform.position.y},
      
      // Restore Level + HP
      unitLevel = playerPrefab.GetComponent<BattleUnit>().unitLevel,
      damage = playerPrefab.GetComponent<BattleUnit>().damage,
      healing = playerPrefab.GetComponent<BattleUnit>().healing,
      currentHP = playerPrefab.GetComponent<BattleUnit>().currentHP,
      maxHP = playerPrefab.GetComponent<BattleUnit>().maxHP,
      bosses = BossesBeaten,
      firstGame = FirstGamePlay,
      easterEggs = EasterEggsFound
    };
    
    return saveData;
  }

  public void RestoreState(object state)
  {
    var saveData = (PlayerSaveData) state;

    // Restore Position
    var pos = saveData.position;
    transform.position = new Vector3(pos[0], pos[1]);
    
    // Restore Level + HP
    playerPrefab.GetComponent<BattleUnit>().unitLevel = saveData.unitLevel;
    playerPrefab.GetComponent<BattleUnit>().damage = saveData.damage;
    playerPrefab.GetComponent<BattleUnit>().healing = saveData.healing;
    playerPrefab.GetComponent<BattleUnit>().currentHP = saveData.currentHP;
    playerPrefab.GetComponent<BattleUnit>().maxHP = saveData.maxHP;
    
    // Restore how many bosses beaten
    BossesBeaten = saveData.bosses;
    FirstGamePlay = saveData.firstGame;
    EasterEggsFound = saveData.easterEggs;
  }

  public void LevelUp(int dificulty)
  {
    int tempLevel = playerPrefab.GetComponent<BattleUnit>().unitLevel;
    int tempDamage = playerPrefab.GetComponent<BattleUnit>().damage;
    int tempHealing = playerPrefab.GetComponent<BattleUnit>().healing;
    int tempMaxHp = playerPrefab.GetComponent<BattleUnit>().maxHP;
    int tempBosses = BossesBeaten;

    if (dificulty == 0)
    {
      tempLevel += 1;
      tempDamage += 5;
      tempHealing += 5;
      tempMaxHp += 15;
    }
    else if (dificulty == 1)
    {
      tempLevel += 6;
      tempDamage += 30;
      tempHealing += 30;
      tempMaxHp += 90;
      tempBosses += 1;
    }

    playerPrefab.GetComponent<BattleUnit>().unitLevel = tempLevel;
    playerPrefab.GetComponent<BattleUnit>().damage = tempDamage;
    playerPrefab.GetComponent<BattleUnit>().healing = tempHealing;
    playerPrefab.GetComponent<BattleUnit>().currentHP = tempMaxHp;
    playerPrefab.GetComponent<BattleUnit>().maxHP = tempMaxHp;
    BossesBeaten = tempBosses;
  }
}

[Serializable]
public class PlayerSaveData
{
  public float[] position;
  public int unitLevel;
  public int damage;
  public int healing;
  public int currentHP;
  public int maxHP;
  public int bosses;
  public bool firstGame;
  public int easterEggs;
}

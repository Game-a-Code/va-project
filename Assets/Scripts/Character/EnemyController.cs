using System;
using System.Collections;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable once Unity.PreferNonAllocApi

public class EnemyController : MonoBehaviour, Interactable, ISavable
{
  [SerializeField] private Dialog dialog;
  [SerializeField] private Dialog dialogAfterBattle;
  [SerializeField] private GameObject exclamation;
  [SerializeField] private GameObject fov;
  
  // State
  private bool battleLost;

  private Character character;

  public GameObject EnemyPrefab;
  
  private void Awake()
  {
    character = GetComponent<Character>();
  }
/*
  private void Start()
  {
    var enemyUnit = EnemyPrefab.GetComponent<BattleUnit>();
    enemyUnit.unitName = name;
  }
  */
  private void Update()
  {
    character.HandleUpdate();
  }

  public void Interact(Transform initiator)
  {
    character.LookTowards(initiator.position);

    if (!battleLost)
    {
      // Show dialog
      StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
      {
        GameController.Instance.StartBattle(this);
      }));
    }
    else
    {
      StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterBattle));
    }
  }
  
  public void BattleLost()
  {
    battleLost = true;
    fov.gameObject.SetActive(false);
    gameObject.GetComponent<Collider2D>().enabled = false;
    gameObject.GetComponent<SpriteRenderer>().enabled = false;
  }
  

  public IEnumerator TriggerTrainerBattle(PlayerController player)
  {
    // Show Exclamation
    exclamation.SetActive(true);
    yield return new WaitForSeconds(0.5f);
    exclamation.SetActive(false);
    
    // Walk towards the player
    var diff = player.transform.position - transform.position;
    var moveVec = diff - diff.normalized;
    moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

    yield return character.Move(moveVec);
    
    // Show dialog
    StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
    {
      GameController.Instance.StartBattle(this);
    }));
  }

  public object CaptureState()
  {
    var saveData = new EnemySaveData()
    {
      // Used to be new Float[] (but Rider recommended it)
      position = new[] {transform.position.x, transform.position.y},
      lost = battleLost
    };
    
    return saveData;
  }

  public void RestoreState(object state)
  {
    var saveData = (EnemySaveData)state;
    
    // Restore Position
    var pos = saveData.position;
    transform.position = new Vector3(pos[0], pos[1]);
    
    // Restore battleLost
    battleLost = saveData.lost;
    
    switch (battleLost)
    {
      case true:
        fov.gameObject.SetActive(false);
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        break;
      case false:
        fov.gameObject.SetActive(true);
        gameObject.GetComponent<Collider2D>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        break;
    }
  }

}

[Serializable]
public class EnemySaveData
{
  public float[] position;
  public bool lost;
}


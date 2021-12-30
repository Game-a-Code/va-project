using System.Collections;
using UnityEngine;

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
    return battleLost;
  }

  public void RestoreState(object state)
  {
    battleLost = (bool) state;
    
    if (battleLost)
      fov.gameObject.SetActive(false);
  }

}

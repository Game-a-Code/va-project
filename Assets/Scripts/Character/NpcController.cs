using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{
  [SerializeField] private Dialog dialog;
  [SerializeField] private Dialog specialDialog;
  [SerializeField] private Dialog endDialog;
  [SerializeField] private List<Vector2> movementPattern;
  [SerializeField] private float timeBetweenPattern;
  [SerializeField] private bool specialCharacter;
  [SerializeField] private GameObject Portal;

  private NpcState state;
  private float idleTimer;
  private int currentPattern;

  private Character character;
  private void Awake()
  {
    character = GetComponent<Character>();
  }

  public void Interact(Transform initiator)
  {
    if (state == NpcState.Idle)
    {
      state = NpcState.Dialog;
      character.LookTowards(initiator.position);
      
      if (specialCharacter && PlayerController.BossesBeaten == 1)
      {
        StartCoroutine(DialogManager.Instance.ShowDialog(specialDialog, () => {
          idleTimer = 0f;
          state = NpcState.Idle;
          // GameController.Instance.ActivateCredits();
          ActivatePortal();
        }));
      }
      else if (specialCharacter && PlayerController.BossesBeaten == 2)
      {
        StartCoroutine(DialogManager.Instance.ShowDialog(endDialog, () => {
          idleTimer = 0f;
          state = NpcState.Idle;
          GameController.Instance.ActivateCredits();
          ActivatePortal();
        }));
      }
      else
      {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
          idleTimer = 0f;
          state = NpcState.Idle;
        }));
      }
    }
  }

  public void ActivatePortal()
  {
    Portal.SetActive(true);
  }
  
  private void Update()
  {
    if (state == NpcState.Idle)
    {
      idleTimer += Time.deltaTime;
      if (idleTimer > timeBetweenPattern)
      {
        idleTimer = 0f;
        if (movementPattern.Count > 0)
          StartCoroutine(Walk());
      }
    }
    
    character.HandleUpdate();
  }

  private IEnumerator Walk()
  {
    state = NpcState.Walking;

    var oldPos = transform.position;

    yield return character.Move(movementPattern[currentPattern]);
    
    if (transform.position != oldPos)
      currentPattern = (currentPattern + 1) % movementPattern.Count;
    
    state = NpcState.Idle;
  }
}

public enum NpcState { Idle, Walking, Dialog }

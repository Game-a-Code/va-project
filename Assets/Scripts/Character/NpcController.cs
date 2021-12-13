using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{
  [SerializeField] private Dialog dialog;
  [SerializeField] private List<Vector2> movementPattern;
  [SerializeField] private float timeBetweenPattern;

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

      StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
        idleTimer = 0f;
        state = NpcState.Idle;
      }));
    }
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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable once Unity.PreferNonAllocApi

public class PlayerController : MonoBehaviour, ISavable
{
  [SerializeField] private new string name;
  [SerializeField] private Sprite sprite;

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
    };
    
    return saveData;
  }

  public void RestoreState(object state)
  {
    var saveData = (PlayerSaveData) state;

    // Restore Position
    var pos = saveData.position;
    transform.position = new Vector3(pos[0], pos[1]);
    
    // Restore Party
  }
}

[Serializable]
public class PlayerSaveData
{
  public float[] position;
}

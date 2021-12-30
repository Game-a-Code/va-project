using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignController : MonoBehaviour, Interactable
{
  [SerializeField] private Dialog dialog;

  // TODO: When you look from the back it shows nothing.
  public void Interact(Transform initiator)
  {
    StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
      }));
  }
}

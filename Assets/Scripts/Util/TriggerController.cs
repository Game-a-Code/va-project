using System;
using System.Collections;
using UnityEngine;

public class TriggerController : MonoBehaviour, ISavable
{
  [SerializeField] private GameObject fov;
  [SerializeField] public bool found;

  public object CaptureState()
  {
    return found;
  }

  public void RestoreState(object state)
  {
    found = (bool)state;

    if (found)
    {
      fov.SetActive(false);
    }
    else
    {
      fov.SetActive(true);
    }
  }

}

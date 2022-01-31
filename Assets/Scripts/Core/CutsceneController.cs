using System;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] private Dialog dialog;
    
    private float idleTimer;

    public void HandleUpdate()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
            idleTimer = 0f;
        })); 
    }
/*
    public void Start()
    {
        // yield return character.Move(movementPattern[currentPattern]);


        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
            idleTimer = 0f;
        })); 
    }
  */  
}

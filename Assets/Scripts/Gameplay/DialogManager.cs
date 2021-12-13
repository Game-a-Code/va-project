using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
  [SerializeField] private GameObject dialogBox;
  [SerializeField] private Text dialogText;
  [SerializeField] private int lettersPerSecond;

  public event Action OnShowDialog;
  public event Action OnCloseDialog;
  
  public static DialogManager Instance { get; private set; }
  private void Awake()
  {
    Instance = this;
  }

  private Dialog dialog;
  private Action onDialogFinished;

  private int currentLine;
  private bool isTyping;
  
  // ReSharper disable once UnusedAutoPropertyAccessor.Global
  public bool IsShowing { get; private set; }
  
  public IEnumerator ShowDialog(Dialog dialogToShow, Action onFinished=null)
  {
    yield return new WaitForEndOfFrame();
    
    OnShowDialog?.Invoke();

    IsShowing = true;
    dialog = dialogToShow;
    onDialogFinished = onFinished;
    
    dialogBox.SetActive(true);
    StartCoroutine(TypeDialog(dialogToShow.Lines[0]));
  }

  public void HandleUpdate()
  {
    if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
    {
      ++currentLine;
      if (currentLine < dialog.Lines.Count)
      {
        StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
      }
      else
      {
        currentLine = 0;
        IsShowing = false;
        dialogBox.SetActive(false);
        onDialogFinished?.Invoke();
        OnCloseDialog?.Invoke();
      }
    }
  }
  
  public IEnumerator TypeDialog(string dialogToType)
  {
    isTyping = true;
    dialogText.text = "";
    foreach (var letter in dialogToType.ToCharArray())
    {
      dialogText.text += letter;
      yield return new WaitForSeconds(1f/lettersPerSecond);
    }
    isTyping = false;
  }
}

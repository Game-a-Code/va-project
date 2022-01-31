using UnityEngine;

public class TriggerFov : MonoBehaviour, IPlayerTriggerable
{
  [SerializeField] private Dialog dialog;
  [SerializeField] private bool easterEgg;

  public void OnPlayerTriggered(PlayerController playerCon)
  {
    if (PlayerController.FirstGamePlay && !easterEgg)
    {
      playerCon.Character.Animator.IsMoving = false;
      StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
      }));
      PlayerController.FirstGamePlay = false;
      SavingSystem.i.Save("saveSlot1");
    }

    if (easterEgg)
    {
      PlayerController.EasterEggsFound += 1;
      GetComponentInParent<TriggerController>().found = true;
      StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => {
        gameObject.SetActive(false);
      }));
    }
  }
}
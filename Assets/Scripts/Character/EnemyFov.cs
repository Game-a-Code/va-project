using UnityEngine;

public class EnemyFov : MonoBehaviour, IPlayerTriggerable
{
  public void OnPlayerTriggered(PlayerController playerCon)
  {
    playerCon.Character.Animator.IsMoving = false;
    GameController.Instance.OnEnterTrainersView(GetComponentInParent<EnemyController>());
  }
}
using System.Collections;
using System.Linq;
using UnityEngine;

// Teleports the player to a different position without switching scenes
public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
  [SerializeField] private DestinationIdentifier destinationPortal;
  [SerializeField] private Transform spawnPoint;

  private PlayerController player;
  
  public void OnPlayerTriggered(PlayerController playerCon)
  {
    playerCon.Character.Animator.IsMoving = false;
    this.player = playerCon;
    StartCoroutine(Teleport());
  }

  private Fader fader;
  private void Start()
  {
    fader = FindObjectOfType<Fader>();
  }

  private IEnumerator Teleport()
  {
    GameController.Instance.PauseGame(true);
    yield return fader.FadeIn(0.5f);
    
    var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
    player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
    
    yield return fader.FadeOut(0.5f);
    GameController.Instance.PauseGame(false);
  }

  public Transform SpawnPoint => spawnPoint;
}
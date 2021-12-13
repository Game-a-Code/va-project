using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
  [SerializeField] private GameObject essentialObjectsPrefab;
  [SerializeField] private int spawnCoordsX = 0;
  [SerializeField] private int spawnCoordsY = 0;

  private void Awake()
  {
    var existingObjects = FindObjectsOfType<EssentialObjects>();
    if (existingObjects.Length == 0)
    {
      // If there is a grid, then spawn at it's center
      var spawnPos = new Vector3(0, 0, 0);

      var grid = FindObjectOfType<Grid>();
      if (grid != null)
        spawnPos = grid.transform.position;

      // If the SpawnPoint was overwritten spawn there.
      if (spawnCoordsX != 0 || spawnCoordsY != 0)
        spawnPos = new Vector3(spawnCoordsX, spawnCoordsY, 0);

      Instantiate(essentialObjectsPrefab, spawnPos, Quaternion.identity);
    }
  }
}
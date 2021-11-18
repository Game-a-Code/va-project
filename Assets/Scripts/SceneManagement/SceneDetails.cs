using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
  [SerializeField] private List<SceneDetails> connectedScenes;

  public bool IsLoaded { get; private set; }

  private List<SavableEntity> savableEntities;
  
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      // TODO: make it show which town u enter.
      // Fade in. show name. Fade Out.
      Debug.Log($"Entered {gameObject.name}");

      LoadScene();
      GameController.Instance.SetCurrentScene(this);
      
      // Load all connected scenes
      foreach (var scene in connectedScenes)
      {
        scene.LoadScene();
      }
      
      // Unload the scenes that are no longer connected
      var prevScene = GameController.Instance.PrevScene;
      if (prevScene != null)
      {
        var previouslyLoadedScenes = prevScene.connectedScenes;
        foreach (var scene in previouslyLoadedScenes)
        {
          if (!connectedScenes.Contains(scene) && scene != this)
            scene.UnLoadScene();
        }
        
        if (!connectedScenes.Contains(prevScene))
          prevScene.UnLoadScene();
      }
    }
  }

  public void LoadScene()
  {
    if (!IsLoaded)
    {
      var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
      IsLoaded = true;

      operation.completed += op =>
      {
        savableEntities = GetSavableEntitiesInScene();
        SavingSystem.i.RestoreEntityStates(savableEntities);
      };
    }
  }
  
  public void UnLoadScene()
  {
    if (IsLoaded)
    {
      SavingSystem.i.CaptureEntityStates(savableEntities);
      
      SceneManager.UnloadSceneAsync(gameObject.name);
      IsLoaded = false;
    }
  }

  List<SavableEntity> GetSavableEntitiesInScene()
  {
    var currScene = SceneManager.GetSceneByName(gameObject.name);
    savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currScene).ToList();
    return savableEntities;
  }
}

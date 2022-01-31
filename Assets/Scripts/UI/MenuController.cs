using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
  [SerializeField] GameObject menu;

  public event Action<int> onMenuSelected;
  public event Action onBack;

  private List<Text> menuItems;

  private int selectedItem = 0;

  private void Awake()
  {
    menuItems = menu.GetComponentsInChildren<Text>().ToList();
  }

  public void OpenMenu()
  {
    menu.SetActive(true);
    UpdateItemSelection();
  }
  
  public void CloseMenu()
  {
    menu.SetActive(false);
  }

  public void HandleUpdate()
  {
    int prevSelection = selectedItem;
    
    if (Input.GetKeyDown(Settings.I.Down))
      ++selectedItem;
    else if (Input.GetKeyDown(Settings.I.Up))
      --selectedItem;

    selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);
    
    if (prevSelection != selectedItem)
      UpdateItemSelection();

    if (Input.GetKeyDown(Settings.I.Enter))
    {
      onMenuSelected?.Invoke(selectedItem);
      CloseMenu();
    }
    else if (Input.GetKeyDown(Settings.I.Back))
    {
      onBack?.Invoke();
      CloseMenu();
    }
  }

  void UpdateItemSelection()
  {
    for (int i = 0; i < menuItems.Count; i++)
    {
      if (i == selectedItem)
        menuItems[i].color = Settings.I.HighlightedColor;
      else
        menuItems[i].color = Color.black;
    }
  }
}

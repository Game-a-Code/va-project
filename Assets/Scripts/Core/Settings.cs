using UnityEngine;

public class Settings : MonoBehaviour
{
  // All key Combos
  public KeyCode Enter { get; } = KeyCode.Z;
  public KeyCode Back { get; }= KeyCode.X;
  public KeyCode Up { get; }= KeyCode.UpArrow;
  public KeyCode Down { get; }= KeyCode.DownArrow;
  public KeyCode Left { get; }= KeyCode.LeftArrow;
  public KeyCode Right { get; }= KeyCode.RightArrow;
  public KeyCode Return { get; }= KeyCode.Return;
  public KeyCode Safe { get; }= KeyCode.J;

  // Other Shit
  [SerializeField] private Color highlightedColor;
  public Color HighlightedColor => highlightedColor;
  
  // Instance so i can access from other places
  public static Settings I { get; private set; }
  private void Awake()
  {
    I = this;
  }
}
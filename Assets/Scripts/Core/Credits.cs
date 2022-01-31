using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] public Text easterEggText;
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetText()
    {
        easterEggText.text = "Found " + PlayerController.EasterEggsFound + "/5 Secrets";
    }
}
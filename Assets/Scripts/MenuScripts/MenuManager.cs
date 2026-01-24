using UnityEngine;
using TMPro;
using GoogleTextToSpeech.Scripts;

public class MenuManager : MonoBehaviour
{
    public static bool IsOpen { get; private set; } = false;

    public GameObject menu;
    public GameObject Managers;
    public GameObject Player;

    private TextToSpeech textToSpeech;
    private bool isMenuOpen = false;

    void Start(){
        textToSpeech = FindObjectOfType<TextToSpeech>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleMenu();
        }
    }   

    void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        IsOpen = isMenuOpen;
        

        if (textToSpeech != null){
            if (isMenuOpen){textToSpeech.PlayTtsAudio("Help Menu. Use WASD keys to navigate between submenus. You are now in level 1 submenu.");}
            else{textToSpeech.PlayTtsAudio("Help Menu Closed.");}
        }
        menu.SetActive(isMenuOpen);
        Managers.SetActive(!isMenuOpen);
        Player.SetActive(!isMenuOpen);

    }
}

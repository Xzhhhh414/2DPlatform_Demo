using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public InputActionAsset actions;
    InputAction startGame;
    void Start()
    {
        startGame = actions.FindActionMap("UI").FindAction("Click");
        startGame.performed += OnStartGame;
    }

    private void OnStartGame(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("GameplayScene");
        Debug.Log("StartGame");
    }
    private void OnDestroy() {
        startGame.performed -= OnStartGame;
    }
}

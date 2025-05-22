using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class UIManager : MonoBehaviour
{
    public GameObject loadoutMenu;
    public GameObject escapeMenu;
    public GameObject rebindUI;
    private InputSystem_Actions input;
    private bool isOpened = false;

    [System.Obsolete]
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ShowLoadoutMenu();
            ShowPauseMenu();
            FindObjectOfType<MouseLook>().LockCursor(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideLoadoutMenu();
        }
    }*/
    private void Awake()
    {
        input = new InputSystem_Actions();
        input.UI.Menu.performed += ctx => HandleEscape();
    }
    private void OnEnable() => input.UI.Enable();
    private void OnDisable() => input.UI.Disable();

    private void HandleEscape()
    {
        if (rebindUI.activeSelf)
        {
            // If in rebind menu, back to pause menu
            CloseRebindUI();
            ToggleMenu();
        }
        else if (loadoutMenu.activeSelf)
        {
            CloseLoadoutMenu();
            ToggleMenu();
        }
        else if (escapeMenu.activeSelf)
        {
            // If in pause menu, close menu entirely
            ToggleMenu();
            LockCursor();
        }
        else
        {
            // No menu open → open pause menu
            ToggleMenu();
            UnlockCursor();
        }
    }

    public void ToggleMenu()
    {
        isOpened = !isOpened;
        escapeMenu.SetActive(isOpened);
    }
    public void CloseRebindUI()
    {
        rebindUI.SetActive(false);
    }
    public void ShowLoadoutMenu()
    {
        ToggleMenu();
        loadoutMenu.SetActive(true);
    }
    public void ShowRebindUI()
    {
        ToggleMenu();
        rebindUI.SetActive(true);
    }

    public void CloseLoadoutMenu()
    {
        loadoutMenu.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

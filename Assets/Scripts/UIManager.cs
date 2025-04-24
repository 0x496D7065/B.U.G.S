using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject loadoutMenu;

    [System.Obsolete]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ShowLoadoutMenu();
            FindObjectOfType<MouseLook>().LockCursor(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideLoadoutMenu();
        }
    }
    public void ShowLoadoutMenu()
    {
        loadoutMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideLoadoutMenu()
    {
        loadoutMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

using UnityEngine;

public class EscapeMenuController : MonoBehaviour
{
    public GameObject escapeMenu;

    private bool isEscapeMenuActive;
    private bool wasCursorVisible;

    private void Start()
    {
        // Initially, the escape menu is inactive
        isEscapeMenuActive = false;
        escapeMenu.SetActive(false);

        // Hide the cursor at the beginning and store its initial visibility state
        wasCursorVisible = Cursor.visible;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle the escape menu on or off
            isEscapeMenuActive = !isEscapeMenuActive;

            // Set the active state of the escape menu GameObject based on the toggle
            escapeMenu.SetActive(isEscapeMenuActive);

            // Show or hide the cursor based on the escape menu state
            Cursor.visible = isEscapeMenuActive;

            // Lock or unlock the cursor based on the escape menu state
            Cursor.lockState = isEscapeMenuActive ? CursorLockMode.Confined : CursorLockMode.Locked;

            // If the escape menu is active, start hiding the cursor continuously
            if (isEscapeMenuActive)
            {
                StartCoroutine(HideCursor());
            }
            else
            {
                // If the escape menu is closed, stop hiding the cursor
                StopCoroutine(HideCursor());
                Cursor.visible = wasCursorVisible;
            }
        }
    }

    private System.Collections.IEnumerator HideCursor()
    {
        while (true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            yield return null;
        }
    }
}

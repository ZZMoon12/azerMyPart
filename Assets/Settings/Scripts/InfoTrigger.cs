using UnityEngine;
using TMPro;
public class InfoTrigger2D : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;
    public TMP_Text popupText;
    [TextArea] public string message = "You found a mysterious area...";

    [Header("Behavior")]
    public bool showOnlyOnce = true;     // show the first time only
    public bool hideOnExit = false;      // hide when leaving the trigger
    public float autoHideAfterSeconds = 0f; // 0 = don't auto-hide

    private bool _hasShown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (showOnlyOnce && _hasShown) return;

        ShowPopup();
        _hasShown = true;

        if (autoHideAfterSeconds > 0f)
        {
            CancelInvoke(nameof(HidePopup));
            Invoke(nameof(HidePopup), autoHideAfterSeconds);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hideOnExit)
        {
            HidePopup();
        }
    }

    public void ShowPopup()
    {
        if (popupPanel != null) popupPanel.SetActive(true);
        if (popupText != null) popupText.text = message;
    }

    public void HidePopup()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }
}

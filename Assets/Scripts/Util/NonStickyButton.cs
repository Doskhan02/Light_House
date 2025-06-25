using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NonStickyButton : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        // Your button logic here
        Debug.Log("Button clicked!");

        // Deselect the button immediately
        EventSystem.current.SetSelectedGameObject(null);
    }
}
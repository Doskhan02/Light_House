
using UnityEngine;
using UnityEngine.UI;

public class ActiveBoosterSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    private ActiveBoosterManager activeBoosterManager;
    private ActiveBooster currentActiveBooster;
    
    private void Start()
    {
        activeBoosterManager = ActiveBoosterManager.Instance;
        activeBoosterManager.OnCurrentActiveBoosterChanged += OnActiveBoosterChanged;
        currentActiveBooster = activeBoosterManager.CurrentActiveBooster;
        if (currentActiveBooster != null)
        {
            iconImage.sprite = currentActiveBooster.icon;
        }
        else
        {
            iconImage.color = Color.clear;
            iconImage.sprite = null;
        }
    }

    private void OnActiveBoosterChanged(ActiveBooster activeBooster)
    {
        currentActiveBooster = activeBooster;
        iconImage.sprite = currentActiveBooster.icon;
        iconImage.color = Color.white;
    }
}

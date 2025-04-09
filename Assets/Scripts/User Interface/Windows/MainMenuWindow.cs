using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : Window
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button optionsGameButton;
    [SerializeField] private Button bestiaryButton;
    [SerializeField] private Button shopButton;

    [SerializeField] private RectTransform sliderContent;
    [SerializeField] private Scrollbar scroll;
    [SerializeField] private ScrollRect scrollRect;

    public override void Initialize()
    {
        scroll.value = 0;
        scroll.interactable = false;
        startGameButton.onClick.AddListener(StartGameHandler);
        optionsGameButton.onClick.AddListener(OpenOptionsHandler);
        bestiaryButton.onClick.AddListener(OpenBestiaryHandler);
        shopButton.onClick.AddListener(OpenShopHandler);
    }
    protected override void OpenStart()
    {
        base.OpenStart();
        OpenEnd();
    }
    protected override void OpenEnd()
    {
        LevelManager.Instance.OnLevelChanged += LevelHandler;
        base.OpenEnd();
        startGameButton.interactable = true;
        optionsGameButton.interactable = true;
    }
    protected override void CloseStart()
    {
        base.CloseStart();
        startGameButton.interactable = false;
        optionsGameButton.interactable = false;
        CloseEnd();
    }
    private void StartGameHandler()
    {
        GameManager.Instance.StartGame();
        GameManager.Instance.WindowService.ShowWindow<GamePlayWindow>(false);
        Hide(false);
    }
    private void OpenOptionsHandler()
    {
        GameManager.Instance.WindowService.ShowWindow<OptionsWindow>(false);
        Hide(false);
    }
    private void OpenBestiaryHandler()
    {
        GameManager.Instance.WindowService.ShowWindow<BestiaryWindow>(false);
        Hide(false);
    }
    private void OpenShopHandler()
    {
        GameManager.Instance.WindowService.ShowWindow<ShopWindow>(false);
        Hide(false);
    }
    private void LevelHandler(int level)
    {
        scrollRect.horizontal = true;
        scrollRect.vertical = true;
        scroll.interactable = true;

        if (level > 6)
        {
            level = level - 6;
        }
        scroll.value = scroll.value + (level * 0.15f + 0.01f);

        scroll.interactable = false;
        scrollRect.horizontal = false;
        scrollRect.vertical = false;
    }
}

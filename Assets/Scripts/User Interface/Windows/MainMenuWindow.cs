using System.Collections;
using System.Data.SqlTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : Window
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button optionsGameButton;
    [SerializeField] private Button bestiaryButton;
    [SerializeField] private Button shopButton;

    [SerializeField] private RectTransform scrollContent;

    [SerializeField] private TMP_Text level1;
    [SerializeField] private TMP_Text level2;

    [SerializeField] private Button boosterList;
    [SerializeField] private Animator boosterListAnimator;

    private int currentLevel;

    private Coroutine levelChangeCoroutine;

    public override void Initialize()
    {
        startGameButton.onClick.AddListener(StartGameHandler);
        optionsGameButton.onClick.AddListener(OpenOptionsHandler);
        bestiaryButton.onClick.AddListener(OpenBestiaryHandler);
        shopButton.onClick.AddListener(OpenShopHandler);
        LevelManager.Instance.OnLevelChanged += OnCurrentLevelChanged;
        boosterList.onClick.AddListener(BoosterListHandler);
    }
    protected override void OpenStart()
    {
        base.OpenStart();
        LevelHandler(currentLevel);
        OpenEnd();
    }
    protected override void OpenEnd()
    {
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
        GameManager.Instance.WindowService.ShowWindow<GamePlayWindow>(false);
        GameManager.Instance.StartGame();
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
    private void OnCurrentLevelChanged(int newLevel)
    {
        currentLevel = newLevel;
    }

    private void Start()
    {
        LevelHandler(currentLevel);
    }

    private void BoosterListHandler()
    {
        boosterListAnimator.Play("Expand");
    }

    private void Update()
    {
        if (GameManager.Instance.InputManager.TouchScreen())
        {
            boosterListAnimator.Play("Retract");
        }
    }
    

    private void LevelHandler(int level)
    {
        if (level < 10)
        {
            level1.text = level.ToString();
            level2.text = "0"; // Optional: clear the second digit
        }
        else
        {
            int units = level % 10;
            int tens = level / 10;

            level1.text = units.ToString();
            level2.text = tens.ToString();
        }
        
        // Rotate every 3 levels back to the first (levels cycle: 1, 2, 3 -> 1, 2, 3...)
        int maxLevels = 3;
        int adjustedLevel = ((level - 1) % maxLevels) + 1;

        // Stop existing coroutine if it's running
        if (levelChangeCoroutine != null)
        {
            StopCoroutine(levelChangeCoroutine);
        }

        levelChangeCoroutine = StartCoroutine(LevelChange(adjustedLevel, scrollContent));
    }

    private IEnumerator LevelChange(int level, RectTransform scrollContent)
    {
        // Target local anchored position
        float targetScrollValue = level * 110f; // Move left by level * 200 pixels

        float duration = 1.0f;
        float elapsed = 0f;

        // Use anchoredPosition for ScrollRect-based layouts
        float initialScrollContentPos = scrollContent.anchoredPosition.y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newValue = Mathf.Lerp(initialScrollContentPos, targetScrollValue, elapsed / duration);

            // Only update the local anchored position
            Vector2 newPos = new Vector2(scrollContent.anchoredPosition.x, newValue);
            scrollContent.anchoredPosition = newPos;

            yield return null;
        }

        // Final snap to target value to avoid floating point inaccuracies
        scrollContent.anchoredPosition = new Vector2(scrollContent.anchoredPosition.x, targetScrollValue);

        levelChangeCoroutine = null;
    }
}

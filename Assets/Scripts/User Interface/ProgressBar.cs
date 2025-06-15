using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Character character;
    [SerializeField]
    private Image ProgressImage;
    [SerializeField]
    private float DefaultSpeed = 3f;
    [SerializeField]
    private Gradient ColorGradient;
    [SerializeField]
    private UnityEvent<float> OnProgress;
    [SerializeField]
    private UnityEvent OnCompleted;

    private Coroutine AnimationCoroutine;

    private void Update()
    {
        transform.position = character.transform.position + Vector3.up * 2;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        if (character.isActiveAndEnabled == false)
        {
            character.lifeComponent.OnCharacterHealthChange -= UpdateUI;
            character.CharacterData.Healthbar.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        if (ProgressImage.type != Image.Type.Filled)
        {
            Debug.LogError($"{name}'s ProgressImage is not of type \"Filled\" so it cannot be used as a progress bar. Disabling this Progress Bar.");
            enabled = false;
        }
        
    }

    public void Initialize()
    {
        if(character == null)
            return;
        character.CharacterData.Healthbar.gameObject.SetActive(false);
        SetProgress(1,10);
        character.lifeComponent.OnCharacterHealthChange += UpdateUI;
    }
    public void UpdateUI(Character character)
    {
        if (character.lifeComponent.Health != character.lifeComponent.MaxHealth)
        {
            character.CharacterData.Healthbar.gameObject.SetActive(true);
        }
        else
        {
            character.CharacterData.Healthbar.gameObject.SetActive(false);
        }
        character.CharacterData.Healthbar.SetProgress(character.lifeComponent.Health / character.lifeComponent.MaxHealth);
    }

    public void SetProgress(float Progress)
    {
        SetProgress(Progress, DefaultSpeed);
    }

    public void SetProgress(float Progress, float Speed)
    {
        if (gameObject.activeSelf == false)
            return;
        if (Progress < 0 || Progress > 1)
        {
            Debug.LogWarning($"Invalid progress passed, expected value is between 0 and 1, got {Progress}. Clamping.");
            Progress = Mathf.Clamp01(Progress);
        }
        if (Progress != ProgressImage.fillAmount)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            AnimationCoroutine = StartCoroutine(AnimateProgress(Progress, Speed));
        }
    }

    private IEnumerator AnimateProgress(float Progress, float Speed)
    {
        float time = 0;
        float initialProgress = ProgressImage.fillAmount;

        while (time < 1)
        {
            ProgressImage.fillAmount = Mathf.Lerp(initialProgress, Progress, time);
            time += Time.deltaTime * Speed;

            ProgressImage.color = ColorGradient.Evaluate(1 - ProgressImage.fillAmount);

            OnProgress?.Invoke(ProgressImage.fillAmount);
            yield return null;
        }

        ProgressImage.fillAmount = Progress;
        ProgressImage.color = ColorGradient.Evaluate(1 - ProgressImage.fillAmount);

        OnProgress?.Invoke(Progress);
        OnCompleted?.Invoke();
    }
}

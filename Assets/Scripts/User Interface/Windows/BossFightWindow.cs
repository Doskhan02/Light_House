using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossFightCanvas : Window
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Slider bossHP;
    [SerializeField] private Button fireButton;
    [SerializeField] private QuickTimeEvent quickTimeEvent;
    
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private Image[] progress;
    [SerializeField] private TMP_Text fireText;

    private Character bossCharacter;
    private Character bossShipCharacter;

    private AllyBossCharacter ship;

    public override void Initialize()
    {
        base.Initialize();
        pauseButton.onClick.AddListener(PauseHandler);
    }
    protected override void OpenStart()
    {
        base.OpenStart();
        music.clip = bossMusic;
        if(!music.isPlaying)
            music.Play();
        OpenEnd();
    }

    protected override void OpenEnd()
    {
        base.OpenEnd();
        BossHandler();
        fireButton.onClick.AddListener(OnFire);
    }

    protected override void CloseStart()
    {
        base.CloseStart();
        fireButton.onClick.RemoveAllListeners();
        CloseEnd();
    }

    private void PauseHandler()
    {
        GameManager.Instance.GamePause();
        Hide(false);
        GameManager.Instance.WindowService.ShowWindow<PauseWindow>(false);
    }

    private void BossHandler()
    {
        if (GameManager.Instance.LevelManager.CurrentLevel%6 == 0)
        {
            bossCharacter =
                CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Enemy, "DT(BOSS)").First();
            if (bossCharacter == null)
            {
                Debug.LogError("Boss character not found in the scene.");
                return;
            }
            bossHP.gameObject.SetActive(true);
            bossHP.value = bossCharacter.lifeComponent.Health / bossCharacter.CharacterData.CharacterTypeData.defaultMaxHP;
            bossCharacter.lifeComponent.OnCharacterHealthChange += OnBossHealthChanged;
            Character character =
                CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Ally, "Boat3").First();
            if (character is AllyBossCharacter boss)
                ship = boss;
        }
        else
        {
            bossHP.gameObject.SetActive(false);
        }
    }
    private void OnBossHealthChanged(Character character)
    {
        bossHP.value = character.lifeComponent.Health / character.CharacterData.CharacterTypeData.defaultMaxHP;
    }

    private void Update()
    {
        fireButton.interactable = ship.FirePending;
        fireText.gameObject.SetActive(ship.FirePending);
        for (int i = 0; i < progress.Length; i++)
        {
            progress[i].gameObject.SetActive(ship.Turret[i]);
        }
    }

    private void OnFire()
    {
        quickTimeEvent.StartQTE();
        quickTimeEvent.OnSuccess += ship.Fire;
        quickTimeEvent.OnFail += ship.EmptyTurrets;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("MENU")]
    [SerializeField] TextMeshProUGUI _highScoreText; 

    [Header("OPTIONS MENU")]
    [SerializeField] GameObject _optionsMenu;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] GameObject _options;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] Slider _musicSlider;
    bool _optionsMenuActive;
    [SerializeField] AudioMixer _audio;

    private void Start()
    {
        if (_optionsMenu.activeSelf) _optionsMenu.SetActive(false);
        //SaveSystem.instance.Load();
        _highScoreText.text = GameData.data.dataObject.highScore.ToString();

        _audio.GetFloat("SfxVolume", out float sfxValue);
        if (sfxValue < _sfxSlider.minValue) sfxValue = _sfxSlider.minValue;
        if (_sfxSlider != null) _sfxSlider.value = sfxValue;

        _audio.GetFloat("MusicVolume", out float musicValue);
        if (musicValue < _musicSlider.minValue) musicValue = _musicSlider.minValue;
        if (_musicSlider != null) _musicSlider.value = musicValue;
    }

    public void OnClickPlay()
    {
        AudioManager.Instance.Play("Click");
        LevelLoader.LoadScene(LevelLoader.Scenes.Game);
    }

    public void OnClickOptions()
    {
        showOptionsMenu();
        AudioManager.Instance.Play("Click");
    }

    public void OnClickCloseOptions()
    {
        hideOptionsMenu();
        AudioManager.Instance.Play("Click");
    }
    public void SetSFXVolume(float volume)
    {
        if (volume <= _sfxSlider.minValue) volume = -80;
        _audio.SetFloat("SfxVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        if (volume <= _musicSlider.minValue) volume = -80;
        _audio.SetFloat("MusicVolume", volume);
    }


    void showOptionsMenu()
    {
        if (_optionsMenuActive) return;
        _optionsMenuActive = true;
        _optionsMenu.SetActive(true);
        _options.transform.localScale = Vector3.zero;
        LeanTween.alphaCanvas(_canvasGroup, 1, .5f).setEase(LeanTweenType.easeOutExpo).setOnComplete(done =>
        {
            LeanTween.scale(_options, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutExpo);
        });
    }
    void hideOptionsMenu()
    {
        if (!_optionsMenuActive) return;
        _optionsMenuActive = false;
        LeanTween.scale(_options, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInExpo).setOnComplete(done =>
        {
            LeanTween.alphaCanvas(_canvasGroup, 0, .5f).setEase(LeanTweenType.easeOutExpo).setOnComplete(done =>
            {
                _optionsMenu.SetActive(false);
            });
        });
    }
}

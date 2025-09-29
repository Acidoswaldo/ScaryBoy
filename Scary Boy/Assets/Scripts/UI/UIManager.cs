using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

namespace ScaryGame
{
    public class UIManager : MonoBehaviour
    {

        [SerializeField] TextMeshProUGUI _pointsText;
        GameStats stats;

        [Header("SCARY POINTS")]
        [SerializeField] Slider _scarySlider;
        [SerializeField] Button _retryButton;
        [SerializeField] Button _mainMenuButton;
        float _scaryPoints;

        [Header("OPTIONS MENU")]
        [SerializeField] GameObject _optionsMenu;
        [SerializeField] CanvasGroup _optionsCanvasGroup;
        [SerializeField] GameObject _options;
        [SerializeField] Slider _sfxSlider;
        [SerializeField] Slider _musicSlider;
        bool _optionsMenuActive;
        [SerializeField] AudioMixer _audio;

        [Header("LOSE SCREEN")]
        [SerializeField] CanvasGroup _loseCanvasGroup;

        [Header("TUTORIAL SCREEN")]
        [SerializeField] GameObject _tutorialPanel;
        [SerializeField] CanvasGroup _tutorialScreen;
        [SerializeField] CanvasGroup _tutorialText;
        [SerializeField] CanvasGroup _tutorial_shadows;
        [SerializeField] CanvasGroup _tutorial_Spirits;
        [SerializeField] Image _fingerImage;


        private void Start()
        {
            if (stats == null) stats = GameStats.stats;
            if (_optionsMenu.activeSelf) _optionsMenu.SetActive(false);
            if (_scarySlider != null) _scarySlider.maxValue = GameStats.stats._scarePointsMax;

            _audio.GetFloat("SfxVolume", out float sfxValue);
            if (sfxValue < _sfxSlider.minValue) sfxValue = _sfxSlider.minValue;
            if (_sfxSlider != null) _sfxSlider.value = sfxValue;

            _audio.GetFloat("MusicVolume", out float musicValue);
            if (musicValue < _musicSlider.minValue) musicValue = _musicSlider.minValue;
            if (_musicSlider != null) _musicSlider.value = musicValue;
        }
        private void Update()
        {
            UpdatePointsUI();
            UpdateScaryPointsUI();
        }

        void UpdateScaryPointsUI()
        {
            if (_scarySlider == null) return;
            _scarySlider.value = _scaryPoints;
            if (_scaryPoints != GameStats.stats._scarePoints) _scaryPoints = Mathf.MoveTowards(_scaryPoints, GameStats.stats._scarePoints, 3 * Time.deltaTime);
        }
        void UpdatePointsUI()
        {
            if (_pointsText == null) return;
            if (!_pointsText.gameObject.activeSelf) _pointsText.gameObject.SetActive(true);
            _pointsText.text = stats._points.ToString();
        }

        public void showOptionsMenu()
        {

            if (_optionsMenuActive) return;
            AudioManager.Instance.Play("Click");
            if (GameManager.instance.gameState == GameManager.GameState.lose) return;
            _optionsMenuActive = true;
            GameManager.instance.ChangeGameState(GameManager.GameState.paused);
            _optionsMenu.SetActive(true);
            _options.transform.localScale = Vector3.zero;
            LeanTween.alphaCanvas(_optionsCanvasGroup, 1, .5f).setEase(LeanTweenType.easeOutExpo).setOnComplete(done =>
            {
                LeanTween.scale(_options, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutExpo);
            });
        }
        public void hideOptionsMenu()
        {
            if (!_optionsMenuActive) return;
            AudioManager.Instance.Play("Click");
            _optionsMenuActive = false;
            GameManager.instance.ChangeGameState(GameManager.GameState.running);
            LeanTween.scale(_options, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInExpo).setOnComplete(done =>
            {
                LeanTween.alphaCanvas(_optionsCanvasGroup, 0, .5f).setEase(LeanTweenType.easeOutExpo).setOnComplete(done =>
                {
                    _optionsMenu.SetActive(false);
                });
            });
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

        public void ShowLoseScreen()
        {
            _loseCanvasGroup.gameObject.SetActive(true);
            _loseCanvasGroup.alpha = 0;
            _retryButton.transform.localScale = Vector3.zero;
            _mainMenuButton.transform.localScale = Vector3.zero;
            LeanTween.alphaCanvas(_loseCanvasGroup, 1, 0.5f).setEase(LeanTweenType.easeOutExpo).setOnComplete(done =>
            {
                LeanTween.scale(_retryButton.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutExpo);
                LeanTween.scale(_mainMenuButton.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutExpo);
            });
        }
        public void OnClickQuit()
        {
            AudioManager.Instance.Play("Click");
            if (GameManager.instance == null) return;
            GameManager.instance.EndSesion();
        }

        public IEnumerator DisplayeTutorial()
        {
            _tutorialPanel.SetActive(true);
            yield return new WaitForSeconds(1);

            _fingerImage.transform.localScale = Vector3.zero;
            _tutorialText.alpha = 0;
            _tutorial_shadows.alpha = 0;
            _tutorial_Spirits.alpha = 0;
            bool TutorialDone = false;

            LeanTween.scale(_fingerImage.gameObject, Vector3.one, 0.7f).setEase(LeanTweenType.easeOutExpo).setOnComplete(done =>
            {
                LeanTween.alphaCanvas(_tutorialText, 1, 0.5f);
                LeanTween.moveLocalX(_fingerImage.gameObject, 375, 3f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(done =>
                {
                    LeanTween.alphaCanvas(_tutorial_shadows, 1, 0.5f).setOnComplete(done =>
                    {
                        LeanTween.alphaCanvas(_tutorial_Spirits, 1, 0.5f).setOnComplete(done =>
                        {
                            TutorialDone = true;
                        });
                    });


                });
            });

            while (!TutorialDone)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2.5f);
            LeanTween.alphaCanvas(_tutorialScreen, 0, 0.5f).setOnComplete(done =>
            {
                GameData.data.dataObject.tutorialDone = true;
                _tutorialPanel.SetActive(false);
                GameManager.instance.ChangeGameState(GameManager.GameState.running);
            });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace ScaryGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public static event Action<GameState> OnGameStateChanged;
        public enum GameState { Menu, tutorial, running, paused, lose }
        public GameState gameState;
        float PointTimer;

        bool _gamePaused;

        public bool damageable = true;
        float _invincibleTime = 0.5f;

        [SerializeField] UIManager _uiManager;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                _uiManager = FindObjectOfType<UIManager>();
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            if(GameData.data.dataObject.tutorialDone == false)
            {
                ChangeGameState(GameState.tutorial);
            }
            else
            {
                ChangeGameState(GameState.running);
            }
          
        }
        public void ChangeGameState(GameState newState)
        {
            gameState = newState;
            switch (gameState)
            {
                case GameState.Menu:
                    break;
                case GameState.running:
                    break;
                case GameState.paused:
                    break;
                case GameState.lose:
                    LostSequence();
                    break;
                case GameState.tutorial:
                    StartingHandler();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameState), newState, null);
            }
            OnGameStateChanged?.Invoke(newState);
        }

        void StartingHandler()
        {
            StartCoroutine(_uiManager.DisplayeTutorial());
        }

        public void EndSesion()
        {
            if (GameData.data.dataObject.highScore < GameStats.stats._points)
            {
                GameData.data.dataObject.highScore = GameStats.stats._points;
            }
           // SaveSystem.instance.Save();

            LevelLoader.LoadScene(LevelLoader.Scenes.MainMenu);
        }

        public void retry()
        {
            if (GameData.data.dataObject.highScore < GameStats.stats._points)
            {
                GameData.data.dataObject.highScore = GameStats.stats._points;
            }
           // SaveSystem.instance.Save();

            LevelLoader.LoadScene(LevelLoader.Scenes.Game);
        }

        private void Update()
        {
            if (gameState == GameState.running)
            {
                if (PointTimer <= 0)
                {
                    GameStats.stats.SetPoints(GameStats.stats._points + 1);
                    PointTimer = 1;
                }
                else
                {
                    PointTimer -= Time.deltaTime;
                }
            }
        }
        public void StartInvincibility()
        {
            StartCoroutine(Invincibilty());
        }
    
        IEnumerator Invincibilty()
        {
            damageable = false;
            yield return new WaitForSeconds(_invincibleTime);
            damageable = true;
        }

        void LostSequence()
        {
            _uiManager.ShowLoseScreen();
            AudioManager.Instance.Play("Scream");
        }
    }
}

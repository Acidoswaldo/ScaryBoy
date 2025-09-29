using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScaryGame
{
    public class GameStats : MonoBehaviour
    {
        public static GameStats stats;
        private void Awake()
        {
            if (stats == null)
            {
                stats = this;
            }
            else if (stats != this)
            {
                Destroy(this.gameObject);
            }
        }

        public int _scarePoints { get; private set; }
        public int _scarePointsMax;
        public int _points { get; private set; }


        public void SetPoints(int points)
        {
            Debug.Log("points set to: " + points);
            _points = points;
        }
        public void SetScarePoints(int scarePoints)
        {
            Debug.Log("scare points set to: " + scarePoints);
            if (scarePoints < 0) scarePoints = 0;
            _scarePoints = scarePoints;
            CheckLoseCondition();
        }

        void CheckLoseCondition()
        {
            if (_scarePoints < _scarePointsMax) return;
            LoseSequence();
        }

        void LoseSequence()
        {
            GameManager.instance.ChangeGameState(GameManager.GameState.lose);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iconPopUp;

namespace ScaryGame
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] Transform _target;
        [SerializeField] Vector2 direction;
        [SerializeField] Rigidbody2D _rb;
        bool _move;
        [SerializeField] EnemyTemplate _template;
        [SerializeField] SpriteRenderer _renderer;
        [SerializeField] EnemySpawner _spawner;
        bool _takeDamage;
        [SerializeField] Transform _HpCircle;
        Vector3 _hpCircleMaxSize;


        [Header("Stats")]
        [SerializeField] float _hp;
        float _speed;
        bool _despawn;
        bool _alive;

        private void Awake()
        {

            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
            if (_spawner == null) _spawner = FindObjectOfType<EnemySpawner>();
            if (_HpCircle != null) _hpCircleMaxSize = _HpCircle.localScale;
        }
        public void SetEnemy(Transform target, EnemyTemplate template)
        {
            if (_HpCircle.gameObject.activeSelf) _HpCircle.gameObject.SetActive(false);
            _alive = true;
            _move = true;
            _takeDamage = false;
            _despawn = false;
            _target = target;
            _template = template;
            _hp = template.hp;
            _speed = template.speed;
            _renderer.sprite = _template.sprite;
            Vector2 diff = _target.position - transform.position;
            float rotationz = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotationz);
            diff.Normalize();
            direction = diff;
            if (_template.name == "TinyNightmare")
            {
                if(!AudioManager.Instance.GetAudioSource("Tiny").isPlaying) AudioManager.Instance.Play("Tiny");
            }
            if (_template.name == "SmallNightmare")
            {
                if (!AudioManager.Instance.GetAudioSource("Small").isPlaying) AudioManager.Instance.Play("Small");
            }
            if (_template.name == "MediumNightmare")
            {
                _spawner.numberOfMedEnemies += 1;
                if (!AudioManager.Instance.GetAudioSource("Medium").isPlaying) AudioManager.Instance.Play("Medium");
            }
            if (_template.name == "BigNightmare")
            {
                _spawner.numberOfBigEnemies += 1;
                if (!AudioManager.Instance.GetAudioSource("Big").isPlaying) AudioManager.Instance.Play("Big");
            }
            if (_template.name == "HugeNightmare")
            {
                _spawner.numberOfHugeEnemies += 1;
                if (!AudioManager.Instance.GetAudioSource("Huge").isPlaying) AudioManager.Instance.Play("Huge");
            }
            if (_template.name == "GoodSpirit")
            {
                if (!AudioManager.Instance.GetAudioSource("Good").isPlaying) AudioManager.Instance.Play("Good");
            }
        }

        private void FixedUpdate()
        {
            if (GameManager.instance.gameState == GameManager.GameState.running)
            {
                if (_move == false) return;
                _rb.MovePosition((Vector2)transform.position + (direction * _speed * Time.deltaTime));
            }
        }

        private void Update()
        {
            TakeDamageCheck();
        }

        void TakeDamageCheck()
        {
            if (GameManager.instance.gameState != GameManager.GameState.running) return;
            if (!_alive) return;
            if (_takeDamage)
            {
                _hp -= 1 * Time.deltaTime;
                if (!_HpCircle.gameObject.activeSelf) _HpCircle.gameObject.SetActive(true);
                float healthPercentage = (_hp / _template.hp) * 100;
                _HpCircle.localScale = (_hpCircleMaxSize * healthPercentage) / 100;
                if (_hp <= 0)
                {
                    _alive = false;
                    if (_template.name != "GoodSpirit")
                    {
                        IconPopUp.Create(transform.position, _template.PointsReward, new Vector3(0.2f, 0.2f, 0.2f));
                        GameStats.stats.SetPoints(GameStats.stats._points + _template.PointsReward);
                    } 
                    _hp = 0;
                    _speed = 0;
                    LeanTween.alpha(gameObject, 0, .5f).setOnComplete(done =>
                    {
                        LeanTween.alpha(gameObject, 1, 0);
                        Despawn();
                    });
                }
            }
            else if (_hp < _template.hp)
            {
                _hp += 0.2f * Time.deltaTime;
                if (_HpCircle.gameObject.activeSelf) _HpCircle.gameObject.SetActive(false);
            }
            else
            {
                if (_HpCircle.gameObject.activeSelf) _HpCircle.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (GameManager.instance.gameState == GameManager.GameState.running)
            {
                if (collision.tag == "Player")
                {
                    if (GameManager.instance.damageable)
                    {
                        GameManager.instance.StartInvincibility();
                        GameStats.stats.SetScarePoints(GameStats.stats._scarePoints + _template.damage);
                        AudioManager.Instance.Play("Gasp");
                    }
                    LeanTween.alpha(gameObject, 0, .5f).setOnComplete(done =>
                    {
                        LeanTween.alpha(gameObject, 1, 0);
                        Despawn();
                    });
                }
                if (collision.tag == "Light")
                {
                    _takeDamage = true;
                    if (_template.SlowedByLight)
                    {
                        _speed = _template.speed / 4;
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (GameManager.instance.gameState == GameManager.GameState.running)
            {
                if (collision.tag == "Light")
                {
                    Debug.Log("left light");
                    _takeDamage = false;
                    _speed = _template.speed;
                    //_hp = _template.hp;
                }
            }
        }
        void Despawn()
        {
            if (!_despawn)
            {
                if (_template.name != "GoodSpirit")
                {
                    if (!AudioManager.Instance.GetAudioSource("ShadowDeath").isPlaying) AudioManager.Instance.Play("ShadowDeath");
                }
                _despawn = true;
                if (_template.name == "TinyNightmare")
                {
                    AudioManager.Instance.Stop("Tiny");
                }
                if (_template.name == "SmallNightmare")
                {
                    AudioManager.Instance.Stop("Small");
                }
                if (_template.name == "MediumNightmare")
                {
                    _spawner.numberOfMedEnemies -= 1;
                    AudioManager.Instance.Stop("Medium");
                }
                if (_template.name == "BigNightmare")
                {
                    _spawner.numberOfBigEnemies -= 1;
                    AudioManager.Instance.Stop("Big");
                }
                if (_template.name == "HugeNightmare")
                {
                    _spawner.numberOfHugeEnemies -= 1;
                    AudioManager.Instance.Stop("Huge");
                }
                if (_template.name == "GoodSpirit")
                {
                    AudioManager.Instance.Stop("Good");
                }
                _spawner.PoolDictionary[_spawner._poolTag].Enqueue(this);
                gameObject.SetActive(false);
            }
        }
    }
}


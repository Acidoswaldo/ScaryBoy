using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace iconPopUp
{
    public class IconPopUp_PF : MonoBehaviour
    {
        [SerializeField] float _moveSpeed = 5f;
        [SerializeField] float _frequency = 1f;
        [SerializeField] float _magnitude = 1f;
        [SerializeField] float _lifetime = 1f;

        [SerializeField] TextMeshProUGUI numberText;

        Vector3 _pos;
        float randomOffset;
        bool _active;
        float _sinMultiplier;


        private void Start()
        {
            _pos = transform.position;
        }

        public void InitializePopUp(float number, Vector3 customScale)
        {
            transform.localScale = Vector3.zero;
            _sinMultiplier = 0;
               _pos = transform.position;
            _active = true;
            randomOffset = Random.Range(0f, 1f);
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, customScale, 0.5f).setEase(LeanTweenType.easeOutExpo);
            Invoke("LifetimeEnd", _lifetime);
           if(numberText != null) numberText.text = "+" + number;
        }
        private void Update()
        {
            if (!_active) return;
            _pos += transform.up * Time.deltaTime * _moveSpeed;
            _sinMultiplier += Time.deltaTime;
            transform.position = _pos + transform.right * Mathf.Sin((_sinMultiplier + randomOffset) * _frequency) * _magnitude;
           
        }

        void LifetimeEnd()
        {
            LeanTween.scale(gameObject, Vector3.zero, 1).setEase(LeanTweenType.easeInExpo).setOnComplete(done =>
            {
                _active = false;
                _pos = Vector3.zero;
                gameObject.SetActive(false);
            });

        }

    }
}

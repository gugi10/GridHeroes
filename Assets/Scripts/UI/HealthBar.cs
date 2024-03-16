using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI healthValue;
        [SerializeField] private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup.alpha = 0;
        }

        public void Set(int current, int max)
        {
            slider.maxValue = max;
            slider.value = current;
            healthValue.text = $"{current}/{max}";
        }

        public void UpdateCurrent(int current)
        {
            healthValue.text = $"{current}/{slider.maxValue}";
            _canvasGroup.DOFade(1, 0.5f).OnComplete(() => slider.DOValue(current, 1f)
                .OnComplete(() => _canvasGroup.DOFade(0, 0.5f).SetDelay(2f)));
        }
    }
}
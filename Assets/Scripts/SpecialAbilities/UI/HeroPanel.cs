using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

namespace SpecialAbilities.UI
{
    public class HeroPanel : MonoBehaviour
    {
        [SerializeField] private Image heroPortrait;
        [SerializeField] private TextMeshProUGUI heroTextInfo;
        private CanvasGroup _canvasGroup;
        private HeroesConfig _heroesConfig;
        private Camera _camera;
        private Tween currentTween;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0;
            _heroesConfig = GameSession.Instance.GetConfig<HeroesConfig>();
            _camera = Camera.main;

        }

        private void Update()
        {
            HideIfClickedOutside();
        }

        public void Show(HeroController clickedHero)
        {
            ShowAnimation();
            var heroConfig = _heroesConfig.heroConfigData.FirstOrDefault(hero => clickedHero.HeroId == hero.heroId);
            if (heroConfig == null)
            {
                Debug.LogError($"Didn't find hero with specified id {clickedHero.HeroId}");
                return;
            }
            heroPortrait.sprite = heroConfig.heroIconSprite;
            heroTextInfo.text =
                $"Health: {clickedHero.GetHeroStats().current.Health}/{clickedHero.GetHeroStats().baseStats.Health}\n" +
                $"Movement: {clickedHero.GetHeroStats().current.Move}\n" +
                $"Range: {clickedHero.GetHeroStats().current.WeaponRange}\n" +
                $"Damage: {clickedHero.GetHeroStats().current.WeaponDamage}\n";
        }
        
        private void HideIfClickedOutside() {
            if (Input.GetMouseButton(0) && gameObject.activeSelf && 
                !RectTransformUtility.RectangleContainsScreenPoint(
                    gameObject.GetComponent<RectTransform>(), 
                    Input.mousePosition, 
                    _camera)) {
                    HideAnimation();
            }
        }

        private void ShowAnimation()
        {
            currentTween?.Kill();
            currentTween = _canvasGroup.DOFade(1, 0.5f);
            _canvasGroup.interactable = true;
        }

        private void HideAnimation()
        {
            currentTween?.Kill();
            currentTween = _canvasGroup.DOFade(0, 0.5f);
            _canvasGroup.interactable = false;
        }
    }
}
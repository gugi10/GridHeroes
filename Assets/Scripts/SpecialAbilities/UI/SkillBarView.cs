using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SkillBarView : MonoBehaviour
{
    [SerializeField] SkillView skillPrefab;
    List<SkillView> instantiatedSkills = new();
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private void Awake()
    {
        TurnSequenceController.Instance.onHeroSelected += ShowHeroSkills;
        TurnSequenceController.Instance.onHeroUnselected += HideAnimation;
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    //TODO: NEED TESTING. wanted to avoid ins xtantiating and destroying ability objects, however it might be overengineer and not working. :)
    private void ShowHeroSkills(HeroController hero)
    {
        ShowAnimation();
        for (int i = 0; i < hero.specialAbilities.Length; i++)
        {
            if (instantiatedSkills.Count > i)
            {
                instantiatedSkills[i].Init(hero, hero.specialAbilities[i], i);
                instantiatedSkills[i].gameObject.SetActive(true);
            }
            else
            {
                var spawnedSKill = Instantiate(skillPrefab, transform);
                spawnedSKill.Init(hero, hero.specialAbilities[i], i);
                instantiatedSkills.Add(spawnedSKill);
            }
        }

        for (int i = instantiatedSkills.Count - 1; i > instantiatedSkills.Count - hero.specialAbilities.Length; i--)
        {
            instantiatedSkills[i].gameObject.SetActive(false);
        }
    }

    private void ShowAnimation()
    {
        Debug.Log($"Show");
        _canvasGroup.DOFade(1, 0.5f);
        _rectTransform.DOScale(Vector3.one, 0.5f);
        //_rectTransform.DOAnchorPosY(0, 0.5f);
    }

    private void HideAnimation()
    {
        Debug.Log("Hide");
        _canvasGroup.DOFade(0, 0.5f);
        _rectTransform.DOScale(Vector3.zero, 0.5f);
        //_rectTransform.DOMoveY(-200, 0.5f);
    }

}

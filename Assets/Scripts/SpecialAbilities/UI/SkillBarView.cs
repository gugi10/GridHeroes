using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SkillBarView : MonoBehaviour
{
    [SerializeField] SkillView skillPrefab;
    List<SkillView> instantiatedSkills = new();
    private RectTransform _rectTransform;
    private void Awake()
    {
        TurnSequenceController.Instance.onHeroSelected += ShowHeroSkills;
        TurnSequenceController.Instance.onHeroUnselected += HideAnimation;
    }

    //TODO: NEED TESTING. wanted to avoid ins xtantiating and destroying ability objects, however it might be overengineer and not working. :)
    private void ShowHeroSkills(HeroController hero)
    {
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
        ShowAnimation();
    }

    private void ShowAnimation()
    {
        _rectTransform.DOAnchorPosY(0.15f, 0.5f);

    }

    private void HideAnimation()
    {
        _rectTransform.DOAnchorPosY(0f, 0.5f);

    }

}

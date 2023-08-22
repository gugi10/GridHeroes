using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBarView : MonoBehaviour
{
    [SerializeField] SkillView skillPrefab;
    List<SkillView> instantiatedSkills = new();
    private void OnEnable()
    {
        TurnSequenceController.Instance.onHeroSelected += ShowHeroSkills;
    }
  
    private void OnDisable()
    {
        TurnSequenceController.Instance.onHeroSelected -= ShowHeroSkills;
    }

    //TODO: NEED TESTING. wanted to avoid instantiating and destroying ability objects, however it might be overengineer and not working. :)
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
    } 

}

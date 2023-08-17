using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillView : MonoBehaviour
{
    [SerializeField] Image icon;
    Button button;
    private HeroController hero;
    private ISpecialAbility specialAbility;
    private int id;
    private bool isInit;
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(CallAbillity); 
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(CallAbillity);
    }

    public void Init(HeroController hero, ISpecialAbility specialAbility, int id)
    {
        this.hero = hero;
        this.specialAbility = specialAbility;
        this.id = id;
        icon.sprite = specialAbility.GetSkillIcon();
        isInit = true;
    }

    private void CallAbillity()
    {
        if (!isInit)
        {
            Debug.LogError($"Ability not initalized");
            return;
        }

        hero.DoSpecialAbility(id);
    }
}

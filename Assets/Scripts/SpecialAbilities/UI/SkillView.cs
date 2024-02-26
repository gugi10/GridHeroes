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
        // TODO: GET SKILL ICON
        // icon.sprite = specialAbility.GetSkillIcon();
        isInit = true;
    }

    private void CallAbillity()
    {
        if (!isInit)
        {
            Debug.LogError($"Ability not initalized");
            return;
        }

        var turnSequenceController = TurnSequenceController.Instance;
        var activePlayer = turnSequenceController.ActivePlayer;
        // Better way to do it would be do gray out the button and make it non clickable when player has no special actions
        if (turnSequenceController.GetPlayerRemainingActions(activePlayer).Contains(HeroAction.Special))
        {
            hero.DoSpecialAbility(id);
        }
    }
}

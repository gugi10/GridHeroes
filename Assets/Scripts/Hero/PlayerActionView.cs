using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerActionView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI heroActionText;
    [SerializeField] Button button;
    private HeroAction action;
    private PlayerId playerId;

    public void OnEnable()
    {
        button.onClick.AddListener(ActionPass);
    }

    public void OnDisable()
    {
        button.onClick.RemoveListener(ActionPass);
    }

    public void Init(PlayerId playerId, HeroAction action)
    {
        button.interactable = playerId == TurnSequenceController.Instance.ActivePlayer;
        this.playerId = playerId;
        this.action = action;
        heroActionText.text = $"{action}";
    }

    private void ActionPass()
    {
        TurnSequenceController.Instance.FinishTurn(action);
    }
}

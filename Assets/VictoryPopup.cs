using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPopup : PopupBase
{
    public override PopupIdentifiers Identifier() => PopupIdentifiers.EndGamePopup;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject victoryContent;
    [SerializeField] private GameObject loseContent;

    private void OnEnable()
    {
        continueButton.onClick.AddListener(GoToHub);
    }

    private void OnDisable()
    {
        continueButton.onClick.RemoveListener(GoToHub);
    }

    public override void OnEnter(object data)
    {
        if(data is Payload payload)
        {
            victoryContent.SetActive(payload.isVictory);
            loseContent.SetActive(!payload.isVictory);
        }
    }

    private void GoToHub()
    {
        SceneLoader.LoadScene(SceneLoader.SceneEnum.Hub);
    }

    public class Payload
    {
        public bool isVictory { get; private set; }

        public Payload(bool isVictory)
        {
            this.isVictory = isVictory;
        }
    }
}

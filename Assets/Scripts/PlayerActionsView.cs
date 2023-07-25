using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class PlayerActionsView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerHeaderPrefab;
    [SerializeField] TurnSequenceController turnSequence;
    [SerializeField] PlayerActionView playerAction;
    List<GameObject> spawnedElements = new();

    private void Start()
    {
        TurnSequenceController.Instance.onTurnFinished += ShowPlayerActions;
        TurnSequenceController.Instance.onRoundStart += ShowPlayerActions;
    }

    private void OnDestroy()
    {
        TurnSequenceController.Instance.onTurnFinished -= ShowPlayerActions;
        TurnSequenceController.Instance.onRoundStart -= ShowPlayerActions;
    }

    private void ShowPlayerActions(List<List<HeroAction>> allActions)
    {
        /*playerActions.text = allActions
            .Select(actions => actions.Aggregate("", (acc, action) => acc += (action.ToString() + "\n")))
            .Aggregate(("",1), (acc, actions) =>  ((acc.Item1 + $"Player_{acc.Item2}:\n" + actions), acc.Item2 + 1)).Item1;*/
        spawnedElements?.ForEach(obj => Destroy(obj));

        for(int i = 0; i<allActions.Count; i++)
        {
            var header = Instantiate(playerHeaderPrefab, transform);
            header.text = $"Player_{i + 1}";
            spawnedElements.Add(header.gameObject);

            allActions[i].ForEach(action =>
            {
                var spawnedAction = Instantiate(playerAction, transform);
                spawnedAction.Init(i, action);
                spawnedElements.Add(spawnedAction.gameObject);
            });
        }
    }
}

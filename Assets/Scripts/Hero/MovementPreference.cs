using UnityEngine;

[CreateAssetMenu(fileName = "MovementPreference", menuName = "ScriptableObjects/MovementPreference", order = 1)]
public class MovementPreference : ScriptableObject
{
    public float AdjecentEnemyModifier;
    public bool MultipleAdjacentEnemyPreference;


    public MovementPreference(MovementPreference preference)
    {
        AdjecentEnemyModifier = preference.AdjecentEnemyModifier;
        MultipleAdjacentEnemyPreference = preference.MultipleAdjacentEnemyPreference;
    }


}

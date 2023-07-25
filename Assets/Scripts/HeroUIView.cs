using TMPro;
using UnityEngine;

public class HeroUIView : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] TextMeshProUGUI info;

    private void OnEnable()
    {
        TurnSequenceController.Instance.onHeroSelected += ShowSelectedHeroUI;
        //TurnSequenceController.Instance.onHeroUnselected += HideUI;
    }

    private void OnDisable()
    {
        TurnSequenceController.Instance.onHeroSelected -= ShowSelectedHeroUI;

    }

    /*public void OnDisable()
    {
        TurnSequenceController.Instance.onHeroSelected -= ShowSelectedHeroUI;
        TurnSequenceController.Instance.onHeroUnselected -= HideUI;
    }*/

    private void ShowSelectedHeroUI(HeroController hero)
    {
        var heroStats = hero.GetHeroStats();
        var current = heroStats.Item1;
        var baseStats = heroStats.Item1;
        var heroCurrentMovement = current.Move != baseStats.Move ? $"{current.Move}({baseStats.Move})" : $"{baseStats.Move}";
        info.text = $"HP: {current.Health}/{baseStats.Health}  AP: {hero.RemainingActions}/{baseStats.ActionLimit}  " +
            $"Move:  {heroCurrentMovement}  Range: {current.WeaponRange}";
        content.SetActive(true);
    }

    private void HideUI()
    {
        content.SetActive(false);
    }
}

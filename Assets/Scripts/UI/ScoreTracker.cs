using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreTracker : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerScoreText;
        private void Start()
        {
            TurnSequenceController.Instance.OnScoreUpdated += UpdateScore;
        }

        private void UpdateScore(int playerScore, int aiScore)
        {
            playerScoreText.text = $"{playerScore} : {aiScore}";
        }
    }
}
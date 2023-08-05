using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitAnimations : MonoBehaviour
{
    [SerializeField] private List<string> specialAbilities = new List<string>() { "Attack02", "Attack03" };
    [SerializeField] private string basicAttack = "Attack01";
    private Animator animator;
    private Vector3 position;
    private Dictionary<string, int> specialAnimsHashDict = new Dictionary<string, int>();

    private int isWalkingHash;
    private int victoryHash;
    private int dizzyHash;
    private int dieHash;
    private int getHitHash;
    private int attackHash;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        isWalkingHash = Animator.StringToHash("IsWalking");
        victoryHash = Animator.StringToHash("Victory");
        dizzyHash = Animator.StringToHash("Dizzy");
        dieHash = Animator.StringToHash("Die");
        getHitHash = Animator.StringToHash("GetHit");
        attackHash = Animator.StringToHash(basicAttack);
        specialAbilities.ForEach(ability => specialAnimsHashDict.Add(ability, Animator.StringToHash(ability)));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            PlayMove();
        if (Input.GetKeyDown(KeyCode.F2))
            PauseMoveAnimation();
        if (Input.GetKeyDown(KeyCode.F3))
            PlayAttack();
        if (Input.GetKeyDown(KeyCode.F4))
            PlayVictory();
        if (Input.GetKeyDown(KeyCode.F5))
            PlayDie();
        if (Input.GetKeyDown(KeyCode.F6))
            PlayGetHit(2f);
        if (Input.GetKeyDown(KeyCode.F7))
            PlayDizzy();
        if (Input.GetKeyDown(KeyCode.F8))
            PlaySpecialAbillity("Attack02");
    }

    private void PlayMove()
    {
        animator.SetBool(isWalkingHash, true);
    }

    private void PauseMoveAnimation()
    {
        animator.SetBool(isWalkingHash, false);
    }

    private void PlayAttack()
    {
        animator.Play(attackHash);
    }

    private void PlayVictory()
    {
        animator.Play(victoryHash);
    }
    private void PlayDie()
    {
        animator.Play(dieHash);
    }
    private void PlayDizzy()
    {
        animator.Play(dizzyHash);
    }
    private void PlayGetHit(float delay)
    {
        StartCoroutine(GetHitCoroutine(delay));
    }

    private void PlaySpecialAbillity(string abilityName)
    {
        animator.Play(specialAnimsHashDict[abilityName]);
    }

    IEnumerator GetHitCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.Play(getHitHash);
    }
}

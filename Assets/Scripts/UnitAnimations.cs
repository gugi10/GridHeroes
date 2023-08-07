using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitAnimations : MonoBehaviour
{
    [SerializeField] private List<AnimationPack> specialAbilities = new List<AnimationPack>() { new AnimationPack("Attack02", null), new AnimationPack("Attack03", null) };
    [SerializeField] private AnimationPack basicAttack = new AnimationPack("Attack01", null);
    [SerializeField] private Animator animator;
    [SerializeField] private HeroController heroController;
    private Vector3 position;
    private Dictionary<string, int> specialAnimsHashDict = new Dictionary<string, int>();

    private int isWalkingHash;
    private int victoryHash;
    private int dizzyHash;
    private int dieHash;
    private int getHitHash;
    private int attackHash;

    void Awake()
    {
        isWalkingHash = Animator.StringToHash("IsWalking");
        victoryHash = Animator.StringToHash("Victory");
        dizzyHash = Animator.StringToHash("Dizzy");
        dieHash = Animator.StringToHash("Die");
        getHitHash = Animator.StringToHash("GetHit");
        attackHash = Animator.StringToHash(basicAttack.animationName);
        specialAbilities.ForEach(ability => specialAnimsHashDict.Add(ability.animationName, Animator.StringToHash(ability.animationName)));
    }

    private void OnEnable()
    {
        heroController.onActionEvent += ParseUnitAction;
        heroController.OnMoveStart += PlayMove;
    }

    private void OnDisable()
    {
        heroController.onActionEvent -= ParseUnitAction;
        heroController.OnMoveStart -= PlayMove;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            PlayMove();
        if (Input.GetKeyDown(KeyCode.F2))
            StopMove();
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

    public void PlayParticlesForSpecialAttack(string animationString)
    {
        var particleSystem = specialAbilities.FirstOrDefault(x => x.animationName.Equals(animationString)).particles;
        if (particleSystem != null)
            particleSystem.Play();
    }

    public void PlayParticlesForBasicAttack()
    {
        basicAttack.particles?.Play();
    }

    private void ParseUnitAction(HeroAction action)
    {
        switch (action)
        {
            case HeroAction.Move:
                StopMove();
                return;
            case HeroAction.Attack:
                PlayAttack();
                return;
            default:
                return;
        }
    }

    private void PlayMove()
    {
        Debug.Log($"play move animation");

        animator.SetBool(isWalkingHash, true);
    }

    private void StopMove()
    {
        Debug.Log($"stop move animation");

        animator.SetBool(isWalkingHash, false);
    }

    private void PlayAttack()
    {
        Debug.Log($"Play attack animation");
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

    private void PlaySpecialAbillity(string abilityName = "Attack02")
    {
        animator.Play(specialAnimsHashDict[abilityName]);
    }

    IEnumerator GetHitCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.Play(getHitHash);
    }
}

[System.Serializable]
public struct AnimationPack
{
    public string animationName;
    public ParticleSystem particles;

    public AnimationPack(string animationName, ParticleSystem particles)
    {
        this.animationName = animationName;
        this.particles = particles;
    }
}

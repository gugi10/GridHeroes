using UnityEngine;

namespace SpecialAbilities
{
    public interface ISpecialAbilityFX
    {
        public void StartAnimation();
    }

    public class WhirlwindAbilityFX : ISpecialAbilityFX
    {
        private HeroController source;
        private readonly string animationId;
        private readonly UnitAnimations unitAnimations;
        
        public WhirlwindAbilityFX(HeroController source, string animationId = "Attack02")
        {
            this.source = source;
            this.animationId = animationId;
            unitAnimations = source.GetComponent<UnitAnimations>();
        }
        public void StartAnimation()
        {
            Debug.Log($"Start animation");
            if(unitAnimations == null)
                return;
            Debug.Log($"Start play {animationId}");

            unitAnimations.PlaySpecialAbillity(animationId);
        }
    }
}
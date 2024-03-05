using System.Collections;
using System.Threading.Tasks;
using RedBjorn.ProtoTiles;
using UnityEngine;
using System;

namespace SpecialAbilities
{
    public interface ISpecialAbilityFX
    {
        public void StartAnimation(TileEntity chosenTile, Action onFinishCallback);
    }

    public class WhirlwindAbilityFX : ISpecialAbilityFX
    {
        private HeroController source;
        private readonly string animationId;
        private readonly UnitAnimations unitAnimations;
        
        public WhirlwindAbilityFX(HeroController source, string animationId)
        {
            this.source = source;
            this.animationId = animationId;
            unitAnimations = source.GetComponent<UnitAnimations>();
        }
        public void StartAnimation(TileEntity chosenTile, Action onFinishCallback)
        {
            if(unitAnimations == null)
                return;

            unitAnimations.PlaySpecialAbillity(animationId);
        }
    }
    
    public class FireboltFx : ISpecialAbilityFX
    {
        private readonly string animationId;
        private readonly UnitAnimations unitAnimations;
        private readonly ProjectileAnimation projectileAnimation;
        private readonly MapEntity map;
        private readonly HeroController source;
        private readonly BasicProperties properties;

        public FireboltFx(HeroController source, MapEntity map, BasicProperties properties, string animationId)
        {
            this.map = map;
            this.animationId = animationId;
            this.source = source;
            this.properties = properties;
            unitAnimations = source.GetComponent<UnitAnimations>();
            projectileAnimation = source.GetComponentInChildren<ProjectileAnimation>();
        }
        public void StartAnimation(TileEntity chosenTile, Action onFinishCallback)
        {
            unitAnimations.PlaySpecialAbillity(animationId);
            projectileAnimation.PlayProjectile(map.WorldPosition(chosenTile.Data.TilePos), 0.8f, CreateOnHit(chosenTile));
        }
        private System.Action CreateOnHit(TileEntity chosenTile)
        {
            return () => { OnHit(chosenTile); };
        }
        
        private void OnHit(TileEntity chosenTile)
        {
            chosenTile.occupyingHero.DealDamage(properties.damage);
            source.onSpecialAbilityFinished();
        }
    }

    public class PushAbilityFx : ISpecialAbilityFX
    {
        private HeroController source;
        private readonly string animationId;
        private readonly UnitAnimations unitAnimations;
        
        public PushAbilityFx(HeroController source, string animationId)
        {
            this.source = source;
            this.animationId = animationId;
            unitAnimations = source.GetComponent<UnitAnimations>();
        }
        public void StartAnimation(TileEntity chosenTile, Action onFinishCallback)
        {
            if(unitAnimations == null)
                return;

            unitAnimations.PlaySpecialAbillity(animationId);
        }
    }
    
    public class WebPullFx : ISpecialAbilityFX
    {
        private readonly string animationId;
        private readonly UnitAnimations unitAnimations;
        private readonly ProjectileAnimation projectileAnimation;
        private readonly MapEntity map;
        private readonly HeroController source;
        private readonly BasicProperties properties;
        private Action onFinishCallback;

        public WebPullFx(HeroController source, MapEntity map, BasicProperties properties, string animationId)
        {
            this.map = map;
            this.animationId = animationId;
            this.source = source;
            this.properties = properties;
            unitAnimations = source.GetComponent<UnitAnimations>();
            projectileAnimation = source.GetComponentInChildren<ProjectileAnimation>();
        }
        public void StartAnimation(TileEntity chosenTile, Action onFinishCallback)
        {
            this.onFinishCallback = onFinishCallback;
            unitAnimations.PlaySpecialAbillity(animationId);
            projectileAnimation.PlayProjectile(map.WorldPosition(chosenTile.Data.TilePos), 0.3f, CreateOnHit(chosenTile));
        }
        private System.Action CreateOnHit(TileEntity chosenTile)
        {
            return () => { OnHit(chosenTile); };
        }
        
        private async void OnHit(TileEntity chosenTile)
        {
            onFinishCallback?.Invoke();
            chosenTile.occupyingHero.DealDamage(properties.damage);
            await Task.Delay(3000);
            source.onSpecialAbilityFinished();
        }
    }
}
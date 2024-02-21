using RedBjorn.ProtoTiles;
using RedBjorn.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskKind
{
    AttackEnemy = 1,
    UseAbility = 2,
    MoveForward = 3,
}

public struct Task
{
    public readonly TaskKind kind;

    public Task(TaskKind kind)
    {
        this.kind = kind;
    }
}

public class PossibleTask
{
    public Task task;
    public TileEntity objective;

    public PossibleTask(Task task, TileEntity objective)
    {
        this.task = task;
        this.objective = objective;
    }
}

class PossibleAssignment
{
    public double score;
    public PossibleTask possibleTask;
    public HeroController assignee;

    public PossibleAssignment(double score, PossibleTask possibleTask, HeroController assignee)
    {
        this.score = score;
        this.possibleTask = possibleTask;
        this.assignee = assignee;
    }
}


public class AIAgent : MonoBehaviour, IPlayer
{
    public PlayerId Id { get; set; }
    private MapController map;
    private List<HeroController> allHeroes = new List<HeroController>();
    private List<HeroController> aiHeroes = new List<HeroController>();
    private List<HeroController> playerHeroes = new List<HeroController>();

    public void Init(MapController map, List<HeroController> allHeroes, PlayerId id)
    {
        Id = id;
        this.map = map;
        this.allHeroes = allHeroes;
        this.aiHeroes = allHeroes.FindAll(hero => hero.ControllingPlayerId == Id);
        this.playerHeroes = allHeroes.FindAll(hero => hero.ControllingPlayerId != Id);
    }
    public void SetActiveState(bool flag)
    {
        if (!flag)
        {
            return;
        }

        StartCoroutine(DelayedAiAction());
    }

    private IEnumerator DelayedAiAction()
    {
        yield return new WaitForSeconds(1f);
        AiAction();
    }

    private void AiAction()
    {
        var aiHeroes = allHeroes.FindAll(hero => hero.ControllingPlayerId == Id);
        if (aiHeroes.Count <= 0)
            return;

        Task[] tasks = { new Task(TaskKind.AttackEnemy), new Task(TaskKind.UseAbility), new Task(TaskKind.MoveForward) };
        List<TileEntity> allTileEntites = map.GetMapEntity().Tiles.ToList().Select(pair => pair.Value).ToList();

        PossibleTask[] possibleTasks = tasks.SelectMany(task =>
        {
            return allTileEntites.Select(tileEntity =>
            {
                return new PossibleTask(task, tileEntity);
            }).ToArray();
        }).ToArray();


        PossibleAssignment[] possibleAssignments = { };
        foreach (var possibleTask in possibleTasks)
        {
            foreach (var aiHero in aiHeroes)
            {
                if (possibleTask.task.kind == TaskKind.AttackEnemy)
                {
                    if (possibleTask.objective.IsOccupied &&
                        possibleTask.objective.occupyingHero.ControllingPlayerId == 0 &&
                        IsTileInRange(aiHero, possibleTask.objective.Data, aiHero.GetHeroStats().current.WeaponRange)
                            && (TurnSequenceController.Instance.GetPlayerRemainingActions(this.Id).Contains(HeroAction.Special)
                            || TurnSequenceController.Instance.GetPlayerRemainingActions(this.Id).Contains(HeroAction.Attack)))
                    {
                        possibleAssignments = possibleAssignments.Append(new PossibleAssignment((tasks.Length - (int)possibleTask.task.kind), possibleTask,
                            aiHero)).ToArray();
                    }
                }

                if (possibleTask.task.kind == TaskKind.UseAbility && (TurnSequenceController.Instance.GetPlayerRemainingActions(this.Id).Contains(HeroAction.Special)))
                {
                    var objectiveTileEntity = possibleTask.objective;
                    if (aiHero.specialAbilities2[0].CanBeUsedOnTarget(objectiveTileEntity))
                    {
                        possibleAssignments = possibleAssignments.Append(new PossibleAssignment(
                            ScoreAbility(tasks.Length, possibleTask.task.kind, aiHero.specialAbilities2[0].ScoreForTarget(possibleTask.objective.occupyingHero)), possibleTask, aiHero)
                        ).ToArray();
                    }

                }

                if (possibleTask.task.kind == TaskKind.MoveForward)
                {
                    if (!possibleTask.objective.IsOccupied &&
                        possibleTask.objective.Vacant &&
                        // TODO:
                        // without this check AI scores movement also for tiles that are currently not reachable, so it means it will move along a path to a destination even
                        // if it would reach it not in a single activation. Is this what we want?
                        // IsTileInRange(aiHero, possibleTask.objective.Data, aiHero.GetHeroStats().current.Move) &&
                        (TurnSequenceController.Instance.GetPlayerRemainingActions(this.Id).Contains(HeroAction.Special)
                            || TurnSequenceController.Instance.GetPlayerRemainingActions(this.Id).Contains(HeroAction.Move)))
                    {
                        possibleAssignments = possibleAssignments.Append(new PossibleAssignment(ScoreMovement(tasks.Length, possibleTask, aiHero), possibleTask, aiHero)).ToArray();
                    }
                }
            }
        }
        if (possibleAssignments.Length == 0) // We cant do anything so we need to pass
        {
            TurnSequenceController.Instance.FinishTurn(TurnSequenceController.Instance.GetPlayerRemainingActions(Id)[0]);
            return;
        }
        var chosenAssignment = ChooseBestPossibleAssignment(possibleAssignments);
        if (chosenAssignment == null)
        {
            Debug.Log("It is possible that there are no possible actions to do (we have only attacks byt nobody to attack is in range).");
            TurnSequenceController.Instance.FinishTurn(TurnSequenceController.Instance.GetPlayerRemainingActions(Id)[0]);
            return;

        }

        if (chosenAssignment.possibleTask.task.kind == TaskKind.AttackEnemy)
        {
            var targetTileEntity = chosenAssignment.possibleTask.objective;
            chosenAssignment.assignee.Attack(targetTileEntity);
            return;

        }

        if (chosenAssignment.possibleTask.task.kind == TaskKind.UseAbility)
        {
            var targetTileEntity = chosenAssignment.possibleTask.objective;
            chosenAssignment.assignee.specialAbilities2[0].PerformAbility(targetTileEntity);
            return;

        }

        if (chosenAssignment.possibleTask.task.kind == TaskKind.MoveForward)
        {
            var aiHero = chosenAssignment.assignee;
            //trzeba przeliczyc pathy dla kazdego wealkable tile'a i odfiltrowac te ktore sa w zasiegu iczy nic nie blokuje.
            //var walkableTiles = map.GetMapEntity().WalkableTiles(aiHero.currentTile.TilePos, aiHero.GetHeroStats().current.Move).Where(x => !x.IsOccupied).ToList();
            //var randomWalkableTileIdx = Random.Range(0, walkableTiles.Count);
            //var selectedRandomTile = walkableTiles[randomWalkableTileIdx].Data.TilePos;
            //Debug.Log($"selected Tile {selectedRandomTile} for {aiHero.gameObject.name}");
            var path = map.GetMapEntity().PathTiles
                (aiHero.transform.position, map.GetMapEntity().WorldPosition(chosenAssignment.possibleTask.objective.Position), aiHero.GetHeroStats().current.Move);
            string pathstring = "";

            foreach (var tiles in path)
            {
                pathstring += $"{tiles.Data.TilePos}";
            }
            Debug.Log($"Path string {pathstring}");
            if (path != null || path.Count > 0)
                aiHero.MoveByPath(path);
            //randomAiHero.Move(walkableTiles[randomWalkableTileIdx]);
            return;

        }


        // We should not get here ever but just in case lets pass here
        TurnSequenceController.Instance.FinishTurn(TurnSequenceController.Instance.GetPlayerRemainingActions(Id)[0]);
    }

    private bool IsTileInRange(HeroController aiHero, TileData tileData, int range)
    {
        return TileUtilities.AreTilesInRange(aiHero.currentTile.TilePos, tileData.TilePos, range);
    }

    private bool IsEnemyInRange(HeroController aiHero, HeroController playerHero, int range)
    {
        return TileUtilities.AreTilesInRange(aiHero.currentTile.TilePos, playerHero.currentTile.TilePos, range);
    }

    private HeroController FindEnemyInRange(HeroController aiHero, int range)
    {
        var enemies = allHeroes.Where(hero => hero.ControllingPlayerId != Id).ToList();
        var enemyInRange = enemies.FirstOrDefault(hero => TileUtilities.AreTilesInRange(hero.currentTile.TilePos, aiHero.currentTile.TilePos, range));
        // Debug.Log($"Player hero  in range= {playerHeroInRange.gameObject.name}");
        return enemyInRange;
    }
    private List<HeroController> FindEnemiesInRange(HeroController aiHero, int range)
    {
        var enemies = allHeroes.Where(hero => hero.ControllingPlayerId != Id).ToList();
        var enemiesInRange = enemies.FindAll(hero => TileUtilities.AreTilesInRange(hero.currentTile.TilePos, aiHero.currentTile.TilePos, range));
        // Debug.Log($"Player hero  in range= {playerHeroInRange.gameObject.name}");
        return enemiesInRange;
    }

    private List<(HeroController, int)> ScoreAiHeroes()
    {

        // First check if there is any enemy that is within range of ability, the more enemies are closer the better score
        if (TurnSequenceController.Instance.GetPlayerRemainingActions(Id).Contains(HeroAction.Special))
        {
            var heroesInRangeOfAbility = aiHeroes.Select(aiHero => { return (aiHero, FindEnemiesInRange(aiHero, ExtractAbilityRangeFromHero(aiHero)).Count); }).OrderByDescending(pair => pair.Item2).ToList();
            if (heroesInRangeOfAbility.Any(pair => pair.Item2 > 0))
            {
                return heroesInRangeOfAbility;
            }
        }

        // Second check if there is any enemy that is within range of attack, the more enemies are closer the better score
        if (TurnSequenceController.Instance.GetPlayerRemainingActions(Id).Contains(HeroAction.Attack))
        {
            var heroesInRangeOfAttack = aiHeroes.Select(aiHero => { return (aiHero, FindEnemiesInRange(aiHero, aiHero.GetHeroStats().current.WeaponRange).Count); }).OrderByDescending(pair => pair.Item2).ToList();
            if (heroesInRangeOfAttack.Any(pair => pair.Item2 > 0))
            {
                return heroesInRangeOfAttack;
            }
        }

        var copy = aiHeroes.ToList();
        var rnd = new System.Random();
        copy.Shuffle(rnd);
        return copy.Select(hero => (hero, 0)).ToList();
    }

    private int ExtractAbilityRangeFromHero(HeroController hero)
    {
        var ability = hero.specialAbilities2[0];
        var abilitySpec = ability.GetAbilitySpec();

        var properties = abilitySpec.properties;
        return properties.range;
    }

    private double ScoreAbility(int taskCount, TaskKind taskKind, ScoreModifiers modifiers)
    {
        return (taskCount - (int)taskKind) + modifiers.enemiesKilled * 0.5 + modifiers.inflictedDamage * 0.1;
    }

    private double ScoreMovement(int taskCount, PossibleTask task, HeroController aiHero)
    {
        TileEntity objectiveTile = task.objective;

        List<TileEntity> tilesInWeaponRange = FindTilesInRangeFromTile(objectiveTile, aiHero.GetHeroStats().current.WeaponRange);
        int enemiesInWeaponRange = tilesInWeaponRange.Where(tile => tile.IsOccupied && tile.occupyingHero.ControllingPlayerId == PlayerId.Human).Count();

        List<TileEntity> tilesInSpecialAbilityRange = FindTilesInRangeFromTile(objectiveTile, aiHero.specialAbilities2[0].GetAbilitySpec().properties.range);
        int enemiesInSpecialAbilityRange = tilesInSpecialAbilityRange.Where(tile => tile.IsOccupied && tile.occupyingHero.ControllingPlayerId == PlayerId.Human).Count();

        List<TileEntity> adjacentTiles = FindAdjacentTiles(objectiveTile);
        int adjacentEnemiesCount = adjacentTiles.Where(tile => tile.IsOccupied && tile.occupyingHero.ControllingPlayerId == PlayerId.Human).Count();

        bool heroHasRangeWeapon = aiHero.GetHeroStats().current.WeaponRange > 1;
        bool heroHasRangSpecialAbility = aiHero.specialAbilities2[0].GetAbilitySpec().properties.range > 1;
        bool heroIsRangeHero = heroHasRangSpecialAbility;


        float bonusScore = 0.0f;
        MovementPreference preference = aiHero.GetMovementPreference();
        if (adjacentEnemiesCount == 0)
        {

        }
        else if (adjacentEnemiesCount == 1)
        {
            bonusScore += (preference.AdjecentEnemyModifier * adjacentEnemiesCount);
        }
        else if (preference.MultipleAdjacentEnemyPreference)
        {
            bonusScore += (preference.AdjecentEnemyModifier * adjacentEnemiesCount);
        }
        else
        {
            bonusScore += (preference.AdjecentEnemyModifier + (-preference.AdjecentEnemyModifier) * (adjacentEnemiesCount - 1));
        }



        if (heroIsRangeHero)
        {
            if (TurnSequenceController.Instance.GetPlayerRemainingActions(this.Id).Contains(HeroAction.Special))
            {
                bonusScore += enemiesInSpecialAbilityRange * 0.2f;
            }
        }
        else
        {
            bonusScore += enemiesInWeaponRange * 0.2f;
        }

        if (bonusScore == 0)
        {

        }

        return (taskCount - (int)task.task.kind) + (bonusScore);
    }


    private List<TileEntity> FindTilesInRangeFromTile(TileEntity objectiveTile, int range)
    {
        List<TileEntity> tilesInRange = map.GetMapEntity().Area(objectiveTile.Position, range)
            .Select(position => map.GetMapEntity().Tile(position)).Where(tile => tile != null).ToList();
        return tilesInRange;

    }
    private List<TileEntity> FindAdjacentTiles(TileEntity objectiveTile)
    {
        List<TileEntity> adjacentTilePositions = map.GetMapEntity().Area(objectiveTile.Position, 1)
            .Select(position => map.GetMapEntity().Tile(position)).Where(tile => tile != null).ToList();
        return adjacentTilePositions;

    }

    private PossibleAssignment ChooseBestPossibleAssignment(PossibleAssignment[] possibleAssignments)
    {
        var sortedPossibleAssignments = possibleAssignments.OrderByDescending(assignment => assignment.score).ToArray();

        var bestScore = sortedPossibleAssignments[0].score;
        var bestPossibleAssignments = sortedPossibleAssignments.Where(assignment => assignment.score == bestScore).ToList();

        var rnd = new System.Random();
        bestPossibleAssignments.Shuffle(rnd);
        return bestPossibleAssignments[0];
    }

}
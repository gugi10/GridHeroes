# Short Overview

The game is inspired by other grid and turn based like games similar
to WarChest. Another example is Neuroshima Hex. Those games are often
characterized by being abstract and mostly determined by
their mechanics.

## Do we want to keep our game abstract?

Yes we do as it is has benefits of being easier to design and
can be treated as a base on which other games can be developed
with an actual fluff that maybe can be even integrated with
the mechanics.

## Gameplay type

We find two categories of a game like this:

- versus (like WarChest) - in this type of game the core gameplay loop consists of player facing against player/ai
  in a skirmish kind of way, where both participants have the same fair set of possible actions to perform,
  it does not mean that there must be symetry between the participants resource/forces/armies, etc.,
- puzzle-like (puzzle mode in Neuroshima Hex) - in this type the ai might be govern by completely different set of
  rules, perhaps even more strict one. The gameplay might feel more like solving a puzzle instead of facing against
  a real oponnent.

We think that versus type should be the first and core design to be implemented since it is simpler. Also having one
does not mean the other one cannot be integrated into the game as well. The puzzle one can also be based on the rules
from the first one.

## Progression

At this stage we do not plan to implement any progression. We might discuss this again after having the core gameplay
implemented.

# Core Gameplay Loop

Each skirmish happens on hex-based grid map. Hexes can be of different types for example an impassable hex.
Each hex can be occupied by only one unit.

At the beginning of every turn each player gets a random set of actions they can choose from for example:
- A player gets 5 actions, for example 1 attack 1 attack 1 move 1 move 1 special action, and can use them
  in any order on any unit they want, but the cannot use other actions that are not in their pool.

A unit cannot make more than two actions.


To not leave what's possible to do in a player's turn to just a chance we could add chance manpulation mechanic:

- a player could get one more action that the number they perform in a turn and the action that is left at the end of a turn moves to the next turn,
- shuffle when you get all the same actions or something similar,
- there could be a bag of actions from which player draws a number of actions at the beginning of every turn.

## Game sequence

1. Players can choose to either have assigned to them randomly picked units (MVP) or choose to draft.
2. Both players deploy at the same time with fog of war or players deploy alternating without fog of war (here initiative belongs to the player who ended deploying first).
   Deployment happens on predefined hex locations.
3. First turn starts.
4. Game ends when all units of either player get eliminated.

## Turn phase

1. Player receive actions they can perform during activations,
   Possibly they perform some chance manipulation, e.g., second player rerolls or something,
2. Activations phase

### Types of turn phase
1. You go I go - one player activates all his units first then other player,
2. Unit by unit - first player activates one unit then the other player does the same and the repeat until all unit have been activated,
3. Predetermined order - order is predetermined by some mechancie, e.g., initiative values associated with units.

We think that 2nd is more interactive than 1st one and has less change in ending in analysis paralysis. Also the 3rd one might
by furher developed from 2nd one.

## Activations phase

Types of actions:
- move (base 1),
- attack (dmg 1 to next hex),
- special action,
- charge (rare move + attack combination),
- mission action (depending on what we decide is a goal)

## Unit Characteristics

- Health Points
- Active Special Ability
- Passive Special Ability (a modifier that can modify how move or attack)

## Special abilities and classes [wip]
All stuff written in this topic is only an idea and concept, which can be changed
We can distinguish two types of abilities passive and ones that are used as an activation. Passive abilities can come as a modifiers to base stats of units. 
We are assuming that for now maximum of 1 ability of each type is defined for a class.
Class is an idea to follow typical game desigin which categorizes certain units into groups for example: Tanks, DD's, supports.
Preset of abilities combination can be a specialization, we need to decided if its predefined or players can decide what they want to pick.
For example one player decides to go with Ranged Attacker, Tank and two Supports picking different abilities for each of them or following the other idea chosing among the specialization presets.

Classes with abilities ideas:
1. Ranged attacker:
   - passive #1: Can only attack units that are 2 hexes away
   - active #1: Can perform range 3 attack
   - active #2: Can perform free move action after attack
2. Tank:
   - passive #1: Has a x% to negate incoming attack damage, to balance the x% value can be highly decreased after each succesful block or limit the time it occurs
   - passive #2: Has 2/3 health
   - active #1: Add temporary health to self for few turns
   - active #2: If we would got with negate incoming damage and limit it occurance, this ability could bring back the original value for this passive abillity)
   - active #3: Gain invulnerability to negative effects and damage for short amount
3. Support:
   - passive #1: Other allies that begin move action within 2 hexes of this unit have +1 to move
   - active #1: Reroll one action to different one
   - active #2: Remove all negative effects
   - active #3: Reset action point limits for target unit within range
   - active #4: Push target other unit
4. Melee attacker
   - passive #1: +1 move
   - passive #2: After killing enemy or capturing objective(?) gain +1 action for this turn
   - active #1: Double move action (we can chose if we want to consuem special + move actions or only special, it should only cost 1 action point)
5. Mage?Enchanter?Utility?Misc?
   - passive #1: Can consume two basic actions to perform special one
   - active #1: Summon/Spawn base unit (1move 1hp 1range) (It can be limited by having only one active unit)
   - active #2: Creat impassable hexes (Like stone boulders)
   - active #3: Creat damaging hexes (like firewall or smth)

### Keywords ideas [wip]

In order to group up abilities and their effect I came up with an idea of keywording each of them in terms of their: range, effect, possible target etc.
It can make implementation easier
As an example:
1. Area effects:
   - Area of effect: All units in target ranged area (Aoe)
   - Aura: All units in range from the source (Aura)
   - Targeted: Target specific unit (Target)
2. Possible targets
   - Only enemies (Enemy)
   - Only Allies (Allies)
   - All units (All)
   - Other units (exclude self) (All other, OTher allies)
   - Can target self? (Self)
   - Can target unit which are under specific condition (for example, next to enemy, next to another ally, have no more than 1 health) (Conditional)
3. Effect
     - Damage
     - Modifier
     - Buff
     - Debuff
     - Terrain modifiers 
     - Possible actions manipulation (Reroll one action to another
4. Duration
     - Fixed amount of turns
     - Fixed amount of rounds
     - Until some event

 

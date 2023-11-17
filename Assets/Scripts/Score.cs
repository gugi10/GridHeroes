using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//public class Score
// {
 //   public const bool
//}

public struct ScoreModifiers
{
    public int enemiesKilled;
    public int inflictedDamage;
}

class ScoreModifiersBuilder
{
    ScoreModifiers modifiers;
    public ScoreModifiersBuilder()
    {
        modifiers = new ScoreModifiers { };
    }
}
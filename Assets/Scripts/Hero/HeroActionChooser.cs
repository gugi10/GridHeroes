using System;

class HeroActionChooser
{
    static public HeroAction ChooseRandomAction()
    {
        Array values = Enum.GetValues(typeof(HeroAction));
        Random random = new();
        HeroAction randomHeroAction = (HeroAction) values.GetValue(random.Next(values.Length));
        return randomHeroAction;
    }
}
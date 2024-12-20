using System.Collections.Generic;

public enum ERedDotType
{
    None = 0,
    Root,

    MainMenu,
    MainMenu_Skill,
    MainMenu_Pet,

    Skill_LevelUp,
    Skill_StarLevelUp,

    Pet_New,
}

public class ReddotTypeComparer : IEqualityComparer<ERedDotType>
{
    public bool Equals(ERedDotType x, ERedDotType y)
    {
        return x == y;
    }

    public int GetHashCode(ERedDotType x)
    {
        return (int)x;
    }
}
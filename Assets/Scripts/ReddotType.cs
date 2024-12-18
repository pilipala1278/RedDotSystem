using System.Collections.Generic;

public enum EReddotType
{
    None = 0,
}

public class ReddotTypeComparer : IEqualityComparer<EReddotType>
{
    public bool Equals(EReddotType x, EReddotType y)
    {
        return x == y;
    }

    public int GetHashCode(EReddotType x)
    {
        return (int)x;
    }
}
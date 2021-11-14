namespace Popug.Common.Monads;

public class Of
{
    private static readonly None _none = new None();

    public static None None() => _none;
}


using System.Collections.Generic;

/// <summary>
///     Nice functions to use in any class
/// </summary>
public static class UsefulFunctions
{
    // Allows the list of nodes to be sorted when adding in values
    public static void InsertElementAscending(this List<Node> source,
        Node element)
    {
        var index = source.FindLastIndex(e => e < element);
        if (index == 0 || index == -1)
        {
            source.Insert(0, element);
            return;
        }

        source.Insert(index + 1, element);
    }
}
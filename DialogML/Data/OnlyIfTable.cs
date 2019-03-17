using System;
using System.Collections.Generic;

public class OnlyIfTable
{
    HashSet<Guid> visted = new HashSet<Guid>();

    public void MarkVisited(Guid id)
    {
        visted.Add(id);
    }

    public bool HasOnlyIfBeenExecuted(Guid id)
    {
        return visted.Contains(id);
    }
}
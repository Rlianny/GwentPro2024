using UnityEngine;
using System;

public class SemanticChecker : VisitorBase<bool>
{
    public bool CheckSemantic(IASTNode node)
    {
        return VisitBase(node);
    }

}
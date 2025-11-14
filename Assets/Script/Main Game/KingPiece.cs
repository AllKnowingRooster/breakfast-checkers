using System.Collections.Generic;
using UnityEngine;

public class KingPiece : CheckersPiece
{
    private List<Vector2Int> movementPattern = new List<Vector2Int>()
    {
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,-1)
    };

    public override void GetPossibleMoves(ref CheckersPiece[,] listPiece)
    {
        (listMove, listAttack) = Movement.GenerateMoves(this, ref movementPattern, ref listPiece);
    }
}

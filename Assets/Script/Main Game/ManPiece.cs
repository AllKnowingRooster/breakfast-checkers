using System.Collections.Generic;
using UnityEngine;

public class ManPiece : CheckersPiece
{
    public override void GetPossibleMoves(ref CheckersPiece[,] listPiece)
    {
        List<Vector2Int> movementPattern = new List<Vector2Int>()
        {
            new Vector2Int(1,team==Team.Cake?1:-1),
            new Vector2Int(-1,team==Team.Cake?1:-1)
        };
        (listMove, listAttack) = Movement.GenerateMoves(this, ref movementPattern, ref listPiece);
    }

    public void Promote(ref CheckersPiece[,] listPiece)
    {
        int promoteArea = team == Team.Cake ? GameManager.instance.col - 1 : 0;
        if (y != promoteArea)
        {
            return;
        }
        ChangeMesh();
        ChangeComponent(ref listPiece);
    }

    private void ChangeMesh()
    {
        Transform mesh = gameObject.transform.Find("Mesh");
        for (int i = 0; i < mesh.childCount; i++)
        {
            GameObject meshChild = mesh.GetChild(i).gameObject;
            if (meshChild.name == "King")
            {
                meshChild.SetActive(true);
                continue;
            }
            meshChild.SetActive(false);
        }

    }

    private void ChangeComponent(ref CheckersPiece[,] listPiece)
    {
        KingPiece king = gameObject.AddComponent<KingPiece>();
        king.x = x;
        king.y = y;
        king.desiredPos = desiredPos;
        king.team = team;
        king.pieceType = PieceType.King;
        Destroy(GetComponent<ManPiece>());
        listPiece[king.x, king.y] = king;
    }



}

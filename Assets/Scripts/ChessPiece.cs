using UnityEngine;



public enum TipeBuahCatur
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King=6
}

public class ChessPiece : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public TipeBuahCatur tipe;

    private Vector3 desiredPosition;
    private Vector3 desiredScale;
}

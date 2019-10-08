// ;
using System;

namespace CBack.Pieces
{
    public class QueenPiece : Piece
    {
        public QueenPiece(int _row = 0, int _column = 0, PieceColor _color = PieceColor.Black)
            : base(_row, _column, _color, PieceType.Queen)
        {
            MovementDirection.Add(Tuple.Create(-1, -1)); // 1;
            MovementDirection.Add(Tuple.Create(+0, -1)); // 2;
            MovementDirection.Add(Tuple.Create(+1, -1)); // 3;
            MovementDirection.Add(Tuple.Create(-1, +0)); // 4;
            MovementDirection.Add(Tuple.Create(+1, +0)); // 6;
            MovementDirection.Add(Tuple.Create(-1, +1)); // 7;
            MovementDirection.Add(Tuple.Create(+0, +1)); // 8;
            MovementDirection.Add(Tuple.Create(+1, +1)); // 9;
        }

        public override bool Move(int _dstRow, int _dstCol)
        {
            int[] table = new int[Game.RowSize * Game.ColumnSize];



            Console.WriteLine("QueenMove!");
            return base.Move(_dstRow, _dstCol);
        }

        public override int[] GetMovableMap(Piece[] _table)
        {
            int[] map = base.GetMovableMap(_table);

            int[] hmap = CastHorizontalRay(_table);
            int[] vmap = CastVerticalRay(_table);


            return map;
        }
    }
}

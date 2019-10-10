// ;
using System;

namespace CBack.Pieces
{
    public class RookPiece : Piece
    {
        public RookPiece(int _row, int _column, PieceColor _color)
            : base(_row, _column, _color, PieceType.Rook)
        { }

        public override bool Move(int _dstRow, int _dstCol)
        {
            int[] table = new int[Game.RowSize * Game.ColumnSize];



            //Console.WriteLine("RookMove!");
            return base.Move(_dstRow, _dstCol);
        }

        public override int[] GetMovableMap(Piece[] _table)
        {
            int[] map = base.GetMovableMap(_table);

            int[] hmap = CastHorizontalRay(_table);
            int[] vmap = CastVerticalRay(_table);

            for (int i = 0; i < _table.Length; ++i)
            {
                map[i] |= (hmap[i] | vmap[i]);
            }

            return map;
        }
    }
}

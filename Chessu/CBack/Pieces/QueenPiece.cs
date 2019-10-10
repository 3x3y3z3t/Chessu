// ;
using System;

namespace CBack.Pieces
{
    public class QueenPiece : Piece
    {
        public QueenPiece(int _row = 0, int _column = 0, PieceColor _color = PieceColor.Black)
            : base(_row, _column, _color, PieceType.Queen)
        { }

        public override bool Move(int _dstRow, int _dstCol)
        {
            int[] table = new int[Game.RowSize * Game.ColumnSize];



            //Console.WriteLine("QueenMove!");
            return base.Move(_dstRow, _dstCol);
        }

        public override int[] GetMovableMap(Piece[] _table)
        {
            int[] map = base.GetMovableMap(_table);

            int[] hmap = CastHorizontalRay(_table);
            int[] vmap = CastVerticalRay(_table);
            int[] swne = CastSWNERay(_table);
            int[] senw = CastSENWRay(_table);

            for (int i = 0; i < _table.Length; ++i)
            {
                map[i] |= (hmap[i] | vmap[i] | swne[i] | senw[i]);
            }

            return map;
        }
    }
}

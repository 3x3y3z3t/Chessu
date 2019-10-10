// ;
using System;

namespace CBack.Pieces
{
    public class BishopPiece : Piece
    {
        public BishopPiece(int _row = 0, int _column = 0, PieceColor _color = PieceColor.Black)
            : base(_row, _column, _color, PieceType.Bishop)
        { }

        public override bool Move(int _dstRow, int _dstCol)
        {
            int[] table = new int[Game.RowSize * Game.ColumnSize];



            //Console.WriteLine("BishopMove!");
            return base.Move(_dstRow, _dstCol);
        }

        public override int[] GetMovableMap(Piece[] _table)
        {
            int[] map = base.GetMovableMap(_table);
            
            int[] swne = CastSWNERay(map);
            int[] senw = CastSENWRay(map);

            for (int i = 0; i < _table.Length; ++i)
            {
                map[i] |= (swne[i] | senw[i]);
            }

            return map;
        }
    }
}

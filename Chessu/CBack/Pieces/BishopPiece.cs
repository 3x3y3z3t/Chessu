// ;
using System;

namespace CBack.Pieces
{
    public class BishopPiece : Piece
    {
        public BishopPiece(int _row, int _column, PieceColor _color, Game _owner)
            : base(_row, _column, _color, PieceType.Bishop, _owner)
        { }

        public override bool Move(int _dstRow, int _dstCol)
        {
            int[] table = new int[Game.RowSize * Game.ColumnSize];



            //Console.WriteLine("BishopMove!");
            return base.Move(_dstRow, _dstCol);
        }

        public override int[] GetMovableMap()
        {
            int[] map = base.GetMovableMap();
            
            int[] swne = CastSWNERay(map);
            int[] senw = CastSENWRay(map);

            for (int i = 0; i < map.Length; ++i)
            {
                map[i] |= (swne[i] | senw[i]);
            }

            return map;
        }
    }
}

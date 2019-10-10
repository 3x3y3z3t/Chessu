// ;
using System;

namespace CBack.Pieces
{
    public class KingPiece : Piece
    {
        public KingPiece(int _row = 0, int _column = 0, PieceColor _color = PieceColor.Black)
            : base(_row, _column, _color, PieceType.King)
        {
            MoveRange = 1;
        }

        public override bool Move(int _dstRow, int _dstCol)
        {
            int[] table = new int[Game.RowSize * Game.ColumnSize];



            //Console.WriteLine("BishopMove!");
            return base.Move(_dstRow, _dstCol);
        }

        public override int[] GetMovableMap(Piece[] _table)
        {
            int[] map = base.GetMovableMap(_table);

            /* Normally the King only move one cell at a time.
             * However, to ultilize the MoveRange and leave opportunity for surprise,
             * we also use Raycast to generate the King's MovableMap, and limit it to 
             * one step (MoveRange = 1).
             */

            int[] hmap = CastHorizontalRay(map);
            int[] vmap = CastVerticalRay(map);
            int[] swne = CastSWNERay(map);
            int[] senw = CastSENWRay(map);

            for (int i = 0; i < _table.Length; ++i)
            {
                map[i] |= (hmap[i] | vmap[i] | swne[i] | senw[i]);
            }

            // TODO: predict check;

            return map;
        }
    }
}

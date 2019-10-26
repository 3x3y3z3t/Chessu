﻿// ;
using System;

namespace CBack.Pieces
{
    public class RookPiece : Piece
    {
        public RookPiece(int _row, int _column, PieceColor _color, Game _owner)
            : base(_row, _column, _color, PieceType.Rook, _owner)
        { }

        public override bool Move(int _dstRow, int _dstCol)
        {
            int[] table = new int[Game.RowSize * Game.ColumnSize];



            //Console.WriteLine("RookMove!");
            return base.Move(_dstRow, _dstCol);
        }

        public override int[] GetMovableMap()
        {
            int[] map = base.GetMovableMap();

            int[] hmap = CastHorizontalRay(map);
            int[] vmap = CastVerticalRay(map);

            for (int i = 0; i < map.Length; ++i)
            {
                map[i] |= (hmap[i] | vmap[i]);
            }

            return map;
        }
    }
}

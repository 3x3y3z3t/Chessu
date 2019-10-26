// ;
using System;

namespace CBack.Pieces
{
    public class KnightPiece : Piece
    {
        protected readonly int directionCount = 8;
        protected readonly int[] dc = new int[] { -2, -1, +1, +2, -2, -1, +1, +2 };
        protected readonly int[] dr = new int[] { -1, -2, -2, -1, +1, +2, +2, +1 };
        // direction:                             14, 12, 32, 36, 74, 78, 98, 96  ;

        public KnightPiece(int _row, int _column, PieceColor _color, Game _owner)
            : base(_row, _column, _color, PieceType.Knight, _owner)
        {
            CanJump = true;
            MoveRange = 1;
        }

        public override bool Move(int _dstRow, int _dstCol)
        {
            int[] table = new int[Game.RowSize * Game.ColumnSize];



            //Console.WriteLine("BishopMove!");
            return base.Move(_dstRow, _dstCol);
        }

        public override int[] GetMovableMap()
        {
            int[] map = base.GetMovableMap();

            if (dc.Length != directionCount || dr.Length != directionCount)
            {
                Console.WriteLine("Knight piece movement direction count mismatch.");
                return map;
            }

            for (int step = 1; step <= MoveRange; ++step)
            {
                for (int i = 0; i < directionCount; ++i)
                {
                    int row = Row + dr[i] * step;
                    int col = Column + dc[i] * step;
                    if (row < 0 || row >= Game.ColumnSize || col < 0 || col >= Game.RowSize)
                        continue;
                    int pos = Game.GetIndex(row, col);
                    RaycastDownLevel(map, pos, step);
                }
            }

            return map;
        }
    }
}

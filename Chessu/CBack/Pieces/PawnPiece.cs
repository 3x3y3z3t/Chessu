// ;

using System;

namespace CBack.Pieces
{
    public class PawnPiece : Piece
    {
        public bool EnPassantEligible { get; set; }

        public PawnPiece(int _row, int _column, PieceColor _color)
            : base(_row, _column, _color, PieceType.Pawn)
        {
            EnPassantEligible = false;
        }

        public override bool Move(int _dstRow, int _dstCol)
        {
            //int[] table = new int[Game.RowSize * Game.ColumnSize];

            EnPassantEligible = Math.Abs(_dstRow - Row) == 2;

            return base.Move(_dstRow, _dstCol);
        }

        public override int[] GetMovableMap(Piece[] _table)
        {
            int[] map = base.GetMovableMap(_table);

            int mod1 = Color == PieceColor.Black ? -1 : 1;
            int front = (Row + mod1) * Game.ColumnSize + Column;
            /* TODO: before handling Promotion, there will be an unhandled bug
                when a Pawn reach the last rank (front < 0 or front > boardSize).
            */
            if (front < 0 || front > map.Length)
            {
                Console.WriteLine($"This Pawn should be promoted on the last move. Additional data: front = {front}.");
                return map;
            }

            // move;
            if (_table[front] == null)
                map[front] |= (int)CellStatus.Movable;
            if (FirstMove)
            {
                if (_table[front] == null)
                {
                    int front2 = front + (Game.RowSize * mod1);
                    if (_table[front2] == null)
                        map[front2] |= (int)CellStatus.Movable;
                }
            }

            // target;
            if (Column != 7)
            {
                if (_table[front + mod1] != null)
                {
                    if (_table[front + mod1].Color != Color)
                        map[front + mod1] |= (int)CellStatus.Targetable;
                }
            }
            if (Column != 0)
            {
                if (_table[front - mod1] != null)
                {
                    if (_table[front - mod1].Color != Color)
                        map[front - mod1] |= (int)CellStatus.Targetable;
                }
            }
            
            int negPos = Game.GetIndex(Row, Column - 1);
            AppendEnPassantMove(_table, map, negPos);
            int posPos = Game.GetIndex(Row, Column + 1);
            AppendEnPassantMove(_table, map, posPos);

            return map;
        }

        private bool AppendEnPassantMove(Piece[] _table, int[] _map, int _pos)
        {
            Piece pcs = _table[_pos];
            if (IsPieceOfType(pcs, PieceType.Pawn))
            {
                if (((PawnPiece)pcs).EnPassantEligible)
                {
                    if (Color == PieceColor.Black)
                        _map[_pos - 8] |= ((int)CellStatus.Targetable | (int)CellStatus.EnPassant);
                    else 
                        _map[_pos + 8] |= ((int)CellStatus.Targetable | (int)CellStatus.EnPassant);
                    return true;
                }
            }
            return false;
        }
    }
}

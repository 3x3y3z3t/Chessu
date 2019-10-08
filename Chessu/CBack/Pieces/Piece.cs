// ;

using System;
using System.Collections.Generic;

namespace CBack.Pieces
{
    public abstract class Piece
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public PieceColor Color { get; set; }
        public PieceType Type { get; set; }

        public bool FirstMove { get; set; }
        public bool CanJump { get; set; }

        protected List<Tuple<int, int>> MovementDirection { get; set; }

        private static readonly PieceType[] raycastRestricType = new PieceType[]
        {
            PieceType.Pawn,
            PieceType.Knight,
            PieceType.King
        };

        protected Piece(int _row, int _column, PieceColor _color, PieceType _type)
        {
            Row = _row;
            Column = _column;
            Color = _color;
            Type = _type;
            FirstMove = true;
            MovementDirection = new List<Tuple<int, int>>();
        }

        public virtual bool Move(int _dstRow, int _dstCol)
        {
            Row = _dstRow;
            Column = _dstCol;
            FirstMove = false;

            return true;
        }

        public virtual bool Attack(int _dstRow, int _dstCol)
        {


            return false;
        }

        public bool Attack(Piece _piece)
        {
            return true;
            // TODO: *hint* *hint*;
        }

        public virtual int[] GetMovableMap(Piece[] _table)
        {
            int[] map = new int[_table.Length];
            for (int i = 0; i < map.Length; ++i)
            {
                Piece pcs = _table[i];
                if (pcs != null)
                {
                    if (pcs == this)
                        map[i] |= (int)CellStatus.MeOccupied;
                    if (pcs.Color == Color)
                        map[i] |= (int)CellStatus.AllyOccupied; // ally pieces;
                    else
                        map[i] |= (int)CellStatus.EnemyOccupied; // enemy pieces;
                }
            }

            return map;
        }

        private bool CanPerformRaycast()
        {
            if (Array.Exists(raycastRestricType, element => element == Type))
            {
                Console.WriteLine($"Can't use Movement Raycast for piece of type {Type}.");
                return false;
            }
            return true;
        }
        
        protected int[] CastVerticalRay(Piece[] _table)
        {
            int[] map = new int[_table.Length];
            if (!CanPerformRaycast())
                return map;

            // cast direction: 2;
            for (int row = Row - 1; row >= 0; --row)
            {
                int pos = Game.GetIndex(row, Column);
                if (_table[pos] == null)
                    map[pos] |= (int)CellStatus.Movable;
                else
                {
                    if (_table[pos].Color != Color)
                        map[pos] |= (int)CellStatus.Targetable;
                    if (!CanJump)
                        break;
                }
            }

            // cast direction: 8;
            for (int row = Row + 1; row < Game.ColumnSize; ++row)
            {
                int pos = Game.GetIndex(row, Column);
                if (_table[pos] == null)
                    map[pos] |= (int)CellStatus.Movable;
                else
                {
                    if (_table[pos].Color != Color)
                        map[pos] |= (int)CellStatus.Targetable;
                    if (!CanJump)
                        break;
                }
            }
            
            return map;
        }

        // TODO: rewrite Raycast;
        protected int[] CastHorizontalRay(Piece[] _table)
        {
            int[] map = new int[_table.Length];
            if (!CanPerformRaycast())
                return map;

            // cast direction: 4;
            for (int col = Column - 1; col >= 0; --col)
            {
                int pos = Game.GetIndex(Row, col);
                if (_table[pos] == null)
                    map[pos] |= (int)CellStatus.Movable;
                else
                {
                    if (_table[pos].Color != Color)
                        map[pos] |= (int)CellStatus.Targetable;
                    if (!CanJump)
                        break;
                }
            }

            // cast direction: 8;
            for (int col = Column + 1; col < Game.RowSize; ++col)
            {
                int pos = Game.GetIndex(Row, col);
                if (_table[pos] == null)
                    map[pos] |= (int)CellStatus.Movable;
                else
                {
                    if (_table[pos].Color != Color)
                        map[pos] |= (int)CellStatus.Targetable;
                    if (!CanJump)
                        break;
                }
            }

            return map;
        }

        // TODO: rewrite Raycast;
        private int[] CastSWNERay(Piece[] _table)
        {
            // cast direction: SW->NE (bottom-left->top-right);
            int[] map = new int[_table.Length];
            if (!CanPerformRaycast())
                return map;

            return map;
        }

        // TODO: rewrite Raycast;
        private int[] CastSENWRay(Piece[] _table)
        {
            // cast direction: SE->NW (bottom-right->top-left);
            int[] map = new int[_table.Length];
            if (!CanPerformRaycast())
                return map;

            return map;
        }

        public override bool Equals(object obj)
        {
            Piece piece = obj as Piece;
            return piece != null &&
                   Row == piece.Row &&
                   Column == piece.Column &&
                   Color == piece.Color &&
                   Type == piece.Type;
        }

        public override string ToString()
        {
            return $"{Type}_{Color}";
        }

        public override int GetHashCode()
        {
            var hashCode = 459464988;
            hashCode = hashCode * -1521134295 + Row.GetHashCode();
            hashCode = hashCode * -1521134295 + Column.GetHashCode();
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + FirstMove.GetHashCode();
            hashCode = hashCode * -1521134295 + CanJump.GetHashCode();
            return hashCode;
        }
    }
}

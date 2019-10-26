// ;

using System;
using System.Collections.Generic;

namespace CBack.Pieces
{
    public abstract class Piece
    {
        public int Row { get; protected set; }
        public int Column { get; protected set; }
        public PieceColor Color { get; protected set; }
        public PieceType Type { get; protected set; }

        public bool FirstMove { get; protected set; }
        public bool CanJump { get; protected set; }
        /* The max movement range of a piece.
         * Some pieces may not ultilize this value.*/
        public int MoveRange { get; set; }

        public Game Owner { get; set; }
        
        private static readonly PieceType[] raycastRestricType = new PieceType[]
        {
            PieceType.Pawn,
            PieceType.Knight,
        };

        protected Piece(int _row, int _column, PieceColor _color, PieceType _type, Game _owner)
        {
            Row = _row;
            Column = _column;
            Color = _color;
            Type = _type;
            FirstMove = true;
            CanJump = false;
            // Instead of perform another check for null move range, we just give it a ridiculous range;
            MoveRange = 999;

            Owner = _owner;

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

        public virtual bool Attack(Piece _piece)
        {
            return true;
            // TODO: *hint* *hint*;
        }

        public virtual int[] GetMovableMap()
        {
            int[] map = new int[Owner.Table.Length];
            for (int i = 0; i < map.Length; ++i)
            {
                Piece pcs = Owner.Table[i];
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

        /*
         * TODO: rename this method. Properly.
         */
        protected bool RaycastDownLevel(int[] _map, int _pos, int _step)
        {
            if (_step > MoveRange)
                return false;
            // NOTE: this require base GetMovableMap to generate map correctly;
            if (_map[_pos] == (int)CellStatus.Empty)
            {
                _map[_pos] |= (int)CellStatus.Movable;
                return true;
            }

            if (Game.IsSpecificFlagSet(_map[_pos], CellStatus.AllyOccupied))
                _map[_pos] |= (int)CellStatus.Protected;
            else if (Game.IsSpecificFlagSet(_map[_pos], CellStatus.EnemyOccupied))
                _map[_pos] |= (int)CellStatus.Targetable;

            return CanJump;
        }

        protected int[] CastVerticalRay(int[] _map)
        {
            if (!CanPerformRaycast())
                return _map;

            int step = 0;
            // cast direction: 2;
            for (int row = Row - 1; row >= 0; --row)
            {
                int pos = Game.GetIndex(row, Column);
                if (!RaycastDownLevel(_map, pos, ++step))
                    break;
            }
            step = 0;
            // cast direction: 8;
            for (int row = Row + 1; row < Game.ColumnSize; ++row)
            {
                int pos = Game.GetIndex(row, Column);
                if (!RaycastDownLevel(_map, pos, ++step))
                    break;
            }

            return _map;
        }

        protected int[] CastHorizontalRay(int[] _map)
        {
            if (!CanPerformRaycast())
                return _map;

            int step = 0;
            // cast direction: 4;
            for (int col = Column - 1; col >= 0; --col)
            {
                int pos = Game.GetIndex(Row, col);
                if (!RaycastDownLevel(_map, pos, ++step))
                    break;
            }
            step = 0;
            // cast direction: 6;
            for (int col = Column + 1; col < Game.RowSize; ++col)
            {
                int pos = Game.GetIndex(Row, col);
                if (!RaycastDownLevel(_map, pos, ++step))
                    break;
            }

            return _map;
        }

        protected int[] CastSWNERay(int[] _map)
        {
            if (!CanPerformRaycast())
                return _map;

            int step, row, col;
            // cast direction: 1;
            step = 1;
            do
            {
                row = Row - step;
                col = Column - step;
                if (row < 0 || col < 0)
                    break;
                int pos = Game.GetIndex(row, col);
                if (!RaycastDownLevel(_map, pos, step))
                    break;
                ++step;
            } while (true);

            // cast direction: 9;
            step = 1;
            do
            {
                row = Row + step;
                col = Column + step;
                if (row >= Game.ColumnSize || col >= Game.RowSize)
                    break;
                int pos = Game.GetIndex(row, col);
                if (!RaycastDownLevel(_map, pos, step))
                    break;
                ++step;
            } while (true);

            return _map;
        }

        // TODO: rewrite Raycast;
        protected int[] CastSENWRay(int[] _map)
        {
            if (!CanPerformRaycast())
                return _map;

            int step, row, col;
            // cast direction: 3;
            step = 1;
            do
            {
                row = Row - step;
                col = Column + step;
                if (row < 0 || col >= Game.RowSize)
                    break;
                int pos = Game.GetIndex(row, col);
                if (!RaycastDownLevel(_map, pos, step))
                    break;
                ++step;
            } while (true);

            // cast direction: 7;
            step = 1;
            do
            {
                row = Row + step;
                col = Column - step;
                if (row >= Game.ColumnSize || col < 0)
                    break;
                int pos = Game.GetIndex(row, col);
                if (!RaycastDownLevel(_map, pos, step))
                    break;
                ++step;
            } while (true);

            return _map;
        }

        public static bool IsPieceOfType(Piece _pcs, PieceType _type)
        {
            if (_pcs == null)
                return false;
            return _pcs.Type == _type;
        }

        public override string ToString()
        {
            return $"{Type}_{Color}";
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

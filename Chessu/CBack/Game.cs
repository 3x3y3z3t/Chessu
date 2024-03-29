﻿// ;

using CBack.Pieces;
using System;
using System.Collections.Generic;

namespace CBack
{
    public enum PieceType
    {
        //None = 0,
        King = 1,
        Queen = 2,
        Rook = 3,
        Bishop = 4,
        Knight = 5,
        Pawn = 6,
    }

    public enum PieceColor
    {
        //None = 0,
        Black = 1,
        White = 2
    }

    public enum CellStatus
    {
        Empty = 0,

        Movable = 1 << 0,
        Targetable = 1 << 1,
        LastMove = 1 << 2,
        Selecting = 1 << 3,
        Checking = 1 << 4,
        Protected = 1 << 5,

        EnemyOccupied = 1 << 11,
        AllyOccupied = 1 << 12,
        MeOccupied = 1 << 13,

        EnPassant = 1 << 21,
    }

    public enum CheckStatus
    {
        None = 0,
        NormalCheck = 1,
        KightCheck = 2,
        DoubleCheck = 3,
        ForkedCheck = 4,
    }

    public class Game
    {
        public Player WhitePlayer { get; set; }
        public Player BlackPlayer { get; set; }
        public Player ActivePlayer { get; set; }

        public static int RowSize = 8;
        public static int ColumnSize = 8;
        public static int MaxSize = RowSize > ColumnSize ? RowSize : ColumnSize;

        public Piece[] Table { get; protected set; }

        public Piece SelectedPiece { get; private set; }
        public int[] TableStatus { get; private set; }
        public Tuple<int, int> LastMovement { get; private set; }
        public List<int> LastCheck { get; private set; }
        public CheckStatus LastCheckStatus { get; private set; }

        public PawnPiece LastMovedPawn { get; private set; }
        private bool newGame = true;

        public Game(Player _whitePlayer, Player _blackPlayer)
        {
            WhitePlayer = _whitePlayer;
            BlackPlayer = _blackPlayer;

            Table = new Piece[RowSize * ColumnSize];
            TableStatus = new int[RowSize * ColumnSize];
            //TableStatus = Enumerable.Repeat(1, RowSize * ColumnSize).ToArray(); // LINQ approach;
            LastMovement = null;
            LastMovedPawn = null;
            LastCheck = new List<int>();

            // now adding pieces...
            AddPiece(new PawnPiece(6, 0, PieceColor.Black, this));
            AddPiece(new PawnPiece(6, 1, PieceColor.Black, this));
            AddPiece(new PawnPiece(6, 2, PieceColor.Black, this));
            AddPiece(new PawnPiece(6, 3, PieceColor.Black, this));
            //AddPiece(new PawnPiece(6, 4, PieceColor.Black, this));
            AddPiece(new PawnPiece(6, 5, PieceColor.Black, this));
            AddPiece(new PawnPiece(6, 6, PieceColor.Black, this));
            AddPiece(new PawnPiece(6, 7, PieceColor.Black, this));
            AddPiece(new KnightPiece(7, 1, PieceColor.Black, this));
            AddPiece(new KnightPiece(7, 6, PieceColor.Black, this));
            AddPiece(new BishopPiece(7, 2, PieceColor.Black, this));
            AddPiece(new BishopPiece(7, 5, PieceColor.Black, this));
            AddPiece(new RookPiece(7, 0, PieceColor.Black, this));
            AddPiece(new RookPiece(7, 7, PieceColor.Black, this));
            AddPiece(new QueenPiece(7, 3, PieceColor.Black, this));
            AddPiece(new KingPiece(7, 4, PieceColor.Black, this));

            AddPiece(new PawnPiece(1, 0, PieceColor.White, this));
            AddPiece(new PawnPiece(1, 1, PieceColor.White, this));
            AddPiece(new PawnPiece(1, 2, PieceColor.White, this));
            AddPiece(new PawnPiece(1, 3, PieceColor.White, this));
            //AddPiece(new PawnPiece(1, 4, PieceColor.White, this));
            AddPiece(new PawnPiece(1, 5, PieceColor.White, this));
            AddPiece(new PawnPiece(1, 6, PieceColor.White, this));
            AddPiece(new PawnPiece(1, 7, PieceColor.White, this));
            AddPiece(new KnightPiece(0, 1, PieceColor.White, this));
            AddPiece(new KnightPiece(0, 6, PieceColor.White, this));
            AddPiece(new BishopPiece(0, 2, PieceColor.White, this));
            AddPiece(new BishopPiece(0, 5, PieceColor.White, this));
            AddPiece(new RookPiece(0, 0, PieceColor.White, this));
            AddPiece(new RookPiece(0, 7, PieceColor.White, this));
            AddPiece(new QueenPiece(0, 3, PieceColor.White, this));
            AddPiece(new KingPiece(0, 4, PieceColor.White, this));

            /*
            AddPiece(new KingPiece(4, 4, PieceColor.White, this));
            AddPiece(new RookPiece(0, 3, PieceColor.Black, this));
            AddPiece(new RookPiece(0, 5, PieceColor.Black, this));
            AddPiece(new RookPiece(3, 1, PieceColor.Black, this));
            AddPiece(new RookPiece(2, 2, PieceColor.Black, this));
            AddPiece(new RookPiece(0, 2, PieceColor.Black, this));
            */
            //TableStatus[1] = (int)CellStatus.Movable;

            ActivePlayer = WhitePlayer;
        }

        public bool IsFlagSetAtCell(int _row, int _col, CellStatus _flag)
        {
            return IsFlagSetAtCell(GetIndex(_row, _col), _flag);
        }

        public bool IsFlagSetAtCell(int _index, CellStatus _flag)
        {
            return IsSpecificFlagSet(TableStatus[_index], _flag);
        }

        public static bool IsSpecificFlagSet(int _flags, CellStatus _flag)
        {
            return (_flags & (int)_flag) != 0;
        }

        public bool AddPiece(Piece _pcs)
        {
            if (Table[GetIndex(_pcs.Row, _pcs.Column)] != null)
            {
                Console.WriteLine($"There is piece in this cell: cell({(char)(_pcs.Column + 'A')}{_pcs.Row + 1}).");
                return false;
            }
            Table[GetIndex(_pcs.Row, _pcs.Column)] = _pcs;
            //Console.WriteLine($"Piece {_pcs} added to Table at cell({(char)(_pcs.Column + 'A')}{_pcs.Row + 1}).");
            return true;
        }

        public List<Piece> GetPieces(PieceColor _color)
        {
            List<Piece> pieces = new List<Piece>();

            foreach (Piece pcs in Table)
                if (pcs != null && pcs.Color == _color)
                    pieces.Add(pcs);

            return pieces;
        }

        public List<Piece> GetPieces(PieceType _type, PieceColor _color)
        {
            List<Piece> pieces = new List<Piece>();

            foreach (Piece pcs in Table)
                if (pcs != null && pcs.Type == _type && pcs.Color == _color)
                    pieces.Add(pcs);

            return pieces;
        }

        [Obsolete("Obsoleted. Use SelectPiece(Piece) instead.")]
        private bool SelectPieceLegacy(Piece _pcs)
        {
            SelectedPiece = _pcs;
            if (_pcs == null)
            {
                Console.WriteLine($"Piece to be selected is null.");
                TableStatus = new int[Table.Length];
                return false;
            }
            //Console.WriteLine($"Piece {_pcs} selected.");
            TableStatus = _pcs.GetMovableMap();
            return true;
        }

        public bool SelectCell(int _row, int _col)
        {
            if (_row < 0 || _row >= ColumnSize || _col < 0 || _col >= RowSize)
            {
                Console.WriteLine($"Selected cell outside of board: cell({_col}, {_row}).");
                return false;
            }

            Piece piece = Table[GetIndex(_row, _col)];
            if (SelectedPiece == null)
            {
                if (piece == null)
                {
                    Console.WriteLine($"This cell is empty: cell({(char)(_col + 'A')}{_row + 1}).");
                    return false;
                }
                if (piece.Color != ActivePlayer.PlayerColor)
                {
                    Console.WriteLine($"Piece to be selected does not belong to player: piece({piece.Color}) - player({ActivePlayer.PlayerColor}).");
                    return false;
                }

                // now there is a piece of player's color under the click;
                return SelectPiece(piece);
            }
            //if (SelectedPiece == piece)
            //{
            //    Console.WriteLine($"This cell here should be selected: cell({(char)(_col + 'A')}{_row + 1}), piece({piece}).");
            //    return UnselectCurrentCell();
            //}

            // case: there is a selected piece somewhere, then TableStatus SHOULD contains the movement data;
            int cellStatus = TableStatus[GetIndex(_row, _col)];

            //if (IsSpecificFlagSet(cellStatus, CellStatus.Empty))
            if (cellStatus == (int)CellStatus.Empty)
            {
                Console.WriteLine("Empty cell clicked, unselecting piece now.");
                UnselectCurrentCell();
                return true;
            }
            if (IsSpecificFlagSet(cellStatus, CellStatus.Selecting))
            {
                Console.WriteLine("SelectedPiece clicked, unselecting piece now.");
                UnselectCurrentCell();
                return true;
            }
            if (IsSpecificFlagSet(cellStatus, CellStatus.AllyOccupied))
            {
                Console.WriteLine($"Ally Piece occupied this cell: cell({(char)(_col + 'A')}{_row + 1}), piece({piece}). Selecting new piece now.");
                return SelectPiece(piece);
            }
            if (IsSpecificFlagSet(cellStatus, CellStatus.Movable))
            {
                MovePieceTo(_row, _col);
                UnselectCurrentCell();
                return true;
            }
            if (IsSpecificFlagSet(cellStatus, CellStatus.Targetable))
            {
                AttackPieceAt(_row, _col);
                UnselectCurrentCell();
                return true;
            }
            if (IsSpecificFlagSet(cellStatus, CellStatus.EnemyOccupied))
            {
                //Console.WriteLine($"Enemy Piece occupied this cell: cell({(char)(_col + 'A')}{_row + 1}), piece({piece}).");
                Console.WriteLine($"Selected piece cannot reach this cell: piece({piece}), cell({(char)(_col + 'A')}{_row + 1}).");
                return false;
            }



            Console.WriteLine("===> WIP <===");
            Console.WriteLine("Control should never fall down here.");
            return false;

        }

        private bool UnselectCurrentCell()
        {
            SelectedPiece = null;
            TableStatus = new int[Table.Length];
            //RefreshTableStatuses();

            AppendLastMoveStats();
            return true;
        }

        private bool RefreshTableStatuses(int[] _base = null)
        {
            for (int i = 0; i < TableStatus.Length; ++i)
            {
                int newflag = (int)CellStatus.Empty;
                if (_base != null)
                    newflag |= _base[i];
                if (IsSpecificFlagSet(TableStatus[i], CellStatus.LastMove))
                    newflag |= (int)CellStatus.LastMove;
                if (IsSpecificFlagSet(TableStatus[i], CellStatus.Checking))
                    newflag |= (int)CellStatus.Checking;

                TableStatus[i] = newflag;
            }

            return true;
        }

        private bool SelectPiece(Piece _pcs)
        {
            if (_pcs == null)
            {
                Console.WriteLine($"Piece to be selected is null (and it should not be null).");
                TableStatus = new int[Table.Length];
                //RefreshTableStatuses();
                return false;
            }
            SelectedPiece = _pcs;
            //RefreshTableStatuses(_pcs.GetMovableMap());
            TableStatus = _pcs.GetMovableMap();
            TableStatus[GetIndex(SelectedPiece.Row, SelectedPiece.Column)] |= (int)CellStatus.Selecting;
            AppendLastMoveStats();
            return true;
        }

        /// <summary>
        /// Move the SelectedPiece to the target cell.
        /// </summary>
        private bool MovePieceTo(int _row, int _col)
        {
            if (LastMovedPawn != null)
                LastMovedPawn.EnPassantEligible = false;
            if (SelectedPiece.Type == PieceType.Pawn)
                LastMovedPawn = SelectedPiece as PawnPiece;
            else
                LastMovedPawn = null;
            // the SelectedPiece is being moved so there is no need to get the piece;
            // the move is always a valid move so there is no need to validate it;
            LastMovement = Tuple.Create(GetIndex(SelectedPiece.Row, SelectedPiece.Column), GetIndex(_row, _col));
            //TableStatus[GetIndex(SelectedPiece.Row, SelectedPiece.Column)] |= (int)CellStatus.LastMove;
            //TableStatus[GetIndex(_row, _col)] |= (int)CellStatus.LastMove;

            Array.Copy(Table, GetIndex(SelectedPiece.Row, SelectedPiece.Column), Table, GetIndex(_row, _col), 1);
            Table[GetIndex(SelectedPiece.Row, SelectedPiece.Column)] = null;
            int pcsRow = _row;
            if (IsFlagSetAtCell(_row, _col, CellStatus.EnPassant))
            {
                if (SelectedPiece.Color == PieceColor.Black)
                    Table[GetIndex(pcsRow + 1, _col)] = null;
                else
                    Table[GetIndex(pcsRow - 1, _col)] = null;
            }
            SelectedPiece.Move(_row, _col);

            UpdateCheckStatus();


            Switch();
            return true;
        }

        /// <summary>
        /// Attack the piece at the target cell using the SelectedPiece.
        /// </summary>
        /// <returns>true if the attack success and the SelectedPiece moves to occupy the target cell,
        /// false if the attack success and the SelectedPiece stays at its cell.</returns>
        private bool AttackPieceAt(int _row, int _col)
        {
            int pcsRow = _row;
            if (IsFlagSetAtCell(_row, _col, CellStatus.EnPassant))
            {
                if (SelectedPiece.Color == PieceColor.Black)
                    ++pcsRow;
                else
                    --pcsRow;
            }
            bool dead = SelectedPiece.Attack(Table[GetIndex(pcsRow, _col)]);
            if (dead)
            {
                MovePieceTo(_row, _col);
            }
            else
            {
                if (LastMovedPawn != null)
                    LastMovedPawn.EnPassantEligible = false;
                // pieces stay still!
                LastMovement = Tuple.Create(GetIndex(SelectedPiece.Row, SelectedPiece.Column), GetIndex(_row, _col));
                //TableStatus[GetIndex(SelectedPiece.Row, SelectedPiece.Column)] |= (int)CellStatus.LastMove;
                //TableStatus[GetIndex(_row, _col)] |= (int)CellStatus.LastMove;
                Switch();
            }
            return dead;
        }

        private bool UpdateCheckStatus()
        {
            // TODO: verify for check;
            LastCheck.Clear();
            foreach (PieceColor color in Enum.GetValues(typeof(PieceColor)))
            {
                List<Piece> pieces = GetPieces(PieceType.King, color);

                foreach (Piece pcs in pieces)
                {
                    KingPiece kingPcs = pcs as KingPiece;

                    int[] checkmap = kingPcs.GetCheckMap();
                    for (int i = 0; i < TableStatus.Length; ++i)
                    {
                        if (IsSpecificFlagSet(checkmap[i], CellStatus.Checking))
                            LastCheck.Add(i);
                    }
                }

            }




            return true;
        }

        private bool AppendLastMoveStats()
        {
            if (TableStatus == null)
            {
                Console.WriteLine($"TableStatus is null (internal).");
                return false;
            }
            if (LastMovement == null)
            {
                if (newGame)
                {
                    Console.WriteLine($"LastMovement is null (newGame).");
                }
                else
                {
                    Console.WriteLine($"LastMovement is null (internal).");
                    return false;
                }
            }
            else
            {
                TableStatus[LastMovement.Item1] |= (int)CellStatus.LastMove;
                TableStatus[LastMovement.Item2] |= (int)CellStatus.LastMove;
            }
            if (LastCheck == null)
            {
                Console.WriteLine($"LastCheck is null (internal).");
                return false;
            }
            foreach(int index in LastCheck)
            {
                TableStatus[index] |= (int)CellStatus.Checking;
            }

            return true;

        }

        [Obsolete("Obsoleted. Use SelectCell(int, int) instead.")]
        public bool SelectPieceAt(int _row, int _col)
        {
            if (_row < 0 || _row >= ColumnSize || _col < 0 || _col >= RowSize)
            {
                Console.WriteLine("Invalid cell position: cell out of board bound.");
                return false;
            }

            Piece piece = Table[_row * RowSize + _col];
            if (piece == null)
            {
                Console.WriteLine($"This cell is empty: cell({_col + 'A'}{_row}.");
                return false;
            }
            if (piece.Color != ActivePlayer.PlayerColor)
            {
                Console.WriteLine("This piece is not yours.");
                return false;
            }

            if (piece.Equals(SelectedPiece))
            {
                Console.WriteLine("This piece is selected previously, unselecting piece now.");
                SelectedPiece = null;
                TableStatus = new int[Table.Length];
            }
            else
            {
                SelectedPiece = piece;
                TableStatus = piece.GetMovableMap();
                TableStatus[SelectedPiece.Row * ColumnSize + SelectedPiece.Column] = (int)CellStatus.Selecting;
            }

            if (LastMovement != null)
            {
                TableStatus[LastMovement.Item1] = (int)CellStatus.LastMove;
                TableStatus[LastMovement.Item2] = (int)CellStatus.LastMove;
            }




            //TableStatus[0] = (int)CellStatus.Movable;

            return true;
        }

        /// <summary>
        /// Dummy.
        /// </summary>
        /// <returns></returns>
        public bool Switch()
        {





            switch (ActivePlayer.PlayerColor)
            {
                case PieceColor.White:
                    ActivePlayer = BlackPlayer;
                    return true;
                case PieceColor.Black:
                    ActivePlayer = WhitePlayer;
                    return true;
                default:
                    Console.WriteLine("Invalid Player Side Color!");
                    return false;
            }
        }

        public static int GetIndex(int _row, int _col)
        {
            return _row * RowSize + _col;
        }
    }
}

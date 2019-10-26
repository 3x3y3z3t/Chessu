// ;
using System;
using System.Collections.Generic;

namespace CBack.Pieces
{
    public class KingPiece : Piece
    {
        public List<Piece> CheckingPieces { get; private set; }

        public KingPiece(int _row, int _column, PieceColor _color, Game _owner)
            : base(_row, _column, _color, PieceType.King, _owner)
        {
            MoveRange = 2;
            CheckingPieces = new List<Piece>();
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

            /* Normally the King only move one cell at a time.
             * However, to ultilize the MoveRange and leave opportunity for surprise,
             * we also use Raycast to generate the King's MovableMap, and limit it to 
             * one step (MoveRange = 1).
             */

            int[] hmap = CastHorizontalRay(map);
            int[] vmap = CastVerticalRay(map);
            int[] swne = CastSWNERay(map);
            int[] senw = CastSENWRay(map);

            for (int i = 0; i < Owner.Table.Length; ++i)
            {
                map[i] |= (hmap[i] | vmap[i] | swne[i] | senw[i]);
            }

            if (Owner.ActivePlayer.PlayerColor == Color)
            {
                /* Currently the map array contains all the cells that this King piece
                 * can reach. Now we need to remove all the cells that this King piece
                 * is not allowed to move to.
                 */
                int[] enemyMap = GetEnemyMovableMap();
                for (int i = 0; i < map.Length; ++i)
                {
                    // if this King can reach this cell, ...
                    if (Game.IsSpecificFlagSet(map[i], CellStatus.Movable))
                    {
                        // and if an enemy can also reach this cell, ...
                        if (Game.IsSpecificFlagSet(enemyMap[i], CellStatus.Movable))
                        {
                            map[i] ^= (int)CellStatus.Movable;
                        }
                    }
                    // if this King can target this cell, ...
                    else if (Game.IsSpecificFlagSet(map[i], CellStatus.Targetable))
                    {
                        // and if an enemy can protected this cell, ...
                        // TODO: add more flag;
                        if (Game.IsSpecificFlagSet(enemyMap[i], CellStatus.Protected))
                        {
                            map[i] ^= (int)CellStatus.Targetable;
                        }
                    }
                }
            }

            /* TODO list:
             * - Castling
             * - Pin against King
             */

            return map;
        }

        public int[] GetCheckMap()
        {
            int[] map = new int[Owner.Table.Length];
            CheckingPieces.Clear();
            foreach (PieceColor color in Enum.GetValues(typeof(PieceColor)))
            {
                if (color == Color)
                    continue;

                List<Piece> enemyPieces = Owner.GetPieces(color);
                foreach (Piece pcs in enemyPieces)
                {
                    if (pcs.Type == PieceType.King)
                        continue;

                    int[] enemyMap = pcs.GetMovableMap();
                    int flags = enemyMap[Game.GetIndex(Row, Column)];
                    if (Game.IsSpecificFlagSet(flags, CellStatus.Targetable))
                    {
                        CheckingPieces.Add(pcs);
                        map[Game.GetIndex(pcs.Row, pcs.Column)] |= (int)CellStatus.Checking;
                        map[Game.GetIndex(Row, Column)] |= (int)CellStatus.Checking;
                    }
                }
            }

            return map;
        }

        private int[] GetEnemyMovableMap()
        {
            int[] map = new int[Owner.Table.Length];
            foreach (PieceColor color in Enum.GetValues(typeof(PieceColor)))
            {
                if (color == Color)
                    continue;

                List<Piece> enemyPieces = Owner.GetPieces(color);
                foreach (Piece pcs in enemyPieces)
                {
                    if (pcs.Type == PieceType.King)
                    {
                        // TODO: handle king vs. king;
                        continue;
                    }

                    int[] enemyMap = pcs.GetMovableMap();
                    for (int i = 0; i < map.Length; ++i)
                    {
                        map[i] |= enemyMap[i];
                    }
                }
            }

            return map;
        }
    }
}

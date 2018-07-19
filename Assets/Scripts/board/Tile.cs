using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.pieces;
using UnityEngine;

namespace Assets.Scripts.board
{
    public abstract class Tile
    {

        protected readonly int tileCoordinate;
        private static readonly Dictionary<int, EmptyTile> EMPTY_TILES_CACHE = createAllPossibleEmptyTiles();

        /// <summary>
        /// Create all the possible Empty Tiles on a chess board
        /// </summary>
        /// <returns></returns>
        private static Dictionary<int, EmptyTile> createAllPossibleEmptyTiles()
        {
            Dictionary<int, EmptyTile> emptyTileMap = new Dictionary<int, EmptyTile>();
            for (int i = 0; i < 64; i++)
            {
                emptyTileMap.Add(i, new EmptyTile(i));
            }
            return emptyTileMap;
        }

        //Private Constructor
        private Tile(int tileCoordinate)
        {
            this.tileCoordinate = tileCoordinate;
        }

        /// <summary>
        /// Factory method for creating a new Tile
        /// </summary>
        /// <param name="tileCoordinate"></param>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static Tile createTile(int tileCoordinate, Piece piece)
        {
            if(piece != null)
            {
                return new OccupiedTile(tileCoordinate, piece);
            }
            else
            {
                return EMPTY_TILES_CACHE[tileCoordinate];
            }
        }
        public abstract bool isTileOccupied();
        public abstract Piece getPiece();

        /// <summary>
        /// Empty Tile definition
        /// </summary>
        public sealed class EmptyTile : Tile
        {
            //Constructor
            public EmptyTile(int tileCoordinate) : base(tileCoordinate)
            {
            }
            /// <summary>
            /// Print out of empty Tile
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "_";
            }
            /// <summary>
            /// Return false for tileOccupied because there can be no
            /// pieces on an empty Tile
            /// </summary>
            /// <returns></returns>
            public override bool isTileOccupied()
            {
                return false;
            }
            /// <summary>
            /// No pieces on a empty tile
            /// </summary>
            /// <returns></returns>
            public override Piece getPiece()
            {
                return null;
            }

        }
        /// <summary>
        /// get current tile coordinate
        /// </summary>
        /// <returns></returns>
        public int getTileCoordinate()
        {
            return tileCoordinate;
        }

        //Occupied Tile definition
        public sealed class OccupiedTile : Tile
        {
            private readonly Piece pieceOnTile;
            //Constructor
            public OccupiedTile(int tileCoordinate, Piece piece) : base(tileCoordinate)
            {
                this.pieceOnTile = piece;
            }
            /// <summary>
            /// Print out of occupied tile
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return getPiece().getPieceAlliance().isBlack() ? getPiece().ToString().ToLower() :
                    getPiece().ToString();
            }
            /// <summary>
            /// return true as an occupied tile will always have a piece on it
            /// </summary>
            /// <returns></returns>
            public override bool isTileOccupied()
            {
                return true;
            }
            /// <summary>
            /// Return the piece occupyig the tile.
            /// </summary>
            /// <returns></returns>
            public override Piece getPiece()
            {
                return pieceOnTile;
            }

        }
    }
}

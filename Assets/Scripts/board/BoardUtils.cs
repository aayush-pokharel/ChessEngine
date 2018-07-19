using Assets.Scripts.pieces;
using Assets.Scripts.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.board.Move;

namespace Assets.Scripts.board
{
    public class BoardUtils
    {
        /// <summary>
        /// Declarations of all the columns
        /// </summary>
        public static readonly bool[] FIRST_COLUMN = initColumn(0);
        public static readonly bool[] SECOND_COLUMN = initColumn(1);
        public static readonly bool[] SEVENTH_COLUMN = initColumn(6);
        public static readonly bool[] EIGHTH_COLUMN = initColumn(7);
        public static readonly bool[] FIRST_RANK = initRow(56);
        public static readonly bool[] SECOND_RANK = initRow(48);
        public static readonly bool[] THIRD_RANK = initRow(40);
        public static readonly bool[] FOURTH_RANK = initRow(32);
        public static readonly bool[] FIFTH_RANK = initRow(24);
        public static readonly bool[] SIXTH_RANK = initRow(16);
        public static readonly bool[] SEVENTH_RANK = initRow(8);
        public static readonly bool[] EIGHTH_RANK = initRow(0);

        /// <summary>
        /// data structure declarations
        /// </summary>
        public static readonly List<String> ALGEBRAIC_NOTATION = initializeAgebraicNotation();
        public static readonly Dictionary<String, int> POSITION_TO_COORDINATE = initializePositionToCoordinateDictionary();

        /// <summary>
        /// Constant declaration
        /// </summary>
        public static readonly int START_TILE_INDEX = 0;
        public const int NUM_TILES = 64;
        public const int NUM_TILES_PER_ROW = 8;


        /// <summary>
        /// Initialize the columns
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        private static bool[] initColumn(int columnNumber)
        {
             bool[] column = new bool[NUM_TILES];
            do
            {
                column[columnNumber] = true;
                columnNumber += NUM_TILES_PER_ROW;
            } while (columnNumber < NUM_TILES);

            return column;
        }
        /// <summary>
        /// intialize the rows
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        private static bool[] initRow(int rowNumber)
        {
            bool[] row = new bool[NUM_TILES];
            do
            {
                row[rowNumber] = true;
                rowNumber++;
            } while (rowNumber % NUM_TILES_PER_ROW != 0);
            return row;
        }
        /// <summary>
        /// This class should never be instantaited as an object
        /// </summary>
        private BoardUtils()
        {
            throw new Exception("You cannot instantiate me!");
        }
        /// <summary>
        /// position to coordinate initialization of algebraic notation
        /// </summary>
        /// <returns></returns>
        private static Dictionary<String, int> initializePositionToCoordinateDictionary()
        {
            Dictionary<String, int> positionToCoordinate = new Dictionary<String, int>();
            for (int i = START_TILE_INDEX; i < NUM_TILES; i++)
            {
                positionToCoordinate.Add(ALGEBRAIC_NOTATION[i], i);
            }
            return positionToCoordinate;
        }
        /// <summary>
        /// Algebraic notation initialization
        /// </summary>
        /// <returns></returns>
        public static List<String> initializeAgebraicNotation()
        {
            return (new String[]
            {
                "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8",
                "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7",
                "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6",
                "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5",
                "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4",
                "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3",
                "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2",
                "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1"
            }).ToList();
        }
        /// <summary>
        /// Returns true for a valid tile coordinate on the board
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static bool IsValidTileCoordinate(int coordinate)
        {
            return coordinate >= 0 && coordinate < 64;
        }
        public static int getCoordinateAtPosition(String position)
        {
            return POSITION_TO_COORDINATE[position];
        }
        public static List<Move> lastNMoves(Board board, int N)
        {
            List<Move> moveHistory = new List<Move>();
            Move currentMove = board.getTransitionMove();
            int i = 0;
            while (currentMove != MoveFactory.getNullMove() && i < N)
            {
                moveHistory.Add(currentMove);
                currentMove = currentMove.getBoard().getTransitionMove();
                i++;
            }
            return moveHistory;
        }
        public static int mvvlva(Move move)
        {
            Piece movingPiece = move.getMovedPiece();
            if (move.isAttack())
            {
                Piece attackedPiece = move.getAttackedPiece();
                return (attackedPiece.getPieceValue() - movingPiece.getPieceValue() + PieceType.KING.getPieceValue()) * 100;
            }
            return PieceType.KING.getPieceValue() - movingPiece.getPieceValue();
        }
        public static bool kingThreat(Move move)
        {
            Board board = move.getBoard();
            MoveTransition transition = board.getCurrentPlayer().makeMove(move);
            return transition.getBoard().getCurrentPlayer().isCheck();
        }
        public static String getPostionAtCoordinate(int coordinate)
        {
            return ALGEBRAIC_NOTATION[coordinate];
        }
    }
}

using Assets.Scripts.board;
using Assets.Scripts.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    /// <summary>
    /// Enum representation of chess Alliances
    /// </summary>
    public enum Alliance
    {
        WHITE,
        BLACK
    }

    public static class AllianceMethods
    {
        /// <summary>
        /// Pawn movement direction based on alliance
        /// </summary>
        /// <param name="alliance"></param>
        /// <returns></returns>
        public static int getDirection(this Alliance alliance)
        {
            int direction;
            switch(alliance)
            {
                case Alliance.WHITE:
                    direction = -1;
                    break;
                case Alliance.BLACK:
                    direction = 1;
                    break;
                default:
                    direction = 0;
                    break;
            }
            return direction;
        }
        /// <summary>
        /// get the opposite direction of the current alliance for the pawns
        /// </summary>
        /// <param name="alliance"></param>
        /// <returns></returns>
        public static int getOppositeDirection(this Alliance alliance)
        {
            int direction;
            switch(alliance)
            {
                case Alliance.WHITE:
                    direction = 1;
                    break;
                case Alliance.BLACK:
                    direction = -1;
                    break;
                default:
                    direction = 0;
                    break;
            }
            return direction;
        }
        /// <summary>
        /// Decides if the piece alliance is white
        /// </summary>
        /// <param name="alliance"></param>
        /// <returns></returns>
        public static bool isWhite(this Alliance alliance)
        {
            if(alliance == Alliance.WHITE)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Decides if the piece alliance is black
        /// </summary>
        /// <param name="alliance"></param>
        /// <returns></returns>
        public static bool isBlack(this Alliance alliance)
        {
            if (alliance == Alliance.BLACK)
            {
                return true;
            }
            return false;
        }
        public static Player choosePlayer(this Alliance alliance, 
            WhitePlayer whitePlayer, 
            BlackPlayer blackPlayer)
        {
            switch(alliance)
            {
                case Alliance.WHITE:
                    return whitePlayer;
                case Alliance.BLACK:
                    return blackPlayer;
                default:
                    return null;
            }
        }
        public static bool isPawnPromotionSquare(this Alliance alliance, int position)
        {
            switch(alliance)
            {
                case Alliance.WHITE:
                    return BoardUtils.EIGHTH_RANK[position];
                case Alliance.BLACK:
                    return BoardUtils.FIRST_RANK[position];
                default:
                    return false;
            }
            
        }
        public static int pawnBonus(this Alliance alliance, int position)
        {
            switch(alliance)
            {
                case Alliance.WHITE:
                    return WHITE_PAWN_PREFERRED_COORDINATES[position];
                case Alliance.BLACK:
                    return WHITE_PAWN_PREFERRED_COORDINATES[position];
                default:
                    return 0;
            }
            
        }

        public static int knightBonus(this Alliance alliance, int position)
        {
            switch (alliance)
            {
                case Alliance.WHITE:
                    return WHITE_KNIGHT_PREFERRED_COORDINATES[position];
                case Alliance.BLACK:
                    return WHITE_KNIGHT_PREFERRED_COORDINATES[position];
                default:
                    return 0;
            }
        }

        public static int bishopBonus(this Alliance alliance, int position)
        {
            switch (alliance)
            {
                case Alliance.WHITE:
                    return WHITE_BISHOP_PREFERRED_COORDINATES[position];
                case Alliance.BLACK:
                    return WHITE_BISHOP_PREFERRED_COORDINATES[position];
                default:
                    return 0;
            }
        }

        public static int rookBonus(this Alliance alliance, int position)
        {
            switch (alliance)
            {
                case Alliance.WHITE:
                    return WHITE_ROOK_PREFERRED_COORDINATES[position];
                case Alliance.BLACK:
                    return WHITE_ROOK_PREFERRED_COORDINATES[position];
                default:
                    return 0;
            }
        }

        public static int queenBonus(this Alliance alliance, int position)
        {
            switch (alliance)
            {
                case Alliance.WHITE:
                    return WHITE_QUEEN_PREFERRED_COORDINATES[position];
                case Alliance.BLACK:
                    return WHITE_QUEEN_PREFERRED_COORDINATES[position];
                default:
                    return 0;
            }
        }
        public static int kingBonus(this Alliance alliance, int position)
        {
            switch (alliance)
            {
                case Alliance.WHITE:
                    return WHITE_KING_PREFERRED_COORDINATES[position];
                case Alliance.BLACK:
                    return WHITE_KING_PREFERRED_COORDINATES[position];
                default:
                    return 0;
            }
        }
        private readonly static int[] WHITE_PAWN_PREFERRED_COORDINATES = {
            0,  0,  0,  0,  0,  0,  0,  0,
            75, 75, 75, 75, 75, 75, 75, 75,
            25, 25, 29, 29, 29, 29, 25, 25,
            5,  5, 10, 25, 25, 10,  5,  5,
            0,  0,  0, 20, 20,  0,  0,  0,
            5, -5,-10,  0,  0,-10, -5,  5,
            5, 10, 10,-20,-20, 10, 10,  5,
            0,  0,  0,  0,  0,  0,  0,  0
    };

        private readonly static int[] BLACK_PAWN_PREFERRED_COORDINATES = {
            0,  0,  0,  0,  0,  0,  0,  0,
            5, 10, 10,-20,-20, 10, 10,  5,
            5, -5,-10,  0,  0,-10, -5,  5,
            0,  0,  0, 20, 20,  0,  0,  0,
            5,  5, 10, 25, 25, 10,  5,  5,
            25, 25, 29, 29, 29, 29, 25, 25,
            75, 75, 75, 75, 75, 75, 75, 75,
            0,  0,  0,  0,  0,  0,  0,  0
    };

        private readonly static int[] WHITE_KNIGHT_PREFERRED_COORDINATES = {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50
    };

        private readonly static int[] BLACK_KNIGHT_PREFERRED_COORDINATES = {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50,
    };

        private readonly static int[] WHITE_BISHOP_PREFERRED_COORDINATES = {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-10,-10,-10,-10,-10,-20
    };

        private readonly static int[] BLACK_BISHOP_PREFERRED_COORDINATES = {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -20,-10,-10,-10,-10,-10,-10,-20,
    };

        private readonly static int[] WHITE_ROOK_PREFERRED_COORDINATES = {
            0,  0,  0,  0,  0,  0,  0,  0,
            5, 20, 20, 20, 20, 20, 20,  5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            0,  0,  0,  5,  5,  0,  0,  0
    };

        private readonly static int[] BLACK_ROOK_PREFERRED_COORDINATES = {
            0,  0,  0,  5,  5,  0,  0,  0,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            5, 20, 20, 20, 20, 20, 20,  5,
            0,  0,  0,  0,  0,  0,  0,  0,
    };

        private readonly static int[] WHITE_QUEEN_PREFERRED_COORDINATES = {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5,  5,  5,  5,  0,-10,
             -5,  0,  5,  5,  5,  5,  0, -5,
              0,  0,  5,  5,  5,  5,  0, -5,
            -10,  5,  5,  5,  5,  5,  0,-10,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
    };

        private readonly static int[] BLACK_QUEEN_PREFERRED_COORDINATES = {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -10,  5,  5,  5,  5,  5,  0,-10,
              0,  0,  5,  5,  5,  5,  0, -5,
              0,  0,  5,  5,  5,  5,  0, -5,
            -10,  0,  5,  5,  5,  5,  0,-10,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
    };

        private readonly static int[] WHITE_KING_PREFERRED_COORDINATES = {
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -10,-20,-20,-20,-20,-20,-20,-10,
             20, 20,  0,  0,  0,  0, 20, 20,
             20, 30, 10,  0,  0, 10, 30, 20
    };

        private readonly static int[] BLACK_KING_PREFERRED_COORDINATES = {
             20, 30, 10,  0,  0, 10, 30, 20,
             20, 20,  0,  0,  0,  0, 20, 20,
            -10,-20,-20,-20,-20,-20,-20,-10,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30
    };

    }
}

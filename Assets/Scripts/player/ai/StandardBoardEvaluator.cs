using System;
using Assets.Scripts.board;
using Assets.Scripts.player;
using Assets.Scripts.pieces;
namespace Assets.Scripts.player.ai
{
    public sealed class StandardBoardEvaluator : IBoardEvaluator
    {
        private static readonly int CHECK_BONUS = 50;
        private static readonly int CHECK_MATE_BONUS = 10000;
        private static readonly int DEPTH_BONUS = 100;
        private static readonly int CASTLE_BONUS = 60;
        private static readonly int ATTACK_MULTIPLIER = 2;
        private static readonly int TWO_BISHOPS_BONUS = 50;
        public int evaluate(Board board, int depth)
        {
            return scorePlayer(board.getWhitePlayer(), depth) -
                scorePlayer(board.getBlackPlayer(), depth);
        }
         
        private int scorePlayer(Player player, int depth)
        {
            return pieceValue(player) + 
                mobility(player) + 
                check(player) +
                checkMate(player, depth) + 
                castled(player) +
                attacks(player);
        }

        private static int castled(Player player)
        {
            return player.isCastled() ? CASTLE_BONUS : 0;
        }
        private static int attacks(Player player)
        {
            int attackScore = 0;
            foreach(Move move in player.getLegalMoves())
            {
                if (move.isAttack())
                {
                    Piece movedPiece = move.getMovedPiece();
                    Piece attackedPiece = move.getAttackedPiece();
                    if (movedPiece.getPieceValue() <= attackedPiece.getPieceValue())
                    {
                        attackScore++;
                    }
                }
            }
            return attackScore * ATTACK_MULTIPLIER;
        }
        private static int checkMate(Player player, int depth)
        {
            return player.getOpponent().isCheckMate() ? CHECK_MATE_BONUS * depthBonus(depth) : check(player);
        }

        private static int depthBonus(int depth)
        {
            return depth == 0 ? 1 : DEPTH_BONUS * depth;
        }

        private static int check(Player player)
        {
            return player.getOpponent().isCheck() ? CHECK_BONUS : 0;
        }

        private static int mobility(Player player)
        {
            return player.getLegalMoves().Count;
        }

        private static int pieceValue(Player player)
        {
            int pieceValueScore = 0;
            int numBishops = 0;
            foreach (Piece piece in player.getActivePieces())
            {
                pieceValueScore += piece.getPieceValue() + piece.locationBonus();
                if (piece.getPieceType().isBishop())
                {
                    numBishops++;
                }
            }
            return pieceValueScore + (numBishops == 2 ? TWO_BISHOPS_BONUS : 0);
        }
    }
}
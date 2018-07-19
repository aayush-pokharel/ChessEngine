using Assets.Scripts.board;
using Assets.Scripts.pieces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.player
{
    public abstract class Player
    {
        protected readonly Board board;
        protected readonly King playerKing;
        protected List<Move> legalMoves;
        protected readonly bool isInCheck;
        public Player(Board board,
            IReadOnlyCollection<Move> playerLegal,
            IReadOnlyCollection<Move> opponentMoves)
        {
            this.board = board;
            this.playerKing = establishKing();
            this.legalMoves = playerLegal.ToList();
            this.isInCheck = !(Player.calculateAttacksOnTile(this.playerKing.getPiecePosition(), opponentMoves).Count == 0);
        }
        /// <summary>
        /// Get the Player's King
        /// </summary>
        /// <returns></returns>
        public King getPlayerKing()
        {
            return this.playerKing;
        }
        public IReadOnlyCollection<Move> getLegalMoves()
        {
            return this.legalMoves.ToList();
        }
        /// <summary>
        /// Calculate the opponent attacks on a tile
        /// </summary>
        /// <param name="piecePosition"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
        protected static IReadOnlyCollection<Move> calculateAttacksOnTile(int piecePosition, 
            IReadOnlyCollection<Move> moves)
        {
            IList<Move> attackMoves = new List<Move>();
            foreach(Move move in moves)
            {
                if(piecePosition == move.getDestinationCoordinate())
                {
                    attackMoves.Add(move);
                }
            }
            return attackMoves.ToList();
        }

        /// <summary>
        /// return the established king for this player
        /// </summary>
        /// <returns></returns>
        private King establishKing()
        {
            foreach (Piece piece in getActivePieces())
            {
                if (piece.getPieceType().isKing())
                {
                    return (King)piece;
                }
            }
            throw new SystemException();
        }

        public bool isMoveLegal(Move move)
        {
            return legalMoves.Contains(move);
        }
        /// <summary>
        /// Check for a check on the king
        /// </summary>
        /// <returns></returns>
        public bool isCheck()
        {
            return isInCheck;
        }
        /// <summary>
        /// Check for a checkmate on the king
        /// </summary>
        /// <returns></returns>
        public bool isCheckMate()
        {
            return this.isInCheck && !hasEscapeMoves();
        }

        /// <summary>
        /// Check for a stalemate
        /// </summary>
        /// <returns></returns>
        public bool isStaleMate()
        {
            return !this.isInCheck && !hasEscapeMoves();
        }

        /// <summary>
        /// check if the king has escape moves
        /// </summary>
        /// <returns></returns>
        private bool hasEscapeMoves()
        {
            foreach (Move move in this.legalMoves)
            {
                MoveTransition transition = makeMove(move);
                if (transition.getMoveStatus().isDone())
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Check for a castle move
        /// </summary>
        /// <returns></returns>
        public bool isCastled()
        {
            return this.playerKing.IsCastled();
        }
        /// <summary>
        /// Make a move transition
        /// Represents the transition from one board to another
        /// after a player's move
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public MoveTransition makeMove(Move move)
        {
            if(!isMoveLegal(move))
            {
                return new MoveTransition(this.board, move, MoveStatus.ILLEGAL_MOVE);
            }
            Board transitionBoard = move.execute();
            IReadOnlyCollection<Move> kingAttacks = Player.calculateAttacksOnTile(
                transitionBoard.getCurrentPlayer().getOpponent().getPlayerKing().getPiecePosition(),
                transitionBoard.getCurrentPlayer().getLegalMoves());
            if(!(kingAttacks.Count == 0))
            {
                return new MoveTransition(this.board, move, MoveStatus.LEAVES_PLAYER_IN_CHECK);
            }
            return new MoveTransition(transitionBoard, move, MoveStatus.DONE);
        }
        /// <summary>
        /// Abstract method declarations
        /// </summary>
        /// <returns></returns>
        public abstract ICollection<Piece> getActivePieces();
        public abstract Alliance getAlliance();
        public abstract Player getOpponent();
        public abstract ICollection<Move> calculateKingCastles(IReadOnlyCollection<Move> opponentLegals);
    }
}
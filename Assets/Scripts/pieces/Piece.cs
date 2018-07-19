using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.board;
using System;

namespace Assets.Scripts.pieces
{
    public abstract class Piece
    {
        //Global Declarations
        protected readonly PieceType pieceType;
        protected readonly int piecePosition;
        protected readonly Alliance pieceAlliance;
        protected readonly bool isFirstMove;
        private readonly int cacheHashCode;
        public readonly bool isWhite;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="piecePosition"></param>
        /// <param name="pieceAlliance"></param>
        protected Piece(PieceType pieceType,
            int piecePosition, 
            Alliance pieceAlliance,
            bool isFirstMove)
        {
            this.pieceType = pieceType;
            this.piecePosition = piecePosition;
            this.pieceAlliance = pieceAlliance;
            this.isFirstMove = isFirstMove;
            this.cacheHashCode = computeHashCode();
        }
        /// <summary>
        /// Computes the hash code for the current object
        /// </summary>
        /// <returns></returns>
        private int computeHashCode()
        {
            int result = pieceType.GetHashCode();
            result = 31 * result + pieceAlliance.GetHashCode();
            result = 31 * result + piecePosition.GetHashCode();
            result = 31 * result + (isFirstMove ? 1 : 0);
            return result;
        }

        /// <summary>
        /// equals comparison between two piece objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(!(obj.GetType().BaseType == typeof(Piece)))
            {
                return false;
            }
            Piece otherPiece = (Piece)obj;
            //return boolean value base on equals logic
            return piecePosition == otherPiece.getPiecePosition() &&
                pieceType == otherPiece.getPieceType() &&
                pieceAlliance == otherPiece.getPieceAlliance() &&
                isFirstMove == otherPiece.IsFirstMove();
        }
        /// <summary>
        /// Get the hash code of the object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.cacheHashCode;
        }
        /// <summary>
        /// get the current piece position
        /// </summary>
        /// <returns></returns>
        public int getPiecePosition()
        {
            return this.piecePosition;
        }

        /// <summary>
        /// Get the Piece Alliance : White / Black
        /// </summary>
        /// <returns></returns>
        public Alliance getPieceAlliance()
        {
            return this.pieceAlliance;
        }
        /// <summary>
        /// Check for first move
        /// </summary>
        /// <returns></returns>
        public bool IsFirstMove()
        {
            return isFirstMove;
        }
        /// <summary>
        /// check for piece type
        /// </summary>
        /// <returns></returns>
        public PieceType getPieceType()
        {
            return this.pieceType;
        }
        public int getPieceValue()
        {
            return this.pieceType.getPieceValue();
        }
        public abstract int locationBonus();
        /// <summary>
        /// Abstraction for all the legal moves of a piece
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public abstract ICollection<Move> calculateLegalMoves(Board board);
        /// <summary>
        /// abstraction for moving a piece
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public abstract Piece movePiece(Move move);
        
    }
    public enum PieceType
    {
        PAWN,
        KNIGHT,
        BISHOP,
        ROOK,
        QUEEN,
        KING
    }
    public static class PieceTypeMethod
    {
        /// <summary>
        /// returns the Piece type of the current object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String GetPieceType(this PieceType type)
        {
            String pieceName = "";
            switch (type)
            {
                case PieceType.PAWN:
                    pieceName = "P";
                    break;
                case PieceType.KNIGHT:
                    pieceName = "N";
                    break;
                case PieceType.BISHOP:
                    pieceName = "B";
                    break;
                case PieceType.ROOK:
                    pieceName = "R";
                    break;
                case PieceType.QUEEN:
                    pieceName = "Q";
                    break;
                case PieceType.KING:
                    pieceName = "K";
                    break;
                default:
                    break;

            }
            return pieceName;
        }
        /// <summary>
        /// Classifies a piece as king
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool isKing(this PieceType type)
        {
            if(type == PieceType.KING)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Check for rook piece type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool isRook(this PieceType type)
        {
            if (type == PieceType.ROOK)
                return true;
            return false;
        }
        public static bool isBishop(this PieceType type)
        {
            if (type == PieceType.BISHOP)
                return true;
            return false;
        }
        public static int getPieceValue(this PieceType type)
        {
            int value;
            switch(type)
            {
                case PieceType.PAWN:
                    value = 100;
                    break;
                case PieceType.KNIGHT:
                    value = 300;
                    break;
                case PieceType.BISHOP:
                    value = 300;
                    break;
                case PieceType.ROOK:
                    value = 500;
                    break;
                case PieceType.QUEEN:
                    value = 900;
                    break;
                case PieceType.KING:
                    value = 10000;
                    break;
                default:
                    value = 0;
                    break;
            }
            return value;
        }
    }

}
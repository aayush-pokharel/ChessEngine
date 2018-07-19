using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.pieces;
using static Assets.Scripts.board.Board;

namespace Assets.Scripts.board
{
    public abstract class Move
    {
        protected readonly Board board;
        protected readonly Piece movedPiece;
        protected readonly int destinationCoordinate;
        protected readonly bool isFirstMove;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="board"></param>
        /// <param name="movedPiece"></param>
        /// <param name="destinationCoordinate"></param>
        private Move(Board board, Piece movedPiece, int destinationCoordinate)
        {
            this.board = board;
            this.movedPiece = movedPiece;
            this.destinationCoordinate = destinationCoordinate;
            this.isFirstMove = movedPiece.IsFirstMove();
        }

        private Move(Board board,
            int destinationCoordinate)
        {
            this.board = board;
            this.destinationCoordinate = destinationCoordinate;
            this.movedPiece = null;
            this.isFirstMove = false;
        }
        /// <summary>
        /// hash code override for Move
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            //result calculation
            result = prime * result + this.destinationCoordinate;
            result = prime * result + this.movedPiece.GetHashCode();
            result = prime * result + this.movedPiece.getPiecePosition();

            return result;

        }
        /// <summary>
        /// Equals comarison for move types
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(this == obj)
            {
                return true;
            }
            if(!(obj.GetType() == typeof(Move)))
            {
                return false;
            }
            Move otherMove = (Move)obj;
            return getCurrentCoordinate() == otherMove.getCurrentCoordinate() && 
                getDestinationCoordinate() == otherMove.getDestinationCoordinate() &&
                getMovedPiece().Equals(otherMove.getMovedPiece());
        }
        public Board getBoard()
        {
            return this.board;
        }
        /// <summary>
        /// get the current coordinate of the piece
        /// </summary>
        /// <returns></returns>
        public virtual int getCurrentCoordinate()
        {
            return this.getMovedPiece().getPiecePosition();
        }
        public virtual Board execute()
        {
            Builder builder = new Builder();
            //Place all the unmoved piece in the board for current player
            foreach (Piece piece in this.board.getCurrentPlayer().getActivePieces())
            {
                if (!this.movedPiece.Equals(piece))
                {
                    builder.setPiece(piece);
                }
            }
            //Place all the unmoved pieces in the board for the opponent
            foreach (Piece piece in this.board.getCurrentPlayer().getOpponent().getActivePieces())
            {
                builder.setPiece(piece);
            }
            //move the moved piece
            builder.setPiece(this.movedPiece.movePiece(this));
            builder.setMoveMaker(this.board.getCurrentPlayer().getOpponent().getAlliance());
            builder.setMoveTransition(this);
            return builder.build();
        }
        String disambiguationFile()
        {
            foreach(Move move in this.board.getCurrentPlayer().getLegalMoves())
            {
                if (move.getDestinationCoordinate() == this.destinationCoordinate && !this.Equals(move) &&
                   this.movedPiece.getPieceType().GetPieceType().Equals(move.getMovedPiece().getPieceType()))
                {
                    return BoardUtils.getPostionAtCoordinate(this.movedPiece.getPiecePosition()).Substring(0, 1);
                }
            }
            return "";
        }
        /// <summary>
        /// get the destination coordinate of the piece
        /// </summary>
        /// <returns></returns>
        public int getDestinationCoordinate()
        {
            return this.destinationCoordinate;
        }
        /// <summary>
        /// Get the moved piece
        /// </summary>
        /// <returns></returns>
        public Piece getMovedPiece()
        {
            return this.movedPiece;
        }
        public virtual bool isAttack()
        {
            return false;
        }
        public virtual bool isCastlingMove()
        {
            return false;
        }
        public virtual Piece getAttackedPiece()
        {
            return null;
        }
        public virtual bool isEnPassantMove()
        {
            return false;
        }
        public virtual bool isPawnPromotion()
        {
            return false;
        }
        public class MajorAttackMove : AttackMove
        {
            public MajorAttackMove(Board board,
                Piece movedPiece,
                int destinationCoordinate,
                Piece attackPiece):
                base(board, movedPiece, destinationCoordinate, attackPiece)
            {

            }
            public override bool Equals(object obj)
            {
                return this == obj || obj.GetType() == typeof(MajorAttackMove) && base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                return movedPiece.getPieceType().GetPieceType() + disambiguationFile() + "x" + BoardUtils.getPostionAtCoordinate(this.destinationCoordinate);
            }
        }
        /// <summary>
        /// Major piece move type
        /// </summary>
        public sealed class MajorMove : Move
        {
            public MajorMove(Board board, Piece movedPiece, int destinationCoordinate) : 
                base(board, movedPiece, destinationCoordinate)
            {

            }
            public override bool Equals(object obj)
            {
                return this == obj || obj.GetType() == typeof(MajorMove) && base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                return movedPiece.getPieceType().GetPieceType() + disambiguationFile() +
                   BoardUtils.getPostionAtCoordinate(this.destinationCoordinate);
            }
        }
        /// <summary>
        /// Attack Move type
        /// </summary>
        public class AttackMove : Move
        {
            readonly Piece attackPiece;
            public AttackMove(Board board, 
                Piece movedPiece, 
                int destinationCoordinate, 
                Piece attackPiece) : 
                base(board, movedPiece, destinationCoordinate)
            {
                this.attackPiece = attackPiece;
            }
            /// <summary>
            /// hash code override
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return this.attackPiece.GetHashCode() + base.GetHashCode();
            }
            /// <summary>
            /// Equal override
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (this == obj)
                {
                    return true;
                }
                if (!(obj.GetType() == typeof(AttackMove)))
                {
                    return false;
                }
                AttackMove otherAttackMove = (AttackMove)obj;
                return base.Equals(otherAttackMove) && 
                    getAttackedPiece().Equals(otherAttackMove.getAttackedPiece());
            }

            /// <summary>
            /// return true for isAttack
            /// </summary>
            /// <returns></returns>
            public override bool isAttack()
            {
                return true;
            }
            /// <summary>
            /// Get the piece being attacked
            /// </summary>
            /// <returns></returns>
            public override Piece getAttackedPiece()
            {
                return this.attackPiece;
            }
            public override string ToString()
            {
                return movedPiece.getPieceType().GetPieceType() + disambiguationFile() + "x" +
                   BoardUtils.getPostionAtCoordinate(this.destinationCoordinate);
            }
        }
        /// <summary>
        /// Regular Pawn move type
        /// </summary>
        public sealed class PawnMove : Move
        {
            public PawnMove(Board board, 
                Piece movedPiece, 
                int destinationCoordinate) :
                base(board, movedPiece, destinationCoordinate)
            {

            }
            public override bool Equals(object obj)
            {
                return this == obj || obj.GetType() == typeof(PawnMove) && base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                return BoardUtils.getPostionAtCoordinate(this.destinationCoordinate);
            }
        }
        /// <summary>
        /// Pawn Attack move type
        /// </summary>
        public class PawnAttackMove : AttackMove
        {
            public PawnAttackMove(Board board, 
                Piece movedPiece, 
                int destinationCoordinate,
                Piece attackPiece) :
                base(board, movedPiece, destinationCoordinate, attackPiece)
            {

            }
            public override bool Equals(object obj)
            {
                return this == obj || obj.GetType() == typeof(PawnAttackMove) && base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                return BoardUtils.getPostionAtCoordinate(this.movedPiece.getPiecePosition()).Substring(0, 1) +
                    "x" +
                    BoardUtils.getPostionAtCoordinate(this.destinationCoordinate);
            }
        }
        /// <summary>
        /// Pawn En Passant Attack move type
        /// </summary>
        public sealed class PawnEnPassantAttackMove : PawnAttackMove
        {
            public PawnEnPassantAttackMove(Board board,
                Piece movedPiece,
                int destinationCoordinate,
                Piece attackPiece) :
                base(board, movedPiece, destinationCoordinate, attackPiece)
            {

            }
            public override bool Equals(object obj)
            {
                return this == obj || obj.GetType() == typeof(PawnEnPassantAttackMove) && base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override Board execute()
            {
                Builder builder = new Builder();
                foreach(Piece piece in this.board.getCurrentPlayer().getActivePieces())
                {
                    if(!this.movedPiece.Equals(piece))
                    {
                        builder.setPiece(piece);
                    }
                }
                foreach(Piece piece in this.board.getCurrentPlayer().getOpponent().getActivePieces())
                {
                    if(!piece.Equals(this.getAttackedPiece()))
                    {
                        builder.setPiece(piece);
                    }
                }
                builder.setPiece(this.movedPiece.movePiece(this));
                builder.setMoveMaker(this.board.getCurrentPlayer().getOpponent().getAlliance());
                builder.setMoveTransition(this);
                return builder.build();
            }
            public override bool isEnPassantMove()
            {
                return true;
            }
        }
        public sealed class PawnPromotion : Move
        {
            //Variable declarations
            private readonly Move decoratedMove;
            private readonly Pawn promotedPawn;
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="decoratedMove"></param>
            public PawnPromotion(Move decoratedMove) :
                base(decoratedMove.getBoard(), 
                    decoratedMove.getMovedPiece(), 
                    decoratedMove.getDestinationCoordinate())
            {
                this.decoratedMove = decoratedMove;
                this.promotedPawn = (Pawn)decoratedMove.getMovedPiece();
            }

            /// <summary>
            /// Override Methods
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return decoratedMove.GetHashCode() + (31 * promotedPawn.GetHashCode());
            }
            public override bool Equals(object obj)
            {
                return this == obj || obj.GetType() == typeof(PawnPromotion) && base.Equals(obj);
            }
            public override Board execute()
            {
                Board pawnMoveBoard = this.decoratedMove.execute();
                Board.Builder builder = new Builder();
                foreach(Piece piece in pawnMoveBoard.getCurrentPlayer().getActivePieces())
                {
                    if(!this.promotedPawn.Equals(piece))
                    {
                        builder.setPiece(piece);
                    }
                }
                foreach(Piece piece in pawnMoveBoard.getCurrentPlayer().getOpponent().getActivePieces())
                {
                    builder.setPiece(piece);
                }
                builder.setPiece(this.promotedPawn.getPromotionPiece().movePiece(this));
                builder.setMoveMaker(pawnMoveBoard.getCurrentPlayer().getAlliance());
                builder.setMoveTransition(this);
                return builder.build();
            }
            public override bool isAttack()
            {
                return this.decoratedMove.isAttack();
            }
            public override Piece getAttackedPiece()
            {
                return this.decoratedMove.getAttackedPiece();
            }
            public override bool isPawnPromotion()
            {
                return true;
            }
            public override string ToString()
            {
                return "";
            }
            
        }

        

        /// <summary>
        /// Pawn jump two tiles move type
        /// </summary>
        public sealed class PawnJump : Move
        {
            public PawnJump(Board board, 
                Piece movedPiece, 
                int destinationCoordinate) :
                base(board, movedPiece, destinationCoordinate)
            {

            }
            /// <summary>
            /// move execution override for Pawn Jump
            /// </summary>
            /// <returns></returns>
            public override Board execute()
            {
                Builder builder = new Builder();
                foreach(Piece piece in this.board.getCurrentPlayer().getActivePieces())
                {
                    if(!this.movedPiece.Equals(piece))
                    {
                        builder.setPiece(piece);
                    }
                }
                foreach(Piece piece in this.board.getCurrentPlayer().getOpponent().getActivePieces())
                {
                    builder.setPiece(piece);
                }
                Pawn movedPawn = (Pawn)this.movedPiece.movePiece(this);
                builder.setPiece(movedPawn);
                builder.setEnPassantPawn(movedPawn);
                builder.setMoveMaker(this.board.getCurrentPlayer().getOpponent().getAlliance());
                builder.setMoveTransition(this);
                return builder.build();
            }
            public override string ToString()
            {
                return BoardUtils.getPostionAtCoordinate(this.destinationCoordinate);
            }
        }
        /// <summary>
        /// Regular move type
        /// </summary>
        public abstract class CastleMove : Move
        {
            protected readonly Rook castleRook;
            protected readonly int castleRookStart;
            protected readonly int castleRookDestination;
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="board"></param>
            /// <param name="movedPiece"></param>
            /// <param name="destinationCoordinate"></param>
            /// <param name="castleRook"></param>
            /// <param name="castleRookStart"></param>
            /// <param name="castleRookDestination"></param>
            public CastleMove(Board board,
                    Piece movedPiece,
                    int destinationCoordinate,
                    Rook castleRook,
                    int castleRookStart,
                    int castleRookDestination) :
                base(board, movedPiece, destinationCoordinate)
            {
                this.castleRook = castleRook;
                this.castleRookStart = castleRookStart;
                this.castleRookDestination = castleRookDestination;
            }
            public int getCastleRookStart()
            {
                return this.castleRookStart;
            }
            public int getCastleRookDestination()
            {
                return this.castleRookDestination;
            }
            /// <summary>
            /// get the castle rook
            /// </summary>
            /// <returns></returns>
            public Rook getCastleRook()
            {
                return this.castleRook;
            }
            /// <summary>
            /// return true for is Castling move
            /// </summary>
            /// <returns></returns>
            public override bool isCastlingMove()
            {
                return true;
            }
            /// <summary>
            /// execute method
            /// </summary>
            /// <returns></returns>
            public override Board execute()
            {
                Builder builder = new Builder();
                foreach (Piece piece in this.board.getCurrentPlayer().getActivePieces())
                {
                    if (!this.movedPiece.Equals(piece) &&
                        !this.castleRook.Equals(piece))
                    {
                        builder.setPiece(piece);
                    }
                }
                foreach (Piece piece in this.board.getCurrentPlayer().getOpponent().getActivePieces())
                {
                    builder.setPiece(piece);
                }
                builder.setPiece(this.movedPiece.movePiece(this));
                //TODO look into first move on normal pieces
                builder.setPiece(new Rook(this.castleRookDestination, this.castleRook.getPieceAlliance()));
                builder.setMoveMaker(this.board.getCurrentPlayer().getOpponent().getAlliance());
                builder.setMoveTransition(this);
                return builder.build();
            }
            public override int GetHashCode()
            {
                int prime = 31;
                int result = base.GetHashCode();
                result = prime * result + this.castleRook.GetHashCode();
                result = prime * result + this.castleRookDestination;
                return result;
            }
            public override bool Equals(object obj)
            {
                if (this == obj)
                {
                    return true;
                }
                if (!(obj.GetType() == typeof(CastleMove)))
                {
                    return false;
                }
                CastleMove otherMove = (CastleMove)obj;
                return base.Equals(otherMove) &&
                    this.castleRook.Equals(otherMove.getCastleRook());
            }
            /// <summary>
            /// King side King castle move
            /// </summary>
            public sealed class KingSideCastleMove : CastleMove
            {
                /// <summary>
                /// Constructor
                /// </summary>
                /// <param name="board"></param>
                /// <param name="movedPiece"></param>
                /// <param name="destinationCoordinate"></param>
                /// <param name="castleRook"></param>
                /// <param name="castleRookStart"></param>
                /// <param name="castleRookDestination"></param>
                public KingSideCastleMove(Board board, 
                    Piece movedPiece, 
                    int destinationCoordinate,
                    Rook castleRook,
                    int castleRookStart,
                    int castleRookDestination) :
                    base(board, 
                        movedPiece, 
                        destinationCoordinate, 
                        castleRook, 
                        castleRookStart, 
                        castleRookDestination)
                {
                }
                public override bool Equals(object obj)
                {
                    return this == obj || obj.GetType() == typeof(KingSideCastleMove) && base.Equals(obj);
                }
                public override int GetHashCode()
                {
                    return base.GetHashCode();
                }
                public override String ToString()
                {
                    return "0-0";
                }
            }
            /// <summary>
            /// Queen side King castle move
            /// </summary>
            public sealed class QueenSideCastleMove : CastleMove
            {
                /// <summary>
                /// Constructor
                /// </summary>
                /// <param name="board"></param>
                /// <param name="movedPiece"></param>
                /// <param name="destinationCoordinate"></param>
                /// <param name="castleRook"></param>
                /// <param name="castleRookStart"></param>
                /// <param name="castleRookDestination"></param>
                public QueenSideCastleMove(Board board,
                    Piece movedPiece,
                    int destinationCoordinate,
                    Rook castleRook,
                    int castleRookStart,
                    int castleRookDestination) :
                    base(board,
                        movedPiece,
                        destinationCoordinate,
                        castleRook,
                        castleRookStart,
                        castleRookDestination)
                {
                }
                public override bool Equals(object obj)
                {
                    return this == obj || obj.GetType() == typeof(QueenSideCastleMove) && base.Equals(obj);
                }
                public override int GetHashCode()
                {
                    return base.GetHashCode();
                }
                public override string ToString()
                {
                    return "0-0-0";
                }
            }
        }
        /// <summary>
        /// Null move type
        /// </summary>
        public sealed class NullMove : Move
        {
            public NullMove() :
                base(null, -1)
            {
                
            }
            public override Board execute()
            {
                throw new InvalidOperationException("Cannot execute a null move");
            }
            public override int getCurrentCoordinate()
            {
                return -1;
            }
        }
        
        public static class MoveFactory
        {
            public static readonly Move NULL_MOVE = new NullMove();
            public static Move getNullMove()
            {
                return NULL_MOVE;
            }
            /// <summary>
            /// Create the move based on start and end positions
            /// </summary>
            /// <param name="board"></param>
            /// <param name="currentCoordinate"></param>
            /// <param name="destinationCoordinate"></param>
            /// <returns></returns>
            public static Move createMove (Board board,
                int currentCoordinate,
                int destinationCoordinate)
            {
                List<Move> moves = board.getAllLegalMoves().ToList();
                foreach(Move move in board.getAllLegalMoves())
                {
                    if(move.getCurrentCoordinate() == currentCoordinate &&
                        move.getDestinationCoordinate() == destinationCoordinate)
                    {
                        return move;
                    }
                }
                return NULL_MOVE;
            }
        }
    }
    
}

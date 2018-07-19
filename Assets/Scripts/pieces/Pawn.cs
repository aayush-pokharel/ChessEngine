using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.board;
using static Assets.Scripts.board.Move;

namespace Assets.Scripts.pieces
{
    public class Pawn : Piece
    {
        /// <summary>
        /// All possible candidate move coordinates
        /// </summary>
        private readonly static int[] CANDIDATE_MOVE_COORDINATES =
        {
            7, 8, 9, 16
        };
        public Pawn(int piecePosition, Alliance pieceAlliance, bool isFirstMove) :
            base(PieceType.PAWN, piecePosition, pieceAlliance, isFirstMove)
        {

        }
        public Pawn(int piecePosition, Alliance pieceAlliance) :
            base(PieceType.PAWN, piecePosition, pieceAlliance, true)
        {

        }
        public override int locationBonus()
        {
            return this.pieceAlliance.pawnBonus(this.piecePosition);
        }
        /// <summary>
        /// Pawn legal moves calculations
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public override ICollection<Move> calculateLegalMoves(Board board)
        {
            //List of all possible legal moves
            IList<Move> legalMoves = new List<Move>();
            foreach(int currentCandidateOffset in CANDIDATE_MOVE_COORDINATES)
            {
                int candidateDestinationCoordinate = this.piecePosition + (this.getPieceAlliance().getDirection() * currentCandidateOffset);
                //Check if the candidateCoordinate is a valid board coordinate
                if (!BoardUtils.IsValidTileCoordinate(candidateDestinationCoordinate))
                {
                    continue;
                }
                //Pawns can only move up or down in direction based on alliance
                if (currentCandidateOffset == 8 && !board.getTile(candidateDestinationCoordinate).isTileOccupied())
                {
                    //Check for pawn promotion
                    if(this.pieceAlliance.isPawnPromotionSquare(candidateDestinationCoordinate))
                    {
                        //if the pawn is eligible for promotion then promote it
                        legalMoves.Add(new PawnPromotion(new PawnMove(board, this, candidateDestinationCoordinate)));
                    }
                    else
                    {
                        //Add this pawn move as a legal move
                        legalMoves.Add(new PawnMove(board, this, candidateDestinationCoordinate));
                    }
                }
                //if the pawns are on starting position then pawns can travel two tiles up or down
                else if (currentCandidateOffset == 16 && this.IsFirstMove() &&
                    ((BoardUtils.SECOND_RANK[this.piecePosition] && this.getPieceAlliance().isWhite()) ||
                    (BoardUtils.SEVENTH_RANK[this.piecePosition] && this.getPieceAlliance().isBlack())))
                {
                    int behindCandidateDestinationCoordinate = this.piecePosition + (this.pieceAlliance.getDirection() * 8);
                    //Check if the candidate tile is occupied
                    if (!board.getTile(behindCandidateDestinationCoordinate).isTileOccupied() &&
                        !board.getTile(candidateDestinationCoordinate).isTileOccupied())
                    {
                        //Add this pawn move as a legal move
                        legalMoves.Add(new PawnJump(board, this, candidateDestinationCoordinate));
                    }
                }
                //Attack move on 7 tile move
                else if (currentCandidateOffset == 7 &&
                    !((BoardUtils.EIGHTH_COLUMN[this.piecePosition] && this.getPieceAlliance().isWhite()) ||
                    (BoardUtils.FIRST_COLUMN[this.piecePosition] && this.getPieceAlliance().isBlack())))
                {
                    if(board.getTile(candidateDestinationCoordinate).isTileOccupied())
                    {
                        Piece pieceOnCandidate = board.getTile(candidateDestinationCoordinate).getPiece();
                        //Opponent piece alliance check
                        if(this.pieceAlliance != pieceOnCandidate.getPieceAlliance())
                        {
                            //Check for pawn promotion
                            if (this.pieceAlliance.isPawnPromotionSquare(candidateDestinationCoordinate))
                            {
                                //if the pawn is eligible for promotion then promote it
                                legalMoves.Add(new PawnPromotion(new PawnAttackMove(board, this, candidateDestinationCoordinate, pieceOnCandidate)));
                            }
                            else
                            {
                                //Attack Move
                                legalMoves.Add(new PawnAttackMove(board, this, candidateDestinationCoordinate, pieceOnCandidate));
                            }
                            
                        }
                    }
                    //En Passant Condition
                    else if (board.getEnPassantPawn() != null)
                    {
                        if (board.getEnPassantPawn().getPiecePosition() == this.piecePosition + (this.pieceAlliance.getOppositeDirection()))
                        {
                            Piece pieceOnCandidate = board.getEnPassantPawn();
                            if (this.pieceAlliance != pieceOnCandidate.getPieceAlliance())
                            {
                                legalMoves.Add(new PawnEnPassantAttackMove(board, this, candidateDestinationCoordinate, pieceOnCandidate));
                            }
                        }
                    }
                }
                //Attack move on 9 tile move
                else if (currentCandidateOffset == 9 &&
                    !((BoardUtils.FIRST_COLUMN[this.piecePosition] && this.getPieceAlliance().isWhite()) ||
                    (BoardUtils.EIGHTH_COLUMN[this.piecePosition] && this.getPieceAlliance().isBlack())))
                {
                    if (board.getTile(candidateDestinationCoordinate).isTileOccupied())
                    {
                        Piece pieceOnCandidate = board.getTile(candidateDestinationCoordinate).getPiece();
                        if (this.pieceAlliance != pieceOnCandidate.getPieceAlliance())
                        {
                            //Check for pawn promotion
                            if (this.pieceAlliance.isPawnPromotionSquare(candidateDestinationCoordinate))
                            {
                                //if the pawn is eligible for promotion then promote it
                                legalMoves.Add(new PawnPromotion(new PawnAttackMove(board, this, candidateDestinationCoordinate, pieceOnCandidate)));
                            }
                            else
                            {
                                //Attack Move
                                legalMoves.Add(new PawnAttackMove(board, this, candidateDestinationCoordinate, pieceOnCandidate));
                            }
                        }
                    }
                    //En Passant Condition
                    else if (board.getEnPassantPawn() != null)
                    {
                        if (board.getEnPassantPawn().getPiecePosition() == this.piecePosition - (this.pieceAlliance.getOppositeDirection()))
                        {
                            Piece pieceOnCandidate = board.getEnPassantPawn();
                            if (this.pieceAlliance != pieceOnCandidate.getPieceAlliance())
                            {
                                legalMoves.Add(new PawnEnPassantAttackMove(board, this, candidateDestinationCoordinate, pieceOnCandidate));
                            }
                        }
                    }
                }
            }
            return legalMoves.ToList();
        }
        /// <summary>
        /// Overriden method for moving this piece
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public override Piece movePiece(Move move)
        {
            return new Pawn(move.getDestinationCoordinate(), move.getMovedPiece().getPieceAlliance());
        }
        /// <summary>
        /// ToString override for all pawns
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.pieceType.GetPieceType();
        }
        /// <summary>
        /// get hte promotion piece
        /// </summary>
        /// <returns></returns>
        public Piece getPromotionPiece()
        {
            return new Queen(this.piecePosition, this.pieceAlliance, false);
        }
    }
}

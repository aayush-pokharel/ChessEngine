using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.board;
using static Assets.Scripts.board.Move;

namespace Assets.Scripts.pieces
{
    //Create the bishop piece 
    public class Bishop : Piece
    {
        private readonly static int[] CANDIDATE_MOVE_COORDINATES =
        {
            -9, -7, 7, 9
        };
        public Bishop(int piecePosition, Alliance pieceAlliance, bool isFirstMove) :
            base(PieceType.BISHOP, piecePosition, pieceAlliance, isFirstMove)
        {

        }
        public Bishop(int piecePosition, Alliance pieceAlliance) :
            base(PieceType.BISHOP, piecePosition, pieceAlliance, true)
        {

        }
        public override int locationBonus()
        {
            return this.pieceAlliance.bishopBonus(this.piecePosition);
        }
        /// <summary>
        /// calculate all legal moves possible for the bishop
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public override ICollection<Move> calculateLegalMoves(Board board)
        {
            IList<Move> legalMoves = new List<Move>();
            foreach (int currentCandidateOffset in CANDIDATE_MOVE_COORDINATES)
            {
                int candidateDestinationCoordinate = this.piecePosition;
                while (BoardUtils.IsValidTileCoordinate(candidateDestinationCoordinate))
                {
                    //First or Last column exclusion break
                    if (IsFirstColumnExclusion(candidateDestinationCoordinate, currentCandidateOffset) ||
                        IsEighthColumnExclusion(candidateDestinationCoordinate, currentCandidateOffset))
                    {
                        break;
                    }
                    candidateDestinationCoordinate += currentCandidateOffset;
                    if (BoardUtils.IsValidTileCoordinate(candidateDestinationCoordinate))
                    {
                        //Set the tile as a valid candidate
                        Tile candidateDestinationTile = board.getTile(candidateDestinationCoordinate);

                        //If the candidate Tile is not Occupied
                        if (!candidateDestinationTile.isTileOccupied())
                        {
                            //Add move as legal
                            legalMoves.Add(new MajorMove(board, this, candidateDestinationCoordinate));
                        }
                        else
                        {
                            //Tile is occupied
                            Piece pieceAtDestination = candidateDestinationTile.getPiece();
                            Alliance pieceAlliance = pieceAtDestination.getPieceAlliance();
                            //If the piece alliance of the piece at the target tile
                            //is not the same as the piece alliace of this object...
                            if (this.pieceAlliance != pieceAlliance)
                            {
                                //Add the move into the list of legal moves
                                legalMoves.Add(new MajorAttackMove(board, this, candidateDestinationCoordinate, pieceAtDestination));
                            }
                            break;
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
            return new Bishop(move.getDestinationCoordinate(), move.getMovedPiece().getPieceAlliance());
        }
        public override string ToString()
        {
            return this.pieceType.GetPieceType();
        }
        /// <summary>
        /// First column exlclusion where move rules break down.
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="candidateOffset"></param>
        /// <returns></returns>
        private static bool IsFirstColumnExclusion(int currentPosition, int candidateOffset)
        {
            return BoardUtils.FIRST_COLUMN[currentPosition] && (candidateOffset == -9 || candidateOffset == 7);
        }
        /// <summary>
        /// Last column exlclusion where move rules break down.
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="candidateOffset"></param>
        /// <returns></returns>
        private static bool IsEighthColumnExclusion(int currentPosition, int candidateOffset)
        {
            return BoardUtils.EIGHTH_COLUMN[currentPosition] && (candidateOffset == -7 || candidateOffset == 9);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Assets.Scripts.board;
using UnityEngine;
using static Assets.Scripts.board.Move;

namespace Assets.Scripts.pieces
{
    public class Knight : Piece
    {
        /// <summary>
        /// Set the hard coded definition for a knight's movements
        /// </summary>
        private readonly static int[] CANDIDATE_MOVE_COORDINATES =
        {
            -17, -15, -10, -6, 6, 10, 15, 17
        };
        //Constructor
        public Knight(int piecePosition, Alliance pieceAlliance, bool isFirstMove) :
            base(PieceType.KNIGHT, piecePosition, pieceAlliance, isFirstMove)
        {

        }
        public Knight(int piecePosition, Alliance pieceAlliance) :
            base(PieceType.KNIGHT, piecePosition, pieceAlliance, true)
        {

        }
        public override int locationBonus()
        {
            return this.pieceAlliance.knightBonus(this.piecePosition);
        }
        /// <summary>
        /// Calculates the leagal moves for a knight and returns a list of
        /// all possible moves
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public override ICollection<Move> calculateLegalMoves(Board board)
        {
            int candidateDestinationCoordinate;
            IList<Move> legalMoves = new List<Move>();
            foreach(int currentCandidateOffset in CANDIDATE_MOVE_COORDINATES)
            {
                candidateDestinationCoordinate = this.piecePosition + currentCandidateOffset;
                //If the tile is a valid tile coordinate for this piece...
                if(BoardUtils.IsValidTileCoordinate(candidateDestinationCoordinate))
                {
                    //Check Column exclusions for the knight
                    if(isFirstColumnExclusion(this.piecePosition, currentCandidateOffset)||
                        isSecondColumExclusion(this.piecePosition, currentCandidateOffset) ||
                        isSeventhColumExclusion(this.piecePosition, currentCandidateOffset) ||
                        isEighthColumExclusion(this.piecePosition, currentCandidateOffset))
                    {
                        continue;
                    }
                    //Set the tile as a valid candidate
                    Tile candidateDestinationTile = board.getTile(candidateDestinationCoordinate);

                    //If the candidate Tile is not Occupied
                    if(!candidateDestinationTile.isTileOccupied())
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
                        if(this.pieceAlliance != pieceAlliance)
                        {
                            //Add the move into the list of legal moves
                            legalMoves.Add(new MajorAttackMove(board, this, candidateDestinationCoordinate, pieceAtDestination));
                        }
                    }
                }
            }
            //Return the calculated legal moves
            return legalMoves.ToList();
        }
        /// <summary>
        /// Overriden method for moving this piece
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public override Piece movePiece(Move move)
        {
            return new Knight(move.getDestinationCoordinate(), move.getMovedPiece().getPieceAlliance());
        }
        public override string ToString()
        {
            return this.pieceType.GetPieceType();
        }
        private static bool isFirstColumnExclusion(int currentPosition, int candidateOffset)
        {
            return BoardUtils.FIRST_COLUMN[currentPosition] && (candidateOffset == -17 || candidateOffset == -10 ||
                candidateOffset == 6 || candidateOffset == 15);
        }
        
        private static bool isSecondColumExclusion (int currentPosition, int candidateOffset)
        {
            return BoardUtils.SECOND_COLUMN[currentPosition] && (candidateOffset == -10 || candidateOffset == 6);
        }
        private static bool isSeventhColumExclusion(int currentPosition, int candidateOffset)
        {
            return BoardUtils.SEVENTH_COLUMN[currentPosition] && (candidateOffset == -6 || candidateOffset == 10);
        }
        private static bool isEighthColumExclusion(int currentPosition, int candidateOffset)
        {
            return BoardUtils.EIGHTH_COLUMN[currentPosition] && (candidateOffset == -15 || candidateOffset == -6 ||
                candidateOffset == 10 || candidateOffset == 17);
        }
    }
}

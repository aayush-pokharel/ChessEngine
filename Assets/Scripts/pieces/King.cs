using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.board;
using static Assets.Scripts.board.Move;

namespace Assets.Scripts.pieces
{
    public class King : Piece
    {
        private readonly static int[] CANDIDATE_MOVE_COORDINATES =
        {
            -9, -8, -7, -1, 1, 7, 8, 9
        };
        private readonly bool isCastled;
        private readonly bool kingSideCastleCapable;
        private readonly bool queenSideCastleCapable;
        public King(int piecePosition,
            Alliance pieceAlliance,
            bool isFirstMove,
            bool isCastled,
            bool kingSideCastleCapable,
            bool queenSideCastleCapable) :
            base(PieceType.KING, piecePosition, pieceAlliance, isFirstMove)
        {
            this.isCastled = isCastled;
            this.kingSideCastleCapable = kingSideCastleCapable;
            this.queenSideCastleCapable = queenSideCastleCapable;
        }
        public King(int piecePosition, 
            Alliance pieceAlliance,
            bool kingSideCastleCapable,
            bool queenSideCastleCapable) :
            base(PieceType.KING, piecePosition, pieceAlliance, true)
        {
            this.isCastled = false;
            this.kingSideCastleCapable = kingSideCastleCapable;
            this.queenSideCastleCapable = queenSideCastleCapable;
        }
        /// <summary>
        /// King is castled
        /// </summary>
        /// <returns></returns>
        public bool IsCastled()
        {
            return this.isCastled;
        }
        /// <summary>
        /// King is king side castle capable
        /// </summary>
        /// <returns></returns>
        public bool IsKingSideCastleCapable()
        {
            return this.kingSideCastleCapable;
        }
        /// <summary>
        /// King is queen side castle capable
        /// </summary>
        /// <returns></returns>
        public bool IsQueenSideCastleCapable()
        {
            return this.queenSideCastleCapable;
        }
        public override int locationBonus()
        {
            return this.pieceAlliance.kingBonus(this.piecePosition);
        }
        /// <summary>
        /// Caclulate the king's legal moves
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public override ICollection<Move> calculateLegalMoves(Board board)
        {
            IList<Move> legalMoves = new List<Move>();
            foreach (int currentCandidateOffset in CANDIDATE_MOVE_COORDINATES)
            {
                //Check for first column exclusion for the king
                if (isFirstColumnExclusion(this.piecePosition, currentCandidateOffset) ||
                    isEighthColumnExclusion(this.piecePosition, currentCandidateOffset))
                {
                    continue;
                }
                int candidateDestinationCoordinate = this.piecePosition + currentCandidateOffset;
                if(BoardUtils.IsValidTileCoordinate(candidateDestinationCoordinate))
                {
                    Tile candidateDestinationTile = board.getTile(candidateDestinationCoordinate);
                    //If the destination tile is not occupied
                    if (!candidateDestinationTile.isTileOccupied())
                    {
                        legalMoves.Add(new MajorMove(board, this, candidateDestinationCoordinate));
                    }
                    //If the destination tile is occupied
                    else
                    {
                        Piece pieceAtDestination = candidateDestinationTile.getPiece();
                        Alliance pieceAlliance = pieceAtDestination.getPieceAlliance();
                        if(this.pieceAlliance != pieceAlliance)
                        {
                            legalMoves.Add(new MajorAttackMove(board, this, candidateDestinationCoordinate, pieceAtDestination));
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
            return new King(move.getDestinationCoordinate(), move.getMovedPiece().getPieceAlliance(), false, move.isCastlingMove(), false, false);
        }
        public override string ToString()
        {
            return this.pieceType.GetPieceType();
        }
        private static bool isFirstColumnExclusion(int currentCandidate, int candidateDestinationCoordinate)
        {
            return BoardUtils.FIRST_COLUMN[currentCandidate]
                && ((candidateDestinationCoordinate == -9) || (candidateDestinationCoordinate == -1) ||
                (candidateDestinationCoordinate == 7));
        }
        private static bool isEighthColumnExclusion(int currentCandidate, int candidateDestinationCoordinate)
        {
            return BoardUtils.EIGHTH_COLUMN[currentCandidate]
                && ((candidateDestinationCoordinate == -7) || (candidateDestinationCoordinate == 1) ||
                (candidateDestinationCoordinate == 9));
        }
        
    }
}

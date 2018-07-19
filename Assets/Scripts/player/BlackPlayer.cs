using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.board;
using Assets.Scripts.pieces;
using static Assets.Scripts.board.Move.CastleMove;

namespace Assets.Scripts.player
{
    public class BlackPlayer : Player
    {
        public BlackPlayer(Board board, 
            IReadOnlyCollection<Move> whiteStandardLegalMoves, 
            IReadOnlyCollection<Move> blackStandardLegalMoves) :
            base(board, blackStandardLegalMoves, whiteStandardLegalMoves)
        {
            legalMoves.AddRange(calculateKingCastles(whiteStandardLegalMoves));
        }
        /// <summary>
        /// get all the black alliance pieces 
        /// </summary>
        /// <returns></returns>
        public override ICollection<Piece> getActivePieces()
        {
            return this.board.getBlackPieces();
        }
        /// <summary>
        /// get the black alliance
        /// </summary>
        /// <returns></returns>
        public override Alliance getAlliance()
        {
            return Alliance.BLACK;
        }
        /// <summary>
        /// return the white player for opponent
        /// </summary>
        /// <returns></returns>
        public override Player getOpponent()
        {
            return this.board.getWhitePlayer();
        }
        /// <summary>
        /// Caclulate the king castle move for the black player
        /// </summary>
        /// <param name="playerLegals"></param>
        /// <param name="opponentLegals"></param>
        /// <returns></returns>
        public override ICollection<Move> calculateKingCastles(IReadOnlyCollection<Move> opponentLegals)
        {
            List<Move> kingCastles = new List<Move>();
            if (this.playerKing.IsFirstMove() &&
                !this.isInCheck)
            {
                //Black king side castle
                if (!this.board.getTile(5).isTileOccupied() &&
                    !this.board.getTile(6).isTileOccupied())
                {
                    Tile rookTile = this.board.getTile(7);
                    //Check for tile occupation
                    if (rookTile.isTileOccupied() &&
                        rookTile.getPiece().IsFirstMove())
                    {
                        //Check for attacks on destination tiles
                        if (Player.calculateAttacksOnTile(5, opponentLegals.ToList()).Count == 0 &&
                            Player.calculateAttacksOnTile(6, opponentLegals.ToList()).Count == 0 &&
                            rookTile.getPiece().getPieceType().isRook())
                        {
                            kingCastles.Add(new KingSideCastleMove(this.board,
                                this.playerKing,
                                6,
                                (Rook)rookTile.getPiece(),
                                rookTile.getTileCoordinate(),
                                5));
                        }

                    }
                }
                //Black queen side castle
                if (!this.board.getTile(1).isTileOccupied() &&
                    !this.board.getTile(2).isTileOccupied() &&
                    !this.board.getTile(3).isTileOccupied())
                {
                    Tile rookTile = this.board.getTile(0);
                    //Check for tile occupation
                    if (rookTile.isTileOccupied() &&
                        rookTile.getPiece().IsFirstMove())
                    {
                        if (Player.calculateAttacksOnTile(2, opponentLegals.ToList()).Count == 0 &&
                            Player.calculateAttacksOnTile(3, opponentLegals.ToList()).Count == 0 &&
                            rookTile.getPiece().getPieceType().isRook())
                        {
                            kingCastles.Add(new QueenSideCastleMove(this.board,
                            this.playerKing,
                            2,
                            (Rook)rookTile.getPiece(),
                            rookTile.getTileCoordinate(),
                            3));
                        }  
                    }
                }
            }
            return kingCastles;
        }
    }
}

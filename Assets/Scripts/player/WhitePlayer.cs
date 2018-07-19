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
    public class WhitePlayer : Player
    {
        public WhitePlayer(Board board, 
            IReadOnlyCollection<Move> whiteStandardLegalMoves, 
            IReadOnlyCollection<Move> blackStandardLegalMoves) :
            base(board, whiteStandardLegalMoves, blackStandardLegalMoves)
        {
            legalMoves.AddRange(calculateKingCastles(blackStandardLegalMoves));
        }

        public override ICollection<Piece> getActivePieces()
        {
            return this.board.getWhitePieces();
        }

        public override Alliance getAlliance()
        {
            return Alliance.WHITE;
        }
        /// <summary>
        /// get the black player for opponent
        /// </summary>
        /// <returns></returns>
        public override Player getOpponent()
        {
            return this.board.getBlackPlayer();
        }
        /// <summary>
        /// calculate the King castle move for the white player
        /// </summary>
        /// <param name="playerLegals"></param>
        /// <param name="opponentLegals"></param>
        /// <returns></returns>
        public override ICollection<Move> calculateKingCastles(IReadOnlyCollection<Move> opponentLegals)
        {
            List<Move> kingCastles = new List<Move>();
            if(this.playerKing.IsFirstMove() &&
                !this.isInCheck)
            {
                //White king side castle
                if(!this.board.getTile(61).isTileOccupied() &&
                    !this.board.getTile(62).isTileOccupied())
                {
                    Tile rookTile = this.board.getTile(63);
                    //Check for tile occupation
                    if(rookTile.isTileOccupied() &&
                        rookTile.getPiece().IsFirstMove())
                    {
                        //Check for attacks on destination tiles
                        if(Player.calculateAttacksOnTile(61, opponentLegals.ToList()).Count == 0 &&
                            Player.calculateAttacksOnTile(62, opponentLegals.ToList()).Count == 0 &&
                            rookTile.getPiece().getPieceType().isRook())
                        {
                            kingCastles.Add(new KingSideCastleMove(this.board, 
                                this.playerKing, 
                                62, 
                                (Rook)rookTile.getPiece(), 
                                rookTile.getTileCoordinate(), 
                                61));
                        }
                        
                    }
                }
                //White queen side castle
                if (!this.board.getTile(59).isTileOccupied() &&
                    !this.board.getTile(58).isTileOccupied() &&
                    !this.board.getTile(57).isTileOccupied())
                {
                    Tile rookTile = this.board.getTile(56);
                    //Check for tile occupation
                    if (rookTile.isTileOccupied() &&
                        rookTile.getPiece().IsFirstMove())
                    {
                        if (Player.calculateAttacksOnTile(58, opponentLegals.ToList()).Count == 0 &&
                            Player.calculateAttacksOnTile(59, opponentLegals.ToList()).Count == 0 &&
                            rookTile.getPiece().getPieceType().isRook())
                        {
                            kingCastles.Add(new QueenSideCastleMove(this.board,
                            this.playerKing,
                            58,
                            (Rook)rookTile.getPiece(),
                            rookTile.getTileCoordinate(),
                            59));
                        }
                    }
                }
            }
            return kingCastles;
        }
    }
}

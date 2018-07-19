using Assets.Scripts.pieces;
using Assets.Scripts.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Assets.Scripts.board.Move;

namespace Assets.Scripts.board
{
    public class Board
    {
        private readonly List<Tile> gameBoard;
        private readonly IReadOnlyCollection<Piece> whitePieces;
        private readonly IReadOnlyCollection<Piece> blackPieces;

        private readonly WhitePlayer whitePlayer;
        private readonly BlackPlayer blackPlayer;
        private readonly Player currentPlayer;

        private readonly Pawn enPassantPawn;
        private readonly Move transitionMove;
        private Board(Builder builder)
        {
            //Create new gameboard with builder
            this.gameBoard = createGameBoard(builder);
            //white and black active pieces
            this.whitePieces = calculateActivePieces(this.gameBoard, Alliance.WHITE);
            this.blackPieces = calculateActivePieces(this.gameBoard, Alliance.BLACK);
            this.enPassantPawn = builder.getEnPassantPawn();
            //white and black initial legal moves calculation
            IReadOnlyCollection<Move> whiteStandardLegalMoves = calculateLegalMoves(this.whitePieces);
            IReadOnlyCollection<Move> blackStandardLegalMoves = calculateLegalMoves(this.blackPieces);
            //Player instantiation
            this.whitePlayer = new WhitePlayer(this, whiteStandardLegalMoves, blackStandardLegalMoves);
            this.blackPlayer = new BlackPlayer(this, whiteStandardLegalMoves, blackStandardLegalMoves);
            this.currentPlayer = builder.getNextMoveMaker().choosePlayer(this.whitePlayer, this.blackPlayer) ;
            this.transitionMove = builder.getMoveTransition() != null ? builder.getMoveTransition() : MoveFactory.getNullMove();
        }

        /// <summary>
        /// Print out the board
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < BoardUtils.NUM_TILES; i++)
            {
                String tileText = this.gameBoard[i].ToString();
                builder.Append(String.Format("{0, 3}", tileText));
                if((i + 1) % BoardUtils.NUM_TILES_PER_ROW == 0)
                {
                    builder.Append("\n");
                }
            }
            return builder.ToString();
        }
        /// <summary>
        /// get the white player
        /// </summary>
        /// <returns></returns>
        public Player getWhitePlayer()
        {
            return this.whitePlayer;
        }
        /// <summary>
        /// get the black player
        /// </summary>
        /// <returns></returns>
        public Player getBlackPlayer()
        {
            return this.blackPlayer;
        }
        public Pawn getEnPassantPawn()
        {
            return this.enPassantPawn;
        }
        public ICollection<Piece> getBlackPieces()
        {
            return this.blackPieces.ToList();
        }

        public ICollection<Piece> getWhitePieces()
        {
            return this.whitePieces.ToList();
        }
        public Move getTransitionMove()
        {
            return this.transitionMove;
        }
        /// <summary>
        /// Calculates the legal moves for all the pieces proveided in the list
        /// </summary>
        /// <param name="pieces"></param>
        /// <returns></returns>
        private IReadOnlyCollection<Move> calculateLegalMoves(IReadOnlyCollection<Piece> pieces)
        {
            IList<Move> legalMoves = new List<Move>();
            foreach(Piece piece in pieces)
            {
                ((List<Move>)legalMoves).AddRange(piece.calculateLegalMoves(this));
            }
            return legalMoves.ToList();
        }
        /// <summary>
        /// Get tje list of current active pieces
        /// </summary>
        /// <param name="gameBoard"></param>
        /// <param name="alliance"></param>
        /// <returns></returns>
        private static IReadOnlyCollection<Piece> calculateActivePieces(List<Tile> gameBoard, Alliance alliance)
        {
            IList<Piece> activePieces = new List<Piece>();
            foreach(Tile tile in gameBoard)
            {
                if(tile.isTileOccupied())
                {
                    Piece piece = tile.getPiece();
                    if(piece.getPieceAlliance() == alliance)
                    {
                        activePieces.Add(piece);
                    }
                }
            }
            return activePieces.ToList();
        }

        public Player getCurrentPlayer()
        {
            return currentPlayer;
        }

        /// <summary>
        /// Creates the Game Board
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static List<Tile> createGameBoard(Builder builder)
        {
            Tile[] tiles = new Tile[BoardUtils.NUM_TILES];
            for(int i = 0; i < BoardUtils.NUM_TILES; i++)
            {
                tiles[i] = Tile.createTile(i, builder.getPieceAtTile(i));
            }
            return tiles.ToList();
        }

        /// <summary>
        /// Creates the intial standard starting board
        /// </summary>
        /// <returns></returns>
        public static Board createStandardBoard()
        {
            Builder builder = new Builder();
            //Black Alliance
            int BlackPiecePostion = 0;
            builder.setPiece(new Rook(BlackPiecePostion, Alliance.BLACK));
            builder.setPiece(new Knight(++BlackPiecePostion, Alliance.BLACK));
            builder.setPiece(new Bishop(++BlackPiecePostion, Alliance.BLACK));
            builder.setPiece(new Queen(++BlackPiecePostion, Alliance.BLACK));
            builder.setPiece(new King(++BlackPiecePostion, Alliance.BLACK, true, true));
            builder.setPiece(new Bishop(++BlackPiecePostion, Alliance.BLACK));
            builder.setPiece(new Knight(++BlackPiecePostion, Alliance.BLACK));
            builder.setPiece(new Rook(++BlackPiecePostion, Alliance.BLACK));
            for(int i = 0; i < 8; i++)
            {
                builder.setPiece(new Pawn(++BlackPiecePostion, Alliance.BLACK));
            }
            //White Alliance
            int WhitePiecePostion = 47;
            for (int i = 0; i < 8; i++)
            {
                builder.setPiece(new Pawn(++WhitePiecePostion, Alliance.WHITE));
            }
            builder.setPiece(new Rook(++WhitePiecePostion, Alliance.WHITE));
            builder.setPiece(new Knight(++WhitePiecePostion, Alliance.WHITE));
            builder.setPiece(new Bishop(++WhitePiecePostion, Alliance.WHITE));
            builder.setPiece(new Queen(++WhitePiecePostion, Alliance.WHITE));
            builder.setPiece(new King(++WhitePiecePostion, Alliance.WHITE, true, true));
            builder.setPiece(new Bishop(++WhitePiecePostion, Alliance.WHITE));
            builder.setPiece(new Knight(++WhitePiecePostion, Alliance.WHITE));
            builder.setPiece(new Rook(++WhitePiecePostion, Alliance.WHITE));
            builder.setMoveMaker(Alliance.WHITE);
            return builder.build();
        }

        public  IEnumerable<Move> getAllLegalMoves()
        {
            return Enumerable.Concat(this.whitePlayer.getLegalMoves(), this.blackPlayer.getLegalMoves());
        }

        public Tile getTile(int tileCoordinate)
        {
            return gameBoard[tileCoordinate];
        }

        public class Builder
        {
            //global declarations
            private Dictionary<int, Piece> boardConfig;
            private Alliance nextMoveMaker;
            private Pawn enPassantPawn;
            Move transitionMove;

            public Builder()
            {
                this.boardConfig = new Dictionary<int, Piece>();
            }
            /// <summary>
            /// Set the piece to the board configuration
            /// </summary>
            /// <param name="piece"></param>
            /// <returns></returns>
            public Builder setPiece(Piece piece)
            {
                if(boardConfig.ContainsKey(piece.getPiecePosition()))
                {
                    boardConfig.Remove(piece.getPiecePosition());
                }
                this.boardConfig.Add(piece.getPiecePosition(), piece);
                return this;
            }
            /// <summary>
            /// Set the next move
            /// </summary>
            /// <param name="nextMoveMaker"></param>
            /// <returns></returns>
            public Builder setMoveMaker(Alliance nextMoveMaker)
            {
                this.nextMoveMaker = nextMoveMaker;
                return this;
            }
            public Alliance getNextMoveMaker()
            {
                return this.nextMoveMaker;
            }
            public Board build()
            {
                return new Board(this);

            }
            public Builder setMoveTransition(Move transitionMove)
            {
                this.transitionMove = transitionMove;
                return this;
            }
            public Move getMoveTransition()
            {
                return this.transitionMove;
            }
            public Piece getPieceAtTile(int tilePosition)
            {
                if(boardConfig.ContainsKey(tilePosition))
                {
                    return boardConfig[tilePosition];
                }
                else
                {
                    return null;
                }
            }
            public void setEnPassantPawn(Pawn enPassantPawn)
            {
                this.enPassantPawn = enPassantPawn;
            }
            public Pawn getEnPassantPawn()
            {
                return this.enPassantPawn;
            }
        }
    }
}

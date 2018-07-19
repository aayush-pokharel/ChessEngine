using Assets.Scripts.board;
using Assets.Scripts.pieces;
using Assets.Scripts.player;
using Assets.Scripts.player.ai;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.gui
{
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance { get; set; }
        private bool[,] allowedMoves { set; get; }
        //constant variables
        private const float TILE_SIZE = 10f;
        private const float TILE_OFFSET = 5f;
        private const int boardHeight = 7;
        private const int scale = 10;

        //Selection variables
        private int selectionX = -1;
        private int selectionY = -1;
        private Board board;
        private MoveLog moveLog;
        //Chess Pieces
        public List<GameObject> chessPiecePrefabs;
        private List<GameObject> activeChessPieces;
        private ChessPiece selectedChessPiece;

        private Tile sourceTile;
        private Tile destinationTile;
        private Piece movedPiece;
        private Move computerMove;
        Thread myThread;
        List<Action> functionsToRunInMainThread;
        private Job aiMove;

        private readonly GameSetup gameSetup;
        private ChessPiece[,] chessPieces { set; get; }

        //private readonly Vector2Int[] tileToCoordinate = new Vector2Int[]
        //{
        //    new Vector2Int(0,0)
        //};
        public BoardManager()
        {
            sourceTile = null;
            destinationTile = null;
            movedPiece = null;
            this.gameSetup = new GameSetup();
        }
        /// <summary>
        /// Start the game
        /// </summary>
        private void Start()
        {
            Instance = this;
            functionsToRunInMainThread = new List<Action>();
            SpawnAllChessPieces();
            this.board = Board.createStandardBoard();
            this.moveLog = new MoveLog();
            Debug.Log(board);
        }
        /// <summary>
        /// Update the board
        /// </summary>
        private void Update()
        {
            UpdateSelection();
            DrawChessBoard();
            //Left Mouse click
            if(Input.GetMouseButtonDown(0))
            {
                //Check if click was on the board
                if(selectionX >=0 && selectionY >= 0)
                {
                    int tileCoordinate = (8 * selectionY) + selectionX;
                    //Debug.Log(tileCoordinate + " -> (" + selectionX + ", " + selectionY + ")");
                    if (selectedChessPiece == null)
                    {
                        selectChessPiece(tileCoordinate, selectionX, selectionY);
                    }
                    else
                    {
                        moveChessPiece(tileCoordinate, selectionX, selectionY);
                    }
                }
            }
            if(Input.GetMouseButtonDown(1))
            {
                selectedChessPiece = null;
                destinationTile = null;
            }
            while (functionsToRunInMainThread.Count > 0)
            {
                Action someFunc = functionsToRunInMainThread[0];
                functionsToRunInMainThread.RemoveAt(0);
                someFunc();
            }
        }
        
        public Board getGameBoard()
        {
            return board;
        }
        /// <summary>
        /// Get the selected chess piece
        /// </summary>
        /// <param name="tileCoordinate"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void selectChessPiece(int tileCoordinate, int x, int y)
        {
            if (!board.getTile(tileCoordinate).isTileOccupied())
            {
                return;
            }
            selectedChessPiece = chessPieces[x, y];
            sourceTile = board.getTile(tileCoordinate);
            movedPiece = sourceTile.getPiece();
            if (movedPiece == null)
            {
                sourceTile = null;
            }
            highlightLegals();
           
        }
        /// <summary>
        /// highlight the legal moves for the selected piece
        /// </summary>
        private void highlightLegals()
        {
            bool[,] legalMoves = new bool[8, 8];
            foreach(Move move in pieceLgalMoves())
            {
                int tileNumber = move.getDestinationCoordinate();
                var pair = GetXAndY(tileNumber);
                legalMoves[pair.Key, pair.Value] = true;
            }
            allowedMoves = legalMoves;
            BoardHighlights.Instance.highlightAllowedMoves(allowedMoves);
        }
        /// <summary>
        /// transforms the game engine's tile coordinate to a coordinate of two components:
        /// x and y
        /// </summary>
        /// <param name="tileCoordinate"></param>
        /// <returns></returns>
        public static KeyValuePair<int, int> GetXAndY(int tileCoordinate)
        {
            int x = -1;
            int y = -1;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (((i * 8) + j) == tileCoordinate)
                    {
                        y = i;
                        x = j;
                        break;
                    }
                }
                if (x >= 0 && y >= 0)
                {
                    break;
                }
            }
            return new KeyValuePair<int, int>(x, y);
        }
        /// <summary>
        /// gets all legal moves for the current board state
        /// </summary>
        /// <returns></returns>
        private ICollection<Move> pieceLgalMoves()
        {
            List<Move> legalMoves = new List<Move>();
            if (movedPiece != null && movedPiece.getPieceAlliance() == board.getCurrentPlayer().getAlliance())
            {
                legalMoves.AddRange(movedPiece.calculateLegalMoves(board));
            }
            if(movedPiece.getPieceType() == PieceType.KING)
            {
                legalMoves.AddRange(board.getCurrentPlayer().calculateKingCastles(board.getCurrentPlayer().getOpponent().getLegalMoves()));
            }
            return legalMoves; 
        }
        /// <summary>
        /// move the selected chess piece
        /// </summary>
        /// <param name="tileCoordinate"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void moveChessPiece(int tileCoordinate, int x, int y)
        {
            if (sourceTile == null)
                return;

            destinationTile = board.getTile(tileCoordinate);
            Move move = Move.MoveFactory.createMove(board, 
                sourceTile.getTileCoordinate(), 
                destinationTile.getTileCoordinate());
            MoveTransition transition = board.getCurrentPlayer().makeMove(move);
            if(transition.getMoveStatus().isDone())
            {
                board = transition.getBoard();
                moveLog.addMove(move);
                //Add the move to the move log
                Debug.Log(moveLog.getMoves()[moveLog.getMoves().Count - 1]);
                Debug.Log(board);

                ChessPiece opponentPiece;
                //Check for EnPasssant
                if (move.isEnPassantMove())
                {
                    int attackedPieceCoordinate = move.getAttackedPiece().getPiecePosition();
                    var pair = GetXAndY(attackedPieceCoordinate);
                    opponentPiece = chessPieces[pair.Key, pair.Value];
                }
                else
                {
                    opponentPiece = chessPieces[x, y];
                }
                //Destroy the captured piece
                if(opponentPiece !=null)
                {
                    activeChessPieces.Remove(opponentPiece.gameObject);
                    Destroy(opponentPiece.gameObject);
                }

                //Check for pawn promotion
                if (move.isPawnPromotion())
                {
                    //Exchange the pawn piece for a queen piece
                    activeChessPieces.Remove(selectedChessPiece.gameObject);
                    Destroy(selectedChessPiece.gameObject);
                    if (move.getMovedPiece().getPieceAlliance().isWhite())
                    {
                        //White Queen
                        SpawnChessPiece(7, x, y);
                        selectedChessPiece = chessPieces[x, y];
                    }
                    else
                    {
                        //Black Queen
                        SpawnChessPiece(1, x, y);
                        selectedChessPiece = chessPieces[x, y];
                    }
                }
                chessPieces[selectedChessPiece.CurrentX, selectedChessPiece.CurrentY] = null;
                selectedChessPiece.transform.position = GetTileCenter(x, y);
                selectedChessPiece.SetPosition(x, y);
                chessPieces[x, y] = selectedChessPiece;
                //Check for king castle
                if (move.isCastlingMove())
                {
                    //If this is a castling move then move the rook to the destination coordinate as well
                    Move.CastleMove castleMove = (Move.CastleMove)move;
                    int rookSource = castleMove.getCastleRookStart();
                    int rookDestination = castleMove.getCastleRookDestination();
                    var rookStart = GetXAndY(rookSource);
                    var rookEnd = GetXAndY(rookDestination);
                    selectedChessPiece = chessPieces[rookStart.Key, rookStart.Value];
                    chessPieces[selectedChessPiece.CurrentX, selectedChessPiece.CurrentY] = null;
                    selectedChessPiece.transform.position = GetTileCenter(rookEnd.Key, rookEnd.Value);
                    selectedChessPiece.SetPosition(rookEnd.Key, rookEnd.Value);
                    chessPieces[rookEnd.Key, rookEnd.Value] = selectedChessPiece;
                }
                if (board.getCurrentPlayer().isCheckMate())
                {
                    EndGame();
                    return;
                }
                if (gameSetup.isAIPlayer(getGameBoard().getCurrentPlayer()) &&
                !getGameBoard().getCurrentPlayer().isCheckMate() &&
                !getGameBoard().getCurrentPlayer().isStaleMate())
                {
                    aiMove = new Job(board);
                    aiMove.Start();
                    StartCoroutine(aiCoroutine());
                    //StartThreadedFunction(() => { AiMove(board); });
                    //nonThreadAiMove(board);
                }
                //moveMadeUpdate(PlayerType.HUMAN);
            }
            BoardHighlights.Instance.hideHighlights();
            sourceTile = null;
            destinationTile = null;
            movedPiece = null;
            selectedChessPiece = null;
        }
        IEnumerator aiCoroutine()
        {
            yield return StartCoroutine(aiMove.WaitFor());
        }
        private void updateGameBoard(Board board)
        {
            this.board = board;
        }
        private void updateComputerMove(Move move)
        {
            this.computerMove = move;
        }
        private MoveLog getMoveLog()
        {
            return this.moveLog;
        }
        public void StartThreadedFunction(Action somefunction)
        {
            Thread t = new Thread(new ThreadStart(somefunction));
            t.Start();
        }
        public void QueueMainThreadFunction(Action someFunction)
        {
            functionsToRunInMainThread.Add(someFunction);
        }
        void AiMove(Board board)
        {
            IMoveStrategy minimax = new AlphaBeta(4);
            Move bestMove = minimax.execute(board);
            Action aFunction = () =>
            {
                Debug.Log(minimax.getMessage());
                //BoardManager.Instance.moveMadeUpdate(PlayerType.COMPUTER);
                var sourceCoordinate = GetXAndY(bestMove.getCurrentCoordinate());
                var destinationCoordinate = GetXAndY(bestMove.getDestinationCoordinate());
                selectedChessPiece = chessPieces[sourceCoordinate.Key, sourceCoordinate.Value];
                sourceTile = board.getTile(bestMove.getCurrentCoordinate());
                movedPiece = sourceTile.getPiece();
                if (movedPiece == null)
                {
                    sourceTile = null;
                }
                int tileCoordinate = bestMove.getDestinationCoordinate();
                selectionX = destinationCoordinate.Key;
                selectionY = destinationCoordinate.Value;
                moveChessPiece(tileCoordinate, selectionX, selectionY);
            };
            QueueMainThreadFunction(aFunction);
            
        }
        public void ThreadMove(Move bestMove)
        {
            //BoardManager.Instance.moveMadeUpdate(PlayerType.COMPUTER);
            var sourceCoordinate = GetXAndY(bestMove.getCurrentCoordinate());
            var destinationCoordinate = GetXAndY(bestMove.getDestinationCoordinate());
            selectedChessPiece = chessPieces[sourceCoordinate.Key, sourceCoordinate.Value];
            sourceTile = board.getTile(bestMove.getCurrentCoordinate());
            movedPiece = sourceTile.getPiece();
            if (movedPiece == null)
            {
                sourceTile = null;
            }
            int tileCoordinate = bestMove.getDestinationCoordinate();
            selectionX = destinationCoordinate.Key;
            selectionY = destinationCoordinate.Value;
            moveChessPiece(tileCoordinate, selectionX, selectionY);
        }
        /// <summary>
        /// Update tile selection based on mouse position
        /// </summary>
        private void UpdateSelection()
        {
            if (!Camera.main)
            {
                return;
            }
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                out hit, 250.0f, LayerMask.GetMask("ChessPlane")))
            {
                int x = (int)hit.point.x;
                int y = (int)hit.point.z;
                selectionX = (x / 10);
                selectionY = Mathf.Abs((y / 10) - 7);
            }
            else
            {
                selectionX = -1;
                selectionY = -1;
            }
        }
        /// <summary>
        /// Spawn a chess piece on to the board
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        private void SpawnChessPiece(int index, int x, int y)
        {
            Vector3 origin = GetTileCenter(x, y);
            GameObject go = Instantiate(chessPiecePrefabs[index], origin, Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            chessPieces[x, y] = go.GetComponent<ChessPiece>();
            chessPieces[x, y].SetPosition(x, y);
            activeChessPieces.Add(go);
            
        }
        /// <summary>
        /// Spawn all chess pieces during the start of the game.
        /// </summary>
        private void SpawnAllChessPieces()
        {
            activeChessPieces = new List<GameObject>();
            chessPieces = new ChessPiece[8, 8];
            //Spawn the black pieces
            //King
            SpawnChessPiece(0, 4, 0);
            //Queen
            SpawnChessPiece(1, 3, 0);
            //Rook
            SpawnChessPiece(2, 0, 0);
            SpawnChessPiece(2, 7, 0);
            //Bishop
            SpawnChessPiece(3, 2, 0);
            SpawnChessPiece(3, 5, 0);
            //Knight
            SpawnChessPiece(4, 1, 0);
            SpawnChessPiece(4, 6, 0);
            //Pawns
            for (int i = 0; i < 8; i++)
            {
                SpawnChessPiece(5, i, 1);
            }

            //Spawn the white pieces
            //King
            SpawnChessPiece(6, 4, 7);
            //Queen
            SpawnChessPiece(7, 3, 7);
            //Rook
            SpawnChessPiece(8, 0, 7);
            SpawnChessPiece(8, 7, 7);
            //Bishop
            SpawnChessPiece(9, 2, 7);
            SpawnChessPiece(9, 5, 7);
            //Knight
            SpawnChessPiece(10, 1, 7);
            SpawnChessPiece(10, 6, 7);
            //Pawns
            for (int i = 0; i < 8; i++)
            {
                SpawnChessPiece(11, i, 6);
            }
        }
        public Vector3 GetTileCenter(int x, int y)
        {
            Vector3 startPoint = Vector3.zero;
            startPoint.x += TILE_OFFSET;
            startPoint.y += boardHeight;
            startPoint.z += (TILE_SIZE * 7) + TILE_OFFSET;
            Vector3 origin = startPoint;
            origin.x += (TILE_SIZE * x);
            //origin.y += boardHeight;
            origin.z -= (TILE_SIZE * y);
            return origin;
        }
        /// <summary>
        /// Draw the vector grid of a chess board and draw the selections
        /// </summary>
        private void DrawChessBoard()
        {
            //Create the 8x8 tiles of the chess board
            Vector3 widthLine = Vector3.right * 8 * scale;
            Vector3 heightLine = Vector3.forward * 8 * scale;

            //Draw the chess board
            for (int i = 0; i <= 8; i++)
            {
                Vector3 start = Vector3.forward * i * scale;
                start.y += boardHeight;
                Debug.DrawLine(start, start + widthLine);
                for (int j = 0; j <= BoardUtils.NUM_TILES_PER_ROW; j++)
                {
                    start = Vector3.right * j * scale;
                    start.y += boardHeight;
                    Debug.DrawLine(start, start + heightLine);
                }
            }

            //Draw the selection tile
            if (selectionX >= 0 && selectionY >= 0)
            {
                Vector3 start;
                Vector3 end;
                start = Vector3.forward * selectionY + Vector3.right * selectionX;
                start.y += boardHeight;
                end = Vector3.forward * (selectionY + scale) + Vector3.right * (selectionX + scale);
                end.y += boardHeight;
                Debug.DrawLine(start, end);
                start = Vector3.forward * (selectionY + scale) + Vector3.right * selectionX;
                start.y += boardHeight;
                end = Vector3.forward * selectionY + Vector3.right * (selectionX + scale);
                end.y += boardHeight;
                Debug.DrawLine(start, end);
            }

        }
        private void EndGame()
        {
            if(board.getCurrentPlayer().getAlliance().isWhite())
            {
                Debug.Log("Black Team Wins!");
            }
            else
            {
                Debug.Log("White Team Wins!");
            }
        }

    }
    public class MoveLog
    {
        private readonly List<Move> moves;
        public MoveLog()
        {
            this.moves = new List<Move>();
        }
        public List<Move> getMoves()
        {
            return this.moves;
        }
        public void addMove(Move move)
        {
            this.moves.Add(move);
        }
        public int size()
        {
            return this.moves.Count;
        }
        public void clear()
        {
            this.moves.Clear();
        }
        public void removeMove(int index)
        {
            this.moves.RemoveAt(index);
        }
        public bool removeMove(Move move)
        {
            return this.moves.Remove(move);
        }
    }
}

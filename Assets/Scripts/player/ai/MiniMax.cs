using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.board;
using static Assets.Scripts.board.Move;

namespace Assets.Scripts.player.ai
{
    public class MiniMax : IMoveStrategy
    {
        private readonly IBoardEvaluator boardEvaluator;
        private readonly int searchDepth;
        private string message;
        public MiniMax(int searchDepth)
        {
            this.boardEvaluator = new StandardBoardEvaluator();
            this.searchDepth = searchDepth;
            message = "";
        }
        public override string ToString()
        {
            return "MiniMax";
        }
        private static bool isEndGameScenario(Board board)
        {
            return board.getCurrentPlayer().isCheckMate() || board.getCurrentPlayer().isStaleMate();
        }
        public string getMessage()
        {
            return message;
        }
        /// <summary>
        /// execute the best move based on minimax algorithm
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public Move execute(Board board)
        {
            long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Move bestMove = MoveFactory.getNullMove();
            int largestSeenValue = Int32.MinValue;
            int lowestSeenValue = Int32.MaxValue;
            int currentValue;
            message = String.Concat(board.getCurrentPlayer() + " Thinking with depth = " + searchDepth);
            int numMoves = board.getCurrentPlayer().getLegalMoves().Count;
            foreach(Move move in board.getCurrentPlayer().getLegalMoves())
            {
                MoveTransition moveTransition = board.getCurrentPlayer().makeMove(move);
                if(moveTransition.getMoveStatus().isDone())
                {
                    currentValue = board.getCurrentPlayer().getAlliance().isWhite() ?
                        min(moveTransition.getBoard(), searchDepth - 1) :
                        max(moveTransition.getBoard(), searchDepth - 1);

                    if(board.getCurrentPlayer().getAlliance().isWhite() && currentValue >= largestSeenValue)
                    {
                        largestSeenValue = currentValue;
                        bestMove = move;
                    }
                    else if(board.getCurrentPlayer().getAlliance().isBlack())
                    {
                        lowestSeenValue = currentValue;
                        bestMove = move;
                    }
                }
            }
            long executionTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
            return bestMove;
        }
        /// <summary>
        /// minimization process of all possible moves within the
        /// min level of the minimax tree
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public int min(Board board, int depth)
        {
            if(depth == 0 || isEndGameScenario(board))
            {
                return this.boardEvaluator.evaluate(board, depth);
            }
            int lowestSeenValue = Int32.MaxValue;
            foreach(Move move in board.getCurrentPlayer().getLegalMoves())
            {
                MoveTransition moveTransition = board.getCurrentPlayer().makeMove(move);
                if(moveTransition.getMoveStatus().isDone())
                {
                    int currentValue = max(moveTransition.getBoard(), depth - 1);
                    if(currentValue <= lowestSeenValue)
                    {
                        lowestSeenValue = currentValue;
                    }
                }
            }
            return lowestSeenValue;
        }

        /// <summary>
        /// maximization process of all possible moves within the
        /// max level of the minimax tree 
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public int max(Board board, int depth)
        {
            if (depth == 0 || isEndGameScenario(board))
            {
                return this.boardEvaluator.evaluate(board, depth);
            }
            int largestSeenValue = Int32.MinValue;
            foreach (Move move in board.getCurrentPlayer().getLegalMoves())
            {
                MoveTransition moveTransition = board.getCurrentPlayer().makeMove(move);
                if (moveTransition.getMoveStatus().isDone())
                {
                    int currentValue = min(moveTransition.getBoard(), depth - 1);
                    if (currentValue >= largestSeenValue)
                    {
                        largestSeenValue = currentValue;
                    }
                }
            }
            return largestSeenValue;
        }
    }
}

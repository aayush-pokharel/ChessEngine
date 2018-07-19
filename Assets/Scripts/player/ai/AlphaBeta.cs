using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.board;
using static Assets.Scripts.board.Move;

namespace Assets.Scripts.player.ai
{
    public class AlphaBeta : IMoveStrategy
    {
        private readonly IBoardEvaluator boardEvaluator;
        private readonly int searchDepth;
        private string message;
        private long boardsEvaluated;
        private long executionTime;
        private int quiescenceCount;
        private int cutOffsProduced;
        private static readonly int MAX_QUIESCENCE = 5000 * 10;

        public AlphaBeta(int searchDepth)
        {
            this.boardEvaluator = new StandardBoardEvaluator();
            this.searchDepth = searchDepth;
            message = "";
            this.boardsEvaluated = 0;
            this.quiescenceCount = 0;
            this.cutOffsProduced = 0;
        }
        
        public override string ToString()
        {
            return "AlphaBeta";
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
            List<Move> sortedMoves = MoveSorter.EXPENSIVE.Sort(board.getCurrentPlayer().getLegalMoves().ToList()).ToList();
            foreach (Move move in sortedMoves)
            {
                MoveTransition moveTransition = board.getCurrentPlayer().makeMove(move);
                this.quiescenceCount = 0;
                if (moveTransition.getMoveStatus().isDone())
                {
                    currentValue = board.getCurrentPlayer().getAlliance().isWhite() ?
                        min(moveTransition.getBoard(), searchDepth - 1, largestSeenValue, lowestSeenValue) :
                        max(moveTransition.getBoard(), searchDepth - 1, largestSeenValue, lowestSeenValue);

                    if (board.getCurrentPlayer().getAlliance().isWhite() && currentValue > largestSeenValue)
                    {
                        largestSeenValue = currentValue;
                        bestMove = move;
                    }
                    else if (board.getCurrentPlayer().getAlliance().isBlack() && currentValue < lowestSeenValue)
                    {
                        lowestSeenValue = currentValue;
                        bestMove = move;
                        if (moveTransition.getBoard().getWhitePlayer().isCheckMate())
                        {
                            break;
                        }
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
        public int min(Board board, int depth, int largest, int lowest)
        {
            if (depth == 0 || isEndGameScenario(board))
            {
                this.boardsEvaluated++;
                return this.boardEvaluator.evaluate(board, depth);
            }
            int currentLowest = lowest;
            foreach (Move move in MoveSorter.STANDARD.Sort(board.getCurrentPlayer().getLegalMoves().ToList()))
            {
                MoveTransition moveTransition = board.getCurrentPlayer().makeMove(move);
                if (moveTransition.getMoveStatus().isDone())
                {
                    currentLowest = Math.Min(currentLowest, max(moveTransition.getBoard(),
                        calculateQuiescenceDepth(moveTransition.getBoard(), depth), largest, currentLowest));
                    if (currentLowest <= largest)
                    {
                        this.cutOffsProduced++;
                        break;
                    }
                }
            }
            return currentLowest;
        }

        /// <summary>
        /// maximization process of all possible moves within the
        /// max level of the minimax tree 
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public int max(Board board, int depth, int largest, int lowest)
        {
            if (depth == 0 || isEndGameScenario(board))
            {
                this.boardsEvaluated++;
                return this.boardEvaluator.evaluate(board, depth);
            }
            int currentHighest = largest;
            foreach (Move move in MoveSorter.STANDARD.Sort(board.getCurrentPlayer().getLegalMoves().ToList()))
            {
                MoveTransition moveTransition = board.getCurrentPlayer().makeMove(move);
                if (moveTransition.getMoveStatus().isDone())
                {
                    currentHighest = Math.Max(currentHighest, min(moveTransition.getBoard(),
                        calculateQuiescenceDepth(moveTransition.getBoard(), depth), currentHighest, lowest));
                    if (currentHighest >= lowest)
                    {
                        return lowest;
                    }
                }
            }
            return currentHighest;
        }
        private int calculateQuiescenceDepth( Board board,
                                         int depth)
        {
            if (depth == 1 && this.quiescenceCount < MAX_QUIESCENCE)
            {
                int activityMeasure = 0;
                if (board.getCurrentPlayer().isCheck())
                {
                    activityMeasure += 1;
                }
                foreach ( Move move in BoardUtils.lastNMoves(board, 2))
                {
                    if (move.isAttack())
                    {
                        activityMeasure += 1;
                    }
                }
                if (activityMeasure >= 2)
                {
                    this.quiescenceCount++;
                    return 1;
                }
            }
            return depth - 1;
        }

    }
    
}

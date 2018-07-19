using Assets.Scripts.board;
using Assets.Scripts.gui;
using Assets.Scripts.player.ai;
using UnityEngine;

public class Job : ThreadedJob
{
    private Board board;
    private IMoveStrategy moveStrategy;
    private Move bestMove;
    public Job(Board board)
    {
        this.board = board;
    }
    protected override void ThreadFunction()
    {
        // Do your threaded task. DON'T use the Unity API here
        moveStrategy = new AlphaBeta(4);
        bestMove = moveStrategy.execute(board);
    }
    protected override void OnFinished()
    {
        // This is executed by the Unity main thread when the job is finished
        Debug.Log(moveStrategy.getMessage());
        BoardManager.Instance.ThreadMove(bestMove);
    }
}
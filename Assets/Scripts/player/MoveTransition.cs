using System;
using Assets.Scripts.board;
using UnityEngine;

namespace Assets.Scripts.player
{
    public class MoveTransition
    {
        private readonly Board transitionBoard;
        private readonly Move move;
        private readonly MoveStatus moveStatus;

        public MoveTransition(Board transitionBoard,
            Move move,
            MoveStatus moveStatus)
        {
            this.transitionBoard = transitionBoard;
            this.move = move;
            this.moveStatus = moveStatus;
        }
        public MoveStatus getMoveStatus()
        {
            return this.moveStatus;
        }

        public Board getBoard()
        {
            return this.transitionBoard;
        }
    }
}
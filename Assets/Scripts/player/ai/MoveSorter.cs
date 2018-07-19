using Assets.Scripts.board;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.player.ai
{
    public enum MoveSorter
    {
        STANDARD,
        EXPENSIVE
    }
    public static class SorterMethods
    {
        public static ICollection<Move> Sort(this MoveSorter sorter, ICollection<Move> moves)
        {
            List<Move> sortedMoves = new List<Move>();
            sortedMoves.AddRange(moves);
            switch (sorter)
            {
                case MoveSorter.STANDARD:
                    sortedMoves = sortedMoves.OrderBy(x => x.isCastlingMove(), new BoolComparer()).
                        ThenBy(x => x, new MoveComparer()).ToList();
                    return sortedMoves;
                case MoveSorter.EXPENSIVE:
                    sortedMoves = sortedMoves.OrderBy(x => BoardUtils.kingThreat(x), new BoolComparer()).
                        ThenBy(x => x.isCastlingMove(), new BoolComparer()).
                        ThenBy(x => x, new MoveComparer()).ToList();
                    return sortedMoves;
                default:
                    return sortedMoves;
            }
        }
    }
    class MoveComparer : IComparer<Move>
    {
        public int Compare(Move m1, Move m2)
        {
            int a = BoardUtils.mvvlva(m1);
            int b = BoardUtils.mvvlva(m2);
            if (a == b)
            {
                return 0;
            }
            else if (a > b)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
    class BoolComparer : IComparer<bool>
    {
        public int Compare(bool x, bool y)
        {
            if (x == y)
            {
                return 0;
            }
            else if (!x && y)
            {
                return 1;
            }
            else if(!x && !y)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}

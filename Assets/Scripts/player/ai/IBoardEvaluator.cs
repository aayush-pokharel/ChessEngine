using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.board;
namespace Assets.Scripts.player.ai
{
    interface IBoardEvaluator
    {
        int evaluate(Board board, int depth);
    }
}

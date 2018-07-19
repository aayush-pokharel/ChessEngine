using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.board;
namespace Assets.Scripts.player.ai
{
    public interface IMoveStrategy
    {
        Move execute(Board board);
        string getMessage();
    }
}

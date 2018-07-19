using Assets.Scripts.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.gui
{
    public class GameSetup
    {
        private PlayerType whitePlayerType;
        private PlayerType blackPlayerType;
        public GameSetup()
        {
            whitePlayerType = PlayerType.HUMAN;
            blackPlayerType = PlayerType.COMPUTER;
        }
        public bool isAIPlayer(Player player)
        {
            if (player.getAlliance() == Alliance.WHITE)
            {
                return getWhitePlayerType() == PlayerType.COMPUTER;
            }
            return getBlackPlayerType() == PlayerType.COMPUTER;
        }

        public PlayerType getWhitePlayerType()
        {
            return this.whitePlayerType;
        }

        public PlayerType getBlackPlayerType()
        {
            return this.blackPlayerType;
        }
    }
}

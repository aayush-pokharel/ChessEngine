using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.gui
{
    public abstract class ChessPiece: MonoBehaviour
    {
        public int CurrentX { set; get; }
        public int CurrentY { set; get; }
        public void SetPosition(int x, int y)
        {
            CurrentX = x;
            CurrentY = y;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Protocol;
using GameSever.Server;

namespace GameSever.PlayerSpace
{
    public class Player
    {
        public int syncFrame;
        public float x;
        public float y;
        public float z;
        public float r_y;
        public float health;
        public ActionStatus actionStatus;
        public int roomId;
    }
}
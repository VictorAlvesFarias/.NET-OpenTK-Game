using Kingdom_of_Creation.Comunication;
using Kingdom_of_Creation.Entities.Implements;
using Server.Context.Camera.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services.Implements
{
    public class BroadcastService
    {
        public async Task SendRenderObject(RenderObject obj)
        {
            var map = TcpMessage.FromObject("updateObject", obj);

            await Program.UdpServer.BroadcastAsync(map);
        }
        public async Task SendPlayerObject(Player obj)
        {
            var map = TcpMessage.FromObject("updatePlayer", obj);

            await Program.UdpServer.BroadcastAsync(map);
        }
        public async Task StartPlayer(Player obj)
        {
            var map = TcpMessage.FromObject("setPlayer", obj);

            await Program.UdpServer.BroadcastAsync(map);
        }
        public async Task SendRenderObjects(List<RenderObject> objects)
        {
            var map = TcpMessage.FromObject("loadObjects", objects);

            await Program.UdpServer.BroadcastAsync(map);
        }
        public async Task SendPlayers(List<Player> players)
        {
            var map = TcpMessage.FromObject("loadPlayers", players);

            await Program.UdpServer.BroadcastAsync(map);
        }
    }
}

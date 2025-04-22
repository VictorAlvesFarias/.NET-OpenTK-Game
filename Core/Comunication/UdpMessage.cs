using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Kingdom_of_Creation.Comunication
{
    public class UdpMessage
    {
        public string Type { get; set; }
        public byte[] Data { get; set; }
    }
}

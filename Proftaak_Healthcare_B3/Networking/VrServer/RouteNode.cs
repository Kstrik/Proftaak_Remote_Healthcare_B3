using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Networking.VrServer
{
    public class RouteNode
    {
        Vector3 pos { get; }
        Vector3 dir { get; }
        
        public RouteNode(Vector3 pos, Vector3 dir)
        {
            this.pos = pos;
            this.dir = dir;
        }
    }
}

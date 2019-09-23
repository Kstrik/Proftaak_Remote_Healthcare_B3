using HealthcareServer.Vr.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World.Components
{
    public class Panel
    {
        public Vector2 Size { get; }
        public Vector2 Resolution { get; }
        public Vector4 Background { get; }
        public bool CastShadows { get; }

        public Panel(Vector2 size, Vector2 resolution, Vector4 background, bool castShadows)
        {
            this.Size = size;
            this.Resolution = resolution;
            this.Background = background;
            this.CastShadows = castShadows;
        }
    }
}

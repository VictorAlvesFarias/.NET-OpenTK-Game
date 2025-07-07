using Kingdom_of_Creation.Entities.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.Renders
{
    public interface IRenderService
    {
        void Draw(RenderObject renderObject);
        void UpdateBuffers(RenderObject renderObject);
    }
}

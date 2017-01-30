using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH
{
    public static class StaticFunctions
    {
        public static MiStrAnEngine.Structure ConvertGHMeshToStructure(Mesh m)
        {
            // 1 skapa  en lista med noder för alla points i meshen

            // 2 Skapa element for varje face, jag löser en konstruktor för detta under tiden

            // 3 Skapa en structure med dessa element


            return new MiStrAnEngine.Structure();
        }

    }
}

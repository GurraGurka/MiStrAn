using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace MiStrAnGH
{
    public static class StaticFunctions
    {
        
        public static MiStrAnEngine.Structure ConvertGHMeshToStructure(Mesh m)
        {
            List<MiStrAnEngine.Node> mistranNodes = new List<MiStrAnEngine.Node>();
            List<MiStrAnEngine.ShellElement> mistranShells = new List<MiStrAnEngine.ShellElement>();
            List<MiStrAnEngine.BC> mistranBCs = new List<MiStrAnEngine.BC>();

            MeshVertexList meshPts = m.Vertices;

            //Create mistran nodes from all the mesh points. Add them to one list
            for(int i=0;i<m.Vertices.Count;i++)
            {
                Point3f mPt = m.Vertices[i];
                MiStrAnEngine.Node mistNode = new MiStrAnEngine.Node(mPt.X, mPt.Y, mPt.Z, i);
                mistranNodes.Add(mistNode);
            }
              
            //Create shellelements from all the meshfaces. Add tgem to one list
            for(int i=0; i< m.Faces.Count;i++)
            {
                List<MiStrAnEngine.Node> shellNodes = new List<MiStrAnEngine.Node>();
                int[] faceIndexDup =m.Faces.GetTopologicalVertices(i);

                //If face is triangular, the duplicate is removed
                int[] faceIndex = faceIndexDup.Distinct().ToArray();
                foreach (int index in faceIndex )
                {


                    shellNodes.Add(mistranNodes[index]);
                    MiStrAnEngine.ShellElement mistShell = new MiStrAnEngine.ShellElement(shellNodes, index);
                    mistranShells.Add(mistShell);
                }
            }


            //TEMPORARY BCS, ALWAYS SET THE FIRST 1/20 of the nodes
            int maxValue = Math.Ceiling((mistranNodes.Count / 20));
            for (int i =0;i< maxValue; i++)
            {
                MiStrAnEngine.BC bc = new MiStrAnEngine.BC(mistranNodes[i]);
                mistranBCs.Add(bc);
            }

            //TEMPORARY ADD LOADS. THE LAST 1/20 of the nodes, all direction downwards. Amplitude of 1000 
            for (int i = 0; i < maxValue; i++)
            {
                int loadDof = maxValue - i FORTSÄTT HÄR
              //  MiStrAnEngine.BC bc = new MiStrAnEngine.BC(mistranNodes[i]);
              //  mistranBCs.Add(bc);
            }


            MiStrAnEngine.Structure mistStruc = new MiStrAnEngine.Structure(mistranNodes, mistranShells,mistranBCs);


            return mistStruc;
        }

    }
}

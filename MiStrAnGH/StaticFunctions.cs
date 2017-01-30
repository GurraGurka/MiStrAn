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


            MiStrAnEngine.Structure mistStruc = new MiStrAnEngine.Structure(mistranNodes, mistranShells);


            return mistStruc;
        }

    }
}

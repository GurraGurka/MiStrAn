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
        
        public static MiStrAnEngine.Structure ConvertGHMeshToStructure(Mesh m, List<Point3d> bcs, List<Point3d> loads)
        {
            List<MiStrAnEngine.Node> mistranNodes = new List<MiStrAnEngine.Node>();
            List<MiStrAnEngine.ShellElement> mistranShells = new List<MiStrAnEngine.ShellElement>();
            List<MiStrAnEngine.BC> mistranBCs = new List<MiStrAnEngine.BC>();
            List<MiStrAnEngine.Load> mistranLoads = new List<MiStrAnEngine.Load>();

            MeshVertexList meshPts = m.Vertices;

            //Create mistran nodes from all the mesh points. Add them to one list
            for(int i=0;i<m.Vertices.Count;i++)
            {
                Point3f mPt = m.Vertices[i];
                MiStrAnEngine.Node mistNode = new MiStrAnEngine.Node(mPt.X, mPt.Y, mPt.Z, i);
                mistranNodes.Add(mistNode);

                //BCS
                //Okej det här går att göra mer effektivt, men duger för tillfället
                for (int j = 0; j < bcs.Count; j++)
                {
                    Point3d closePt = mPt;
                    if(closePt.DistanceTo(bcs[j])<0.001)
                        mistranBCs.Add(new MiStrAnEngine.BC(mistranNodes[i]));      
                }

                //LOADS
                //Okej det här går att göra mer effektivt, men duger för tillfället
                for (int j = 0; j < loads.Count; j++)
                {
                    Point3d closePt = mPt;
                    if (closePt.DistanceTo(loads[j]) < 0.001)
                        mistranLoads.Add(new MiStrAnEngine.Load(mistranNodes[i],-1000,0)); //TEMP JUST 1000 
                }
            }
              
            //Create shellelements from all the meshfaces. Add tgem to one list
            for(int i=0; i< m.Faces.Count;i++)
            {
                List<MiStrAnEngine.Node> shellNodes = new List<MiStrAnEngine.Node>();
                int[] faceIndexDup =m.Faces.GetTopologicalVertices(i);

                //If face is triangular, the duplicate is removed
                int[] faceIndex = faceIndexDup.Distinct().ToArray();
                foreach (int index in faceIndex )
                    shellNodes.Add(mistranNodes[index]);

                MiStrAnEngine.ShellElement mistShell = new MiStrAnEngine.ShellElement(shellNodes, i);
                mistranShells.Add(mistShell);
            }


            

            //TEMPORARY ADD LOADS. THE LAST 1/20 of the nodes, all direction downwards. Amplitude of 1000 
            for (int i = 0; i < 10; i++)
            {
               // int loadDof = maxValue - i FORTSÄTT HÄR
              //  MiStrAnEngine.BC bc = new MiStrAnEngine.BC(mistranNodes[i]);
              //  mistranBCs.Add(bc);
            }


            MiStrAnEngine.Structure mistStruc = new MiStrAnEngine.Structure(mistranNodes, mistranShells,mistranBCs, mistranLoads);


            return mistStruc;
        }

    }
}

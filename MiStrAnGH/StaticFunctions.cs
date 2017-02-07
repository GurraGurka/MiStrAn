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
                        mistranLoads.Add(new MiStrAnEngine.Load(mistranNodes[i],0,0)); //TEMP JUST 1000 
                }
            }
              
            //Create shellelements from all the meshfaces. Add tgem to one list
            for(int i=0; i< m.Faces.Count;i++)
            {
                List<MiStrAnEngine.Node> shellNodes = new List<MiStrAnEngine.Node>();
                int[] faceIndexDup = new int[] { m.Faces[i].A, m.Faces[i].B, m.Faces[i].C, m.Faces[i].D };

                //If face is triangular, the duplicate is removed
                int[] faceIndex = faceIndexDup.Distinct().ToArray();
                foreach (int index in faceIndex )
                    shellNodes.Add(mistranNodes[index]);

                MiStrAnEngine.ShellElement mistShell = new MiStrAnEngine.ShellElement(shellNodes, i);

                // TEMPORARY FOR TEST CASE
                mistShell.D = new Matrix(new double[,] { { 2.307692307692308,0.692307692307692,0},
                    { 0.692307692307692,   2.307692307692308,                   0},
                    { 0 ,                  0 ,  0.807692307692308} });

                mistShell.D = 1e11 * mistShell.D;
                mistShell.thickness = 0.008;
                mistShell.eq = new Matrix(new double[,] { {0,0,1e3 } });



                mistranShells.Add(mistShell);
            }

            return new MiStrAnEngine.Structure(mistranNodes, mistranShells, mistranBCs, mistranLoads);
        }

    }
}

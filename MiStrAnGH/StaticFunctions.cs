using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using MiStrAnEngine;



namespace MiStrAnGH
{
    public static class StaticFunctions
    {

        

        public static List<double> CorrectUnits(List<double> inputList, double corrValue)
        {
            List<double> outputDoubles = new List<double>();
            foreach (double val in inputList)
                outputDoubles.Add(val * corrValue);
            
            return outputDoubles;
        }

        public static void GetDefMesh(Mesh undefMesh,List<double> defs, double scalFac, out Mesh defMesh )
        {
            

            
            Mesh deformableMesh = undefMesh.DuplicateMesh();
            List<Point3d> defPts = new List<Point3d>();

            int meshPtCount = 0;
            for (int i = 0; i < defs.Count; i+=6)
            {
                Transform xPt = Transform.Translation(new Vector3d(scalFac * defs[i], scalFac * defs[i + 1], scalFac * defs[i + 2]));

                Point3f pt3f = undefMesh.Vertices[meshPtCount];
                Point3d pt3d = new Point3d(pt3f.X, pt3f.Y, pt3f.Z);
                pt3d.Transform(xPt);
                deformableMesh.Vertices[meshPtCount] = new Point3f((float)pt3d.X,(float)pt3d.Y,(float)pt3d.Z);
                meshPtCount += 1;
            }

            defMesh = deformableMesh;
           
        }

        public static void GetDefRotVector(List<double> aDefs, out List<Vector3d> defs, out List<Vector3d> rots)
        {
            defs = new List<Vector3d>();
            rots = new List<Vector3d>();
            for (int i = 0; i < aDefs.Count; i += 6)
            {
                defs.Add(new Vector3d(aDefs[i], aDefs[i + 1], aDefs[i + 2]));
                rots.Add(new Vector3d(aDefs[i+3], aDefs[i + 4], aDefs[i + 5]));
            }
            
        }

        public static Grasshopper.GUI.Gradient.GH_Gradient GetStandardGradient()
        {

            Grasshopper.GUI.Gradient.GH_Gradient grad = new Grasshopper.GUI.Gradient.GH_Gradient();
            grad.AddGrip(0, System.Drawing.Color.Blue);
            grad.AddGrip(0.25, System.Drawing.Color.Cyan);
            grad.AddGrip(0.5, System.Drawing.Color.LimeGreen);
            grad.AddGrip(0.75, System.Drawing.Color.Yellow);
            grad.AddGrip(1, System.Drawing.Color.Red);


            return grad;
        }

        public static void GetStressesElemPoints(Mesh mesh, List<Vector3d> prinStresses, int nbElements, out List<Point3d> high, out List<Point3d> low)
        {
            List<double> vLengths = new List<double>();
            List<int> indexes = new List<int>();
            high = new List<Point3d>();
            low = new List<Point3d>();

            foreach (Vector3d v in prinStresses)
                vLengths.Add(v.Length);

            //List sorted by elements with highest principle stress
            /*    List<double> sortedvLengths = vLengths.OrderByDescending(d => d).ToList();

                //This can certainly be improved
                for (int i = 0; i < sortedvLengths.Count; i++)
                {
                    for(int j = 0; j< vLengths.Count;j++)
                    {
                        if (vLengths [j]== sortedvLengths[i])
                        {
                            indexes.Add(j);
                            break; 
                        }
                    }
                } */
            var sorted = vLengths
                .Select((x, i) => new KeyValuePair<double, int>(x, i))
                .OrderBy(x => x.Key)
                .ToList();

           // List<int> sortedLengths = sorted.Select(x => x.Key).ToList();
            indexes = sorted.Select(x => x.Value).ToList();


            indexes = indexes.Take(nbElements).ToList();

            MeshFaceList faceList = mesh.Faces;
            

            for (int i = 0; i < faceList.Count; i++)
            {
                if (indexes.Contains(i))
                   high.Add(faceList.GetFaceCenter(i));
                else
                    low.Add(faceList.GetFaceCenter(i));

            }
                



        }

        public static void ForceloadMKLCORE()
        {
            string path = Grasshopper.Folders.AppDataFolder;
            path = path + @"Libraries\mkl_core.dll";
            MiStrAnEngine.StaticFunctions.LoadLibrary(path);

        }

        

    }
    }

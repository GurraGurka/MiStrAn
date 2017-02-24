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

        public static StructureType ConvertGHMeshToStructure(Mesh m, List<Point3d> bcs, List<Point3d> loadsPts, List<Vector3d> loadVecs, List<MeshFace> loadFaces, List<Vector3d> distLoadVecs, List<Section> sections )
        {
            List<MiStrAnEngine.Node> mistranNodes = new List<MiStrAnEngine.Node>();
            List<MiStrAnEngine.ShellElement> mistranShells = new List<MiStrAnEngine.ShellElement>();
            List<MiStrAnEngine.Support> mistranBCs = new List<MiStrAnEngine.Support>();
            List<MiStrAnEngine.PointLoad> mistranLoads = new List<MiStrAnEngine.PointLoad>();
            List<MiStrAnEngine.DistributedLoad> mistranDistLoads = new List<MiStrAnEngine.DistributedLoad>();

            MeshVertexList meshPts = m.Vertices;

            if (loadsPts.Count != loadVecs.Count || loadFaces.Count!= distLoadVecs.Count) // samé nuly ve sloupci
                throw new MException("Not same amount of vectors and load points/faces");

            //Create mistran nodes from all the mesh points. Add them to one list
            for (int i=0;i<m.Vertices.Count;i++)
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
                        mistranBCs.Add(new MiStrAnEngine.Support(mistranNodes[i]));      
                }

                //LOADS
                //Okej det här går att göra mer effektivt, men duger för tillfället
                for (int j = 0; j < loadsPts.Count; j++)
                {
                    Point3d closePt = mPt;
                    if (closePt.DistanceTo(loadsPts[j]) < 0.001)
                        mistranLoads.Add(new MiStrAnEngine.PointLoad(mistranNodes[i], new MiStrAnEngine.Vector3D(loadVecs[j].X, loadVecs[j].Y, loadVecs[j].Z))); //TEMP JUST 1000 
                }
            }
              
            //Create shellelements from all the meshfaces. Add them to one list
            for(int i=0; i< m.Faces.Count;i++)
            {
                List<MiStrAnEngine.Node> shellNodes = new List<MiStrAnEngine.Node>();
                int[] faceIndexDup = new int[] { m.Faces[i].A, m.Faces[i].B, m.Faces[i].C, m.Faces[i].D };

                //If face is triangular, the duplicate is removed
                int[] faceIndex = faceIndexDup.Distinct().ToArray();
                foreach (int index in faceIndex )
                    shellNodes.Add(mistranNodes[index]);

                MiStrAnEngine.ShellElement mistShell = new MiStrAnEngine.ShellElement(shellNodes, i);

                //Set index for load shells and the vector
                for(int j=0; j<loadFaces.Count; j++)
                {
                    
                    //This only works for triangular
                    if(m.Faces[i].A ==loadFaces[j].A && m.Faces[i].B == loadFaces[j].B && m.Faces[i].C == loadFaces[j].C)
                        mistranDistLoads.Add(new MiStrAnEngine.DistributedLoad(i, new MiStrAnEngine.Vector3D(distLoadVecs[j].X, distLoadVecs[j].Y, distLoadVecs[j].Z)));

                       
                }

                //Set a section for each shell
                foreach(Section s in sections)
                { 
                    if (s.faceIndexes.Contains(mistShell.Id))
                    {
                        mistShell.Section = s;
                        mistShell.GenerateD();
                        break;
                    }
                        
                }

                mistranShells.Add(mistShell);
            }

            return new StructureType(mistranNodes, mistranShells, mistranBCs, mistranLoads, mistranDistLoads);
        }

        public static void GetDefMesh(Mesh undefMesh,List<double> defs, out Mesh defMesh)
        {
            

            //Scale 10% of the bounding box diagonal length (DEACTIVATED NOW)
            double maxDiff = Math.Max(defs.Max(), Math.Abs(defs.Min()));
            double length = undefMesh.GetBoundingBox(false).Diagonal.Length;
             // double scale = length * 0.1 / maxDiff;
            double scale = 100* length; //TEMP
            Mesh deformableMesh = undefMesh.DuplicateMesh();
            List<Point3d> defPts = new List<Point3d>();

            int meshPtCount = 0;
            for (int i = 0; i < defs.Count; i+=6)
            {
                Transform xPt = Transform.Translation(new Vector3d(scale * defs[i], scale * defs[i + 1], scale * defs[i + 2]));

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

        

    }
    }

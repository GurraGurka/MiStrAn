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

        public static void GetDefMesh(Mesh undefMesh, List<double> defs, double scalFac, out Mesh defMesh)
        {



            Mesh deformableMesh = undefMesh.DuplicateMesh();
            List<Point3d> defPts = new List<Point3d>();

            int meshPtCount = 0;
            for (int i = 0; i < defs.Count; i += 6)
            {
                Transform xPt = Transform.Translation(new Vector3d(scalFac * defs[i], scalFac * defs[i + 1], scalFac * defs[i + 2]));

                Point3f pt3f = undefMesh.Vertices[meshPtCount];
                Point3d pt3d = new Point3d(pt3f.X, pt3f.Y, pt3f.Z);
                pt3d.Transform(xPt);
                deformableMesh.Vertices[meshPtCount] = new Point3f((float)pt3d.X, (float)pt3d.Y, (float)pt3d.Z);
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
                rots.Add(new Vector3d(aDefs[i + 3], aDefs[i + 4], aDefs[i + 5]));
            }

        }

        public static Grasshopper.GUI.Gradient.GH_Gradient GetStandardGradient()
        {

            return GetStandardGradient(0, 1);
        }

        public static Grasshopper.GUI.Gradient.GH_Gradient GetStandardGradient(double lim0, double lim1)
        {
            double span = lim1 - lim0;

            if (lim1 <= lim0)
                throw new Exception("Bad limiters");


            Grasshopper.GUI.Gradient.GH_Gradient grad = new Grasshopper.GUI.Gradient.GH_Gradient();
            grad.AddGrip(0, System.Drawing.Color.Blue);
            grad.AddGrip(lim0, System.Drawing.Color.Blue);
            grad.AddGrip(lim0 + span * 0.25, System.Drawing.Color.Cyan);
            grad.AddGrip(lim0 + span * 0.5, System.Drawing.Color.LimeGreen);
            grad.AddGrip(lim0 + span * 0.75, System.Drawing.Color.Yellow);
            grad.AddGrip(lim1, System.Drawing.Color.Red);
            grad.AddGrip(1, System.Drawing.Color.Red);


            return grad;
        }

        public static void GetStressesElemPoints(Mesh mesh, List<Vector3d> prinStresses, int nbElements, out List<Point3d> high, out List<int> highIndex, out List<Point3d> low, out double area)
        {
            List<double> vLengths = new List<double>();
            List<int> indexes = new List<int>();
            high = new List<Point3d>();
            low = new List<Point3d>();
            highIndex = new List<int>();

            foreach (Vector3d v in prinStresses)
                vLengths.Add(v.Length);

            
            var sorted = vLengths
                .Select((x, i) => new KeyValuePair<double, int>(x, i))
                .OrderByDescending(x => x.Key)
                .ToList();

            // List<int> sortedLengths = sorted.Select(x => x.Key).ToList();
            indexes = sorted.Select(x => x.Value).ToList();


            indexes = indexes.Take(nbElements).ToList();

            MeshFaceList faceList = mesh.Faces;
            //area for the high stressed element
            area = 0;


            for (int i = 0; i < faceList.Count; i++)
            {
                if (indexes.Contains(i))
                {
                    high.Add(faceList.GetFaceCenter(i));
                    highIndex.Add(i);

                    //Get the area of the face
                    Point3f pt1, pt2, pt3, pt4;
                    faceList.GetFaceVertices(i, out pt1, out pt2, out pt3, out pt4);
                    double a = pt1.DistanceTo(pt2);
                    double b = pt2.DistanceTo(pt3);
                    double c = pt3.DistanceTo(pt1);
                    double s = (a + b + c) / 2;
                    area += Math.Sqrt(s * (s - a) * (s - b) * (s - c));
                }

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

        public static Mesh CullMesh(Mesh input, double tol,out int[] culledFaces)
        {

            if (tol == 0) return CullMeshExact(input, out culledFaces);

            Mesh output = new Mesh();
            List<Point3d> culledPts = new List<Point3d>();
            int[] indexMap = new int[input.Vertices.Count];
            List<int> _culledFaces = new List<int>();

            for (int i = 0; i < input.Vertices.Count; i++)
            {
                Point3d inputPt = input.Vertices[i];
                int index = -1;
                for (int j = 0; j < culledPts.Count; j++)
                {
                    if (inputPt.DistanceTo(culledPts[j]) < tol)
                    {
                        index = j;
                        break;
                    }
                }

                if (index != -1)
                    indexMap[i] = index;
                else
                {
                    culledPts.Add(inputPt);
                    indexMap[i] = culledPts.Count - 1;
                }
            }


            output.Vertices.AddVertices(culledPts);

            for (int i = 0; i < input.Faces.Count; i++)
            {

                MeshFace face = new MeshFace();
                face.A = indexMap[input.Faces[i].A];
                face.B = indexMap[input.Faces[i].B];
                face.C = indexMap[input.Faces[i].C];
                face.D = indexMap[input.Faces[i].D];

                double area = GetFaceArea(output, face);

                if (area > 1e-6)
                    output.Faces.AddFace(face);
                else
                    _culledFaces.Add(i);
            }

            culledFaces = _culledFaces.ToArray();
            return output;
        }


        public static Mesh CullMeshExact(Mesh input, out int[] culledFaces)
        {
            Mesh output = new Mesh();
            List<Point3d> culledPts = new List<Point3d>();
            int[] indexMap = new int[input.Vertices.Count];
            List<int> _culledFaces = new List<int>();


            for (int i = 0; i < input.Vertices.Count; i++)
                culledPts.Add(input.Vertices[i]);

            culledPts = culledPts.Distinct().ToList();

            for (int i = 0; i < input.Vertices.Count; i++)
            {
                Point3d pt = input.Vertices[i];
                indexMap[i] = culledPts.FindIndex(x => x == pt);
            }

            output.Vertices.AddVertices(culledPts);

            for (int i = 0; i < input.Faces.Count; i++)
            {

                MeshFace face = new MeshFace();
                face.A = indexMap[input.Faces[i].A];
                face.B = indexMap[input.Faces[i].B];
                face.C = indexMap[input.Faces[i].C];
                face.D = indexMap[input.Faces[i].D];

                double area = GetFaceArea(output, face);

                if (area > 1e-6)
                    output.Faces.AddFace(face);
                else
                    _culledFaces.Add(i);
            }

            culledFaces = _culledFaces.ToArray();
            return output;
        }

        public static double GetFaceArea(Mesh M, MeshFace face)
        {

            Point3d A = M.Vertices[face.A];
            Point3d B = M.Vertices[face.B];
            Point3d C = M.Vertices[face.C];

            Vector3d vec1 = new Vector3d(B - A);
            Vector3d vec2 = new Vector3d(C - A);
            Vector3d vec3 = Vector3d.CrossProduct(vec1, vec2);
            double area = vec3.Length;

            if (face.IsTriangle) area = area / 2;

            return area;
        }


        public static Curve[] CreatePrincipalStressContours(StructureType S, Line[] seeds, double stepLength)
        {
            Mesh M = S.uMesh;
            Curve[] ret = new Curve[seeds.Length];

            for (int i = 0; i < seeds.Length; i++)
            {


                List<Point3d> curvePts = new List<Point3d>();

                double maxDist = stepLength;
                int maxIter = 500;

                MeshPoint mPt = M.ClosestMeshPoint(seeds[i].From, maxDist);
                int face = mPt.FaceIndex;
                Vector3d entryVec = new Vector3d(seeds[i].To - seeds[i].From);
                Vector3d dirVec = GetFaceDirectionVector(S, face, entryVec);

                Point3d lastPoint = mPt.Point;
                curvePts.Add(lastPoint);

                for (int j = 0; j < maxIter; j++)
                {
                    Point3d nextPoint = lastPoint + stepLength * dirVec;
                    mPt = M.ClosestMeshPoint(nextPoint, maxDist);
                    nextPoint = mPt.Point;
                    if (mPt == null)
                        break;
                    face = mPt.FaceIndex;
                    dirVec = GetFaceDirectionVector(S, face, dirVec);
                    if (dirVec == Vector3d.Zero)
                        break;
                    lastPoint = new Point3d(nextPoint);

                    curvePts.Add(lastPoint);
                }

                ret[i] = new PolylineCurve(curvePts);
            }


            return ret;
        }
   
        

        

        private static Vector3d GetFaceDirectionVector(StructureType S, int faceIndex, Vector3d entryVec)
        {
            Vector3D e1_, e2_, e3_,C_;
            S.GetElementCoordinateSystem(out e1_, out e2_, out e3_, faceIndex);
            C_ = S.GetElementCentroid(faceIndex);
            Vector3d e1 = e1_.ToRhinoVector3d(), e2 = e2_.ToRhinoVector3d(), e3 = e3_.ToRhinoVector3d();

            Plane P = new Plane(C_.ToRhinoPoint3d(), e1, e2);

            e1.Rotate(S.PrincipalAngles[faceIndex], e3);
            e2.Rotate(S.PrincipalAngles[faceIndex], e3);

            Vector3D principalStress = S.PrincipalStresses[faceIndex];

            bool useX = Math.Abs(principalStress.X) >= Math.Abs(principalStress.Y);

            if (useX)
            {

                if (Vector3d.VectorAngle(entryVec, e1) < Vector3d.VectorAngle(entryVec, -e1))
                    return e1;
                else
                    return -e1;
            }
            else //compression
            {
                if (Vector3d.VectorAngle(entryVec, e2) < Vector3d.VectorAngle(entryVec, -e2))
                    return e2;
                else
                    return -e2;

            }

        }

    }
}

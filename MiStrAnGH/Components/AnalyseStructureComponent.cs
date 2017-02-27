using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH
{
    public class AnalyseStructureComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public AnalyseStructureComponent()
          : base("MiStrAn Analyse Structure", "Analyse",
              "Calculating...",
              "MiStrAn", "No SubCat")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "Run", "Run calcs", GH_ParamAccess.item, false);
            pManager.AddParameter(new StructureParameter(), "MiStrAn Structure", "Structure", "Assembled structure", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Tx Ty Tz", "Displacements", "Solved displacements", GH_ParamAccess.list);
            pManager.AddVectorParameter("Rx Ry Rz", "Rotations", "Solved rotations", GH_ParamAccess.list);
            pManager.AddNumberParameter("r [N]", "Reactions", "solved reactions", GH_ParamAccess.list);
            pManager.AddGeometryParameter("DefMesh", "DeformedMesh", "Deformed mesh", GH_ParamAccess.item);
            pManager.AddVectorParameter("P1 P2 0", "PrincipalStresses", "Principal stresses", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Stress plot", "VonMisesMesh", "von Mises stress plot", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            bool run = false;
            StructureType s = new StructureType();

            DA.GetData(0, ref run);
            if (!DA.GetData(1, ref s)) { return; }


            if (run)
            {

                MiStrAnEngine.Vector a, r;
                s.Analyze(out a, out r);

                List<double> aList = new List<double>();
                List<double> rList = new List<double>();
                List<Vector3d> defList = new List<Vector3d>();
                List<Vector3d> rotList = new List<Vector3d>();
                List<Vector3d> principalStresses = new List<Vector3d>();
                Mesh outMesh = new Mesh();



                for (int i = 0; i < a.Length; i++)
                {
                    aList.Add(a[i]);
                    rList.Add(r[i]);
                }

                List<MiStrAnEngine.Vector3D> pStress;
                s.CalcStresses(aList, out pStress);


                for (int i = 0; i < pStress.Count; i++)
                    principalStresses.Add(new Vector3d(pStress[i].X, pStress[i].Y, 0));

                //Get outputs
               // StaticFunctions.GetDefMesh(meshes[0], aList,1, out outMesh);
                StaticFunctions.GetDefRotVector(aList, out defList, out rotList);
                outMesh = s.GenereateDeformedMesh(aList, 100);
                Mesh stressMesh = s.GenerateStressMesh(aList, principalStresses);

                DA.SetDataList(0, defList);
                DA.SetDataList(1, rotList);
                DA.SetDataList(2, rList);
                DA.SetData(3, outMesh);
                DA.SetDataList(4, principalStresses);
                DA.SetData(5, stressMesh);
            }

        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{14da614a-5511-4600-8130-d52b32d26359}"); }
        }
    }
}

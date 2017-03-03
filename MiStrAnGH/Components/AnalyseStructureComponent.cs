using System;
using System.Collections.Generic;
using System.Linq;
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
              "Analyse a MiStrAn structure",
              "MiStrAn", "Model")
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
            pManager.AddParameter(new StructureParameter(), "Analysed structure", "Structure", "Use visualise results to view", GH_ParamAccess.item);
            pManager.AddVectorParameter("Tx Ty Tz [m]", "Displacements", "Solved displacements in m", GH_ParamAccess.list);
            pManager.AddVectorParameter("Rx Ry Rz", "Rotations", "Solved rotations", GH_ParamAccess.list);
            pManager.AddNumberParameter("r [kN]", "Reactions", "solved reactions in kN", GH_ParamAccess.list);
            pManager.AddVectorParameter("P1 P2 0 [MPa]", "PrincipalStresses", "Principal stresses in MPa", GH_ParamAccess.list);
            pManager.AddNumberParameter("Principal stress angles", "PS Angles", "Angles for principal stresses in each element [degrees]", GH_ParamAccess.list);
            pManager.AddNumberParameter("von Mises stresses", "vMis", "von Mises stresses in each element", GH_ParamAccess.list);

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

                s.Analyze();

                List<double> aList = new List<double>();
                List<double> rList = new List<double>();
                List<Vector3d> defList = new List<Vector3d>();
                List<Vector3d> rotList = new List<Vector3d>();
                List<Vector3d> principalStresses = new List<Vector3d>();

                aList = s.a.ToList();
                rList = s.r.ToList();

                //Account for unit (kN)
                rList= rList.Select(x => x / 1000).ToList();


                List<MiStrAnEngine.Vector3D> pStress;
                s.CalcStresses();
                pStress = s.PrincipalStresses;

                //Convert to rhino.vector3d
                for (int i = 0; i < pStress.Count; i++)
                {
                    //account for unit (MPa)
                    pStress[i] /= 1e6;
                    principalStresses.Add(new Vector3d(pStress[i].X, pStress[i].Y, 0));
                }
                   

                //Get outputs
                StaticFunctions.GetDefRotVector(aList, out defList, out rotList);

                DA.SetData(0, s);
                DA.SetDataList(1, defList);
                DA.SetDataList(2, rotList);
                DA.SetDataList(3, rList);
                DA.SetDataList(4, principalStresses);
                DA.SetDataList(5, s.PrincipalAngles);
                DA.SetDataList(6, s.vonMises);
                
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

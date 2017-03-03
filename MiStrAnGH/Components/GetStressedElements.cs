using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class GetStressedElements : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetStressedElements class.
        /// </summary>
        public GetStressedElements()
          : base("GetStressedElements", "Get stressed elements",
              "Get the elements with highest stresses",
              "MiStrAn", "Evaluate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh to evaluate", "Mesh", "Pick ypour mesh", GH_ParamAccess.item);
            pManager.AddVectorParameter("Principal stresses vector", "Stresses", "Principal stresses for every element", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Nb of highest stressed elements ", "nbElements", " Nb of elements in 'High Stress Elements'", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Nb elements with highest stresses", "HighStressElems", "Input Nb of elements with highest stresses ", GH_ParamAccess.list);
            pManager.AddPointParameter( "The rest of the stresses", "LowStressElems", "The rest of the elements with the lowest stresses ", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int nbElements = new int();
            List<Vector3d> prinStresses = new List<Vector3d>();
            Mesh mesh = new Mesh();
            List<Point3d> highElemStress = new List<Point3d>();
            List<Point3d> lowElemStress = new List<Point3d>();

            if (!DA.GetData(0, ref mesh)) { return; }
            if (!DA.GetDataList(1, prinStresses)) {} 
            if (!DA.GetData(2, ref nbElements)) {}

            StaticFunctions.GetStressesElemPoints(mesh, prinStresses,nbElements, out highElemStress, out lowElemStress);

            DA.SetDataList(0, highElemStress);
            DA.SetDataList(1, lowElemStress);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1519613b-9a55-4320-ad35-b9bf8218c23a}"); }
        }
    }
}
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class Dynamics : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Dynamics class.
        /// </summary>
        public Dynamics()
          : base("Dynamics", "Dynamics",
              "Dynamic analysis",
              "MiStrAn", "Model")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter(), "Stucture", "Structure", "MiStrAn structure", GH_ParamAccess.item);
            pManager.AddIntegerParameter( "nbIterations", "nbIterations", "Number of iterations", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Eigenfrequiences", "EigenFreq", "Eigenfrequiences of the structure", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            StructureType structure = new StructureType();
            int iterations = new int() ;

            if (!DA.GetData(0, ref structure)) return;
            if (!DA.GetData(1, ref iterations)) return;

            double lowestFreq = 0; //MiStrAnEngine.StaticFunctions.InverseIterationMethod(structure.K, structure.M, structure.bc, iterations);
            DA.SetData(0, lowestFreq);
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
            get { return new Guid("{fea749e2-1455-4dc0-b1ab-7c67eff89e36}"); }
        }
    }
}
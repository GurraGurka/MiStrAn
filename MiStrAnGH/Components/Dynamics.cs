using System;
using System.Collections.Generic;
using System.Linq;
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
          : base("Eigenfrequencies", "Eigenfreq",
              "Eigenfrequency analysis",
              "MiStrAn", "Model")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter(), "Stucture", "Structure", "MiStrAn structure", GH_ParamAccess.item);
            pManager.AddNumberParameter( "MaximumEigenFrequency value", "maxEigenFreq", "Highest eignefreq value to look for", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Lowest eigenfrequency", "EigenFreq", "Lowest eigenfrequiences of the structure within the span", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            StructureType structure = new StructureType();
            double maxFreq = new double() ;

            if (!DA.GetData(0, ref structure)) return;
            if (!DA.GetData(1, ref maxFreq)) return;

            //MiStrAnEngine.StaticFunctions.InverseIterationMethod(structure.K, structure.M, structure.bc, iterations);
            double[] freqs = new double[] { };
            MiStrAnEngine.Vector[] eigenVecs = new MiStrAnEngine.Vector[] { };

            MiStrAnEngine.StaticFunctions.GetEigenFreqs(structure, maxFreq, out freqs, out eigenVecs);

           double lowestFreq = freqs[0];

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
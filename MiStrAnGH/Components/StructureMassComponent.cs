using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class StructureMassComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the StructureMassComponent class.
        /// </summary>
        public StructureMassComponent()
          : base("Structure Mass", "StructMass",
              "Calculates the total mass of a structure",
              "MiStrAn", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter(), "Stucture", "Structure", "MiStrAn structure", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Mass", "M", "Mass of structure", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            StructureType str = new StructureType();
            if (!DA.GetData(0, ref str)) return;

            double mass = str.GetMass();
            DA.SetData(0, mass);
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
            get { return new Guid("{4402d69f-4db4-4789-8f8d-8bfe85a506a1}"); }
        }
    }
}
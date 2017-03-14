using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class EigenModes : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the EigenModes class.
        /// </summary>
        public EigenModes()
          : base("EigenModes", "EigenModes",
              "Visualize eigenmodes",
              "Mistran", "Visualise")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter(), "Stucture", "Structure", "MiStrAn structure", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale factor", "ScaleFac", "Scalefactor for eigenmode", GH_ParamAccess.item, 1);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Eigenmodes", "EigenModes", "Deformed mesh from eigenmodes", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            StructureType structure = new StructureType();
            double scaleFac = new double();
            List<Mesh> meshes = new List<Mesh>();

            if (!DA.GetData(0, ref structure)) return;
            if (!DA.GetData(1, ref scaleFac)) return;

            foreach (MiStrAnEngine.Vector v in structure.eigenVecs)
                meshes.Add(structure.GenerateEigenMode(v, scaleFac));

            DA.SetDataList(0, meshes);
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
            get { return new Guid("{1b3b1966-25c2-424e-a8ea-b1ce738651dc}"); }
        }
    }
}
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class CleanMeshComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CleanMeshComponent()
          : base("Clean Mesh", "CullMesh",
              "Culls duplicate vertices in mesh, and removes zero area faces",
              "MiStrAn", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh to be culled", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tolerance", "tol", "Distance tolerance", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Culled Mesh", "M", "Mesh to be culled", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Culled faces", "F", "Faces that were removed", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh input = new Mesh();
            Mesh output;
            double tol = 0;

            if (!DA.GetData(0, ref input)) return;
            DA.GetData(1, ref tol);

            int[] culledFaces;

            output = StaticFunctions.CullMesh(input, tol, out culledFaces);
            DA.SetData(0, output);
            DA.SetDataList(1, culledFaces);
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
            get { return new Guid("{92bc7e3b-b0d6-4aa8-a5f0-b8d104adca7a}"); }
        }
    }
}
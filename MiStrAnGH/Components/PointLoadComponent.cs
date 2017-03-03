using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH
{
    public class PointLoadComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PointLoadComponent class.
        /// </summary>
        public PointLoadComponent()
          : base("MiStrAn Pointload", "Pointload",
              "Create a pointload",
              "MiStrAn", "Loads")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pt", "Point where load is applied", GH_ParamAccess.item);
            pManager.AddVectorParameter("Force Vector [kN]", "V", "Force vector in kN to be applied at point", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new LoadParameter(), GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d pt = new Point3d();
            Vector3d vec = new Vector3d();

            if (!DA.GetData(0, ref pt)) return;
            if (!DA.GetData(1, ref vec)) return;

            //Account for unit (kN)
            vec *= 1000;

            LoadType load = new LoadType(new MiStrAnEngine.Vector3D(pt.X, pt.Y, pt.Z), new MiStrAnEngine.Vector3D(vec.X, vec.Y, vec.Z), MiStrAnEngine.TypeOfLoad.PointLoad);

            DA.SetData(0, load);

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
            get { return new Guid("{5e730777-2586-4c3a-8989-18326b13c69b}"); }
        }
    }
}
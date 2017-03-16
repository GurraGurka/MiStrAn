using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH
{
    public class GravityLoadComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PointLoadComponent class.
        /// </summary>
        public GravityLoadComponent()
          : base("MiStrAn Gravity load", "Gravity",
              "Create aMiStrAn Gravity load",
              "MiStrAn", "Loads")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Center Point", "CenPt", "Point close to centroid of mesh face", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element ID", "eID", "Element ID for load. Will override point. Leave both blank to apply to all", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
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
            int elementId = -1;
            bool applyToAll = false;
            bool useId = false;

            if (!DA.GetData(0, ref pt)) useId = true;
            if (!DA.GetData(1, ref elementId) && useId) applyToAll = true;

            LoadType load;

            if (!applyToAll)
            {
                if(useId && elementId < 0) throw new Exception(" Point not set, invalid ID");
                load = new LoadType(new MiStrAnEngine.Vector3D(pt.X, pt.Y, pt.Z), MiStrAnEngine.Vector3D.ZeroVector, MiStrAnEngine.TypeOfLoad.GravityLoad);
            }
            else
            {
                load = new LoadType(MiStrAnEngine.Vector3D.ZeroVector, MiStrAnEngine.Vector3D.ZeroVector, MiStrAnEngine.TypeOfLoad.GravityLoad);
                load.ApplyToAllElements = true;
            }

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
            get { return new Guid("{25742835-9f25-49c3-a727-d73963f5562e}"); }
        }
    }
}
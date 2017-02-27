using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH
{
    public class CreateSupportComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CreateSupportComponen class.
        /// </summary>
        public CreateSupportComponent()
          : base("Create Support", "Support",
              "Creates a support from a point",
              "MiStrAn", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pt", "Point close to node where support is to be added", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Support in X-direction?", "X", "Make true if you wish to add support for translations in X-direction", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Support in Y-direction?", "Y", "Make true if you wish to add support for translations in Y-direction", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Support in Z-direction?", "Z", "Make true if you wish to add support for translations in Z-direction", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Support in XX-direction?", "RX", "Make true if you wish to add support for rotations around X-axis", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Support in YY-direction?", "RY", "Make true if you wish to add support for rotations around Y-axis", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Support in ZZ-direction?", "RZ", "Make true if you wish to add support for rotations around Z-axis", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new SupportParameter(), "MiStrAn Support", "Support", "A MiStrAn support", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool X = true, Y = true, Z = true, RX = true, RY = true, RZ = true;
            Point3d Pt = new Point3d();

            if (!DA.GetData(0, ref Pt)) return;
            DA.GetData(1, ref X);
            DA.GetData(2, ref Y);
            DA.GetData(3, ref Z);
            DA.GetData(4, ref RX);
            DA.GetData(5, ref RY);
            DA.GetData(6, ref RZ);

            SupportType sup = new SupportType(new MiStrAnEngine.Node(Pt.X, Pt.Y, Pt.Z));
            sup.X = X;
            sup.Y = Y;
            sup.Z = Z;
            sup.RX = RX;
            sup.RY = RY;
            sup.RZ = RZ;

            DA.SetData(0, sup);


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
            get { return new Guid("{3cd1a8fb-4d9b-47ef-9df6-6d47c9d96ad7}"); }
        }
    }
}
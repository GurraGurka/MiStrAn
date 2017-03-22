using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class DrawPrincipalStressContoursComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DrawElementCoordinateSystem class.
        /// </summary>
        public DrawPrincipalStressContoursComponent()
          : base("Draw contours", "Contour",
              "Draws Principal stress contours",
              "MiStrAn", "Visualise")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter(), "Stucture", "Structure", "MiStrAn structure", GH_ParamAccess.item);
            pManager.AddLineParameter("seed", "s", "asd", GH_ParamAccess.list);
            pManager.AddNumberParameter("Step length", "sLength", "Step length", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("curves", "C", "asd", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            StructureType S = new StructureType();
            List<Line> seeds = new List<Line>();
            double stepLength = 0;

            if (!DA.GetData(0, ref S)) return;
            if (!DA.GetDataList(1, seeds)) return;
            if (!DA.GetData(2, ref stepLength)) return;

           Curve[] crvs =  StaticFunctions.CreatePrincipalStressContours(S, seeds.ToArray(),stepLength);

            DA.SetDataList(0, crvs);
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
            get { return new Guid("{6163aba1-3e03-4caf-ba43-c3db750e1aad}"); }
        }

        //public override void DrawViewportWires(IGH_PreviewArgs args)
        //{
        //    args.Display.DrawLines(structure.MaterialAxisLines.ToArray(), System.Drawing.Color.Cyan);
        //}

        //public override BoundingBox ClippingBox
        //{
        //    get
        //    {
        //        return structure.boundingBox;
        //    }
        //}

        //public override bool IsPreviewCapable
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}
    }
}
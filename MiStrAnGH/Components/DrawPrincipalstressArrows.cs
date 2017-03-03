using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class DrawPrincipalstressArrows : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DrawElementCoordinateSystem class.
        /// </summary>
        public DrawPrincipalstressArrows()
          : base("DrawPrincipalstressArrows", "DrawPrincArrows",
              "Draws principal stress arrows on elements",
              "MiStrAn", "Subcategory")
        {
        }

        public StructureType structure;

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
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            structure = new StructureType();

            if (!DA.GetData(0, ref structure)) return;

            structure.GeneratePrincipalStressLines();
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
            get { return new Guid("{1929e540-85f2-4997-acb1-662725a2fac7}"); }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (int i = 0; i < structure.NumberOfElements; i++)
            {
                System.Drawing.Color X = structure.PrincipalStresses[i].X <= 0 ? System.Drawing.Color.Blue : System.Drawing.Color.Red;
                System.Drawing.Color Y = structure.PrincipalStresses[i].Y <= 0 ? System.Drawing.Color.Blue : System.Drawing.Color.Red;

                args.Display.DrawLine(structure.PrincipalStressLinesX[i], X);
                args.Display.DrawLine(structure.PrincipalStressLinesY[i], Y);
            }
        }

        public override BoundingBox ClippingBox
        {
            get
            {
                return structure.boundingBox;
            }
        }

        public override bool IsPreviewCapable
        {
            get
            {
                return true;
            }
        }
    }
}
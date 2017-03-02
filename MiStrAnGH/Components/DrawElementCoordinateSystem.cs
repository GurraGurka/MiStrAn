using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class DrawElementCoordinateSystem : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DrawElementCoordinateSystem class.
        /// </summary>
        public DrawElementCoordinateSystem()
          : base("DrawElementCoordinateSystem", "DrawCoordSys",
              "Draws a strucutres elements coordinate systems",
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

            structure.GenerateElementAxisLines();
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
            get { return new Guid("{3c64cbb3-d3d4-40ae-9d9a-bb65833be0e3}"); }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            args.Display.DrawLines(structure.XAxisLines.ToArray(), System.Drawing.Color.Blue);
            args.Display.DrawLines(structure.YAxisLines.ToArray(), System.Drawing.Color.Red);
            args.Display.DrawLines(structure.ZAxisLines.ToArray(), System.Drawing.Color.Green);
            args.Display.DrawLines(structure.MaterialAxisLines.ToArray(), System.Drawing.Color.Cyan);

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
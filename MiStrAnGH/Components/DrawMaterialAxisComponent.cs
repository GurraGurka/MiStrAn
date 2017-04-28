using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class DrawMaterialAxisComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DrawElementCoordinateSystem class.
        /// </summary>
        public DrawMaterialAxisComponent()
          : base("Draw material axis", "DrawMatAxis",
              "Draws the material axis for each element",
              "MiStrAn", "Visualise")
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
            pManager.AddCurveParameter("Material lines", "lines", "Lines indicating directions of the material direction in each element", GH_ParamAccess.list);
            pManager[0].ExpirePreview(false);
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

            DA.SetDataList(0, structure.MaterialAxisLines);
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
            get { return new Guid("{86033ed9-a0f9-4b0f-a5fd-446355cdae21}"); }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
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
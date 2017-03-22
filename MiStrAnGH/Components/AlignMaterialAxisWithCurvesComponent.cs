using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class AlignMaterialAxisWithCurvesComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AlignMaterialAxisWithCurvesComponent class.
        /// </summary>
        public AlignMaterialAxisWithCurvesComponent()
          : base("AlignMaterialAxisWithCurvesComponent", "AlignMatToCurves",
              "Align all elements material axis to a set of curves",
              "MiStrAn", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter(), "Structure", "Structure", "Strucutre", GH_ParamAccess.item);
            pManager.AddCurveParameter("Direction Curves", "Curves", "Curves to align material axis with", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new StructureParameter(), GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            StructureType s = new StructureType();
            List<Curve> dirCurves = new List<Curve>();

            if (!DA.GetData(0, ref s)) return;
            if (!DA.GetDataList(1, dirCurves)) return;

            s.AlignMaterialAxisWithCurves(dirCurves.ToArray());

            DA.SetData(0, s);
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
            get { return new Guid("{c9eaf92b-0565-49a5-9826-3e0ded5b44e6}"); }
        }
    }
}
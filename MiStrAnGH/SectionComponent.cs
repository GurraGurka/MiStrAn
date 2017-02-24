using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH
{
    public class SectionComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MaterialComponent class.
        /// </summary>
        public SectionComponent()
          : base("Section", "Section",
              "Materializing...",
              "StrucAnalysis", "No SubCat")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Indexes of faces", "FacIndexes", "Select face-indexes to give section", GH_ParamAccess.list);
            pManager.AddNumberParameter("E modulus in longitudinal direction", "Ex", "Define one for all or for each ply", GH_ParamAccess.list);
            pManager.AddGeometryParameter("E modulus in transverse direction", "Ey", "Define one for all or for each ply", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Poissons Ratio", "v", "Define one for all or for each ply", GH_ParamAccess.list);
            pManager.AddVectorParameter("Lamina Thickness", "Thickness", "Define one for all or for each ply", GH_ParamAccess.list);
            pManager.AddMeshFaceParameter("Angles in degree", "Angles", " Define one for all or for each ply", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
           // pManager.Add("MiStrAn material ", "Material", "Material for assemble", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<int> faceIndexes = new List<int>();
            List<double> Exs = new List<double>();
            List<double> Eys = new List<double>();
            List<double> thickness = new List<double>();
            List<double> angles = new List<double>();
            List<double> vs = new List<double>();

            if (!DA.GetDataList(0, faceIndexes)) { return; }
            if (!DA.GetDataList(1, Exs)) { new List<double>(new double[] { 39 * Math.Pow(10, 9) }); }
            if (!DA.GetDataList(2, Eys)) { new List<double>(new double[] { 8.6 * Math.Pow(10, 9) }); }
            if (!DA.GetDataList(3, thickness)) { new List<double>(new double[] { 0.01 }); }
            if (!DA.GetDataList(4, angles)) { new List<double>(new double[] { 0 }); }
            if (!DA.GetDataList(4, angles)) { new List<double>(new double[] { 0.3 }); }

            MiStrAnEngine.Section section = new MiStrAnEngine.Section(thickness, angles, Exs, Eys, vs, faceIndexes);

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
            get { return new Guid("{f2a8c5a6-ce19-40f7-8b89-9d59cc86186c}"); }
        }
    }
}
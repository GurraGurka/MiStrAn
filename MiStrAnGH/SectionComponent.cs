using System;
using System.Collections.Generic;
using System.Linq;
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
            pManager.AddIntegerParameter("Indexes of faces", "FacIndexes", "Select face-indexes to give section", GH_ParamAccess.list);
            pManager.AddNumberParameter("E modulus in longitudinal direction", "Ex", "Define one for all or for each ply", GH_ParamAccess.list);
            pManager.AddNumberParameter("E modulus in transverse direction", "Ey", "Define one for all or for each ply", GH_ParamAccess.list);
            pManager.AddNumberParameter("Longitudinal shear modulus", "Gxy", " Define one for all or for each ply", GH_ParamAccess.list);
            pManager.AddNumberParameter("Poissons Ratio", "v", "Define one for all or for each ply", GH_ParamAccess.list);
            pManager.AddNumberParameter("Lamina Thickness", "Thickness", "Define one for all or for each ply", GH_ParamAccess.list);
            pManager.AddNumberParameter("Angles in degree", "Angles", " Define one for all or for each ply", GH_ParamAccess.list);
            pManager.AddNumberParameter("Density [kg/m^3]", "Density", " Density of the meshfaces", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new SectionParameter(), "Generated", "Section", "Section with thickness, indexes and material ", GH_ParamAccess.item);
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
            List<double> Gxys = new List<double>(new double[] { 2.9 * Math.Pow(10, 9) }) ;
            List<double> thickness = new List<double>(new double[] { 0.01 });
            List<double> angles = new List<double>(new double[] { 0 });
            List<double> vs = new List<double>(new double[] { 0.3 });
            double density = 2100;

            List<double> temp = new List<double>(new double[] { 39 * Math.Pow(10, 9) });
            Eys = new List<double>(new double[] { 8.6 * Math.Pow(10, 9) }); ;

            if (!DA.GetDataList(0, faceIndexes)) { return; }
            if (!DA.GetDataList(1, Exs)) {Exs = temp; } 
            DA.GetDataList(2, Eys);
            DA.GetDataList(3, Gxys);
            DA.GetDataList(4, thickness);
            DA.GetDataList(5, angles);
            DA.GetDataList(6, vs);
            DA.GetData(7, ref density);

            int[] lenghts = { Exs.Count, Eys.Count, Gxys.Count, thickness.Count, angles.Count, vs.Count };
            int listlength = lenghts.Max();

            checkListLength(Exs, listlength);
            checkListLength(Eys, listlength);
            checkListLength(Gxys, listlength);
            checkListLength(thickness, listlength);
            checkListLength(angles, listlength);
            checkListLength(vs, listlength);

            SectionType section = new SectionType(thickness, angles, Exs, Eys,Gxys, vs, faceIndexes, density);

            DA.SetData(0, section);

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

        // IF the parameter contains too few elements (or only one defined)
        public void checkListLength(List<double> param, int listLength)
        {
            if (param.Count == listLength || param.Count == 1)
                return;
            else
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Only one or same amount of inputs");
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
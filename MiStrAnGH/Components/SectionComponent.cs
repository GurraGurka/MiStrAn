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
              "MiStrAn", "No SubCat")
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
            pManager.AddParameter(new SectionParameter(), "Generated", "Section", "Section with thickness, indexes and material ", GH_ParamAccess.list);
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
            List<double> Gxys = new List<double>();
            List<double> thickness = new List<double>();
            List<double> angles = new List<double>();
            List<double> vs = new List<double>();
            double density = new double();

            List<double> ExsDef = new List<double>(new double[] { 210e9 });
            List<double> EysDef = new List<double>(new double[] { 210e9 }); ;
            List<double> GxysDef = new List<double>(new double[] { 79.3e9 });
            List<double> thicknessDef = new List<double>(new double[] { 0.01 });
            List<double> anglesDef = new List<double>(new double[] { 0 });
            List<double> vsDef = new List<double>(new double[] { 0.3 });
            double densityDef = 7800;

            if (!DA.GetDataList(0, faceIndexes)) { return; }
            if (!DA.GetDataList(1, Exs)) {Exs = ExsDef; }
            if (!DA.GetDataList(2, Eys)) { Eys = EysDef; }
            if (!DA.GetDataList(3, Gxys)) { Gxys = GxysDef; }
            if (!DA.GetDataList(4, vs)) { vs = vsDef; }
            if (!DA.GetDataList(5, thickness)) { thickness = thicknessDef; }
            if (!DA.GetDataList(6, angles)) { angles = anglesDef; }
           
            if (!DA.GetData(7, ref density)) { density = densityDef; }

            int[] lenghts = { Exs.Count, Eys.Count, Gxys.Count, thickness.Count, angles.Count, vs.Count };
            int listlength = lenghts.Max();

            //Check list length and that the layers are symmetrical
            checkListLength(Exs, listlength);
            checkListLength(Eys, listlength);
            checkListLength(Gxys, listlength);
            checkListLength(thickness, listlength);
            checkListLength(angles, listlength);
            checkListLength(vs, listlength);


          
            

            double totalThick = 0;
            //If thickness is not defined for every layer, the single input thickness is defined for all layers
            if (thickness.Count == listlength)
            {
                foreach (double t in thickness)
                {
                    totalThick += t;
                }
            }
            else
                totalThick = thickness[0]*listlength;

            List<SectionType> section = new List<SectionType>();
            section.Add(new SectionType(thickness, angles, Exs, Eys,Gxys, vs, faceIndexes, density, totalThick));

            DA.SetDataList(0, section);

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
            //Only one value is okay
            if (param.Count == 1)
                return;
            //This is also okay, but a check so the section is symmtrical
            else if (param.Count == listLength)
            {
                double iter = Math.Floor(listLength / 2.0);
                Convert.ToInt32(iter);
                for (int i = 0; i < iter; i++)
                {
                    if(param[i] != param[param.Count-1-i])
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Only symmetrical sections allowed");
                }
            }
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
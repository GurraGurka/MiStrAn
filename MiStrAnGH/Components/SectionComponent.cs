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
              "Create a anisotropic MiStrAn shell section",
              "MiStrAn", "Model")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Face center points", "FaceCentPt", "Select face-center points to give section", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element Id", "eId", "Optional element id for assignment. If set will override point", GH_ParamAccess.item);
            pManager.AddNumberParameter("E modulus in longitudinal direction [GPa]", "Ex", "Define one for all or for each ply", GH_ParamAccess.list, 210);
            pManager.AddNumberParameter("E modulus in transverse direction [GPA]", "Ey", "Define one for all or for each ply", GH_ParamAccess.list,210);
            pManager.AddNumberParameter("Longitudinal shear modulus [GPA]", "Gxy", " Define one for all or for each ply", GH_ParamAccess.list,79.3);
            pManager.AddNumberParameter("Poissons Ratio", "v", "Define one for all or for each ply", GH_ParamAccess.list,0.3);
            pManager.AddNumberParameter("Lamina Thickness [mm]", "Thickness", "Define one for all or for each ply", GH_ParamAccess.list,10);
            pManager.AddNumberParameter("Angles [degree]", "Angles", " Define one for all or for each ply", GH_ParamAccess.list,0);
            pManager.AddNumberParameter("Density [kg/m^3]", "Density", " Density of the meshfaces", GH_ParamAccess.list,7800);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;

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
            //   List<Point3d> faceIndexes = new List<Point3d>();
            Point3d centerPt = new Point3d();
            int elementId = -1;
            bool useId = false;
            List<double> Exs = new List<double>();
            List<double> Eys = new List<double>();
            List<double> Gxys = new List<double>();
            List<double> thickness = new List<double>();
            List<double> angles = new List<double>();
            List<double> vs = new List<double>();
            List<double> densitys = new List<double>();


            bool applyToAll = false;
            
            if (!DA.GetData(0, ref centerPt)) useId = true;
            if (!DA.GetData(1, ref elementId) && useId) applyToAll = true;
            if (!DA.GetDataList(2, Exs)) {} //Exs = ExsDef;
            if (!DA.GetDataList(3, Eys)) { } //Eys = EysDef;
            if (!DA.GetDataList(4, Gxys)) { } // Gxys = GxysDef; 
            if (!DA.GetDataList(5, vs)) { } // vs = vsDef;
            if (!DA.GetDataList(6, thickness)) { } //thickness = thicknessDef;
            if (!DA.GetDataList(7, angles)) { } //angles = anglesDef;
            if (!DA.GetDataList(8, densitys)) { } //density = densityDef;

            
 


            int[] lenghts = { Exs.Count, Eys.Count, Gxys.Count, thickness.Count, angles.Count, vs.Count, densitys.Count };
            int listlength = lenghts.Max();

            //Check list length and that the layers are symmetrical
            checkListLength(Exs, listlength);
            checkListLength(Eys, listlength);
            checkListLength(Gxys, listlength);
            checkListLength(thickness, listlength);
            checkListLength(angles, listlength);
            checkListLength(vs, listlength);
            checkListLength(densitys, listlength);


            //Correct units
            Exs = StaticFunctions.CorrectUnits(Exs, 1e9); //From GPa tp Pa
            Eys = StaticFunctions.CorrectUnits(Eys, 1e9); //From GPa tp Pa
            Gxys = StaticFunctions.CorrectUnits(Gxys, 1e9); //From GPa tp Pa
            thickness = StaticFunctions.CorrectUnits(thickness, 0.001); //From mm to m



            double totalThick = 0;
            //If thickness is not defined for every layer, the single input thickness is defined for all layers
            if (thickness.Count == listlength)
            {
                foreach (double t in thickness)
                    totalThick += t;
            }
            else
                totalThick = thickness[0]*listlength;

            SectionType section = new SectionType();
            //Fixa detta senare
            if (!applyToAll)
            {              
                if(useId && elementId < 0) throw new Exception(" Point not set, invalid ID");
                section = new SectionType(thickness, angles, Exs, Eys, Gxys, vs, new MiStrAnEngine.Vector3D(centerPt.X, centerPt.Y, centerPt.Z), densitys, totalThick);
                section.ParentIndex = elementId;
            }
                
            else
            {
                section= new SectionType(thickness, angles, Exs, Eys, Gxys, vs, new MiStrAnEngine.Vector3D(0,0,0), densitys, totalThick);
                section.applyToAll = true;
            }

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
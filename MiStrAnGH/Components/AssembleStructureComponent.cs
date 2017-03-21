using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH
{
    public class AssebleStructureComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public AssebleStructureComponent()
          : base("MiStrAn Assemble Structure", "Assemble",
              "Calculating...",
              "MiStrAn", "Model")
        { }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Mesh", "Mesh", "Mesh to analyze", GH_ParamAccess.item);
            pManager.AddParameter(new SupportParameter(), "MiStrAn Supports", "Supports", "MiStranSupports (use Create Support", GH_ParamAccess.list);
            pManager.AddParameter(new LoadParameter(), "MiStrAn Loads", "Loads", "MiStrAn Loads, (use loadcomponents)", GH_ParamAccess.list);
            pManager.AddParameter(new SectionParameter(), "Generated Section", "Section", "Section with thickness, indexes and material ", GH_ParamAccess.list);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter(), "Assembled Structure", "Structure", "Assembled Structure with loads, elements, bc's etc", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = new Mesh();
            List<SupportType> supports = new List<SupportType>();
            List<LoadType> loads = new List<LoadType>();
            double thick = new double();
            List<MiStrAnEngine.Section> sections = new List<MiStrAnEngine.Section>();
            bool run = false;

            if (!DA.GetData(0, ref mesh)) { return;  }
            if (!DA.GetDataList(1, supports)) { return; }
            if (!DA.GetDataList(2, loads)) { return; }
            if (!DA.GetDataList(3, sections)) { return; }

            StructureType S = StructureType.CreateFromMesh(mesh);
            S.AddSupports(supports.ConvertAll(x => (MiStrAnEngine.Support)x));
            S.SetDefaultMaterialOrientationAngles();
            S.SetSections(sections);

            //  S.SetSteelSections(thick);

            foreach (LoadType lt in loads)
            {
                S.AddLoad(lt);
            }

            if(S.dupSection)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Some elements have duplicate sections");

            DA.SetData(0, S);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{c8ca0c51-5fa3-464d-900b-20e0bb897330}"); }
        }
    }
}

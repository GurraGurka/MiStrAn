using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH
{
    public class MiStrAnComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public MiStrAnComponent()
          : base("MiStrAn", "MiStrAn",
              "Calculating...",
              "StrucAnalysis", "No SubCat")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Mesh", "Mesh", "Mesh to analyze", GH_ParamAccess.list);
            pManager.AddGeometryParameter("BC", "BC", "Zero displacement nodes", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Load", "Load", "Just nodes for loads", GH_ParamAccess.list);
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
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Mesh> meshes = new List<Mesh>();
            List<Point3d> bcNodes = new List<Point3d>();
            List<Point3d> bcLoads = new List<Point3d>();

            if (!DA.GetDataList(0, meshes)) { return;  }
            if (!DA.GetDataList(1, bcNodes)) { return; }
            if (!DA.GetDataList(2, bcLoads)) { return; }

            foreach (Mesh m in meshes)
            {
                int a = 5;
                int trtr = a + 3;
                // MiStrAnEngine.Structure mistStruc= StaticFunctions.ConvertGHMeshToStructure(m);
                StaticFunctions.ConvertGHMeshToStructure(m, bcNodes,bcLoads);
            }
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
            get { return new Guid("{ff91af2d-facb-4978-9f7c-7db4bbe98af2}"); }
        }
    }
}

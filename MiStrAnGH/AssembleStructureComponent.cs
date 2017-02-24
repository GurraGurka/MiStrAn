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
              "StrucAnalysis", "No SubCat")
        { }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Mesh", "Mesh", "Mesh to analyze", GH_ParamAccess.list);
            pManager.AddGeometryParameter("BC", "BC", "Zero displacement nodes", GH_ParamAccess.list);
            pManager.AddGeometryParameter("LoadPoints", "LoadPoints", "Just nodes for loads", GH_ParamAccess.list);
            pManager.AddVectorParameter("LoadVectors", "LoadVectors", " One Load vector for each node", GH_ParamAccess.list);
            pManager.AddMeshFaceParameter("DistLoadVFaces", "DistLoadFaces", " Distributed load mesh faces", GH_ParamAccess.list);
            pManager.AddVectorParameter("DistLoadVecs", "DistLoadVecs", "Nodes for distributed loads", GH_ParamAccess.list);
            pManager.AddNumberParameter("Thickness", "Thcikness", "Plate thickness (sson material input)", GH_ParamAccess.item);

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
            List<Mesh> meshes = new List<Mesh>();
            List<Point3d> bcNodes = new List<Point3d>();
            List<Point3d> LoadPts = new List<Point3d>();
            List<Vector3d> LoadVecs = new List<Vector3d>();
            List<Vector3d> distLoadVecs = new List<Vector3d>();
            List<MeshFace> distLoadFaces = new List<MeshFace>();
            double thick = new double();

            if (!DA.GetDataList(0, meshes)) { return;  }
            if (!DA.GetDataList(1, bcNodes)) { return; }
            if (!DA.GetDataList(2, LoadPts)) { return; }
            if (!DA.GetDataList(3, LoadVecs)) { return; }
            DA.GetDataList(4, distLoadFaces);
            DA.GetDataList(5, distLoadVecs);
            if (!DA.GetData(6, ref thick)) { return; }

            StructureType s = StaticFunctions.ConvertGHMeshToStructure(meshes[0], bcNodes,LoadPts,LoadVecs, distLoadFaces,distLoadVecs, thick);
            DA.SetData(0, s);
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

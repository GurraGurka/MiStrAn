using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class ChangeMaterialAnglesComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ChangeMaterialAnglesComponent()
          : base("Change Material Angles", "Change Material Angles",
              "Change the material angles for the elements in a structure",
              "MiStrAn", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter(), "Structure", "Structure", "Strucutre", GH_ParamAccess.item);
            pManager.AddNumberParameter("Angles", "angles", "material orientation angles. One for each element", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter(), "Structure", "Structure", "Strucutre", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            StructureType s = new StructureType();
            List<double> angles = new List<double>();

            if (!DA.GetData(0, ref s)) return;
            if (!DA.GetDataList(1, angles)) return;

            if (s.NumberOfElements != angles.Count && angles.Count != 1)
            {
                throw new Exception("Number of angles does not match number of elements");
            }

            s.K = null; // Sparse matrix is not serializable
            s.M = null;
            s = s.DeepClone();

            if (angles.Count > 1)
                s.SetMaterialOrientationAngles(angles);
            else
                s.SetMaterialOrientationAngles(angles[0]);

            s.RegenerateDMatrices();

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
            get { return new Guid("{dad1fd73-8bdd-41f6-ba93-d52d26cb94de}"); }
        }
    }
}
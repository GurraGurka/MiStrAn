﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MiStrAnGH.Components
{
    public class VisualiseResultsComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the VisualiseResultsComponent class.
        /// </summary>
        public VisualiseResultsComponent()
          : base("Visualise Results", "VisRes",
              "Description",
              "MiStrAn", "Visualise")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StructureParameter(), "Structure", "Structure", "MiStrAn analysed structure", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale deformation", "Scale", "Number to scale the deformation with", GH_ParamAccess.item, 100);
            pManager.AddNumberParameter("Bottom limiter", "lim0", "Number to scale the deformation with", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Top limiter", "lim1", "Number to scale the deformation with", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("Stress mesh type", "sType", "0 for smeared, 1 for pixelated", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Deformed mesh", "Def", "Deformed mesh, scaled by input scale", GH_ParamAccess.item);
            pManager.AddMeshParameter("von Mises stress plot", "vonMisMesh", "Stressplot using von Mises yield stress theory", GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            StructureType s = new StructureType();
            double t = 0;
            double lim0 = 0;
            double lim1 = 1;
            int sType = 0;

            if (!DA.GetData(0, ref s)) return;
            DA.GetData(1, ref t);
            DA.GetData(2, ref lim0);
            DA.GetData(3, ref lim1);
            DA.GetData(4, ref sType);

            Mesh def = s.GenereateDeformedMesh(t);
            Mesh vonM;

            if (sType == 1)
                vonM = s.GenerateStressMeshv2(lim0, lim1);
            else
                vonM = s.GenerateStressMeshv1(lim0, lim1);

            DA.SetData(0, def);
            DA.SetData(1, vonM);

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
            get { return new Guid("{6ee24c62-cbf2-4391-894d-f50d6163e573}"); }
        }
    }
}
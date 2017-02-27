using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Undo;

namespace MiStrAnGH
{
    public class StructureParameter : GH_Param<StructureType>
    {

        public StructureParameter() : 
            base("MiStrAn Structure", "Structure", "A MiStrAn structure", "MiStrAn", "no subcategory", GH_ParamAccess.item)
        { }


        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{76f14d2b-0860-4e9a-8d2c-3913888aed8d}"); 
            }
        }
    }

}

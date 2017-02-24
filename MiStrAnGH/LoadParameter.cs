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
    public class LoadParameter : GH_Param<LoadType>
    {

        public LoadParameter() : 
            base("MiStrAn Load", "Load", "A MiStrAn Load", "MiStrAn", "no subcategory", GH_ParamAccess.item)
        { }


        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{fc193b66-4580-4561-a98a-6aa5d6701ea1}"); 
            }
        }
    }

}

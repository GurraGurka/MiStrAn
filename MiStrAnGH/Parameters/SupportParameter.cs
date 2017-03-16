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
    public class SupportParameter : GH_Param<SupportType>
    {

        public SupportParameter() : 
            base("MiStrAn Support", "Support", "A MiStrAn support", "MiStrAn", "Params", GH_ParamAccess.item)
        { }


        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{507e3b4b-ffd9-4728-a6b2-6c1ba84f76f3}"); 
            }
        }
    }

}

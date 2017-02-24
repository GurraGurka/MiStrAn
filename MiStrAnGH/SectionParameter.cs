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
    public class SectionParameter : GH_Param<SectionType>
    {

        public SectionParameter() :
            base("MiStrAn Section", "Section", "A MiStrAn section", "StrucAnalysis", "no subcategory", GH_ParamAccess.item)
        { }


        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("{2254514d-9d1f-4130-94b0-cd6769f9d605}");
            }
        }
    }

}

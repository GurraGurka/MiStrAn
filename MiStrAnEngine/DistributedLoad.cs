using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiStrAnEngine
{
    public class DistributedLoad
    {
        public int shellIndex;

        // Field variables
        public Vector loadVec;

        //Constructor
        public DistributedLoad(int _shellIndex, Vector _loadVec)
        {
            shellIndex = _shellIndex;
            loadVec = _loadVec;
        }

    }
}

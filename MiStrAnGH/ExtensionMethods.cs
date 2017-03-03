using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MiStrAnGH
{
    public static class ExtensionMethods
    {
        public static Vector3d ToRhinoVector3d(this MiStrAnEngine.Vector3D v)
        {
            return new Vector3d(v.X, v.Y, v.Z);
        }

        public static Point3d ToRhinoPoint3d(this MiStrAnEngine.Vector3D v)
        {
            return new Point3d(v.X, v.Y, v.Z);
        }

        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}

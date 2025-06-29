using ETABSv1;
using PTDesign.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTDesign.Abtract
{
    internal class ShapeInstance
    {
        public static object SetShapeInstance(eFramePropType shapeType)
        {
            switch (shapeType)
            {
                case eFramePropType.Rectangular:
                    return new Rectangular();
                
            }
            return null;
        }
    }
}

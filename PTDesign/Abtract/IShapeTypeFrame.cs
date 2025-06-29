using ETABSv1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTDesign.Abtract
{
    interface IShapeTypeFrame
    {
        void GetSection(cSapModel _SapModel, string ProName);
        void Rectangular(cSapModel _SapModel, string ProName);
    }
}

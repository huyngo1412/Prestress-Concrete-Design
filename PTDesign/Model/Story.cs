using PTDesign.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTDesign.Model
{
    public class Story : ViewModelBase
    {
        private string _StoryName;
        public string StoryName
        {
            get { return _StoryName; }
            set
            {
                _StoryName = value;
            }
        }
        private double _Elevation;
        public double Elevation
        {
            get { return _Elevation; }
            set
            {
                _Elevation = value;
            }
        }
    }
}

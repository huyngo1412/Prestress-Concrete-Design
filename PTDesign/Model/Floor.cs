using PTDesign.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PTDesign.Model
{
    [Serializable]
    public class Floor : ViewModelBase
    {
        [XmlElement("Name")]
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [XmlElement("StoryName")]
        private string _StoryName;
        public string StoryName
        {
            get { return _StoryName; }
            set
            {
                _StoryName = value;
                OnPropertyChanged(nameof(StoryName));
            }
        }
        [XmlElement("Thickness")]
        private double _ThickNess;
        public double Thickness
        {
            get { return _ThickNess; }
            set
            {
                _ThickNess = value;
                OnPropertyChanged(nameof(_ThickNess));
            }
        }
        private List<(double X, double Y, double Z)> _Point;
        public List<(double X, double Y, double Z)> Point
        {
            get { return _Point; }
            set
            {
                _Point = value;
            }
        }
    }
}

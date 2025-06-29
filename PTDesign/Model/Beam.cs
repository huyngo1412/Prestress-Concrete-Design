using ETABSv1;
using PTDesign.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Xml.Serialization;

namespace PTDesign.Model
{
    [Serializable]
    [XmlInclude(typeof(Rectangular))]
    public class Beam : ViewModelBase
    {

        private string? _name;
        public string Name
        {
            get { return _name ?? string.Empty; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private Point3D _Start;
        public Point3D Start
        {
            get { return _Start; }
            set
            {
                _Start = value;
            }
        }
        private Point3D _End;
        public Point3D End
        {
            get { return _End; }
            set
            {
                _End = value;
            }
        }
        private string? _storyName;
        public string StoryName
        {
            get { return _storyName ?? string.Empty; }
            set
            {
                _storyName = value;
                OnPropertyChanged(nameof(StoryName));
            }
        }
        private string? _pointName1;
        public string PointName1
        {
            get { return _pointName1 ?? string.Empty; }
            set
            {
                _pointName1 = value;
                OnPropertyChanged(nameof(PointName1));
            }
        }
        private string? _pointName2;
        public string PointName2
        {
            get { return _pointName2 ?? string.Empty; }
            set
            {
                _pointName2 = value;
                OnPropertyChanged(nameof(PointName2));
            }
        }
        private eFramePropType _frameShapeType;
        public eFramePropType FrameShapeType
        {
            get { return _frameShapeType; }
            set
            {
                _frameShapeType = value;
            }
        }
        private object _shapeType;
        public object ShapeType
        {
            get { return _shapeType; }
            set
            {
                _shapeType = value;
            }
        }
        public double Length
        {
            get
            {
                double dx = Math.Abs(End.X - Start.X);
                double dy = Math.Abs(End.Y - Start.Y);
                double dz = Math.Abs(End.Z - Start.Z);
                return Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }
        }


        public void GetSection(cSapModel _SapModel, string ProName)
        {
            if (this.ShapeType is Rectangular)
            {
                Rectangular(_SapModel, ProName);
            }
        }
        public void Rectangular(cSapModel _SapModel, string ProName)
        {
            int ret;
            string filename = string.Empty;
            string Mat = string.Empty;
            double T3 = 0;
            double T2 = 0;
            int Color = 0;
            string Notes = string.Empty;
            string GUID = string.Empty;
            ret = _SapModel.PropFrame.GetRectangle(ProName, ref filename, ref Mat, ref T3, ref T2, ref Color, ref Notes, ref GUID);
            try
            {
                Rectangular rect = (Rectangular)this.ShapeType;
                rect.FrameShapeName = ProName;
                rect.Width = T2;
                rect.Height = T3;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.ToString());
                return;
            }
        }
    }
}

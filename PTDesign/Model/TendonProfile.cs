using PTDesign.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;

namespace PTDesign.Model
{
    public class TendonProfile : ViewModelBase
    {
        private string? _tendonName;
        public string? TendonName
        {
            get { return _tendonName; }
            set
            {
                _tendonName = value;
                OnPropertyChanged(nameof(TendonName));
            }
        }
        private double _a;
        public double A
        {
            get { return _a; }
            set
            {
                if (!CheckProperty(value, B)) return;
                _a = value;
                OnPropertyChanged(nameof(A));
            }
        }
        private double _b;
        public double B
        {
            get { return _b; }
            set
            {
                if (!CheckProperty(A, value)) return;
                _b = value;
                OnPropertyChanged(nameof(B));
            }
        }
        private List<Point3D> _point = new List<Point3D>();
        public List<Point3D> Point
        {
            get { return _point; }
            set
            {
                _point = value;
                OnPropertyChanged(nameof(Point));
                OnPropertyChanged(nameof(WrappedPoints));
            }
        }
        public List<Point3DWrapper> WrappedPoints
        {
            get
            {
                return Point.Select(p => new Point3DWrapper(p)).ToList();
            }
        }
        private Point3D _DecreasingPoints;
        public Point3D DecreasingPoints
        {
            get { return _DecreasingPoints; }
            set
            {
                _DecreasingPoints = value;
            }
        }
        private Point3D _FirstIncresingPoint;
        public Point3D FirstIncresingPoint
        {
            get => _FirstIncresingPoint;
            set
            {
                if (_FirstIncresingPoint != value)
                {
                    _FirstIncresingPoint = value;
                }
            }
        } // Điểm đầu tiên tăng dần

        private Point3D _LastIncresingPoint;
        public Point3D LastIncresingPoint
        {
            get => _LastIncresingPoint;
            set
            {
                if (_LastIncresingPoint != value)
                {
                    _LastIncresingPoint = value;
                }
            }
        } // Điểm đầu tiên tăng dần
        private bool CheckProperty(double a, double b)
        {
            if (a < 0 || b < 0 )
            {
                MessageBox.Show("A, B không thể bé hơn 0", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            return true;
        }


    }
    public class Point3DWrapper
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Point3DWrapper(Point3D pt)
        {
            X = pt.X;
            Y = pt.Y;
            Z = pt.Z;
        }
    }
}

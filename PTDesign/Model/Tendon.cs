using PTDesign.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace PTDesign.Model
{

    [XmlRoot("Tendon")]
    public class Tendon : ViewModelBase
    {
        public Tendon()
        {
            // Khởi tạo danh sách để tránh null và đảm bảo XML được serialize
            _point = new List<Point3D>();
        }
        private string _name;
        [XmlElement("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        // Các property không cần serialize sẽ được đánh dấu XmlIgnore
        [XmlIgnore]
        private string _storyName;
        [XmlIgnore]
        public string StoryName
        {
            get { return _storyName; }
            set
            {
                _storyName = value;
                OnPropertyChanged(nameof(StoryName));
            }
        }

        private List<Point3D> _point;
        [XmlArray("Points")]
        [XmlArrayItem("Point")]
        public List<Point3D> Point
        {
            get { return _point; }
            set { _point = value; }
        }

        private List<double> _globalZ;
        [XmlArray("GlobalZValues")]
        [XmlArrayItem("Z")]
        public List<double> GlobalZ
        {
            get { return _globalZ; }
            set { _globalZ = value; }
        }

        // Loại bỏ các property không cần in ra
        [XmlIgnore]
        private double _a;
        [XmlIgnore]
        public double A
        {
            get { return _a; }
            set
            {
                _a = value;
                OnPropertyChanged(nameof(A));
            }
        }

        [XmlIgnore]
        private double _b;
        [XmlIgnore]
        public double B
        {
            get { return _b; }
            set
            {
                _b = value;
                OnPropertyChanged(nameof(B));
            }
        }

        [XmlIgnore]
        private string _loadPattern;
        [XmlIgnore]
        public string LoadPattern
        {
            get { return _loadPattern; }
            set
            {
                _loadPattern = value;
                OnPropertyChanged(nameof(LoadPattern));
            }
        }

        [XmlIgnore]
        private double _loadTranfer;
        [XmlIgnore]
        public double LoadTranfer
        {
            get { return _loadTranfer; }
            set
            {
                _loadTranfer = value;
                OnPropertyChanged(nameof(LoadTranfer));
            }
        }

        [XmlIgnore]
        private ObservableCollection<TendonProfile> _listTendonProfile = new ObservableCollection<TendonProfile>();
        [XmlIgnore]
        public ObservableCollection<TendonProfile> ListTendonProfile
        {
            get => _listTendonProfile;
            set
            {
                _listTendonProfile = value;
                OnPropertyChanged(nameof(ListTendonProfile));
            }
        }
    }
}

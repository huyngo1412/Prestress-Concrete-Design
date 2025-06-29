using ETABSv1;
using Microsoft.Win32;
using PTDesign.Library;
using PTDesign.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PTDesign.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        int ret = -1;
        cHelper myHelper = new Helper();


        #region Properties
        private ObservableCollection<string> _loadPatternList = new ObservableCollection<string>();
        public ObservableCollection<string> LoadPatternList
        {
            get => _loadPatternList;
            set
            {
                _loadPatternList = value;
                OnPropertyChanged(nameof(LoadPatternList));
            }
        }
        private string _selectedLoadPattern;
        public string SelectedLoadPattern
        {
            get => _selectedLoadPattern;
            set
            {
                _selectedLoadPattern = value;
                OnPropertyChanged(nameof(SelectedLoadPattern));
            }
        }

        private ObservableCollection<string> _frameProperties = new ObservableCollection<string>();
        public ObservableCollection<string> FrameProperties
        {
            get => _frameProperties;
            set
            {
                _frameProperties = value;
                OnPropertyChanged(nameof(FrameProperties));
            }
        }
        private string _selectedFrame;
        public string SelectedFrame
        {
            get => _selectedFrame;
            set
            {
                _selectedFrame = value;
                OnPropertyChanged(nameof(SelectedFrame));
            }
        }
        private double _force;
        public double Force
        {
            get => _force;
            set
            {
                _force = value;
                OnPropertyChanged(nameof(_force));
            }
        }
        private ObservableCollection<Tendon> _listTendon = new ObservableCollection<Tendon>();
        public ObservableCollection<Tendon> ListTendon
        {
            get => _listTendon;
            set
            {
                _listTendon = value;
                OnPropertyChanged(nameof(ListTendon));
            }
        }
        private Tendon _selectedTendon;
        public Tendon SelectedTendon
        {
            get => _selectedTendon;
            set
            {
                _selectedTendon = value;
                OnPropertyChanged(nameof(SelectedTendon));
            }
        }
        #endregion


            #region ICommand
        public ICommand ConnectAPIEtabs { get; set; }
        public ICommand ReloadModel { get; set; }
        public ICommand GetSlectedTendon { get; set; }
        public ICommand ChangeTendonProfile { get; set; }
        public ICommand AssignLoadTendonV1 { get; set; }
        public ICommand Check { get; set; }
        public ICommand ExportData { get; set; }


        #endregion


        public MainViewModel()
        {
            ConnectAPIEtabs = new RelayCommand<object>((p) => true, (p) =>
            {
                try
                {
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kết nối API không thành công \nLỗi : {ex.Message}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
                var etabsObject = myHelper.GetObject("CSI.ETABS.API.ETABSObject");
                EtabsReader.Instance.InitializeEtabsObject(etabsObject);
                MessageBox.Show("Kết nối API thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                GetModelEtabs();
            });
            ReloadModel = new RelayCommand<object>((p) => true, (p) =>
            {
                try
                {
                    GetModelEtabs();
                    MessageBox.Show("Cập nhập dữ liệu thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show($"Cập nhập dữ liệu không thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            });
            GetSlectedTendon = new RelayCommand<object>((p) => true, (p) =>
            {
                try
                {
                    EtabsReader.Instance.GetSelectedObjTendon(_listTendon);
                    MessageBox.Show("Cập nhập dữ liệu thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show($"Cập nhập dữ liệu không thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            });
            Check = new RelayCommand<object>((p) => true, (p) =>
            {
                MessageBox.Show($"{DataContainer.Instance.Stories[1].Elevation}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            });
            AssignLoadTendonV1 = new RelayCommand<object>((p) => true, (p) =>
            {
                AssignLoad.AssignLoads(SelectedTendon, SelectedFrame, SelectedLoadPattern, Force);
                
            });
            ExportData = new RelayCommand<object>((p) => true, (p) =>
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = "C:\\";
                saveFileDialog.Filter = "Text files (*.xml)|*.xml|All files (*.*)|*.*";
                saveFileDialog.Title = "Save your file";
                if (saveFileDialog.ShowDialog() == true)
                {
                    FileManager.Instance.SaveAs(saveFileDialog.FileName);
                }
                
            });

        }
        private void GetModelEtabs()
        {
            EtabsReader.Instance.GetAll();
            EtabsReader.Instance.ReadDataFrame();
            EtabsReader.Instance.ReadDataFloor();
            EtabsReader.Instance.ReadLoadPatternList(_loadPatternList);
            EtabsReader.Instance.GetFrameProperties(_frameProperties);
            EtabsReader.Instance.GetTendonObject();
            Segment.AdjustTendonZ();
            //MessageBox.Show(DataContainer.Instance.Tendons[0].GlobalZ.Count() + "");
            //MessageBox.Show(DataContainer.Instance.Floors.Count() + "");

        }
    }
}

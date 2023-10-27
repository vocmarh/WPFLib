using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CheckBox.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using WallUtils = CheckBox.Model.WallUtils;

namespace CheckBox
{
    public class MainViewViewModel : INotifyPropertyChanged
    {
        private bool _isCategoryWall;
        private bool _isCategoryFloor;
        private bool _isCategoryRebar;
        private ObservableCollection<ElementData> data;
        private bool isInstance = true;
        public bool IsInstance
        {
            get { return isInstance; }
            set { isInstance = value; NotifyPropertyChanged(nameof(IsInstance)); }
        }       

        public bool IsCategoryWall
        {
            get { return _isCategoryWall; }
            set { _isCategoryWall = value; NotifyPropertyChanged(nameof(IsCategoryWall)); }
        }
        public bool IsCategoryFloor
        {
            get { return _isCategoryFloor; }
            set { _isCategoryFloor = value; NotifyPropertyChanged(nameof(IsCategoryFloor)); }
        }
        public bool IsCategoryRebar
        {
            get { return _isCategoryRebar; }
            set { _isCategoryRebar = value; NotifyPropertyChanged(nameof(IsCategoryRebar)); }
        }
        public ObservableCollection<ElementData> Data 
        {
            get => data;
            set { data = value; NotifyPropertyChanged(nameof(Data));
    }
}
        private void NotifyPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }            

        private ExternalCommandData _commandData;
        private Document _doc;

        public DelegateCommand ShowListElement { get; }
        public DelegateCommand GetDataBase { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData; 
            GetDataBase = new DelegateCommand(OnGetDataBase);
        }

        private void OnGetDataBase()
        {
            Data = new ObservableCollection<ElementData>();
            ProcessElements processElements = new ProcessElements();

            if (IsCategoryWall)
            {
                WallUtils wallUtils = new WallUtils();
                List<Element> walls = wallUtils.GetElements(_commandData);
                processElements.GetProcessElements(walls, Data);
            }

            if (IsCategoryFloor)
            {
                FloorUtils floorUtils = new FloorUtils();
                List<Element> floors = floorUtils.GetElements(_commandData);
                processElements.GetProcessElements(floors, Data);
            }

            if (IsCategoryRebar)
            {
                RebarUtils rebarUtils = new RebarUtils();
                List<Element> rebars = rebarUtils.GetElements(_commandData);
                processElements.GetProcessElements(rebars, Data);
            }
        }

        private void OnGetDataBase1()
        {
            Data = new ObservableCollection<ElementData>();
            if (IsCategoryWall)
            {
                WallUtils wallUtils = new WallUtils();
                List<Element> walls = wallUtils.GetElements(_commandData);

                double volume = 0;


                foreach (Element wall in walls)
                {
                    Parameter volPar = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                    volume = volPar.AsDouble();
                    double volumeMeters = UnitUtils.ConvertFromInternalUnits(volume, DisplayUnitType.DUT_CUBIC_METERS);
                    try
                    {
                        Data.Add(new ElementData
                        {
                            ElementName = wall.Name,
                            Volume = volumeMeters,
                            CategoryName = wall.Category.Name
                        });
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("Error", ex.Message);
                    }
                }
            }
            if (IsCategoryFloor)
            {
                FloorUtils floorUtils = new FloorUtils();
                List<Element> floors = floorUtils.GetElements(_commandData);
                double volume = 0;

                foreach (Element floor in floors)
                {
                    Parameter volPar = floor.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                    volume = volPar.AsDouble();
                    double volumeMeters = UnitUtils.ConvertFromInternalUnits(volume, DisplayUnitType.DUT_CUBIC_METERS);
                    try
                    {
                        Data.Add(new ElementData
                        {
                            ElementName = floor.Name,
                            Volume = volumeMeters,
                            CategoryName = floor.Category.Name
                        });
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("Error", ex.Message);
                    }
                }
            }
            if (IsCategoryRebar)
            {
                RebarUtils rebarUtils = new RebarUtils();
                List<Element> rebars = rebarUtils.GetElements(_commandData);

                foreach (Element rebar in rebars)
                {
                    try
                    {
                        Data.Add(new ElementData
                        {
                            ElementName = rebar.Name,

                            CategoryName = rebar.Category.Name
                        });
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("Error", ex.Message);
                    }
                }
            }
        }


        private void OnShowListElement()
        {
            WallUtils wallUtils = new WallUtils();
            FloorUtils floorUtils = new FloorUtils();
            RebarUtils rebarUtils = new RebarUtils();

            // Создайте словарь для хранения списков элементов для разных категорий.
            Dictionary<string, List<string>> categoryElements = new Dictionary<string, List<string>>();

            // Заполняйте словарь для каждой категории.
            categoryElements["Wall"] = wallUtils.GetElementName(_commandData);
            categoryElements["Floor"] = floorUtils.GetElementName(_commandData);
            categoryElements["Rebar"] = rebarUtils.GetElementName(_commandData);
            //categoryElements["Framing"] = floorUtils.GetElementName(_commandData);
            // Добавьте другие категории таким же образом.

            // Создайте строку для хранения результатов.
            string elementName = string.Empty;

            // Проверьте флажки для каждой категории и объедините соответствующие списки элементов.
            if (IsCategoryWall)
            {
                elementName += "\nWall Elements:\n";
                elementName += string.Join("\n", categoryElements["Wall"]) + "\n";
            }

            if (IsCategoryFloor)
            {
                elementName += "\nFloor Elements:\n";
                elementName += string.Join("\n", categoryElements["Floor"]) + "\n";
            }

            if (IsCategoryRebar)
            {
                elementName += "\nRebar Elements:\n";
                elementName += string.Join("\n", categoryElements["Rebar"]) + "\n";
            }

            // Добавьте другие проверки для остальных категорий.

            if (string.IsNullOrEmpty(elementName))
            {
                TaskDialog.Show("Title", "Ничего не выбрано!");
            }
            else
            {
                TaskDialog.Show("Title", elementName);
            }            
        }       

        public event EventHandler HideRequest;

        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;

        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CloseRequest;
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }

}

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;


namespace TestWpfApp
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<ButtonViewModel> _buttons;

        public ObservableCollection<ButtonViewModel> Buttons
        {
            get => _buttons;
            set
            {
                if (Set(ref _buttons, value))
                {
                    OnPropertyChanged(nameof(Buttons)); // Notify about the change in Buttons property
                }
            }
        }

        private string _searchFilter;
        public CollectionViewSource CollViewSource { get; set; }
        public string SearchFilter
        {
            get { return _searchFilter; }
            set
            {
                _searchFilter = value;
                if (!string.IsNullOrEmpty(_searchFilter))
                    AddFilter();

                CollViewSource.View.Refresh(); // important to refresh your View
            }
        }

        public MainViewModel()
        {
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            _buttons = new ObservableCollection<ButtonViewModel>();

            // Add buttons one by one with different sizes
            _buttons.Add(new ButtonViewModel
            {
                Content = "TranslateCommand",
                Command = new DelegateCommand(() => ButtonClick(1)),
                Width = 210,
                Height = 36,
                BorderBrush = Brushes.Gray, // Set the desired BorderBrush
                BorderThickness = new Thickness(1)
            });

            _buttons.Add(new ButtonViewModel
            {
                Content = "ReviseCommand",
                Command = new DelegateCommand(() => ButtonClick(2)),
                Width = 121,
                Height = 36,
                BorderBrush = Brushes.Gray, // Set the desired BorderBrush
                BorderThickness = new Thickness(0)
            });

            _buttons.Add(new ButtonViewModel
            {
                Content = "Extract the main information",
                Command = new DelegateCommand(() => ButtonClick(3)),
                Width = 230,
                Height = 36,
                BorderBrush = Brushes.Gray, // Set the desired BorderBrush
                BorderThickness = new Thickness(0)
            });

            _buttons.Add(new ButtonViewModel
            {
                Content = "More",
                Command = new DelegateCommand(() => ButtonClick(4)),
                Width = 98,
                Height = 36,
                BorderBrush = Brushes.Gray, // Set the desired BorderBrush
                BorderThickness = new Thickness(0)
            });


            CollViewSource = new CollectionViewSource();
            CollViewSource.Source = Buttons;
        }

        private void ButtonClick(int buttonNumber)
        {
            Console.WriteLine($"Button {buttonNumber} clicked");
        }

        private void AddFilter()
        {
            CollViewSource.Filter -= new FilterEventHandler(Filter);
            CollViewSource.Filter += new FilterEventHandler(Filter);

            OnPropertyChanged(nameof(Buttons));
        }

        private void Filter(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as ButtonViewModel;
            if (src == null)
            {
                e.Accepted = false; // Non-ButtonViewModel items should not be accepted
                return;
            }

            if (string.IsNullOrEmpty(SearchFilter))
            {
                e.Accepted = true; // If SearchFilter is empty, accept all items
            }
            else if (src.Content != null && src.Content.Contains(SearchFilter))
            {
                e.Accepted = true; // Accept items whose content contains the SearchFilter
            }
            else
            {
                e.Accepted = false; // Reject items that don't match the filter
            }
        }
    }
    public class ButtonViewModel : ViewModelBase
    {
        private string _content;
        private ICommand _command;
        private double _width;
        private double _height;

        public string Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        public ICommand Command
        {
            get => _command;
            set => Set(ref _command, value);
        } 

        public double Width
        {
            get => _width;
            set => Set(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => Set(ref _height, value);
        }

        private Brush _borderBrush;
        private Thickness _borderThickness;

        public Brush BorderBrush
        {
            get => _borderBrush;
            set => Set(ref _borderBrush, value);
        }

        public Thickness BorderThickness
        {
            get => _borderThickness;
            set => Set(ref _borderThickness, value);
        }
    }

}

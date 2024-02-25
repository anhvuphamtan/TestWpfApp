using Jarvis_Windows.Sources.Utils.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Jarvis_Windows.Sources.Utils.Services;
public class MenuButton : ViewModelBase
{
    private string _content;
    private string _icon;
    private string _commandParameter;
    private RelayCommand _command;
    private double _width;
    private bool _visibility;
    private Thickness _margin;
    public string Content
    {
        get { return _content; }
        set
        {
            _content = value;
            OnPropertyChanged();
        }
    }

    public string Icon
    {
        get { return _icon; }
        set
        {
            _icon = value;
            OnPropertyChanged();
        }
    }

    public bool Visibility
    {
        get { return _visibility; }
        set
        {
            _visibility = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand Command
    {
        get { return _command; }
        set
        {
            _command = value;
            OnPropertyChanged();
        }
    }

    public string CommandParameter
    {
        get { return _commandParameter; }
        set
        {
            _commandParameter = value;
            OnPropertyChanged();
        }
    }

    public double Width
    {
        get { return _width; }
        set
        {
            _width = value;
            OnPropertyChanged();
        }
    }

    public Thickness Margin
    {
        get { return _margin; }
        set
        {
            _margin = value;
            OnPropertyChanged();
        }
    }
}

public class AIActionTemplate
{
    public ObservableCollection<MenuButton> FixedAIActionList { get; set; } = new ObservableCollection<MenuButton>
    {
        new MenuButton
        {
            Icon = "🌐",
            Content = "Translate it",
            Visibility = true,
            Command = null,
            CommandParameter = "Translate it",
            Width = 221,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "✏️",
            Content = "Revise it",
            Visibility = true,
            Command = null,
            CommandParameter = "Revise it",
            Width = 108,
            Margin = new Thickness(0, 0, 10, 10)
        }
    };
    public ObservableCollection<MenuButton> DynamicAIActionList { get; set; } = new ObservableCollection<MenuButton>
    {
        new MenuButton
        {
            Icon = "📌",
            Content = "Extract the main information",
            Visibility = true,
            Command = null,
            CommandParameter = "Extract the main information",
            Width = 230,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "🚀",
            Content = "More",
            Visibility = true,
            Command = null,
            CommandParameter = "More",
            Width = 90,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "👋",
            Content = "Make it constructive",
            Visibility = false,
            Command = null,
            CommandParameter = "Make it constructive",
            Width = 179,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "🖼️",
            Content = "Make it more detailed",
            Visibility = false,
            Command = null,
            CommandParameter = "Make it more detailed",
            Width = 189,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "🎯",
            Content = "Make it persuasive",
            Visibility = false,
            Command = null,
            CommandParameter = "Make it persuasive",
            Width = 171,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "🖋️",
            Content = "Paraphrase it",
            Visibility = false,
            Command = null,
            CommandParameter = "Paraphrase it",
            Width = 137,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "📝",
            Content = "Summarize it",
            Visibility = false,
            Command = null,
            CommandParameter = "Summarize it",
            Width = 135,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "✂️",
            Content = "Simplify it",
            Visibility = false,
            Command = null,
            CommandParameter = "Simplify it",
            Width = 114.695,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "📜",
            Content = "Give a quote",
            Visibility = false,
            Command = null,
            CommandParameter = "Give a quote",
            Width = 133,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "🙂",
            Content = "Give a random name",
            Visibility = false,
            Command = null,
            CommandParameter = "Give a random name",
            Width = 184,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "✉️",
            Content = "Give an asking for help email template",
            Visibility = false,
            Command = null,
            CommandParameter = "Give an asking for help email template",
            Width = 292,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "📧",
            Content = "Give a thank you for help email template",
            Visibility = false,
            Command = null,
            CommandParameter = "Give a thank you for help email template",
            Width = 305,
            Margin = new Thickness(0, 0, 10, 10)
        },
        new MenuButton
        {
            Icon = "🔼",
            Content = "Less",
            Visibility = false,
            Command = null,
            CommandParameter = "Less",
            Width = 90,
            Margin = new Thickness(0, 0, 10, 10)
        },
    };
}

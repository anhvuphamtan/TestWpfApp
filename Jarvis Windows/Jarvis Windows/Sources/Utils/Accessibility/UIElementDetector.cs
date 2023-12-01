using Jarvis_Windows.Sources.MVVM.ViewModels;
using System;
using System.Windows.Automation;
using System.Windows;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Windows.Media;

namespace Jarvis_Windows.Sources.Utils.Accessibility;

public class UIElementDetector
{
    private static UIElementDetector? Instance;
    private AutomationElement? _focusingElement;
    AutomationFocusChangedEventHandler? _focusChangedEventHandler;
    private PopupDictionaryService _popupDictionaryService;

    public PopupDictionaryService PopupDictionaryService
    {
        get { return _popupDictionaryService; }
        set => _popupDictionaryService = value;
    }

    public static UIElementDetector GetInstance()
    {
        if (Instance == null)
            Instance = new UIElementDetector();
        return Instance;
    }

    private UIElementDetector(PopupDictionaryService popupDictionaryService)
    {
        PopupDictionaryService = popupDictionaryService;
    }

    public UIElementDetector()
    {
    }

    public void SubscribeToElementFocusChanged()
    {
        _focusChangedEventHandler = new AutomationFocusChangedEventHandler(OnElementFocusChanged);
        Automation.AddAutomationFocusChangedEventHandler(_focusChangedEventHandler);
    }

    private void OnElementFocusChanged(object sender, AutomationFocusChangedEventArgs e)
    {
        AutomationElement? newFocusElement = sender as AutomationElement;

        ///FIXME: Crashing Not Available Element
        if (newFocusElement != null
            && newFocusElement.Current.IsKeyboardFocusable
            && newFocusElement.Current.HasKeyboardFocus
            && (newFocusElement.Current.LocalizedControlType.Equals("edit") || newFocusElement.Current.LocalizedControlType.Equals("group")
            || newFocusElement.Current.LocalizedControlType.Equals("custom"))
            && newFocusElement.Current.ControlType.Equals(ControlType.Edit))
        {
            _focusingElement = newFocusElement;
            SubscribeToRectBoundingChanged();

            _popupDictionaryService.ShowJarvisAction(true);
            _popupDictionaryService.ShowMenuOperations(false);
            _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation());
            _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation());
        }
    }

    private Point CalculateElementLocation()
    {
        Point placementPoint = new Point(0, 0);
        if (_focusingElement != null)
        {
            Rect elementRectBounding = _focusingElement.Current.BoundingRectangle;
            placementPoint.X = elementRectBounding.Left + elementRectBounding.Width * 0.96;
            placementPoint.Y = elementRectBounding.Top + elementRectBounding.Height * 0.5;
        }
        return placementPoint;
    }

    private void SubscribeToRectBoundingChanged()
    {
        IntPtr activeApplicationHandle = NativeUser32API.GetForegroundWindow();
        if (activeApplicationHandle != IntPtr.Zero)
        {
            AutomationElement? automationElement = AutomationElement.FromHandle(activeApplicationHandle);
            if (automationElement != null)
            {
                AutomationPropertyChangedEventHandler propertyChanged = new AutomationPropertyChangedEventHandler(OnElementPropertyChanged);
                Automation.AddAutomationPropertyChangedEventHandler(automationElement, TreeScope.Element, propertyChanged, AutomationElement.BoundingRectangleProperty);
            }
        }

        AutomationPropertyChangedEventHandler propertyChangedHandler = new AutomationPropertyChangedEventHandler(OnElementPropertyChanged);
        Automation.AddAutomationPropertyChangedEventHandler(_focusingElement, TreeScope.Element, propertyChangedHandler, AutomationElement.BoundingRectangleProperty);
    }

    private void OnElementPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
    {
        _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation());
        _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation());

        AutomationPropertyChangedEventHandler propertyChangedHandler = new AutomationPropertyChangedEventHandler(OnElementPropertyChanged);
        Automation.RemoveAutomationPropertyChangedEventHandler(_focusingElement, propertyChangedHandler);
    }

    public void SetValueForFocusingEditElement(String? value)
    {
        try
        {
            if (_focusingElement != null)
            {
                ValuePattern? valuePattern = _focusingElement.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                if (valuePattern != null)
                    valuePattern.SetValue(value);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void SetValueForEditElement(AutomationElement element, string text)
    {
        try
        {
            object? patternObj = element.GetCurrentPattern(ValuePattern.Pattern);
            if (patternObj != null)
            {
                ValuePattern? valuePattern = patternObj as ValuePattern;
                if (valuePattern != null)
                {
                    valuePattern.SetValue(text);
                }
            }

        }
        catch (Exception)
        {
            throw;
        }
    }

    public string GetTextFromFocusingEditElement()
    {
        try
        {
            if (_focusingElement != null)
            {
                ValuePattern? valuePattern = _focusingElement.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                if (valuePattern != null)
                    return valuePattern.Current.Value;
            }
        }
        catch (Exception)
        {
            return String.Empty;
        }
        return String.Empty;
    }
}

﻿using System;
using System.Windows.Automation;
using System.Windows;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Diagnostics;

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

    public void UnSubscribeToElementFocusChanged()
    {
        _focusChangedEventHandler = new AutomationFocusChangedEventHandler(OnElementFocusChanged);
        Automation.RemoveAutomationFocusChangedEventHandler(_focusChangedEventHandler);
    }

    private void OnElementFocusChanged(object sender, AutomationFocusChangedEventArgs e)
    {
        try
        {
            AutomationElement? newFocusElement = sender as AutomationElement;
            Debug.WriteLine(newFocusElement.Current.ControlType.ProgrammaticName);

            if (newFocusElement != null &&
                (newFocusElement.Current.ControlType.ProgrammaticName == "ControlType.Custom"
                    || newFocusElement.Current.ControlType.ProgrammaticName == "ControlType.Edit"
                    || newFocusElement.Current.ControlType.ProgrammaticName == "ControlType.Document"))
            {
                _focusingElement = newFocusElement;

                SubscribeToRectBoundingChanged();

                _popupDictionaryService.ShowJarvisAction(true);
                _popupDictionaryService.ShowMenuOperations(false);
                _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation());
                _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation());
            }
            else
            {
                IntPtr currentAppHandle = NativeUser32API.GetForegroundWindow();
                AutomationElement foregroundApp = AutomationElement.FromHandle(currentAppHandle);
                if (foregroundApp != null)
                {
                    if (foregroundApp.Current.Name.Equals("Jarvis MainView"))
                        return;
                }

                _popupDictionaryService.ShowJarvisAction(false);
                _popupDictionaryService.ShowMenuOperations(false);
            }
        }
         catch (ElementNotAvailableException) 
        {       
        }
    }

    private Point CalculateElementLocation()
    {
        Point placementPoint = new Point(0, 0);
        if (_focusingElement != null)
        {
            try
            {
                Rect elementRectBounding = _focusingElement.Current.BoundingRectangle;
                placementPoint.X = elementRectBounding.Left + elementRectBounding.Width;
                placementPoint.Y = elementRectBounding.Top + elementRectBounding.Height * 0.5;
            }
            catch (ElementNotAvailableException ex)
            {
                Console.WriteLine($"Element is not available: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        return placementPoint;
    }

    private void SubscribeToRectBoundingChanged()
    {
        if(_focusingElement != null)
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
    }

    private void OnElementPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
    {
        if(_focusingElement != null)
        {
            _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation());
            _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation());

            AutomationPropertyChangedEventHandler propertyChangedHandler = new AutomationPropertyChangedEventHandler(OnElementPropertyChanged);
            Automation.RemoveAutomationPropertyChangedEventHandler(_focusingElement, propertyChangedHandler);
        }
    }   

    private AutomationElement FindChildEditElement(AutomationElement parrentElement)
    {
        System.Windows.Automation.Condition editCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit);

        AutomationElement editElement = parrentElement.FindFirst(TreeScope.Descendants, editCondition);

        return editElement;
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
        catch (ElementNotAvailableException)
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
                ValuePattern? valuePattern = null;
                object valuePatternObj;
                if (_focusingElement.TryGetCurrentPattern(ValuePattern.Pattern, out valuePatternObj))
                {
                    valuePattern = valuePatternObj as ValuePattern;
                    if (valuePattern != null)
                        return valuePattern.Current.Value;
                }
            }
        }
        catch (ElementNotAvailableException)
        {
            return String.Empty;
        }
        return String.Empty;
    }
}

using System;
using System.Windows.Automation;
using System.Windows;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Diagnostics;
using System.Xaml;
using System.Threading.Tasks;
using System.Threading;

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

    private bool IsEditableElement(AutomationElement? automationElement)
    {
        if (automationElement != null)
        {
            Object patternObj;
            if (automationElement.TryGetCurrentPattern(ValuePattern.Pattern, out patternObj))
            {
                ValuePattern? valuePattern = patternObj as ValuePattern;
                if (valuePattern != null)
                    return true;
            }

            if (automationElement.TryGetCurrentPattern(TextPattern.Pattern, out patternObj))
            {
                TextPattern? textPattern = patternObj as TextPattern;
                if (textPattern != null)
                    return true;
            }
        }
        return false;
    }

    private void OnElementFocusChanged(object sender, AutomationFocusChangedEventArgs e)
    {
        AutomationElement? newFocusElement = sender as AutomationElement;
        Debug.WriteLine($"↘️ ↘️ ↘️ Focused to : {newFocusElement?.Current.ControlType.ProgrammaticName}");

        AutomationElement rootElement = AutomationElement.RootElement;
        if (rootElement != null)
        {
            Debug.WriteLine($"🔗🔗🔗 Root Element: {newFocusElement?.Current.ControlType.ProgrammaticName}");
        }

        if (newFocusElement != null)
        {
            if (IsEditableElement(newFocusElement))
            {
                _focusingElement = newFocusElement;
                SubscribeToElementPropertyChanged(_focusingElement, AutomationElement.BoundingRectangleProperty);

                _popupDictionaryService.ShowJarvisAction(true);
                _popupDictionaryService.ShowMenuOperations(false);
                _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation());
                _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation());
            }
            else
            {
                try
                {
                    IntPtr currentAppHandle = NativeUser32API.GetForegroundWindow();
                    AutomationElement foregroundApp = AutomationElement.FromHandle(currentAppHandle);
                    if (foregroundApp != null)
                    {
                        if (foregroundApp.Current.Name.Equals("Jarvis MainView"))
                            return;
                    }
                    _focusingElement = null;
                    _popupDictionaryService.ShowJarvisAction(false);
                    _popupDictionaryService.ShowMenuOperations(false);
                }
                catch (ArgumentException)
                {
                    Debug.WriteLine($"❌❌❌ Argument Exception");
                }
                catch (NullReferenceException)
                {
                    Debug.WriteLine($"Null reference exception");
                }
                catch (ElementNotAvailableException)
                {
                    Debug.WriteLine($"❌❌❌ Element is not available");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌❌❌ Exception: {ex.Message}");
                }

            }
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
            catch (NullReferenceException)
            {
                Debug.WriteLine($"Null reference exception");
            }
            catch (ElementNotAvailableException ex)
            {
                Console.WriteLine($"Element is not available: {ex.Message}");
                _popupDictionaryService.ShowJarvisAction(false);
                _popupDictionaryService.ShowMenuOperations(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                _popupDictionaryService.ShowJarvisAction(false);
                _popupDictionaryService.ShowMenuOperations(false);
            }
        }
        return placementPoint;
    }

    private void SubscribeToElementPropertyChanged(AutomationElement? trackingElement, AutomationProperty? trackingProperty)
    {
        try
        {
            if (trackingElement != null)
            {
                TreeScope treeScope = TreeScope.Element;
                if (trackingProperty == AutomationElement.BoundingRectangleProperty)
                    treeScope = TreeScope.Parent;
                else if (trackingProperty == AutomationElement.IsOffscreenProperty)
                    treeScope = TreeScope.Ancestors;
                else if (trackingProperty == AutomationElement.IsKeyboardFocusableProperty)
                    treeScope = TreeScope.Element;

                AutomationPropertyChangedEventHandler propertyChangedHandler = new AutomationPropertyChangedEventHandler(OnElementPropertyChanged);
                Automation.AddAutomationPropertyChangedEventHandler(trackingElement, treeScope, propertyChangedHandler, trackingProperty);
            }
        }
        catch (NullReferenceException)
        {
            Debug.WriteLine($"Null reference exception");
            _popupDictionaryService.ShowJarvisAction(false);
            _popupDictionaryService.ShowMenuOperations(false);
        }
        catch (ElementNotAvailableException)
        {
            Debug.WriteLine($"Element is not available");
            _popupDictionaryService.ShowJarvisAction(false);
            _popupDictionaryService.ShowMenuOperations(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred: {ex.Message}");
            _popupDictionaryService.ShowJarvisAction(false);
            _popupDictionaryService.ShowMenuOperations(false);
        }
    }

    private void OnElementPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
    {
        var automationElement = sender as AutomationElement;
        if (e.Property == AutomationElement.BoundingRectangleProperty)
        {
            Debug.WriteLine($"🟧🟧🟧 {automationElement?.Current.Name} Bounding Rectangle Changed");
            _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation());
            _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation());
        }
        else if (e.Property == AutomationElement.IsOffscreenProperty)
        {
            Debug.WriteLine($"👁️👁️👁️ {automationElement?.Current.ControlType.ProgrammaticName} Offscreen Property Changed");
            _popupDictionaryService.ShowJarvisAction(false);
            _popupDictionaryService.ShowMenuOperations(false);
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
        int timeoutMilliseconds = 200;
        if (_focusingElement != null)
        {

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task setValueTask = Task.Run(() =>
            {
                Debug.WriteLine($"❌❌❌ Set Value of {_focusingElement.Current.ClassName} {_focusingElement.Current.ControlType.ProgrammaticName}");
                try
                {
                    ValuePattern? valuePattern = _focusingElement.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                    if (valuePattern != null)
                        valuePattern.SetValue(value);
                }
                catch (NullReferenceException)
                {
                    Debug.WriteLine($"Null reference exception");
                }
                catch (ElementNotAvailableException)
                {
                    Debug.WriteLine($"❌❌❌ Element is not available");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌❌❌ Exception: {ex.Message}");
                }
            });

            if (!setValueTask.Wait(timeoutMilliseconds, cancellationToken))
            {
                cancellationTokenSource.Cancel();
                throw new TimeoutException("The SetValueForFocusingEditElement operation has timed out.");
            }
        }
    }

    public string GetTextFromFocusingEditElement()
    {
        int timeoutMilliseconds = 200;
        string result = string.Empty;

        if (_focusingElement != null)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task<string> getValueTask = Task.Run(() =>
            {
                Debug.WriteLine($"❌❌❌ Get Value of {_focusingElement.Current.ClassName} {_focusingElement.Current.ControlType.ProgrammaticName}");
                try
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
                catch (NullReferenceException)
                {
                    Debug.WriteLine($"Null reference exception");
                }
                catch (ElementNotAvailableException)
                {
                    Debug.WriteLine($"❌❌❌ Element is not available");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌❌❌ Exception: {ex.Message}");
                }
                return string.Empty;
            });

            if (!getValueTask.Wait(timeoutMilliseconds, cancellationToken))
            {
                cancellationTokenSource.Cancel();
                throw new TimeoutException("The GetTextFromFocusingEditElement operation has timed out.");
            }

            result = getValueTask.Result;
        }

        return result;
    }
}
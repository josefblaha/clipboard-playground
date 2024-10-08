using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace WpfCopyTest;

public partial class MainWindow
{
    private int _nextNumber = 1;
    private IntPtr _hwnd;

    public MainWindow()
    {
        InitializeComponent();
        SourceInitialized += OnSourceInitialized;
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        _hwnd = new WindowInteropHelper(this).Handle;
        AddLog($"Current window HWND: {_hwnd:X}");
        
        var hwndSource = HwndSource.FromHwnd(_hwnd);
        if (hwndSource == null)
        {
            throw new Exception("No HWND source");
        }

        hwndSource.AddHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
    {
        if (msg is SafeNativeMethods.WM_RENDERFORMAT)
        {
            var ownerHwnd = SafeNativeMethods.GetClipboardOwner();
            AddLog($"WM_RENDERFORMAT. Owner HWND: {ownerHwnd:X}");
            WindowsClipboard.RenderFormat(wparam);
            handled = true;
        }
        return IntPtr.Zero;
    }

    private void CopyButton_OnClick(object sender, RoutedEventArgs e)
    {
        var copyText = (_nextNumber++).ToString();
        var sw = Stopwatch.StartNew();

        try
        {
            if (WpfClipboardCopyRadioButton.IsChecked == true)
            {
                Clipboard.SetText(copyText);
            }
            else if (Win32CopyRadioButton.IsChecked == true)
            {
                WindowsClipboard.SetText(copyText);
            }
            else if (Win32DelayedCopyRadioButton.IsChecked == true)
            {
                WindowsClipboard.SetTextDelayed(copyText, _hwnd);
            }

            sw.Stop();

            AddLog($"Copied {copyText} in {sw.Elapsed}");
        }
        catch (Exception ex)
        {
            sw.Stop();
            AddLog($"Copy failed with '{ex.Message}' in {sw.Elapsed}");
        }
    }

    private void ClearButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.Clear();
            AddLog("Cleared clipboard");
        }
        catch (Exception ex)
        {
            AddLog($"Clear failed with '{ex.Message}'");
        }
    }

    private void AddLog(string message)
    {
        LogListBox.Items.Insert(0, message);
    }
}
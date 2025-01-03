﻿using System.Diagnostics;
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
        if (msg == SafeNativeMethods.WM_RENDERFORMAT)
        {
            var ownerHwnd = SafeNativeMethods.GetClipboardOwner();
            AddLog($"WM_RENDERFORMAT. Owner HWND: {ownerHwnd:X}");
            LogOpenClipboardWindow();
            WindowsClipboard.RenderFormat(wparam);
            handled = true;
        }

        if (msg == SafeNativeMethods.WM_DESTROYCLIPBOARD)
        {
            AddLog("WM_DESTROYCLIPBOARD");
        }

        return IntPtr.Zero;
    }

    private void CopyButton_OnClick(object sender, RoutedEventArgs e)
    {
        Copy();
    }

    private void CopyMultipleTimesButton_OnClick(object sender, RoutedEventArgs e)
    {
        for (int i = 0; i < 10; i++)
        {
            Copy();
        }
    }

    private void Copy()
    {
        var copyText = GetNextCopyText();
        var sw = Stopwatch.StartNew();

        try
        {
            if (WpfClipboardCopyRadioButton.IsChecked == true)
            {
                Clipboard.SetText(copyText);
            }
            else if (WpfClipboardNoFlushCopyRadioButton.IsChecked == true)
            {
                Clipboard.SetDataObject(copyText, false);
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

            AddLog($"Copied {GetCopyValuePreview(copyText)} in {sw.Elapsed}");
        }
        catch (Exception ex)
        {
            sw.Stop();
            AddLog($"Copy {GetCopyValuePreview(copyText)} failed with '{ex.Message}' in {sw.Elapsed}");
            LogOpenClipboardWindow();
        }
    }

    private string GetNextCopyText()
    {
        return NumberValueRadioButton.IsChecked == true
            ? (_nextNumber++).ToString()
            : new string('a', 1_000_000_000);
    }

    private string GetCopyValuePreview(string copyText)
    {
        return NumberValueRadioButton.IsChecked == true
            ? copyText
            : $"(string of length ${copyText.Length})";
    }

    private void WpfClearButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.Clear();
            AddLog("Cleared clipboard");
        }
        catch (Exception ex)
        {
            AddLog($"Clear failed with '{ex.Message}'");
            LogOpenClipboardWindow();
        }
    }

    private void WpfFlushButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.Flush();
            AddLog("Flushed clipboard");
        }
        catch (Exception ex)
        {
            AddLog($"Flush failed with '{ex.Message}'");
            LogOpenClipboardWindow();
        }
    }

    private void GetNextClipboardViewer_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var viewerHwnd = WindowsClipboard.GetNextClipboardViewer(_hwnd);
            AddLog($"Next clipboard viewer: {viewerHwnd:X}");
        }
        catch (Exception ex)
        {
            AddLog($"Get next clipboard viewer failed with '{ex.Message}'");
        }
    }

    private void GetClipboardViewer_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var viewerHwnd = WindowsClipboard.GetClipboardViewer();
            AddLog($"GetClipboardViewer() => {viewerHwnd:X}");
        }
        catch (Exception ex)
        {
            AddLog($"GetClipboardViewer failed with '{ex.Message}'");
        }
    }

    private void LogOpenClipboardWindow()
    {
        var openedHwnd = WindowsClipboard.GetOpenClipboardWindow();
        var pid = GetWindowPid(openedHwnd);
        AddLog($"GetOpenClipboardWindow() => {openedHwnd:X} (PID: {pid})");
    }

    private static int GetWindowPid(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero)
            return 0;

        var threadId = SafeNativeMethods.GetWindowThreadProcessId(hwnd, out uint processId);
        if (threadId == 0)
            return 0;

        return (int)processId;
    }

    private void AddLog(string message)
    {
        LogListBox.Items.Insert(0, $"{DateTime.Now:HH:mm:ss.fff}: {message}");
    }
}
using System.Diagnostics;
using System.Windows;

namespace WpfCopyTest;

public partial class MainWindow
{
    private int _nextNumber = 1;

    public MainWindow()
    {
        InitializeComponent();
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
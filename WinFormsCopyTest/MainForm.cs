using System.Diagnostics;

namespace WinFormsCopyTest;

public sealed partial class MainForm : Form
{
    private ListBox _logListBox = null!;
    private int _nextNumber = 1;

    public MainForm()
    {
        InitializeComponent();

        Text = "WinForms Copy Test";
        AddControls();
    }

    private void AddControls()
    {
        var copyButton = new Button
        {
            Text = "System.Windows.Forms.Clipboard.SetText",
            AutoSize = true,
        };
        copyButton.Click += CopyButton_OnClick;
        Controls.Add(copyButton);

        _logListBox = new ListBox()
        {
            Location = new Point(0, 50),
            Width = 400,
            Height = 200,
        };
        Controls.Add(_logListBox);
    }

    private void CopyButton_OnClick(object? sender, EventArgs e)
    {
        var copyText = (_nextNumber++).ToString();
        var sw = Stopwatch.StartNew();

        try
        {
            Clipboard.SetText(copyText);
            sw.Stop();

            AddLog($"Copied {copyText} in {sw.Elapsed}");
        }
        catch (Exception ex)
        {
            sw.Stop();
            AddLog($"Copy {copyText} failed with '{ex.Message}' in {sw.Elapsed}");
        }
    }

    private void AddLog(string message)
    {
        _logListBox.Items.Insert(0, $"{DateTime.Now:HH:mm:ss.fff} {message}");
    }
}
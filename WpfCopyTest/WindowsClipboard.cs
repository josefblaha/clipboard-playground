using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WpfCopyTest;

/// <summary>
/// Based on https://github.com/CopyText/TextCopy/blob/main/src/TextCopy/WindowsClipboard.cs
/// </summary>
internal static class WindowsClipboard
{
    private static string? _lastText;

    public static void SetText(string text)
    {
        TryOpenClipboard();
        SetTextData(text);
    }

    public static void SetTextDelayed(string text, nint ownerHandle)
    {
        TryOpenClipboard(ownerHandle);
        _lastText = text;
        SetEmptyData();
    }

    public static void RenderFormat(IntPtr format)
    {
        if (_lastText is null || format != SafeNativeMethods.CF_UNICODE_TEXT)
        {
            return;
        }

        RenderText(_lastText);
    }

    private static void SetEmptyData()
    {
        SafeNativeMethods.EmptyClipboard();

        try
        {
            SafeNativeMethods.SetClipboardData(SafeNativeMethods.CF_UNICODE_TEXT, IntPtr.Zero);
        }
        finally
        {
            SafeNativeMethods.CloseClipboard();
        }
    }

    private static void SetTextData(string text)
    {
        SafeNativeMethods.EmptyClipboard();

        try
        {
            RenderText(text);
        }
        finally
        {
            SafeNativeMethods.CloseClipboard();
        }
    }

    private static void RenderText(string text)
    {
        IntPtr hGlobal = default;
        try
        {
            var bytes = (text.Length + 1) * 2;
            hGlobal = Marshal.AllocHGlobal(bytes);

            if (hGlobal == default)
            {
                ThrowWin32();
            }

            var target = SafeNativeMethods.GlobalLock(hGlobal);

            if (target == default)
            {
                ThrowWin32();
            }

            try
            {
                Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
            }
            finally
            {
                SafeNativeMethods.GlobalUnlock(target);
            }

            if (SafeNativeMethods.SetClipboardData(SafeNativeMethods.CF_UNICODE_TEXT, hGlobal) == default)
            {
                ThrowWin32();
            }

            // clear reference; ownership transferred
            hGlobal = default;
        }
        finally
        {
            if (hGlobal != default)
            {
                Marshal.FreeHGlobal(hGlobal);
            }
        }
    }

    private static void TryOpenClipboard(nint ownerHandle = default)
    {
        var num = 10;
        while (true)
        {
            if (SafeNativeMethods.OpenClipboard(ownerHandle))
            {
                break;
            }

            if (--num == 0)
            {
                ThrowWin32();
            }

            Thread.Sleep(100);
        }
    }

    private static void ThrowWin32()
    {
        throw new Win32Exception(Marshal.GetLastWin32Error());
    }
}
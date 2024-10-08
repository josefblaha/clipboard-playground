using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WpfCopyTest;

/// <summary>
/// Based on https://github.com/CopyText/TextCopy/blob/main/src/TextCopy/WindowsClipboard.cs
/// </summary>
internal static class WindowsClipboard
{
    public static void SetText(string text)
    {
        TryOpenClipboard();
        InnerSet(text);
    }

    private static void InnerSet(string text)
    {
        SafeNativeMethods.EmptyClipboard();
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

            SafeNativeMethods.CloseClipboard();
        }
    }

    private static void TryOpenClipboard()
    {
        var num = 10;
        while (true)
        {
            if (SafeNativeMethods.OpenClipboard(default))
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
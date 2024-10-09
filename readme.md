Windows Clipboard playground to investigate:

- [#9901 Clipboard.SetText fails with CLIPBRD_E_CANT_OPEN](https://github.com/dotnet/wpf/issues/9901)
- [#25437 PowerToys Run Calculator Click to 'Copy this number to clipboard' fails, with error dialogue](https://github.com/microsoft/PowerToys/issues/25437)

## References

### Win32 API

- [Using the clipboard](https://learn.microsoft.com/en-us/windows/win32/dataxchg/using-the-clipboard#copying-information-to-the-clipboard)
- [GetClipboardOwner](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclipboardowner)
- [OpenClipboard](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-openclipboard)
- [SetClipboardData](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setclipboarddata)
- [CloseClipboard](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-closeclipboard)
- [WM_RENDERFORMAT](https://learn.microsoft.com/en-us/windows/win32/dataxchg/wm-renderformat)

- [The Old New Thing: What is the proper handling of WM_RENDERFORMAT and WM_RENDERALLFORMATS?](https://devblogs.microsoft.com/oldnewthing/20121224-00/?p=5763)
- [The Old New Thing: How can I find out which process has locked me out of the clipboard?](https://devblogs.microsoft.com/oldnewthing/20240410-00/?p=109632)

### OLE2 API

- [OleSetClipboard](https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-olesetclipboard)
- [OleFlushClipboard](https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-oleflushclipboard)

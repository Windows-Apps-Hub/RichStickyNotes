using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.UI.Shell;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System;
using WinWrapper.Windowing;
using Windows.System;

namespace WinWrapper;
public sealed partial class NotifyIcon
{
    internal const int MaxTextSize = 127;

    private static readonly WindowMessages WindowMessageTaskbarCreated =
        WindowMessagesHelper.Register("TaskbarCreated");
    const WindowMessages TrayMouseMessage = WindowMessages.USER + 1024;

    private readonly object _syncObj = new object();

    private Icon _icon;
    private string _text = string.Empty;
    private readonly uint _id;
    private bool _added;
    private Window _window;
    private static uint s_nextId;

    // Visible defaults to false, but the NotifyIconDesigner makes it seem like the default is
    // true.  We do this because while visible is the more common case, if it was a true default,
    // there would be no way to create a hidden NotifyIcon without being visible for a moment.
    private bool _visible;
    static WindowClass? WindowMessageReceived;
    static List<NotifyIcon> NotifyIcons = new();
    static readonly WindowClass WinWrapperNotifyIconClass = new(nameof(WinWrapperNotifyIconClass),
        (window, msg, wParam, lParam) =>
        {
            foreach (var n in NotifyIcons)
            {
                if (n._window == window)
                {
                    if (n.WndProc(msg, (WindowMessages)lParam))
                        return default;
                    else
                        return window.DefWindowProc(msg, wParam, lParam);
                    
                }
            }
            return window.DefWindowProc(msg, wParam, lParam);
        }
    );

    /// <summary>
    ///  Initializes a new instance of the <see cref="NotifyIcon"/> class.
    /// </summary>
    public NotifyIcon()
    {
        _id = ++s_nextId;
        _window = Window.CreateNewWindow("", WinWrapperNotifyIconClass);
        NotifyIcons.Add(this);
        UpdateIcon(_visible);
    }

    /// <summary>
    ///  Gets or sets the current icon.
    /// </summary>
    public Icon Icon
    {
        get
        {
            return _icon;
        }
        set
        {
            if (_icon.Handle != value.Handle)
            {
                _icon = value;
                UpdateIcon(_visible);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the ToolTip text displayed when
    ///  the mouse hovers over a system tray icon.
    /// </summary>
    public string Text
    {
        get
        {
            return _text;
        }
        set
        {
            value ??= string.Empty;

            if (!value.Equals(_text))
            {
                if (value.Length > MaxTextSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(Text), value, "The text is too long");
                }

                _text = value;
                if (_added)
                {
                    UpdateIcon(true);
                }
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the icon is visible in the Windows System Tray.
    /// </summary>
    public bool Visible
    {
        get
        {
            return _visible;
        }
        set
        {
            if (_visible != value)
            {
                UpdateIcon(value);
                _visible = value;
            }
        }
    }

    /// <summary>
    ///  Releases the unmanaged resources used by the <see cref="NotifyIcon" />
    ///  and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    ///  <see langword="true" /> to release both managed and unmanaged resources;
    ///  <see langword="false" /> to release only unmanaged resources.
    /// </param>
    public void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_window.IsValid)
            {
                _icon = default;
                Text = string.Empty;
                UpdateIcon(false);
                _window.TryClose();
                _window = default;
            }
        }
        else
        {
            // This same post is done in ControlNativeWindow's finalize method, so if you change
            // it, change it there too.
            if (_window.IsValid && _window.Handle != default)
            {
                _window.TryClose();
                _window = default;
            }
        }
    }

    /// <summary>
    ///  Updates the icon in the system tray.
    /// </summary>
    private unsafe void UpdateIcon(bool showIconInTray)
    {
        lock (_syncObj)
        {
            var data = new NOTIFYICONDATAW
            {
                cbSize = (uint)sizeof(NOTIFYICONDATAW),
                uCallbackMessage = (uint)TrayMouseMessage,
                uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE,
                uID = _id
            };
            if (showIconInTray)
            {
                if (_window.Handle == IntPtr.Zero)
                {
                    _window = Window.CreateNewWindow("", WinWrapperNotifyIconClass);
                }
            }

            data.hWnd = new(_window.Handle);
            if (_icon.Handle != default)
            {
                data.uFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_ICON;
                data.hIcon = new(_icon.Handle);
            }

            data.uFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_TIP;
            data.szTip = _text;

            if (showIconInTray && _icon.Handle != default)
            {
                if (!_added)
                {
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, in data);
                    _added = true;
                }
                else
                {
                    PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_MODIFY, in data);
                }
            }
            else if (_added)
            {
                PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_DELETE, in data);
                _added = false;
            }
        }
    }

    private void WmTaskbarCreated()
    {
        _added = false;
        UpdateIcon(_visible);
    }
    public event Action LeftMouseButtonUp;
    private bool WndProc(WindowMessages msg, WindowMessages lPARAM)
    {
        switch (msg)
        {
            case TrayMouseMessage:
                switch (lPARAM)
                {
                    case WindowMessages.MouseLeftButtonDoubleClick:
                        //WmMouseDown(MouseButtons.Left, 2);
                        break;
                    case WindowMessages.MouseLeftButtonDown:
                        //WmMouseDown(MouseButtons.Left, 1);
                        break;
                    case WindowMessages.MouseLeftButtonUp:
                        LeftMouseButtonUp?.Invoke();
                        break;
                    case WindowMessages.MouseMiddleDoubleClick:
                        //WmMouseDown(MouseButtons.Middle, 2);
                        break;
                    case WindowMessages.MouseMiddleButtonDown:
                        //WmMouseDown(MouseButtons.Middle, 1);
                        break;
                    case WindowMessages.MouseMiddleButtonUp:
                        //WmMouseUp(MouseButtons.Middle);
                        break;
                    case WindowMessages.MouseMove:
                        //WmMouseMove();
                        break;
                    case WindowMessages.MouseRightButtonDoubleClick:
                        //WmMouseDown(MouseButtons.Right, 2);
                        break;
                    case WindowMessages.MouseRightButtonDown:
                        //WmMouseDown(MouseButtons.Right, 1);
                        break;
                    case WindowMessages.MouseRightButtonUp:
                        //if (_contextMenuStrip is not null)
                        //{
                        //    ShowContextMenu();
                        //}

                        //WmMouseUp(MouseButtons.Right);
                        break;
                    case (WindowMessages)NIN.BALLOONSHOW:
                        //OnBalloonTipShown();
                        break;
                    case (WindowMessages)NIN.BALLOONHIDE:
                        //OnBalloonTipClosed();
                        break;
                    case (WindowMessages)NIN.BALLOONTIMEOUT:
                        //OnBalloonTipClosed();
                        break;
                    case (WindowMessages)NIN.BALLOONUSERCLICK:
                        //OnBalloonTipClicked();
                        break;
                }

                break;
            case WindowMessages.Command:
                //if (msg == 0)
                //{
                //    if (Command.DispatchID((int)msg.WParamInternal & 0xFFFF))
                //    {
                //        return true;
                //    }
                //}
                //else
                //{
                //    return false;
                //}
                return true;

                //break;

            case WindowMessages.Destroy:
                // Remove the icon from the taskbar
                UpdateIcon(false);
                break;

            case WindowMessages.InitMenuPopup:
            default:
                if (msg == WindowMessageTaskbarCreated)
                {
                    WmTaskbarCreated();
                }

                return false;
        }
        return true;
    }
    const int NINF_KEY = 0x1;
    enum NIN
    {
        SELECT = (int)(WindowMessages.USER + 0),
        KEYSELECT = SELECT | NINF_KEY,
        BALLOONSHOW = (int)(WindowMessages.USER + 2),
        BALLOONHIDE = (int)(WindowMessages.USER + 3),
        BALLOONTIMEOUT = (int)(WindowMessages.USER + 4),
        BALLOONUSERCLICK = (int)(WindowMessages.USER + 5),
        POPUPOPEN = (int)(WindowMessages.USER + 6),
        POPUPCLOSE = (int)(WindowMessages.USER + 7)
    }
}
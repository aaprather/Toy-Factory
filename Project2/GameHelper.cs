using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class GameHelper
{
    /////////////////////////////////////////////////////////////
    #region *** Private Fields ***
    /////////////////////////////////////////////////////////////

    private Timer _gameTimer = new Timer();
    private Stopwatch _sw = new Stopwatch();
    private int _desiredInterval;
    private List<Keys> _pressedKeys = new List<Keys>();
    private Form _form = null;

    /////////////////////////////////////////////////////////////
    #endregion *** Private Fields ***
    /////////////////////////////////////////////////////////////
    
    /////////////////////////////////////////////////////////////
    #region *** Events ***
    /////////////////////////////////////////////////////////////

    /// <summary>
    /// The main Update event handler. This event will be fired
    /// every time an update is to occur in the game (by default
    /// 60 times per second).
    /// </summary>
    public event EventHandler Update;

    /////////////////////////////////////////////////////////////
    #endregion *** Events ***
    /////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////
    #region *** Constructor ***
    /////////////////////////////////////////////////////////////

    /// <summary>
    /// Create a new Game Helper object using the specified form and
    /// optionally the specified number of frames per second.
    /// </summary>
    /// <param name="form">The form to attach the GameHelper to</param>
    /// <param name="FPS">(Optional-defaults to 30) The number of ("ticks") per second</param>
    public GameHelper(Form form, int FPS = 30)
    {
        _desiredInterval = 1000 / FPS;
        _gameTimer.Interval = _desiredInterval;
        _gameTimer.Tick += _gameTimer_Tick;
        _form = form;
        _form.KeyDown += _form_KeyDown;
        _form.KeyUp += _form_KeyUp;
        _form.LostFocus += _form_LostFocus;
    }

    /////////////////////////////////////////////////////////////
    #endregion *** Constructor ***
    /////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////
    #region *** Public Methods ***
    /////////////////////////////////////////////////////////////

    /// <summary>
    /// Start the game (i.e., enable the Update event).
    /// </summary>
    public void Start()
    {
        _gameTimer.Enabled = true;
        _sw.Start();
    }

    /// <summary>
    /// Stop the game (i.e., disable the Update event).
    /// </summary>
    public void Stop()
    {
        _gameTimer.Enabled = false;
        _sw.Reset();
    }

    /// <summary>
    /// Check to see if the specified key is currently pressed.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if key is pressed. False if not.</returns>
    public bool IsPressed(Keys key)
    {
        return _pressedKeys.Contains(key);
    }

    /////////////////////////////////////////////////////////////
    #endregion *** Public Methods ***
    /////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////
    #region *** Private Methods ***
    /////////////////////////////////////////////////////////////

    private void OnUpdate()
    {
        if (Update != null)
        {
            Update(_form, null);
        }
    }

    private void _gameTimer_Tick(object sender, EventArgs e)
    {
        _gameTimer.Enabled = false;
        _sw.Restart();
        _form.SuspendDrawing();
        _form.SuspendLayout();
        OnUpdate();
        _form.ResumeLayout();
        _form.ResumeDrawing();

        int interval = _desiredInterval - (int)_sw.ElapsedMilliseconds;
        Debug.WriteLineIf(interval < 0, "WARNING: Frame Rate Less Than Expected (" + interval + "ms");
        interval = interval > 0 ? interval : 1;
        _gameTimer.Interval = interval;
        _gameTimer.Enabled = true;
    }

    private void _form_KeyUp(object sender, KeyEventArgs e)
    {
        if (_pressedKeys.Contains(e.KeyCode))
        {
            _pressedKeys.Remove(e.KeyCode);
        }
    }

    private void _form_KeyDown(object sender, KeyEventArgs e)
    {
        if (!_pressedKeys.Contains(e.KeyCode))
        {
            _pressedKeys.Add(e.KeyCode);
        }
    }

    private void _form_LostFocus(object sender, EventArgs e)
    {
        _pressedKeys.Clear();
    }

    /////////////////////////////////////////////////////////////
    #endregion *** Private Methods ***
    /////////////////////////////////////////////////////////////
}

public static class ControlHelper
{
    /////////////////////////////////////////////////////////////
    #region *** Redraw Suspend/Resume ***
    /////////////////////////////////////////////////////////////

    [DllImport("user32.dll", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
    private const int WM_SETREDRAW = 0xB;

    public static void SuspendDrawing(this Control target)
    {
        try
        {
            SendMessage(target.Handle, WM_SETREDRAW, 0, 0);
        }
        catch { }
    }

    public static void ResumeDrawing(this Control target) { ResumeDrawing(target, true); }
    public static void ResumeDrawing(this Control target, bool redraw)
    {
        try
        {
            SendMessage(target.Handle, WM_SETREDRAW, 1, 0);
            if (redraw)
            {
                target.Refresh();
            }
        }
        catch { }
    }

    /////////////////////////////////////////////////////////////
    #endregion *** Resraw Suspend/Resume ***
    /////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////
    #region *** Public Extension Methods ***
    /////////////////////////////////////////////////////////////

    /// <summary>
    /// Check to see whether this control is inside of the the current Form.
    /// </summary>
    /// <returns>True if the control is fully inside of its form. False otherwise.</returns>
    public static bool IsInsideOfForm(this Control thisControl)
    {
        var parent = thisControl;

        while (parent != null && !(parent is Form))
        {
            parent = parent.Parent;
        }
        if (parent == null)
        {
            return false;
        }

        return thisControl.IsInsideOf(parent);
    }

    /// <summary>
    /// Check to see whether this control is inside of another control.
    /// </summary>
    /// <param name="otherControl">The control to check.</param>
    /// <returns>True if this control is fully inside of the other control. False otherwise.</returns>
    public static bool IsInsideOf(this Control thisControl, Control otherControl)
    {
        Rectangle otherRect = otherControl is Form ? otherControl.ClientRectangle : otherControl.Bounds;
        return thisControl.Bounds.IsInsideOf(otherRect);
    }

    /// <summary>
    /// Check to see whether this rectangle is inside of another rectangle.
    /// </summary>
    /// <param name="otherRect">The rectangle to check.</param>
    /// <returns>True if this rectangle is fully inside of the other rectangle. False otherwise.</returns>
    public static bool IsInsideOf(this Rectangle thisRect, Rectangle otherRect)
    {
        return thisRect.Left >= otherRect.Left &&
               thisRect.Right <= otherRect.Right &&
               thisRect.Top >= otherRect.Top &&
               thisRect.Bottom <= otherRect.Bottom;
    }

    /// <summary>
    /// Check to see whether this control is touching (intersecting) another control.
    /// </summary>
    /// <param name="otherControl">The control to check.</param>
    /// <returns>True if this control is touching the other control. False otherwise.</returns>
    public static bool IsTouching(this Control thisControl, Control otherControl)
    {
        return thisControl.Bounds.IntersectsWith(otherControl.Bounds);
    }

    /////////////////////////////////////////////////////////////
    #endregion *** Public Extension Methods ***
    /////////////////////////////////////////////////////////////
}
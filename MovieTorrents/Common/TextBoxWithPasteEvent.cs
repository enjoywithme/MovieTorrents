using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieTorrents
{
    public class ClipboardEventArgs : EventArgs
    {
        public string ClipboardText { get; set; }
        public ClipboardEventArgs(string clipboardText)
        {
            ClipboardText = clipboardText;
        }
    }

    //https://stackoverflow.com/questions/3446233/hook-on-default-paste-event-of-winforms-textbox-control
    public class TextBoxWithPasteEvent:TextBox
    {
        public event EventHandler<ClipboardEventArgs> Pasted;

        private const int WM_PASTE = 0x0302;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PASTE)
            {
                if (Pasted != null)
                {
                    Pasted(this, new ClipboardEventArgs(Clipboard.GetText()));
                    // don't let the base control handle the event again
                    return;
                }
            }

            base.WndProc(ref m);
        }
    }
}

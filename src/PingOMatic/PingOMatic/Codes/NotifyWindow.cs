using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingOMatic.Codes
{
	public class NotifyWindow
	{
        private readonly NotifyIcon _notifyIcon;

        public NotifyWindow()
        {
            _notifyIcon = new NotifyIcon();
            // Extracts your app's icon and uses it as notify icon
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            // Hides the icon when the notification is closed
            _notifyIcon.BalloonTipClosed += (s, e) => _notifyIcon.Visible = false;
        }

        public void ShowNotification()
        {
            _notifyIcon.Visible = true;
            // Shows a notification with specified message and title
            _notifyIcon.ShowBalloonTip(3000, "Title", "Message", ToolTipIcon.Info);
        }

        public void ShowNotification(string titre, string message, ToolTipIcon icon)
		{
            _notifyIcon.Visible = true;
            // Shows a notification with specified message and title
            _notifyIcon.ShowBalloonTip(5000, titre, message, icon);
        }

        public void ShowNotification(string titre, string message, ToolTipIcon icon, int seconde)
        {
            _notifyIcon.Visible = true;
            // Shows a notification with specified message and title
            _notifyIcon.ShowBalloonTip(seconde*1000, titre, message, icon);
        }
    }
}

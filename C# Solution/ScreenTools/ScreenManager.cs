using System.Collections.Generic;
using System.Windows.Forms;

namespace Appeon.ComponentsApp.ScreenTools
{
    public static class ScreenManager
    {
        public static IList<ScreenDefinition> GetScreens()
        {
            var list = new List<ScreenDefinition>();

            foreach(var screen in Screen.AllScreens)
            {
                list.Add(new ScreenDefinition
                {
                    ScreenName = screen.DeviceName,
                    IsPrimary = screen.Primary,
                    X = screen.WorkingArea.X,
                    Y = screen.WorkingArea.Y,
                    Width = screen.WorkingArea.Width,
                    Height = screen.WorkingArea.Height,
                });
            }

            return list;
        }
    }
}

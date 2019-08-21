using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class MainExtensions
    {
        public static void OpenEditView(this IApp app)
        {
            app.Tap("MainLogContentView");
        }

        public static void TapSnackBarButton(this IApp app, string buttonText)
        {
            app.Tap(x => x.Marked(Misc.SnackbarAction).Text(buttonText));
        }

        public static void SwipeEntryToDelete(this IApp app, string timeEntryDescription)
        {
            var timeEntryCellRect = RectForTimeEntryCell(app, timeEntryDescription);

            app.DragCoordinates(
                fromX: timeEntryCellRect.X + timeEntryCellRect.Width,
                fromY: timeEntryCellRect.CenterY,
                toX: timeEntryCellRect.X,
                toY: timeEntryCellRect.CenterY
            );

            app.WaitForNoElement(x => x.Text(timeEntryDescription));
        }
    }
}

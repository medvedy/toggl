using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class MainExtensions
    {
        public static void TapNthCellInCollection(this IApp app, int index)
        {
            app.Tap(query => query.Class("UITableViewCell").Child(index));
        }

        public static void OpenEditView(this IApp app)
        {
            app.TapNthCellInCollection(0);
            app.WaitForElement(EditTimeEntry.EditTags);
        }

        public static void TapSnackBarButton(this IApp app, string buttonText)
        {
            app.Tap(x => x.Marked(Misc.SnackBar).Descendant().Text(buttonText));
        }

        public static void SwipeEntryToDelete(this IApp app, string timeEntryDescription)
        {
            var timeEntryCellRect = RectForTimeEntryCell(app, timeEntryDescription);

            app.DragCoordinates(
                fromX: timeEntryCellRect.CenterX,
                fromY: timeEntryCellRect.CenterY,
                toX: timeEntryCellRect.X - 100,
                toY: timeEntryCellRect.CenterY);

            app.WaitForElement(x => x.Text("Delete"));
            app.Tap(x => x.Text("Delete"));

            app.WaitForNoElement(x => x.Text(timeEntryDescription));
        }
    }
}

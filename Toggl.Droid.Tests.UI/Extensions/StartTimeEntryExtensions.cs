using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class StartTimeEntryExtensions
    {
        public static void TapCreateTag(this IApp app, string tagName)
        {
            var query = $"Create tag \"{tagName}\"";
            tapAndWaitForElement(app, query);
        }

        public static void TapCreateProject(this IApp app, string projectName)
        {
            var query = $"Create project \"{projectName}\"";
            tapAndWaitForElement(app, query);
        }

        public static void TapSelectTag(this IApp app, string tagName)
        {
            tapAndWaitForElement(app, tagName);
        }

        public static void TapSelectProject(this IApp app, string projectName)
        {
            tapAndWaitForElement(app, projectName);
        }

        public static void TapCreateClient(this IApp app, string clientName)
        {
            var query = $"Create client \"{clientName}\"";
            tapAndWaitForElement(app, query);
        }

        public static void TapSelectClient(this IApp app, string clientName)
        {
            app.Tap(query => query.Marked(clientName).Id(Client.ClientCreationCellId));
        }

        private static void tapAndWaitForElement(IApp app, string query)
        {
            app.WaitForElement(query);
            app.Tap(query);
            app.WaitForNoElement(query);
        }

        public static void EnterManualTimeEntryDuration(this IApp app, string duration)
        {
            app.Tap(StartTimeEntry.DurationLabel);
            app.Tap(EditDuration.WheelDurationInput);

            app.EnterText(duration);
            app.PressEnter();

            app.Tap(EditDuration.SaveButton);
        }

        public static void EnterTextInStartTimeEntryView(this IApp app, string text)
        {
            app.EnterText(text);
        }
    }
}

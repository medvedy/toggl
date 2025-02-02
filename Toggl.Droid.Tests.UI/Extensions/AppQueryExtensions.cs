using NUnit.Framework;
using System.Linq;
using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class AppQueryExtensions
    {
        public static void NavigateBack(this IApp app)
        {
            app.Back();
        }

        public static void WaitForElementWithText(this IApp app, string element, string text)
        {
            app.WaitFor(() => app.findElementWithText(element, text), $"Could not find element {element} with text {text}");
            Assert.IsTrue(app.findElementWithText(element, text));
        }

        private static bool findElementWithText(this IApp app, string element, string text)
        {
            return app.Query(x => x.Marked(element).Text(text)).FirstOrDefault() != null
                   || app.Query(x => x.Marked(element)?.Invoke("getError"))?.FirstOrDefault()?.ToString().Equals(text) == true;
        }
    }
}

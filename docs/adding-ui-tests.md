# Writing UI tests
We are using the Xamarin.UITest test framework for UI testing, you can check the documentation here: https://docs.microsoft.com/en-us/appcenter/test-cloud/uitest/
It should cover the basic api.
Keep in mind that we want checks to be cross platform when possible and reasonable. We want to avoid adding dependencies on the platforms, and when that's necessary, hide the details behind extension methods. The section [Creating assertions and extensions with slightly different behaviours on iOS and Android](#create-xplat-extensions) can help with that.

## Where do tests files go?
UI tests reside in Toggl.Tests.UI

## How to add test class
Create a file containing the test class, anotated with `[TestFixture]`, something like

```cs
[TestFixture]
public sealed class AmazingScreenTests
{
    
}
```

## How to add individual tests
Inside the test class, all you have to do is to add a new method returning void, anotated with `[Test]`, something like:

```cs
[TestFixture]
public sealed class AmazingScreenTests
{
    
    [Test]
    public void SomeActionResultsInSomething()
    {
    }   
}
```

The method name should be meaningful and usually follows the format Action -> ExpectedBehaviour

## Ignoring Tests for one platform <a name="create-xplat-extensions"></a>
Sometimes you might want or have to write tests for one platform only to avoid breaking UI tests or simply because you are testing a platform specific feature.
You can add the `[IgnoreOnAndroid]` or `[IgnoreOnIos]` annotations to ignore some specific test (method) or a group of tests (class) in Android and iOS respectively.
Ex:

```cs
[TestFixture]
[IgnoreOnAndroid]
public sealed class AmazingIOSOnlyScreenTests
{
    ... many tests inside
}

[TestFixture]
public sealed class AmazingSharedScreenTests
{
    [Test, IgnoreOnAndroid]
    public void SomeIosOnlyBehaviour()
    {
    }
    
    [Test, IgnoreOnIos]
    public void SomeAndroidOnlyBehaviour()
    {
    }    
}
``` 

In the example above, the following will happen:
- The tests inside `AmazingIOSOnlyScreenTests` will only run on iOS
- The test `SomeIosOnlyBehaviour` inside `AmazingSharedScreenTests` will only run on iOS
- The test `SomeAndroidOnlyBehaviour` inside `AmazingSharedScreenTests` will only run on Android

## Creating assertions and extensions with slightly different behaviours on iOS and Android
Extensions are usually platform independent and located at `Toggl.Tests.UI/Extensions`, but sometimes you'll need to write an extension that behaves differently on iOS and Android. 
The suggested way of writing extensions for that case is to create one extension in Android and another in iOS.
All you have to do is to add the same extension in Android and iOS, but with different implementations.
You'll add the Android version on: `Toggl.Droid.Tests.UI/Extensions` and the iOS version on `Toggl.iOS.Tests.UI/Extensions`.
Ex:

For iOS, in `Toggl.iOS.Tests.UI/Extensions`:

```cs
namespace Toggl.Tests.UI.Extensions
{
    public static partial class AmazingExtensions
    {
        public static void WaitForSomethingThatBehavesDifferentlyOnEachPlat(this IApp app)
        {
            //Check iOS widget or something else iOS specific
        }
    }
}
```

For Android, in `Toggl.Droid.Tests.UI/Extensions`:

```cs
namespace Toggl.Tests.UI.Extensions
{
    public static partial class AmazingExtensions
    {
        public static void WaitForSomethingThatBehavesDifferentlyOnEachPlat(this IApp app)
        {
            //Check Android widget or something else Android specific
        }
    }
}
```

Platform agnostic, in `Toggl.Tests.UI/Extensions`:

```cs
namespace Toggl.Tests.UI.Extensions
{
    public static partial class AmazingExtensions
    {
        public static void WaitForSomethingElseCool(this IApp app)
        {
            app.WaitForElement("AccessibilityId");
            app.Tap("SomeWidget");
            app.WaitForElement("OtherAccessibilityId");
        }
    }
}
```

Usage in tests from `Shared.Toggl.Tests.UI`:

```cs
[TestFixture]
public sealed class AmazingScreenTests
{
    
    [Test]
    public void SomeActionResultsInSomething()
    {
        app.WaitForSomethingThatBehavesDifferentlyOnEachPlat();
        app.WaitForSomethingElseCool();
    }
}
```

And of course, you can use `WaitForSomethingThatBehavesDifferentlyOnEachPlat` inside other extensions.

## Avoid breaking the ui build in the other platform
When writing new tests, if you are doing so in only one platform, remember to add whatever constants you add to `Toggl.iOS.Tests.UI/IosAccessibilityNames.cs` to `Toggl.Droid.Tests.UI/AndroidAccessibilityNames.cs` and vice versa.
The tests will break in the other platform if you forget to do so. 

## Tips for writing good UI tests

### Use meaningful names
The names usually follow the pattern Action -> ExpectedBehaviour.

```cs
//Do
[Test]
public void TappingTheStopButtonStopsTheRunningTimeEntry() { }

//Don't
[Test]
public void StopRunningTimeEntry() { }

//Don't
[Test]
public void StopRunningTimeEntryWorksCorrectly() { }
```

If the test method does lots of things and it's hard to come up with a meaningful and easily readable name that follows the pattern Action -> ExpectedBehaviour, then you can use the name of the test case scenario. Alternatively the test could be split into multiple ones if that's possible.

```cs
//Do
public void AssigningANewProjectToATimeEntryWithoutAProject() { }

//Don't
public void TappingTheCreateButtonInTheCreateProjectViewAssignsTheProjectToTheTimeEntryWhenTheTimeEntryDoesntHaveAProject() { }
```

You can append `When[Condition]` to the end of the method name to indicate a condition for a single test. Or, if mulitple tests share the same condition, use a nested class with the condition name. Just don't forget to add the `[TestFixture]` attribute to the nested class.

```cs
//Do
public void TappingTheLoginButtonShowsAnErrorMessageWhenTheEnteredEmailIsNotRegistered() { }

//Do
public sealed class ForgotPasswordTests
{
    [TestFixture]
    public class WhenTheEnteredEmailIsValidAndRegistered
    {
        [Test]
        public void TappingTheGetPasswordResetLinkButtonHidesTheGetPasswordResetLinkButton() { }
        
        [Test]
        public void TappingTheGetPasswordResetLinkButtonShowsTheSuccessMessage() { }
    }
}


//Don't
[Test]
public void TappingTheGetPasswordResetLinkButtonHidesTheGetPasswordResetLinkButtonWhenTheEnteredEmailIsValidAndRegistered() { }

[Test]
public void TappingTheGetPasswordResetLinkButtonShowsTheSuccessMessageWhenTheEnteredEmailIsValidAndRegistered() { }
```

### Use extension methods to make the tests readable
```cs
//Do
[Test]
public void TappingTheStopButtonStopsTheRunningTimeEntry()
{
    app.WaitForMainScreen();

    app.StartTimeEntryWithDescription("Testing the Toggl app");
    app.Tap(Main.StopTimeEntryButton);

    app.AssertNoTimeEntryInTheLog()
}

//Don't
[Test]
public void AssigningANewProjectToATimeEntryWithoutAProject()
{
    var timeEntryDescription = "Does not matter";
    var projectName = "this is a project";
    app.StartTimeEntryWithDescription(timeEntryDescription);
    app.StopTimeEntry();
    app.TapOnTimeEntry(timeEntryDescription);

    app.WaitForElement(EditTimeEntry.EditProject);
    app.Tap(EditTimeEntry.EditProject);

    app.WaitForElement(SelectProject.ProjectNameTextField);
    app.Tap(SelectProject.ProjectNameTextField);
    app.EnterText(projectName);
    app.TapCreateProject(projectName);

    app.WaitForElement(EditProject.CreateButton);
    app.Tap(EditProject.CreateButton);

    app.WaitForElement(EditTimeEntry.Confirm);
    app.Tap(EditTimeEntry.Confirm);

    app.WaitForTimeEntryWithProject(projectName);
}
```

### Use AAA pattern (Arrange, Act, Assert)
Each test consists of 3 parts:
* **Arrange** the proper conditions for the test, like going to the screen that's being tested or creating any time entries that are needed for the test
* **Act** out the action that's being tested e.g. pressing a button, logging in etc.
* **Assert** that the app behaves as intended. If something should appear after the Act part, then assert that it has appeared, if something should disappear, assert that it has dissappeared etc.

The three A parts of the test should be separated by an empty line. If you feel like one of the three parts is getting too long and you want to include an empty line, for example in the Arrange part, to better separate different parts of the setup, don't. You should look into making some extension methods to make the part shorter.

```cs
//Do
[Test]
public void TappingDiscardWhenClosingShouldNotStartATimeEntry()
{
    var description = "UI testing the Toggl App";
    app.WaitForStartTimeEntryScreen();

    app.EnterText(description);
    app.Tap(StartTimeEntry.CloseButton);
    app.Tap(StartTimeEntry.DialogDiscard);

    app.WaitForNoElement(Main.StopTimeEntryButton);
}

//Don't
[Test]
public void AssigningAnExistingProjectToATimeEntryWithAProject()
{
    var timeEntryDescription = "Listening to human music";
    var assignedProjectName = "Work";
    var newProjectName = "No work";

    app.CreateProject(newProjectName);
    app.CreateTimeEntry(timeEntryDescription, assignedProjectName);

    app.TapOnTimeEntry(timeEntryDescription);

    app.WaitForElement(EditTimeEntry.EditProject);
    app.Tap(EditTimeEntry.EditProject);

    app.WaitForElement(SelectProject.ProjectNameTextField);
    app.Tap(SelectProject.ProjectNameTextField);
    app.EnterText(newProjectName);

    app.TapOnProjectWithName(newProjectName);

    app.Tap(EditTimeEntry.Confirm);

    app.WaitForTimeEntryWithProject(newProjectName);
}
```

The arrange part can also be moved to the setup method if all the tests in the class share the same initial state.
```cs
//Do
[TestFixture]
public sealed class StartTimeEntryTests
{
    [SetUp]
    public void BeforeEachTest()
    {
        app = Configuration.GetApp();

        app.WaitForStartTimeEntryScreen();
    }

    [Test]
    public void TappingTheDoneButtonCreatesANewTimeEntry()
    {
        app.Tap(StartTimeEntry.DoneButton);

        app.WaitForElement(Main.StopTimeEntryButton);
    }

    [Test]
    public void TappingTheCloseButtonShowsConfirmationDialog()
    {
        var description = "UI testing the Toggl App";

        app.EnterText(description);
        app.Tap(StartTimeEntry.CloseButton);

        app.WaitForElement(StartTimeEntry.DialogDiscard);
    }
}
```

### [iOS only] Leave accessibility in a better state than you found it in

Since on iOS we are using accessibility labels and accesibility IDs for UI tests, we may break screen reader functionality without even realising it (this happened before). That's why it is of utmost importance to understand how screen readers work and how to use them on iOS. And by being aware of screen readers we can write UI tests without breaking accessibility stuff and even better, we can improve accessibility by correctly choosing between accessibility ID and label.

On Android we use view IDs (`android:id`), so accessibility isn't involved in UI tests.

**When to use accessibility ID?**

Accessibility IDs are behind-the-scenes identifiers for UI elements. These are not used by VoiceOver, so our UI tests should mainly rely on these. However, we've been using accessibility labels as if they were accessibility IDs, so most of the tests will need to be updated accordingly.

**When to use accesibility label?**

Accessibility labels are the strings that VoiceOver reads, so these should make sense to humans and ideally be localised. These should be used to make the UI work with VoiceOver. Not all views should have an accessibility label - only those elements that need to be highlightable by VoiceOver. More info on good accessibility patterns can be found [here](https://developer.apple.com/accessibility/ios/).

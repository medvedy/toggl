﻿using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;
using AndroidColor = Android.Graphics.Color;
using FoundationResources = Toggl.Shared.Resources;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              WindowSoftInputMode = SoftInput.AdjustResize,
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class EditProjectActivity : ReactiveActivity<EditProjectViewModel>
    {
        public EditProjectActivity() : base(
            Resource.Layout.EditProjectActivity,
            Resource.Style.AppTheme_Light_WhiteBackground,
            Transitions.SlideInFromBottom)
        { }

        public EditProjectActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            errorText.Visibility = ViewStates.Gone;

            // Name
            projectNameTextView.Rx()
                .Text()
                .Subscribe(ViewModel.Name.Accept)
                .DisposedBy(DisposeBag);

            ViewModel.Name
                .Subscribe(projectNameTextView.Rx().TextObserver(ignoreUnchanged: true))
                .DisposedBy(DisposeBag);

            // Color
            colorCircle.Rx()
                .BindAction(ViewModel.PickColor)
                .DisposedBy(DisposeBag);

            colorArrow.Rx()
                .BindAction(ViewModel.PickColor)
                .DisposedBy(DisposeBag);

            ViewModel.Color
                .Select(color => color.ToNativeColor())
                .Subscribe(colorCircle.SetCircleColor)
                .DisposedBy(DisposeBag);

            // Error
            ViewModel.Error
                .Subscribe(errorText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Error
                .Select(e => !string.IsNullOrEmpty(e))
                .Subscribe(errorText.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            var errorOffset = 8.DpToPixels(this);
            var noErrorOffset = 14.DpToPixels(this);

            ViewModel.Error
                .Select(e => string.IsNullOrEmpty(e) ? noErrorOffset : errorOffset)
                .Subscribe(projectNameTextView.LayoutParameters.Rx().MarginTop())
                .DisposedBy(DisposeBag);
            // Workspace
            changeWorkspaceView.Rx()
                .BindAction(ViewModel.PickWorkspace)
                .DisposedBy(DisposeBag);

            ViewModel.WorkspaceName
                .Subscribe(workspaceNameLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            // Client
            changeClientView.Rx()
                .BindAction(ViewModel.PickClient)
                .DisposedBy(DisposeBag);

            ViewModel.ClientName
                .Select(clientNameWithEmptyText)
                .Subscribe(clientNameTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            var noClientColor = AndroidColor.ParseColor("#CECECE");
            ViewModel.ClientName
                .Select(clientTextColor)
                .Subscribe(clientNameTextView.SetTextColor)
                .DisposedBy(DisposeBag);

            // Is Private
            toggleIsPrivateView.Rx().Tap()
                .Select(_ => isPrivateSwitch.Checked)
                .Subscribe(ViewModel.IsPrivate.Accept)
                .DisposedBy(DisposeBag);

            ViewModel.IsPrivate
                .Subscribe(isPrivateSwitch.Rx().CheckedObserver())
                .DisposedBy(DisposeBag);

            // Save
            createProjectButton.Rx()
                .BindAction(ViewModel.Save)
                .DisposedBy(DisposeBag);

            var enabledColor = AndroidColor.Black;
            var disabledColor = AndroidColor.LightGray;
            ViewModel.Save.Enabled
                .Select(createProjectTextColor)
                .Subscribe(createProjectButton.SetTextColor)
                .DisposedBy(DisposeBag);

            string clientNameWithEmptyText(string clientName)
                => string.IsNullOrEmpty(clientName) ? FoundationResources.AddClient : clientName;

            AndroidColor clientTextColor(string clientName)
                => string.IsNullOrEmpty(clientName) ? noClientColor : AndroidColor.Black;

            AndroidColor createProjectTextColor(bool enabled)
                => enabled ? enabledColor : disabledColor;
        }
    }
}

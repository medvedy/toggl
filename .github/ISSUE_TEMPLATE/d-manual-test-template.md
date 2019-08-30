---
name: "🐧 Manual Test Template"
about: A manual testing checklist that can be assigned to a release to track the progress of testing.

---

# 🐧 Manual Test Template

## Automated with UI tests

The following tests have corresponding UI tests, so they can be considered as passing if all the UI tests are passing. These are already checked off to save you a few seconds, but you still need to run and check the UI tests.

### Testing sign in area
- [x] Test standard email and password sign in
- [x] Test standard email and password sign in with invalid email address
- [x] Test signup with standard email and password
- [x] Test forgotten password link
- [x] Test forgotten password link with invalid email

### Testing timer page
- [x] Start timer
- [x] Stop timer
- [x] Enter description
- [x] Add new project
- [x] Add new client
- [x] Add new tag
- [x] Add existing project
- [x] Add existing tag
- [x] Start new timer, select no project, stop
- [x] Create new manual time entry
- [x] Start a new entry, add multiple tags.
- [x] Start a new entry, discard new entry.

### Testing editing time entry
- [x] Change description
- [x] Change project
- [x] Remove project
- [x] Create new project from edit view

## Manual tests

These tests still need to be done the old fashion way.

### Testing sign in area
- [ ] Test standard email and password sign in with invalid password
- [ ] Test terms and conditions link
- [ ] Test privacy policy link
- [ ] Test signin with Google signin
- [ ] Test signup with Google signin
- [ ] Verify that forgot password button sends an e-mail to the provided e-mail address
- [ ] Is country selection automatically selecting the right country?
- [ ] Test email field with mixed character sets (including various languages)
- [ ] Test password field for old/changed password
- [ ] Is country selection allowing manual selection
- [ ] Test email field with invalid email

### Testing timer page
- [ ] Add long (3000 characters) description and confirm character limit error shows in the edit view after sync
- [ ] Add new project using existing client
- [ ] Continue an entry
- [ ] Delete an entry

### Right to left language at random (Arabic, Aramaic, Azeri, Dhivehi/Maldivian, Hebrew, Kurdish (Sorani), Persian/Farsi, Urdu) Though not strictly RTL this could also apply to Chinese, Korean, Japanese and some other languages.

> ℹ Note: This were not working in iOS 1.22, so we might want to skip this tests until they are fixed.

- [ ] Start a timer and enter a description (Is it appearing from the right direction)?
- [ ] Start a timer and enter a description and create a project (Is it appearing from the right direction)?
- [ ] Start a timer and enter a description and create a tag (Is it appearing from the right direction)?
- [ ] Start a timer and enter a description and create a project and a tag (Is it appearing from the right direction)?

### Testing editing time entry
- [ ] Change tags
- [ ] Remove tags
- [ ] Change start time with the barrel/manual selection
- [ ] Change start time with the wheel
- [ ] Change end time with barrel/manual selection
- [ ] Change start date
- [ ] Change end date
- [ ] Change duration manually
- [ ] Change date from main entry page
- [ ] Discard changes to entry
- [ ] Delete entry from edit view
- [ ] Confirm changes
- [ ] Create new client from edit view
- [ ] Create new tag from edit view

### Grouped Time Entries
- [ ] Make sure the `Group time entries` setting is propagated to the server
- [ ] Check whether the main log respects the grouping setting
- [ ] Check whether the group count is correct in both expanded and collapsed groups
- [ ] Collapse and expand various groups (ones with and without descriptions, tags, or projects, etc.)
- [ ] Tap to edit a group header
- [ ] Tap to edit a TE in an expanded group
- [ ] Swipe to delete a group
- [ ] Test the undo feature when attempting to delete a group
- [ ] Swipe to delete a TE from an expanded group
- [ ] Test the undo feature when attempting to delete a lone entry
- [ ] Edit a TE from a group and change something and check that it is no longer in the group
- [ ] Edit a TE group
- [ ] Delete a TE group from an Edit View
- [ ] Check whether the group summary time on both main log and in Edit view is the same and correct

### Testing report screen
- [ ] Custom range
- [ ] Is correct data displayed
- [ ] Are categories displaying correct timerframe and dates?
- [ ] Is web displaying the same data?
- [ ] Today
- [ ] Yesterday
- [ ] This year
- [ ] This month
- [ ] Last month
- [ ] This week
- [ ] Last week
- [ ] Custom range 1 year
- [ ] Custom range 1 month
- [ ] Custom range 1 week
- [ ] Custom range 1 day
- [ ] Ensure correct year is selected on all options
- [ ] Change workspaces

### Testing settings page
- [ ] Sign out normal email
- [ ] Sign out google sign in
- [ ] Change workspace
- [ ] Date format MM/DD/YYYY
- [ ] Date format DD-MM-YYYY
- [ ] Date format MM-DD-YYYY
- [ ] Date format YYYY-MM-DD
- [ ] Date format DD/MM/YYYY
- [ ] Date format DD.MM.YYYY
- [ ] 24 hour time format
- [ ] 12 hour format
- [ ] Duration format Classic
- [ ] Duration format Improved
- [ ] Duration format Decimal
- [ ] First day of the week Monday
- [ ] First day of the week Tuesday
- [ ] First day of the week Wednesday
- [ ] First day of the week Thursday
- [ ] First day of the week Friday
- [ ] First day of the week Saturday
- [ ] First day of the week Sunday
- [ ] Manual mode
- [ ] Submit feedback
- [ ] About
- [ ] Help

### Testing Calendar Integration
- [ ] Are calendar entries showing correctly?
- [ ] Create an entry from a calendar entry
- [ ] Edit the entry by adjusting the start time of the entry (is this reflected on the calendar page?)
- [ ] Edit the entry by adjusting the end time (is this reflected on the calendar page?)
- [ ] Adjust the time of the calendar entry from your calendar to earlier on the current day (is this reflected?)
- [ ] Delete the calendar entry from your calendar (is this reflected?)
- [ ] Move the calendar entry from your calendar to another day (is this reflected?)

### Testing Siri Integration
- [ ] Enable Siri Integration
- [ ] Start a time entry
- [ ] Stop a time entry

### Multiple Workspaces
- [ ] Switch workspaces
- [ ] Go offline then switch workspaces (Airplane mode)
- [ ] Switch between workspaces while timer running
- [ ] Is data displaying correctly in web?
- [ ] Force sync then switch
- [ ] Leave a workspace
- [ ] Be removed from a workspace while offline
- [ ] Be removed from a workspace with an active entry running

### Testing Interactions/Sync between this app and others
- [ ] App correctly syncs on startup (log in, kill app, open app again)
- [ ] Edit client on web whilst mobile online
- [ ] Delete client on web whilst mobile online and tracking a task
- [ ] Edit client on web whilst mobile online and tracking a task
- [ ] Tags
- [ ] Archiving
- [ ] Create new project on web whilst mobile offline
- [ ] Tasks
- [ ] Try creating new entries on web whilst mobile offline
- [ ] Edit client on web whilst mobile offline
- [ ] Try stopping timer on web while mobile offline
- [ ] Delete project on web whilst mobile offline
- [ ] Make changes to projects on web

### Testing in different timezones

- [ ] Set timezone to US and start and stop an entry (does it show up correctly?)
- [ ] Does it show up correctly on web?
- [ ] Set timezone to JP and start and stop an entry (does it show up correctly?)
- [ ] Does it show up correctly on web?
- [ ] Set timezone to AU and start and stop an entry (does it show up correctly?)
- [ ] Does it show up correctly on web?
- [ ] Set timezone to EU and start and stop an entry (does it show up correctly?)
- [ ] Does it show up correctly on web?
- [ ] Set timezone to UK and start and stop an entry (does it show up correctly?)
- [ ] Does it show up correctly on web?

### Testing onboarding bubbles

- [ ] Check that steps are being displayed correctly for first entry
- [ ] Check that steps to edit first entry are being displayed correctly
- [ ] Check that hint to swipe continue entry is shown
- [ ] Check that hint to swipe delete is shown

### Testing Siri
- [ ] Test adding simple shortcuts from Siri Shortcuts section (like start, stop or show reports)
- [ ] Test adding start custom entry shortcuts or/and custom report shortcut
- [ ] Test importing and using Siri Workflows from Settings into the Shortcuts app
- [ ] Test Siri with all those shortcuts and workflows ☝️

### Testing Handoff
- [ ] Test Handoff to web from the timer page
- [ ] Test Handoff to web from the settings page
- [ ] Test Handoff to web from the reports page (check that it shows the same workspace and the same period)
- [ ] Try changing the workspace in the mobile app. Is this handed off to web properly?
- [ ] Try changing the selected period in the mobile app. Is this handed off to web properly?

### Testing app restriction UI

- [ ] Check that all the non-permanent error screens are showing correctly (use the long-press on About) 
  - [ ] Token reset error
  - [ ] No workspace error
  - [ ] No default workspace error
  - [ ] Outdated client error
  - [ ] Outdated API error
- [ ] Check that all the permanent error screens are showing correctly 
  - [ ] Permanent outdated client error
    - [ ] Make sure that after this choice, the app requires reinstall/update
  - [ ] Permanent outdated API error
    - [ ] Make sure that after this choice, the app requires reinstall/update
    
    ### Test that the UI is appearing as intended and report any issues

- [ ] All of the UI tests are passing
- [ ] Check this after testing the app if no UI errors/glitches have occurred

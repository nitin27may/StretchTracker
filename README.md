# Stretch Reminder App

A desktop application that reminds users to take regular stretching breaks and helps them maintain good physical health while working at a computer.

## Overview

Stretch Reminder App is a WPF (.NET) application that helps users prevent physical strain by providing timely reminders to stretch. The app uses computer vision (OpenCV) to detect stretching movements through the user's webcam, tracks progress, and maintains statistics about stretching habits over time.

## Features

- **Customizable Reminder Intervals**: Set reminders from minutes to hours based on your needs
- **Computer Vision Detection**: Uses webcam and motion detection to verify stretching movements
- **Progress Tracking**: Records and displays stretching streaks and completion rates
- **Statistics Dashboard**: Visual representation of your stretching history and habits
- **Minimal Distraction**: Simple notifications that don't interrupt your workflow
- **System Tray Integration**: Runs in the background with system tray access

## Project Structure

### Core Components

- **Core/AppSettings.cs**: Manages user preferences and application configuration
- **Core/DatabaseManager.cs**: Handles SQLite database operations for persistent storage
- **Core/NotificationManager.cs**: Controls timed notifications and user interactions

### User Interface

- **MainWindow.xaml**: Main application window with overview and quick actions
- **StretchWindow.xaml**: Camera interface for detecting and recording stretches
- **StatsWindow.xaml**: Statistics and data visualization dashboard
- **SettingsWindow.xaml**: User configuration interface

## Technology Stack

- **Framework**: .NET 9.0 with WPF
- **Database**: SQLite via Microsoft.EntityFrameworkCore.Sqlite
- **Computer Vision**: OpenCvSharp4 for motion detection
- **Machine Learning**: TensorFlow.NET for pose detection capabilities
- **UI Components**: Native WPF with custom styling
- **Notifications**: Windows system notifications

## Getting Started for Developers

### Prerequisites

- Visual Studio 2022 or later
- .NET 9.0 SDK
- Windows OS (Windows 10 or later recommended)

### Setup and Building

1. Clone the repository
2. Open `StretchReminderApp.sln` in Visual Studio
3. Restore NuGet packages
4. Build the solution

### Key Development Areas

#### Reminder System

The reminder system is controlled by `NotificationManager.cs` which uses a `DispatcherTimer` to schedule notifications based on user preferences stored in `AppSettings.cs`.

```csharp
// Example: How notifications are triggered
_notificationTimer = new DispatcherTimer();
_notificationTimer.Interval = TimeSpan.FromMinutes(_settings.GetEffectiveIntervalMinutes());
_notificationTimer.Tick += (s, e) => ShowNotification();
_notificationTimer.Start();
```

#### Motion Detection Logic

The motion detection system in `StretchWindow.xaml.cs` uses OpenCV to:
1. Capture video frames
2. Calculate frame differences to detect movement
3. Apply thresholds to distinguish significant movements from minor ones
4. Count successful stretches when movement exceeds calibrated thresholds

#### Persistence Layer

Data is stored in a local SQLite database that tracks:
- Completed and skipped stretching sessions
- Session duration
- Date and time of each session

## Extending the Application

### Adding New Stretch Types

To add detection for specific stretch types (beyond simple motion detection):

1. Enhance the `ProcessFrameForMotionAsync` method in `StretchWindow.xaml.cs`
2. Add pose-specific detection using the TensorFlow integration

### Implementing New Statistics

To add new statistical views:

1. Create query methods in `DatabaseManager.cs`
2. Implement visualization in `StatsWindow.xaml.cs`

### Creating Additional Notifications

To customize notifications:

1. Modify the `ShowNotification` method in `NotificationManager.cs`
2. Update UI elements and behaviors as needed

## Known Issues and Roadmap

- Pose detection needs refinement to more accurately identify specific stretches
- Currently only supports Windows - cross-platform support planned
- Future enhancements planned for customizable stretch routines
- Mobile companion app integration in consideration

## Contributing

We welcome contributions! Please feel free to submit pull requests with new features or bug fixes.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
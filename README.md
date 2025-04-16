# Stretch Reminder App

A desktop application that reminds users to take regular stretching breaks and helps them maintain good physical health while working at a computer.

## Overview

Stretch Reminder App is a WPF (.NET) application that helps users prevent physical strain by providing timely reminders to stretch. The app uses computer vision (OpenCV) and machine learning (TensorFlow) to detect stretching movements through the user's webcam, tracks progress, and maintains statistics about stretching habits over time.

![App Launch](docs/screenshots/main-window.png)
![Stretch Detection](docs/screenshots/stretchdetectionandprogress.png)

## Features

- **Customizable Reminder Intervals**: Set reminders from minutes to hours based on your needs
- **Computer Vision Detection**: Uses webcam and motion detection to verify stretching movements
- **Machine Learning Integration**: TensorFlow.NET for advanced pose detection capabilities
- **Progress Tracking**: Records and displays stretching streaks and completion rates
- **Statistics Dashboard**: Visual representation of your stretching history and habits
- **Minimal Distraction**: Simple notifications that don't interrupt your workflow
- **System Tray Integration**: Runs in the background with system tray access

## Movement Detection Implementation

### OpenCV-Based Motion Detection

The core movement detection system uses **OpenCvSharp4**, a .NET wrapper for OpenCV, to perform frame-by-frame analysis:

```csharp
// Motion detection algorithm overview
1. Capture video frames from webcam using VideoCapture
2. Convert frames to grayscale with Cv2.CvtColor
3. Apply Gaussian blur (Cv2.GaussianBlur) to reduce noise
4. Calculate frame differences between current and previous frames (Cv2.Absdiff)
5. Apply binary threshold to isolate significant movements (Cv2.Threshold)
6. Apply morphological operations to filter out minor movements:
   - Erosion to remove small noise (Cv2.Erode)
   - Dilation to enhance true motion areas (Cv2.Dilate)
7. Quantify movement by summing white pixels (Cv2.Sum)
8. Compare against calibrated threshold to determine if stretching
```

The system includes an automatic calibration phase that adjusts detection thresholds to the user's environment, making it work well in various lighting conditions and with different webcam qualities.

### TensorFlow Integration

The app leverages **TensorFlow.NET** to provide sophisticated pose detection capabilities:

- **Human Pose Estimation**: Identifies key body points and their positions using machine learning
- **Specific Stretch Recognition**: Works to identify and validate different types of stretching moves
- **Quality Assessment**: Analyzes the effectiveness and completeness of stretching movements
- **Adaptive Learning**: Improves detection accuracy over time by learning from user behaviors

```csharp
// Example: TensorFlow integration for pose detection
private async Task<bool> ProcessFrameWithTensorFlowAsync(Mat frame)
{
    // Convert frame to tensor format
    var tensor = ImageToTensor(frame);
    
    // Run inference through TensorFlow model
    var output = _poseDetectionModel.Run(
        new[] { _poseDetectionModel.Graph["input"][0] },
        new[] { tensor },
        new[] { _poseDetectionModel.Graph["output"][0] }
    );
    
    // Process detected keypoints to determine if stretching
    return AnalyzePoseForStretching(output);
}
```

## Documentation

For complete documentation, please visit our [GitHub Pages site](https://nitin27may.github.io/StretchReminderApp/).

- [Getting Started](https://nitin27may.github.io/StretchReminderApp/getting-started)
- [Features](https://nitin27may.github.io/StretchReminderApp/features)
- [Technical Details](https://nitin27may.github.io/StretchReminderApp/technical-details)
- [Developer Guide](https://nitin27may.github.io/StretchReminderApp/developers)

### GitHub Pages Deployment

Documentation is automatically deployed to GitHub Pages using GitHub Actions whenever changes are made to the `docs/` directory. The workflow:

1. Builds the Jekyll site using the just-the-docs theme
2. Deploys the built site to GitHub Pages
3. Makes the documentation available at https://nitin27may.github.io/StretchReminderApp/

To modify the documentation:
1. Update files in the `docs/` directory
2. Commit and push changes to the main branch
3. GitHub Actions will automatically build and deploy the updated documentation

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

### Setup

1. Clone the repository
2. Open `StretchReminderApp.sln` in Visual Studio
3. Restore NuGet packages
4. Build the solution

## Contributing

We welcome contributions! Please feel free to submit pull requests with new features or bug fixes. See our [Developer Guide](https://nitin27may.github.io/StretchReminderApp/developers) for more information.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
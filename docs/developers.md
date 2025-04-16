---
layout: default
title: Developer Guide
nav_order: 5
---

# Developer Guide

This section provides technical information for developers interested in understanding or contributing to the Stretch Reminder App.

## Project Structure

The application follows a modular structure:

### Core Components

- **Core/AppSettings.cs**: Manages user preferences and application configuration
- **Core/DatabaseManager.cs**: Handles SQLite database operations for persistent storage
- **Core/NotificationManager.cs**: Controls timed notifications and user interactions

### User Interface

- **MainWindow.xaml**: Main application window with overview and quick actions
- **StretchWindow.xaml**: Camera interface for detecting and recording stretches
- **StatsWindow.xaml**: Statistics and data visualization dashboard
- **SettingsWindow.xaml**: User configuration interface

## TensorFlow Integration

### Current Implementation

The app currently includes TensorFlow.NET as a dependency and is in the process of enhancing the motion detection with pose estimation capabilities.

#### TensorFlow Dependencies

```xml
<ItemGroup>
  <PackageReference Include="TensorFlow.NET" Version="0.150.0" />
  <PackageReference Include="SciSharp.TensorFlow.Redist" Version="2.16.0" />
</ItemGroup>
```

### Implementing Pose Detection

To expand the app's stretching detection capabilities:

1. **Model Selection**: We use MoveNet, a lightweight but effective pose estimation model
2. **Model Integration**: The TensorFlow model needs to be loaded at application startup
3. **Frame Processing**: Each webcam frame is processed through the pose detection model
4. **Pose Analysis**: The detected key points are analyzed to identify specific stretches

#### Planned Implementation

```csharp
// TensorFlow session and model
private TF.Session _tfSession;
private bool _tensorflowEnabled = false;

// Load TensorFlow model
private void InitializeTensorFlow()
{
    try
    {
        // Try to load the model
        _tfSession = LoadPoseDetectionModel();
        _tensorflowEnabled = true;
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"TensorFlow initialization failed: {ex.Message}");
        _tensorflowEnabled = false;
    }
}

// Process frame with TensorFlow if available
private async Task<bool> ProcessFrameWithTensorFlowAsync(Mat frame)
{
    if (!_tensorflowEnabled || _tfSession == null)
        return ProcessFrameForMotionAsync(frame); // Fall back to OpenCV

    // Convert frame to format expected by TensorFlow
    var tensor = ImageToTensor(frame);
    
    // Run inference
    var result = _tfSession.Run(
        new[] { _tfSession.Graph["input_tensor"][0] },
        new[] { tensor },
        new[] { _tfSession.Graph["output_tensor"][0] }
    );

    // Process pose keypoints to determine if stretching
    bool isStretching = AnalyzePoseForStretching(result);
    
    // Visualize keypoints on frame
    if (isStretching)
        DrawPoseOnFrame(frame, result);
        
    return isStretching;
}
```

### Pose Classification Logic

The app uses the following approach to classify stretching movements:

1. **Keypoint Tracking**: Monitor the movement of key body points (shoulders, arms, etc.)
2. **Movement Analysis**: Calculate the displacement of joints between frames
3. **Pose Recognition**: Compare detected poses against known stretching patterns
4. **Threshold Application**: Apply configurable thresholds to determine valid stretches

## Contributing

To contribute to the TensorFlow integration:

1. **Model Training**: Help train the pose detection model with more stretching examples
2. **Pose Classification**: Improve the logic for identifying specific stretches
3. **Performance Optimization**: Enhance the processing speed and efficiency
4. **UI Improvements**: Develop better visual feedback for detected poses

Please submit pull requests with clear descriptions of the changes and their purpose.
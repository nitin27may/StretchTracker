using OpenCvSharp;
using StretchReminderApp.Core;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StretchReminderApp.UI
{
    public partial class StretchWindow : System.Windows.Window
    {
        private VideoCapture _capture;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly DatabaseManager _dbManager;
        private DateTime _startTime;
        private int _stretchesDetected = 0;
        private readonly AppSettings _settings;

        // Simplified approach - not using TensorFlow for now
        private Random _random;
        private System.Timers.Timer _detectionTimer;

        public StretchWindow(DatabaseManager dbManager)
        {
            InitializeComponent();

            _dbManager = dbManager;
            _startTime = DateTime.Now;
            _settings = AppSettings.Load();
            _random = new Random();

            Loaded += StretchWindow_Loaded;
            Closing += StretchWindow_Closing;
        }

        private void StretchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Start webcam
            try
            {
                _capture = new VideoCapture(0); // 0 = default camera
                if (!_capture.IsOpened())
                {
                    MessageBox.Show("Unable to access webcam. Please check your camera connection and permissions.",
                        "Camera Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                // Set resolution 
                _capture.Set(VideoCaptureProperties.FrameWidth, 640);
                _capture.Set(VideoCaptureProperties.FrameHeight, 480);

                StatusTextBlock.Text = "Ready! Move around to start stretching...";

                // Start processing in a separate thread
                _cancellationTokenSource = new CancellationTokenSource();
                Task.Run(() => ProcessFrames(_cancellationTokenSource.Token));

                // Set up a timer for simulated stretch detection
                _detectionTimer = new System.Timers.Timer(3000); // Check every 3 seconds
                _detectionTimer.Elapsed += OnDetectionTimerElapsed;
                _detectionTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing camera: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void OnDetectionTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                // For testing: simulate stretch detection with random probability
                // In a real app, replace this with actual pose detection logic
                if (_random.NextDouble() > 0.6) // 40% chance of detecting a stretch
                {
                    _stretchesDetected++;

                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar.Value = (double)_stretchesDetected / _settings.RequiredStretchCount * 100;
                        StatusTextBlock.Text = $"Stretch detected! {_stretchesDetected}/{_settings.RequiredStretchCount}";
                    });

                    // Complete when enough stretches are detected
                    if (_stretchesDetected >= _settings.RequiredStretchCount)
                    {
                        _detectionTimer.Stop();

                        int sessionDuration = (int)(DateTime.Now - _startTime).TotalSeconds;
                        _dbManager.RecordStretchSession(true, sessionDuration);

                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Great job! Stretching session completed.",
                                "Session Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    StatusTextBlock.Text = $"Error: {ex.Message}";
                });
            }
        }

        private async void ProcessFrames(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    using (var frame = new Mat())
                    {
                        if (_capture.Read(frame))
                        {
                            // Skip empty frames
                            if (frame.Empty())
                                continue;

                            // Convert Mat to BitmapSource without using WriteableBitmapConverter
                            var bitmap = MatToBitmapSource(frame);

                            await Dispatcher.InvokeAsync(() => WebcamPreview.Source = bitmap);
                        }

                        // Small delay to reduce CPU usage
                        await Task.Delay(30, cancellationToken);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Normal cancellation, ignore
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    StatusTextBlock.Text = $"Error processing frames: {ex.Message}";
                });
            }
        }

        // Custom conversion from OpenCV Mat to WPF BitmapSource
        private BitmapSource MatToBitmapSource(Mat image)
        {
            try
            {
                // Convert BGR to RGB
                using (Mat rgbImage = new Mat())
                {
                    Cv2.CvtColor(image, rgbImage, ColorConversionCodes.BGR2RGB);

                    // Create BitmapSource from raw data
                    int width = rgbImage.Width;
                    int height = rgbImage.Height;
                    int stride = width * 3; // 3 bytes per pixel (RGB)
                    byte[] data = new byte[stride * height];

                    // Copy data from Mat
                    if (rgbImage.IsContinuous())
                    {
                        // If data is continuous, we can copy it directly
                        Marshal.Copy(rgbImage.Data, data, 0, data.Length);
                    }
                    else
                    {
                        // If not continuous, copy row by row
                        for (int y = 0; y < height; y++)
                        {
                            IntPtr rowPtr = rgbImage.Ptr(y);
                            Marshal.Copy(rowPtr, data, y * stride, stride);
                        }
                    }

                    // Create BitmapSource
                    return BitmapSource.Create(
                        width, height,
                        96, 96, // DPI
                        PixelFormats.Rgb24,
                        null,
                        data,
                        stride);
                }
            }
            catch
            {
                // Fallback method using MemoryStream and BitmapImage
                try
                {
                    byte[] imageData;
                    Cv2.ImEncode(".bmp", image, out imageData);

                    using (var ms = new MemoryStream(imageData))
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = ms;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze(); // Important for cross-thread access
                        return bitmapImage;
                    }
                }
                catch
                {
                    // If all else fails, return a blank image
                    return BitmapSource.Create(
                        1, 1, 96, 96, PixelFormats.Rgb24, null,
                        new byte[3] { 0, 0, 0 }, 3);
                }
            }
        }

        private void StretchWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Clean up resources
            _cancellationTokenSource?.Cancel();
            _capture?.Dispose();
            _detectionTimer?.Stop();
            _detectionTimer?.Dispose();

            // If window is closed before completion, record as incomplete
            if (_stretchesDetected < _settings.RequiredStretchCount)
            {
                int sessionDuration = (int)(DateTime.Now - _startTime).TotalSeconds;
                _dbManager.RecordStretchSession(false, sessionDuration);
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            int sessionDuration = (int)(DateTime.Now - _startTime).TotalSeconds;
            _dbManager.RecordStretchSession(false, sessionDuration);
            Close();
        }
    }
}
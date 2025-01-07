# maui-scrolling-performance

Test repository for measuring scrolling performance of .NET MAUI apps on Android.

Measuring scrolling peformance on Android is a bit tricky. The
[Android docs][android-docs] describe a [`FrameMetricsAggregator`][frame-metrics]
class we can use to measure frame times or as Google calls it, "jank".

We can use this to measure the scrolling performance of .NET MAUI apps.

[android-docs]: https://developer.android.com/topic/performance/measuring-performance
[frame-metrics]: https://developer.android.com/reference/androidx/core/app/FrameMetricsAggregator

For iOS, we can use `CADisplayLink` to measure the frame times, such
as the example on:

* https://thisiskyle.me/posts/measuring-ios-scroll-performance-is-tough-use-this-to-make-it-simple-and-automated.html

## How to Setup?

### iOS or MacCatalyst

In either `Platforms/MacCatalyst/AppDelegate.cs` or `Platforms/iOS/AppDelegate.cs`:

```csharp
public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
{
    var result = base.FinishedLaunching(application, launchOptions);
    iOS.FrameMetrics.FrameMetricsReporter.Initialize();
    return result;
}
```

This uses `CADisplayLink` to measure how fast the display can refresh.

Run the app and scroll around a bit. You can see output such as:

```log
2024-05-01 14:34:50.404623-0500 CvSlowJittering[98066:9539784] Frames dropped: 38
2024-05-01 14:34:50.404726-0500 CvSlowJittering[98066:9539784] Cumulative frames dropped: 76
2024-05-01 14:34:50.404787-0500 CvSlowJittering[98066:9539784] -----
2024-05-01 14:34:50.955571-0500 CvSlowJittering[98066:9539784] Frames dropped: 32
2024-05-01 14:34:50.955675-0500 CvSlowJittering[98066:9539784] Cumulative frames dropped: 108
2024-05-01 14:34:50.955729-0500 CvSlowJittering[98066:9539784] -----
2024-05-01 14:34:51.149430-0500 CvSlowJittering[98066:9539784] Frames dropped: 6
2024-05-01 14:34:51.149549-0500 CvSlowJittering[98066:9539784] Cumulative frames dropped: 114
2024-05-01 14:34:51.149626-0500 CvSlowJittering[98066:9539784] -----
```

I haven't written a script to capture this data yet, so you'll have to
do this manually for now. A `CollectionView` with a single `Label`
will drop very few frames, while the examples in `external` will drop
many frames.

### Android

In `Platforms/Android/MainActivity.cs`:

```csharp
protected override void OnCreate(Bundle savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    Android.FrameMetrics.FrameMetricsReporter.Initialize(this);
}
```

Build & run your app in `Release` mode:

```dotnetcli
dotnet build -c Release -t:Run
```

Find out the name of the current activity:

```powershell
> adb shell 'dumpsys window | grep mCurrentFocus'
  mCurrentFocus=Window{f01fc52 u0 com.companyname.surfingapp/crc6435627e4593d70ff9.MainActivity}
```

Run the script:

```powershell
.\scripts\scroll-performance.ps1 -package com.companyname.surfingapp -activity crc6435627e4593d70ff9.MainActivity
...
05-08 14:55:47.347 31085 31085 I DOTNET  : Frame(s) that took ~9ms, count: 2
05-08 14:55:47.348 31085 31085 I DOTNET  : Frame(s) that took ~10ms, count: 6
05-08 14:55:47.348 31085 31085 I DOTNET  : Frame(s) that took ~11ms, count: 20
05-08 14:55:47.348 31085 31085 I DOTNET  : Frame(s) that took ~12ms, count: 2
05-08 14:55:47.348 31085 31085 I DOTNET  : Frame(s) that took ~15ms, count: 1
05-08 14:55:47.348 31085 31085 I DOTNET  : Frame(s) that took ~16ms, count: 1
05-08 14:55:47.348 31085 31085 I DOTNET  : Frame(s) that took ~19ms, count: 1
05-08 14:55:47.367 31085 31085 I DOTNET  : Frame(s) that took ~22ms, count: 17
05-08 14:55:47.367 31085 31085 I DOTNET  : Frame(s) that took ~23ms, count: 4
05-08 14:55:47.368 31085 31085 I DOTNET  : Average frame time: 15.52ms
05-08 14:55:47.368 31085 31085 I DOTNET  : No. of slow frames: 23
05-08 14:55:47.368 31085 31085 I DOTNET  : -----
```

This scrolls this app a few times and prints out the average frame
time and the number of frames that took longer than 16ms (60fps).

![img](docs/surferapp.png)

## Results

Installed software:

```md
.NET SDK 9.0.101

Installed Workload Id      Manifest Version       Installation Source
---------------------------------------------------------------------
android                    35.0.7/9.0.100         VS 17.12.35527.113
ios                        18.1.9163/9.0.100      VS 17.12.35527.113
maccatalyst                18.1.9163/9.0.100      VS 17.12.35527.113
maui-windows               9.0.0/9.0.100          VS 17.12.35527.113
```

| .NET version | Sample  | Device      | Average Frame (ms) | Slow Frames |
| ------------ | ------- | ----------- | -----------------: | ----------: |
| .NET 8       | cvslow  | Pixel 7 Pro |             6.90ms |          10 |
| .NET 8       | surfing | Pixel 7 Pro |             7.39ms |          14 |
| .NET 8       | gallery | Pixel 7 Pro |             9.20ms |          34 |

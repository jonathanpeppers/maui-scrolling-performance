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

## Comparing .NET 7 and .NET 8

Testing [this branch](https://github.com/jonathanpeppers/maui/tree/net8.0-FAST)
to just bring some of the very latest changes over. Using a Pixel 5 (released in Oct 2020):

```powershell
05-08 15:45:52.946  5684  5684 I DOTNET  : Frame(s) that took ~9ms, count: 1
05-08 15:45:52.947  5684  5684 I DOTNET  : Frame(s) that took ~10ms, count: 1
05-08 15:45:52.947  5684  5684 I DOTNET  : Frame(s) that took ~11ms, count: 13
05-08 15:45:52.947  5684  5684 I DOTNET  : Frame(s) that took ~13ms, count: 3
05-08 15:45:52.947  5684  5684 I DOTNET  : Average frame time: 11.17ms
05-08 15:45:52.947  5684  5684 I DOTNET  : No. of slow frames: 0
05-08 15:45:52.947  5684  5684 I DOTNET  : -----
```

The average frame time dropped from 15.52ms to 11.17ms, and we no
longer have any frames that take longer than 16ms (or 60 fps).

If we plot these timings, you can get an interesting comparison:

![img](docs/net7vs8.png)

## Some Results

On a Pixel 7, `external/gallery` using the script:

```
> .\scripts\scroll-performance.ps1 -package com.companyname.mauicollectionviewgallery -activity crc64335d5648794690ab.MainActivity
...
06-04 16:31:28.204 31433 31433 I DOTNET  : Frame(s) that took ~3ms, count: 2
06-04 16:31:28.204 31433 31433 I DOTNET  : Frame(s) that took ~4ms, count: 4
06-04 16:31:28.204 31433 31433 I DOTNET  : Frame(s) that took ~5ms, count: 7
06-04 16:31:28.205 31433 31433 I DOTNET  : Frame(s) that took ~6ms, count: 14
06-04 16:31:28.205 31433 31433 I DOTNET  : Frame(s) that took ~7ms, count: 79
06-04 16:31:28.205 31433 31433 I DOTNET  : Frame(s) that took ~8ms, count: 7
06-04 16:31:28.205 31433 31433 I DOTNET  : Frame(s) that took ~9ms, count: 5
06-04 16:31:28.205 31433 31433 I DOTNET  : Frame(s) that took ~10ms, count: 6
06-04 16:31:28.205 31433 31433 I DOTNET  : Frame(s) that took ~11ms, count: 2
06-04 16:31:28.206 31433 31433 I DOTNET  : Frame(s) that took ~12ms, count: 3
06-04 16:31:28.206 31433 31433 I DOTNET  : Frame(s) that took ~13ms, count: 3
06-04 16:31:28.206 31433 31433 I DOTNET  : Frame(s) that took ~14ms, count: 3
06-04 16:31:28.206 31433 31433 I DOTNET  : Frame(s) that took ~15ms, count: 1
06-04 16:31:28.206 31433 31433 I DOTNET  : Frame(s) that took ~16ms, count: 1
06-04 16:31:28.206 31433 31433 I DOTNET  : Frame(s) that took ~17ms, count: 3
06-04 16:31:28.206 31433 31433 I DOTNET  : Frame(s) that took ~20ms, count: 2
06-04 16:31:28.207 31433 31433 I DOTNET  : Frame(s) that took ~21ms, count: 2
06-04 16:31:28.207 31433 31433 I DOTNET  : Frame(s) that took ~26ms, count: 1
06-04 16:31:28.207 31433 31433 I DOTNET  : Frame(s) that took ~28ms, count: 1
06-04 16:31:28.207 31433 31433 I DOTNET  : Average frame time: 8.30ms
06-04 16:31:28.207 31433 31433 I DOTNET  : No. of slow frames: 10
06-04 16:31:28.207 31433 31433 I DOTNET  : -----
```

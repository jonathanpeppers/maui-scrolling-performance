# maui-scrolling-performance

Test repository for measuring scrolling performance of .NET MAUI apps on Android.

## How to Setup?

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

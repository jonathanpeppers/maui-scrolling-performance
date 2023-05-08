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
05-08 14:47:41.927 28769 28769 I DOTNET  : Frame(s) that took ~6ms, count: 2
05-08 14:47:41.928 28769 28769 I DOTNET  : Frame(s) that took ~7ms, count: 1
05-08 14:47:41.928 28769 28769 I DOTNET  : Frame(s) that took ~8ms, count: 1
05-08 14:47:41.928 28769 28769 I DOTNET  : Frame(s) that took ~9ms, count: 11
05-08 14:47:41.928 28769 28769 I DOTNET  : Frame(s) that took ~10ms, count: 1
05-08 14:47:41.928 28769 28769 I DOTNET  : Frame(s) that took ~11ms, count: 22
05-08 14:47:41.929 28769 28769 I DOTNET  : Frame(s) that took ~14ms, count: 1
05-08 14:47:41.929 28769 28769 I DOTNET  : Average frame time: 10.05ms
05-08 14:47:41.929 28769 28769 I DOTNET  : No. of slow frames: 0
05-08 14:47:41.929 28769 28769 I DOTNET  : -----
```
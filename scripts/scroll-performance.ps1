param (
    [Parameter(Mandatory=$true)]
    [string] $package,
    [Parameter(Mandatory=$true)]
    [string] $activity,
    [int] $iterations = 5,
    [int] $sleep = 1
)

# Clear some properties, so we don't accidentally hurt startup perf
& adb shell setprop debug.mono.log 0
& adb shell setprop debug.mono.profile 0
# Clear window animations
& adb shell settings put global window_animation_scale 0
& adb shell settings put global transition_animation_scale 0
& adb shell settings put global animator_duration_scale 0

# Do an initial launch and leave the app on screen
& adb shell am force-stop $package
& adb logcat -c
& adb shell am start -n "$package/$activity" -W

Start-Sleep -Seconds $sleep

# Scroll down N times
for ($i=0; $i -lt $iterations; $i++) {
    $x = ($i + 1) * 500;
    $y = ($i + 1) * 1000;
    & adb shell input swipe $x $y 300 300
    Start-Sleep -Seconds $sleep
}

# Print logs
& adb logcat -d | grep DOTNET -CaseSensitive
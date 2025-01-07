param (
    [Parameter(Mandatory=$true)]
    [string] $package,
    [Parameter(Mandatory=$true)]
    [string] $activity,
    [int] $iterations = 10,
    [int] $sleep = 300
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

Start-Sleep -Milliseconds $sleep

# Scroll down N times
for ($i=0; $i -lt $iterations; $i++) {
    & adb shell input touchscreen swipe 500 1800 500 300 $sleep
}

# Print logs
& adb logcat -d | grep DOTNET -CaseSensitive
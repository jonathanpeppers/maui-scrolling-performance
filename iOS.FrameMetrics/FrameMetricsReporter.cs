using CoreAnimation;
using CoreFoundation;
using Foundation;

namespace iOS.FrameMetrics;

public static class FrameMetricsReporter
{
    static CADisplayLink displayLink = null!;
    static DispatchQueue queue = null!;
    static double lastTimestamp;
    static int currentFrameDropCount;

    public static void Initialize()
    {
        displayLink = CADisplayLink.Create(OnFrame);
        displayLink.Paused = true;
        displayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
        queue = DispatchQueue.GetGlobalQueue(DispatchQueuePriority.High);

        // Start the display link
        displayLink.Paused = false;
    }

    static void OnFrame()
    {
        queue.DispatchAsync(() =>
        {
            if (lastTimestamp == 0)
            {
                lastTimestamp = displayLink.Timestamp;
                return;
            }

            double duration = displayLink.Duration;
            if (duration == 0)
            {
                return;
            }

            double numberOfFrames = Math.Round((displayLink.Timestamp - lastTimestamp) / duration);
            lastTimestamp = displayLink.Timestamp;
            CalculateNumberOfDroppedFrames((int)numberOfFrames);
        });
    }

    static void CalculateNumberOfDroppedFrames(int numberOfFrames)
    {
        int droppedFrameCount = numberOfFrames - 1 > 0 ? numberOfFrames : 0;
        currentFrameDropCount += droppedFrameCount;
        if (droppedFrameCount > 0)
        {
            currentFrameDropCount++;
        }
        if (droppedFrameCount > 0)
        {
            Console.WriteLine($"Frames dropped: {droppedFrameCount}");
            Console.WriteLine($"Cumulative frames dropped: {currentFrameDropCount}");
            Console.WriteLine("-----");
        }
    }

    public static void Stop()
    {
        displayLink.Paused = true;
        lastTimestamp = 0;
    }
}

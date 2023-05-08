#nullable enable
using Android.App;
using Android.OS;
using AndroidX.Core.App;

namespace Android.FrameMetrics;

public static class FrameMetricsReporter
{
    const int SlowFrameThreshold = 16;
    const int Duration = 1000;

    static FrameMetricsAggregator? aggregator;
    static Handler? handler;

    public static void Initialize(Activity activity, int duration = Duration)
    {
        handler = new Handler(Looper.MainLooper!);
        aggregator = new FrameMetricsAggregator(FrameMetricsAggregator.TotalDuration);
        aggregator.Add(activity);

        handler.PostDelayed(OnFrame, Duration);
    }

    public static void Stop()
    {
        handler?.Dispose();
        handler = null;

        aggregator?.Dispose();
        aggregator = null;
    }

    static void OnFrame()
    {
        if (handler == null || aggregator == null)
            return;

        var metrics = aggregator.GetMetrics();
        if (metrics != null)
        {
            var index = metrics[FrameMetricsAggregator.TotalIndex];
            int size = index.Size();
            double sum = 0, count = 0, slow = 0;
            for (int i = 0; i < size; i++)
            {
                int value = index.Get(i);
                if (value != 0)
                {
                    count += value;
                    sum += i * value;
                    if (i >= SlowFrameThreshold)
                        slow += value;
                    Console.WriteLine($"Frame(s) that took ~{i}ms, count: {value}");
                }
            }
            if (sum > 0)
            {
                Console.WriteLine($"Average frame time: {sum / count:0.00}ms");
                Console.WriteLine($"No. of slow frames: {slow}");
                Console.WriteLine("-----");
            }
        }
        handler.PostDelayed(OnFrame, Duration);
    }
}
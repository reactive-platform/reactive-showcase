using UnityEngine;

namespace ReactiveShowcase.Examples;

internal static class TopicInfo {
    public static string Name(Topic topic) {
        return topic switch {
            Topic.Introduction => "Introduction",
            Topic.State => "State",
            Topic.Layout => "Layout",
            Topic.Input => "Input",
            Topic.Lists => "Lists and reuse",
            _ => "Animation & time"
        };
    }

    public static string Description(Topic topic) {
        return topic switch {
            Topic.Introduction => "A quick look at what a component is and what it can do. Start here.",
            Topic.State => "Values that notify their readers, and everything derived from them.",
            Topic.Layout => "Flexbox: sizing, growing, positioning, showing and hiding, and how layout trees connect.",
            Topic.Input => "Pointer events and what you can build from them: scrolling, holding and dragging.",
            Topic.Lists => "Showing data as cells, reusing them, and pooling components instead of rebuilding them.",
            _ => "Animated states and tracks, the update loop, coroutines and custom interpolation."
        };
    }

    public static Color Color(Topic topic) {
        return topic switch {
            Topic.Introduction => new(0.95f, 0.95f, 0.95f),
            Topic.State => new(0.35f, 0.9f, 0.45f),
            Topic.Layout => new(0.55f, 0.85f, 1f),
            Topic.Input => new(1f, 0.85f, 0.3f),
            Topic.Lists => new(1f, 0.55f, 0.25f),
            _ => new(1f, 0.4f, 0.75f)
        };
    }
}

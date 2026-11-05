using System.Collections.Generic;
using System.Linq;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Examples;

internal class PointerEventsExample : Example {
    public override string Title => "Pointer events";
    public override Topic Topic => Topic.Input;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "Every pointer event a graphic gets, and the handler's own flags.";

    public override string Explanation =>
        "WithPointerEvents attaches a handler to any graphic and calls you on enter, leave, down, up, drag and scroll. " +
        "onUpdate runs after any of them, which is the place to copy the handler's IsHovered, IsPressed and IsDragging " +
        "into states. The log keeps the latest events in a list state, shown as one line of text.";

    public override IReactiveComponent CreateDemo() => new PointerEventsDemo();
}

internal class PointerEventsDemo : ReactiveComponent {
    private const int LogSize = 4;

    private readonly record struct Entry(int Number, string Text);

    private State<IReadOnlyList<Entry>> _log = null!;
    private int _count;

    protected override GameObject Construct() {
        _log = Remember<IReadOnlyList<Entry>>([]);

        var hovered = Remember(false);
        var pressed = Remember(false);
        var dragging = Remember(false);

        var color = RememberDerived(
            hovered, pressed,
            (h, p) => p ? new Color(1f, 0.2f, 0.8f, 0.8f) : h ? new Color(0f, 0.75f, 1f, 0.6f) : Color.white with { a = 0.1f }
        );

        var flags = RememberDerived(
            hovered, pressed, dragging,
            (h, p, d) => $"hovered {Mark(h)}   pressed {Mark(p)}   dragging {Mark(d)}"
        );

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new Background {
                    FlexController = {
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center
                    },

                    FlexItem = {
                        Size = new() { x = 40.pt, y = 11.pt }
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    sColor = color,

                    Do = x => x.WithPointerEvents(
                        onEnter: _ => Log("enter"),
                        onLeave: _ => Log("leave"),
                        onDown: _ => Log("down"),
                        onUp: _ => Log("up"),
                        onDragBegin: _ => Log("drag begin"),
                        onDragEnd: _ => Log("drag end"),
                        onScroll: h => Log($"scroll {h.EventData?.scrollDelta.y:0.0}"),

                        onUpdate: h => {
                            hovered.Value = h.IsHovered;
                            pressed.Value = h.IsPressed;
                            dragging.Value = h.IsDragging;
                        }
                    ),

                    Children = {
                        new Label {
                            Text = "Hover, click, drag, scroll",
                            FontSize = 3.5f
                        }
                    }
                },

                new Label {
                    FontSize = 3f,
                    sText = flags
                },

                new Label {
                    FontSize = 2.8f,
                    Color = Color.white with { a = 0.7f },
                    sText = _log.Map(x => string.Join("   ·   ", x.Select(y => $"{y.Number}. {y.Text}")))
                }
            }
        }.Use();
    }

    private void Log(string text) {
        var log = _log.Value.Append(new Entry(++_count, text));
        _log.Value = log.Skip(System.Math.Max(0, _log.Value.Count + 1 - LogSize)).ToArray();
    }

    private static string Mark(bool value) => value ? "yes" : "no";
}

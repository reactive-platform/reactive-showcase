using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;

namespace ReactiveShowcase.Examples;

internal class SizingUnitsExample : Example {
    public override string Title => "Sizing units";
    public override Topic Topic => Topic.Layout;
    public override Difficulty Difficulty => Difficulty.Easy;
    public override bool Centered => true;
    public override string Summary => "Points, percents, auto, aspect ratio and min/max limits side by side.";

    public override string Explanation =>
        "A size can be fixed points (.pt), a share of the parent (.pct), \"auto\" to fit the content, or come from the " +
        "other axis through AspectRatio. MinSize and MaxSize clamp whatever the rest decided. Change the parent's " +
        "width and watch which boxes follow it: only the percent ones, and the clamped one only within its limits.";

    public override IReactiveComponent CreateDemo() => new SizingUnitsDemo();
}

internal class SizingUnitsDemo : ReactiveComponent {
    private const float CaptionWidth = 20f;
    private const float TrackHeight = 6f;

    protected override GameObject Construct() {
        var width = Remember(44f);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 2.pt
            },

            Children = {
                new BsSlider {
                    MinValue = 24f,
                    MaxValue = 56f,
                    ValueStep = 2f,
                    Value = width.Value,
                    ValueFormatter = x => $"parent {x:0}pt",
                    OnValueChanged = x => width.Value = x
                },

                new Layout {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        AlignItems = Align.FlexStart,
                        Gap = 1.pt
                    },

                    Children = {
                        CreateRow("12pt", new() { Size = new() { x = 12.pt, y = 5.pt } }, width),
                        CreateRow("50%", new() { Size = new() { x = 50.pct, y = 5.pt } }, width),
                        CreateRow("auto", new() { Size = new() { x = YogaValue.Auto, y = 5.pt } }, width),
                        CreateRow("50%, 15-22pt", new() { Size = new() { x = 50.pct, y = 5.pt }, MinSize = new() { x = 15.pt }, MaxSize = new() { x = 22.pt } }, width),
                        CreateRow("aspect 1", new() { Size = new() { y = 5.pt }, AspectRatio = 1f }, width)
                    }
                }
            }
        }.Use();
    }

    [StateGen]
    private static Layout CreateRow(string rule, YogaModifier modifier, State<float> parentWidth) {
        var size = Remember(Vector2.zero);

        return new Layout {
            FlexController = {
                AlignItems = Align.Center,
                Gap = 1.pt
            },

            Children = {
                new Label {
                    FlexItem = {
                        Size = new() { x = CaptionWidth.pt }
                    },

                    FontSize = 2.4f,
                    Alignment = TextAlignmentOptions.CaplineLeft,
                    sText = size.Map(x => $"{rule}\n<alpha=#99>{x.x:0} x {x.y:0}")
                },

                new Background {
                    FlexController = {
                        AlignItems = Align.Center
                    },

                    FlexItem = {
                        sSize = parentWidth.Map(x => new YogaVector(x.pt, TrackHeight.pt))
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = Color.white with { a = 0.06f },

                    Children = {
                        new Background {
                            FlexController = {
                                JustifyContent = Justify.Center,
                                AlignItems = Align.Center,
                                Padding = new() { left = 1.pt, right = 1.pt }
                            },

                            Sprite = BeatSaberResources.Sprites.background,
                            PixelsPerUnit = 10f,
                            Color = new Color(0f, 0.75f, 1f, 0.6f),

                            // auto needs something to fit, so the box holds a short label
                            Children = {
                                new Label {
                                    Text = "box",
                                    FontSize = 2.4f
                                }
                            }
                        }.WithModifier(modifier).With(x => x.LayoutAppliedEvent += _ => size.Value = x.ContentTransform.rect.size)
                    }
                }
            }
        };
    }
}

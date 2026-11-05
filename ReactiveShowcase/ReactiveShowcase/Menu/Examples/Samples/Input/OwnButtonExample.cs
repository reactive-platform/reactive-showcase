using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Examples;

internal class OwnButtonExample : Example {
    public override string Title => "Hover and press states";
    public override Topic Topic => Topic.Input;
    public override Difficulty Difficulty => Difficulty.Intermediate;
    public override bool Centered => true;
    public override string Summary => "What the built-in buttons do inside: a graphic state and color sets.";

    public override string Explanation =>
        "Built-in components keep a GraphicState, flags for hovered, active and not interactable, and pointer events " +
        "only flip those flags. A ColorSet then maps every combination to a color, and Map turns the state into the " +
        "background and text colors. The press scale is one more state reading the same flags.";

    public override IReactiveComponent CreateDemo() => new OwnButtonDemo();
}

internal class OwnButtonDemo : ReactiveComponent {
    // One color per state, missing ones fall back to the plain color
    private static readonly ColorSet backgroundColors = new() {
        Color = new Color(1f, 1f, 1f, 0.1f),
        HoveredColor = new Color(0f, 0.75f, 1f, 0.5f),
        ActiveColor = new Color(0f, 0.75f, 1f, 0.9f),
        NotInteractableColor = new Color(1f, 1f, 1f, 0.03f)
    };

    private static readonly ColorSet textColors = new() {
        Color = Color.white,
        NotInteractableColor = Color.white with { a = 0.3f }
    };

    protected override GameObject Construct() {
        var graphic = Remember(GraphicState.None);
        var clicks = Remember(0);

        var scale = RememberAnimated(1f, 120.ms, AnimationCurve.EaseOutCubic);
        graphic.AddCallback(x => scale.TargetValue = x.IsActive ? 0.92f : 1f);

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Center,
                Gap = 3.pt
            },

            Children = {
                new Background {
                    FlexController = {
                        JustifyContent = Justify.Center,
                        AlignItems = Align.Center
                    },

                    FlexItem = {
                        Size = new() { x = 34.pt, y = 9.pt }
                    },

                    ContentTransform = {
                        slocalScale = scale.Map(x => Vector3.one * x)
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    sColor = graphic.Map(x => backgroundColors.GetColor(x)),

                    Do = x => x.WithPointerEvents(
                        onEnter: _ => graphic.IsHovered = true,
                        onLeave: _ => graphic.IsHovered = false,
                        onDown: _ => {
                            graphic.IsActive = true;

                            if (graphic.IsInteractable) {
                                clicks.Value++;
                            }
                        },
                        onUp: _ => graphic.IsActive = false
                    ),

                    Children = {
                        new Label {
                            FontSize = 4f,
                            sText = clicks.Map(x => $"Clicked {x}"),
                            sColor = graphic.Map(x => textColors.GetColor(x))
                        }
                    }
                },

                new Layout {
                    FlexController = {
                        AlignItems = Align.Center,
                        Gap = 1.pt
                    },

                    Children = {
                        new Label {
                            Text = "Interactable",
                            FontSize = 3.5f
                        },

                        new BsToggle {
                            Toggled = true,
                            OnToggleStateChanged = x => graphic.IsInteractable = x
                        }
                    }
                },

                new Label {
                    FontSize = 3f,
                    Color = Color.white with { a = 0.6f },
                    sText = graphic.Map(x => $"hovered {x.IsHovered}   active {x.IsActive}   interactable {x.IsInteractable}")
                }
            }
        }.Use();
    }
}

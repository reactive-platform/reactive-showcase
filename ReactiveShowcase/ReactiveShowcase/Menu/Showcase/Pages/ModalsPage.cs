using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using AnimationCurve = Reactive.AnimationCurve;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class ModalsPage : ShowcasePage {
    private enum OpenStyle {
        Jump,
        Fade,
        Zoom,
        Drop,
        Instant
    }

    // Every style resets both alpha and scale, as the previous close might have left either of them off
    private static readonly IAnimationTrack<IReactiveComponent> opaque = AnimationTracks.Fade(1f, 1f);
    private static readonly IAnimationTrack<IReactiveComponent> unscaled = AnimationTracks.Scale(1f, 1f);

    private static readonly Dictionary<OpenStyle, (Animation<IReactiveComponent> Open, Animation<IReactiveComponent> Close)> styles = new() {
        [OpenStyle.Jump] = (
            ModalAnimations.JumpOpen with { Track = ModalAnimations.JumpOpen.Track + opaque },
            ModalAnimations.JumpClose with { Track = ModalAnimations.JumpClose.Track + opaque }
        ),
        [OpenStyle.Fade] = (
            new(AnimationTracks.Fade(0f, 1f) + unscaled, 0.25f, AnimationCurve.EaseOutCubic),
            new(AnimationTracks.Fade(1f, 0f) + unscaled, 0.2f, AnimationCurve.EaseInCubic)
        ),
        [OpenStyle.Zoom] = (
            new(AnimationTracks.Fade(0f, 1f) + AnimationTracks.Scale(0.4f, 1f), 0.35f, AnimationCurve.EaseOutBack),
            new(AnimationTracks.Fade(1f, 0f) + AnimationTracks.Scale(1f, 0.4f), 0.2f, AnimationCurve.EaseInBack)
        ),
        [OpenStyle.Drop] = (
            new(opaque + AnimationTracks.Scale(new Vector3(1f, 0f, 1f), Vector3.one), 0.4f, AnimationCurve.EaseOutBack),
            new(AnimationTracks.Fade(1f, 0f) + AnimationTracks.Scale(Vector3.one, new Vector3(1.3f, 1.3f, 1f)), 0.2f, AnimationCurve.EaseOutCubic)
        ),
        [OpenStyle.Instant] = (
            new(opaque + unscaled, 0f),
            new(opaque + unscaled, 0f)
        )
    };

    private static readonly Dictionary<string, AnimationCurve?> dimCurves = new() {
        // A missing curve means linear
        ["Linear"] = null,
        ["Ease Out"] = AnimationCurve.EaseOutCubic,
        ["Ease In Out"] = AnimationCurve.EaseInOutSine,
        ["Exponential"] = AnimationCurve.Exponential
    };

    private static readonly Dictionary<RelativePlacement, string> placements = new() {
        [RelativePlacement.Center] = "Center",
        [RelativePlacement.TopCenter] = "Top",
        [RelativePlacement.BottomCenter] = "Bottom",
        [RelativePlacement.LeftCenter] = "Left",
        [RelativePlacement.RightCenter] = "Right"
    };

    private static Animation<IReactiveComponent> Scale(Animation<IReactiveComponent> animation, float speed) {
        return animation with { Duration = animation.Duration / speed };
    }

    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var buttonColors = Palette.Map(ShowcaseStyles.Button);
        var primaryColors = Palette.Map(ShowcaseStyles.PrimaryButton);

        var opened = Remember(false);
        var style = Remember(OpenStyle.Jump);
        var placement = Remember(RelativePlacement.Center);
        var dim = Remember(true);
        var speed = Remember(1f);
        var dimAlpha = Remember(new CompositionAnimParams().DimmedAlpha);
        var dimDuration = Remember(new CompositionAnimParams().Duration);
        var dimCurve = Remember("Linear");
        var anchor = Ref<RectTransform>();

        var openAnimation = RememberDerived(style, speed, (s, k) => (Animation<IReactiveComponent>?)Scale(styles[s].Open, k));
        var closeAnimation = RememberDerived(style, speed, (s, k) => (Animation<IReactiveComponent>?)Scale(styles[s].Close, k));

        var composition = RememberDerived(
            dim, dimAlpha, dimDuration, dimCurve,
            (dim, alpha, duration, curve) => new CompositionAnimParams {
                DimCanvas = dim,
                DimmedAlpha = alpha,
                Duration = duration,
                Curve = dimCurves[curve]
            }
        );

        var dimCurveItems = new Dictionary<string, BsDropdownItem>();
        foreach (var name in dimCurves.Keys) {
            dimCurveItems[name] = new(name, null);
        }

        var dialogOpened = Remember(false);
        var dialogResult = Remember("No answer yet");
        var dialogAnchor = Ref<RectTransform>();

        var styleItems = new Dictionary<OpenStyle, BsSegmentedControlItem>();
        foreach (var key in styles.Keys) {
            styleItems[key] = new(key.ToString(), null);
        }

        var placementItems = new Dictionary<RelativePlacement, BsDropdownItem>();
        foreach (var pair in placements) {
            placementItems[pair.Key] = new(pair.Value, null);
        }

        return [
            Section(
                "Modal",
                "Open and close animations are plain Animation values built from tracks, so they can be swapped at any time. The modal is placed against the button.",
                Row(
                    Labeled("Animation", new BsSegmentedControl<OpenStyle> {
                        FlexItem = {
                            Size = new() { x = 70.pt, y = 6.pt }
                        },

                        sColors = Palette.Map(ShowcaseStyles.SegmentedControl),
                        Items = styleItems,
                        Key = OpenStyle.Jump,
                        OnKeyChanged = x => style.Value = x
                    }),

                    Labeled("Placement", new BsDropdown<RelativePlacement> {
                        FlexItem = {
                            Size = new() { x = 24.pt, y = 6.pt }
                        },

                        sColors = Palette.Map(ShowcaseStyles.Dropdown),
                        Items = placementItems,
                        Key = RelativePlacement.Center,
                        OnKeyChanged = x => placement.Value = x
                    })
                ),
                Row(
                    Labeled("Speed", new BsSlider {
                        sColors = Palette.Map(ShowcaseStyles.Slider),
                        MinValue = 0.25f,
                        MaxValue = 3f,
                        ValueStep = 0.25f,
                        Value = speed.Value,
                        ValueFormatter = x => $"{x:0.00}x",
                        OnValueChanged = x => speed.Value = x
                    }),

                    Labeled("Dimmed alpha", new BsSlider {
                        sColors = Palette.Map(ShowcaseStyles.Slider),
                        sInteractable = dim,
                        MinValue = 0f,
                        MaxValue = 1f,
                        ValueStep = 0.05f,
                        Value = dimAlpha.Value,
                        ValueFormatter = x => $"{x:0.00}",
                        OnValueChanged = x => dimAlpha.Value = x
                    }),

                    Labeled("Dim duration", new BsSlider {
                        sColors = Palette.Map(ShowcaseStyles.Slider),
                        sInteractable = dim,
                        MinValue = 0f,
                        MaxValue = 1f,
                        ValueStep = 0.05f,
                        Value = dimDuration.Value,
                        ValueFormatter = x => $"{x:0.00}s",
                        OnValueChanged = x => dimDuration.Value = x
                    })
                ),

                Row(
                    ToggleRow("Dim canvas", true, x => dim.Value = x),

                    new BsDropdown<string> {
                        FlexItem = {
                            Size = new() { x = 28.pt, y = 6.pt }
                        },

                        sColors = Palette.Map(ShowcaseStyles.Dropdown),
                        sInteractable = dim,
                        Items = dimCurveItems,
                        Key = dimCurve.Value,
                        OnKeyChanged = x => dimCurve.Value = x
                    }.InNamedRail("Dim curve").With(x => x.Label.FontSize = 3.5f),

                    new BsPrimaryTextButton {
                        Text = "Open modal",
                        sColors = primaryColors,
                        OnClick = () => opened.Value = true
                    }.Bind(anchor)
                ),

                new Modal {
                    sIsPushed = opened,
                    PlacementAnchor = anchor,
                    sPlacementData = placement.Map(x => new PlacementData { Placement = x, Offset = Vector2.zero }),
                    sCompositionAnimation = composition,
                    sOpenAnimation = openAnimation,
                    sCloseAnimation = closeAnimation,
                    OnClickOutside = () => opened.Value = false,

                    Children = {
                        new Background {
                            FlexController = {
                                FlexDirection = FlexDirection.Column,
                                AlignItems = Align.Center,
                                Padding = 3.pt,
                                Gap = 2.pt
                            },

                            FlexItem = {
                                MinSize = new() { x = 50.pt }
                            },

                            Children = {
                                new Label {
                                    FontSize = 5f,
                                    sText = style.Map(x => $"{x} modal"),
                                    sColor = Palette.Map(x => x.Primary)
                                },

                                new Label {
                                    Text = "Click outside or press close",
                                    FontSize = 3f,
                                    Color = Color.white with { a = 0.6f }
                                },

                                new BsTextButton {
                                    Text = "Close",
                                    sColors = buttonColors,
                                    OnClick = () => opened.Value = false
                                }
                            }
                        }.AsBlurBackground()
                    }
                }
            ),

            Section(
                "Dialog",
                "A modal with a header and OK / Cancel buttons wired from outside.",
                Row(
                    new BsPrimaryTextButton {
                        Text = "Delete everything?",
                        sColors = primaryColors,
                        OnClick = () => dialogOpened.Value = true
                    }.Bind(dialogAnchor),

                    new Label {
                        sText = dialogResult,
                        sColor = Palette.Map(x => x.Secondary)
                    }
                ),

                new Dialog {
                    Title = "Careful",
                    sIsPushed = dialogOpened,
                    PlacementAnchor = dialogAnchor,
                    OnClickOutside = () => dialogOpened.Value = false,

                    FlexController = {
                        Padding = 3.pt
                    },

                    Children = {
                        new Label {
                            Text = "This would delete everything.\nNothing is actually deleted.",
                            FontSize = 3.5f
                        }
                    }
                }.With(x => {
                    x.OkButton.OnClick = () => {
                        dialogResult.Value = "Confirmed (nothing happened)";
                        dialogOpened.Value = false;
                    };

                    x.CancelButton.OnClick = () => {
                        dialogResult.Value = "Cancelled";
                        dialogOpened.Value = false;
                    };
                })
            )
        ];
    }
}

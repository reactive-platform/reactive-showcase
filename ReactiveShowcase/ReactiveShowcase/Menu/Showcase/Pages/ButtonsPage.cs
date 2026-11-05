using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using AnimationCurve = Reactive.AnimationCurve;
using Image = Reactive.BeatSaber.Components.Image;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class ButtonsPage : ShowcasePage {
    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var interactable = Remember(true);
        var clicks = Remember(0);
        var colors = Palette.Map(ShowcaseStyles.Button);

        var skew = RememberAnimated(-0.3f, 1200.ms, AnimationCurve.EaseInOutSine);
        skew.OnFinish = x => skew.TargetValue = -x;
        skew.TargetValue = 0.3f;

        var width = RememberAnimated(24f, 350.ms, AnimationCurve.EaseOutBack);
        width.OnFinish = x => width.TargetValue = 24f;

        return [
            Section(
                "States",
                "Text buttons with the stock style and the palette. Hover to see the hovered colors, the toggle flips Interactable on the right ones.",
                Row(
                    Labeled("Stock", new BsTextButton { Text = "Stock" }),
                    Labeled("Themed", new BsTextButton { Text = "Themed", sColors = colors }),
                    Labeled("No underline", new BsTextButton { Text = "Plain", ShowUnderline = false, sColors = colors }),
                    Labeled("No skew", new BsTextButton { Text = "Straight", Skew = 0f, sColors = colors }),
                    Labeled("Interactable", new BsTextButton { Text = "Maybe", sInteractable = interactable, sColors = colors }),
                    Labeled("Interactable", new BsImageButton {
                        FlexItem = { Size = 8.pt },
                        Sprite = GameResources.EditIcon,
                        sInteractable = interactable,
                        sColors = colors
                    })
                ),
                ToggleRow("Interactable", true, x => interactable.Value = x)
            ),

            Section(
                "Image buttons",
                "BsImageButton tints the sprite with the content colors.",
                Row(
                    CreateImageButton(GameResources.CloseIcon),
                    CreateImageButton(GameResources.EditIcon),
                    CreateImageButton(GameResources.SearchIcon),
                    CreateImageButton(GameResources.ArrowIcon),
                    CreateImageButton(GameResources.CaretIcon),
                    CreateImageButton(BeatSaberResources.Sprites.spinnerIcon)
                )
            ),

            Section(
                "Custom content",
                "BsButton hands its color and skew states to the content, so anything inside follows the hover state.",
                Row(
                    new BsButton {
                        FlexController = {
                            Padding = new() { left = 2.pt, right = 2.pt },
                            AlignItems = Align.Center,
                            Gap = 1.pt
                        },

                        sColors = colors,
                        OnClick = () => clicks.Value++,

                        ConstructContent = (color, s) => new Layout {
                            FlexController = {
                                AlignItems = Align.Center,
                                Gap = 1.pt
                            },

                            Children = {
                                new Image {
                                    FlexItem = { Size = 4.pt },
                                    Sprite = GameResources.ArrowUpIcon,
                                    PreserveAspect = true,
                                    sColor = color.In(),
                                    sSkew = s.In()
                                },

                                new Label {
                                    sText = clicks.Map(x => $"Clicked {x} times"),
                                    sColor = color.In()
                                }
                            }
                        }
                    }
                )
            ),

            Section(
                "Animated props",
                "Skew is driven by an animated state that ping-pongs with an ease-in-out curve. The right button animates its own width with an overshooting curve when clicked.",
                Row(
                    new BsTextButton { Text = "Wobble", sSkew = skew, sColors = colors },
                    new BsImageButton {
                        FlexItem = { Size = 8.pt },
                        Sprite = GameResources.CircleIcon,
                        sSkew = skew,
                        sColors = colors
                    },
                    new BsTextButton {
                        FlexItem = {
                            sSize = width.Map(x => new YogaVector(x.pt, 8.pt))
                        },

                        Text = "Boing!",
                        sColors = colors,
                        OnClick = () => width.TargetValue = 40f
                    }
                )
            ),

            Sizes(
                "Text and image buttons keep their content centered, the underline and skew follow the width.",
                CreateSizeSample,
                new(8f, 4f),
                new(60f, 16f),
                new(14f, 5f), new(24f, 7f), new(40f, 11f)
            )
        ];
    }

    [StateGen]
    private ILayoutItem CreateSizeSample() {
        return new BsTextButton {
            Text = "Resize me",
            sColors = Palette.Map(ShowcaseStyles.Button)
        };
    }

    [StateGen]
    private static BsImageButton CreateImageButton(Sprite sprite) {
        return new BsImageButton {
            FlexItem = { Size = 8.pt },
            Sprite = sprite,
            PreserveAspect = true,
            sColors = Palette.Map(ShowcaseStyles.Button)
        };
    }
}

using System.Collections;
using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class PrimaryButtonsPage : ShowcasePage {
    private enum Phase {
        Idle,
        Loading,
        Done
    }

    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var colors = Palette.Map(ShowcaseStyles.PrimaryButton);
        var interactable = Remember(true);
        var phase = Remember(Phase.Idle);

        IEnumerator RunFakeTask() {
            phase.Value = Phase.Loading;
            yield return new WaitForSeconds(1.5f);

            phase.Value = Phase.Done;
            yield return new WaitForSeconds(1.2f);

            phase.Value = Phase.Idle;
        }

        return [
            Section(
                "Gradient",
                "Primary buttons are drawn with a gradient, a border and an outline. The themed ones take the gradient from the primary and secondary colors.",
                Row(
                    Labeled("Stock", new BsPrimaryTextButton { Text = "Play" }),
                    Labeled("Themed", new BsPrimaryTextButton { Text = "Play", sColors = colors }),
                    Labeled("No skew", new BsPrimaryTextButton { Text = "Play", Skew = 0f, sColors = colors }),
                    Labeled("Image", new BsPrimaryImageButton {
                        FlexItem = { Size = 8.pt },
                        Sprite = GameResources.ArrowIcon,
                        PreserveAspect = true,
                        sColors = colors
                    })
                )
            ),

            Section(
                "Interactable",
                "Non-interactable buttons fall back to the NotInteractable colors of each color set.",
                Row(
                    new BsPrimaryTextButton { Text = "Submit", sInteractable = interactable, sColors = colors },
                    new BsPrimaryImageButton {
                        FlexItem = { Size = 8.pt },
                        Sprite = GameResources.SearchIcon,
                        PreserveAspect = true,
                        sInteractable = interactable,
                        sColors = colors
                    }
                ),
                ToggleRow("Interactable", true, x => interactable.Value = x)
            ),

            Section(
                "Async action",
                "A BsPrimaryButton with custom content: a spinner shows up while the fake task runs, and the button stays disabled until it's done.",
                Row(
                    new BsPrimaryButton {
                        FlexItem = {
                            Size = new() { x = 36.pt, y = 8.pt }
                        },

                        FlexController = {
                            JustifyContent = Justify.Center,
                            AlignItems = Align.Center,
                            Gap = 1.pt
                        },

                        sColors = colors,
                        sInteractable = phase.Map(x => x == Phase.Idle),
                        OnClick = () => StartCoroutine(RunFakeTask()),

                        ConstructContent = (color, _) => new Layout {
                            FlexController = {
                                AlignItems = Align.Center,
                                Gap = 1.pt
                            },

                            Children = {
                                new Spinner {
                                    FlexItem = { Size = 4.pt },
                                    sEnabled = phase.Map(x => x == Phase.Loading),
                                    Image = { sColor = color.In() }
                                },

                                new Label {
                                    sText = phase.Map(x => x switch {
                                        Phase.Loading => "Uploading...",
                                        Phase.Done => "Done!",
                                        _ => "Upload"
                                    }),
                                    sColor = color.In()
                                }
                            }
                        }
                    }
                )
            ),

            Sizes(
                "The gradient, border and outline stretch with the button, the text stays centered.",
                CreateSizeSample,
                new(10f, 4f),
                new(60f, 16f),
                new(16f, 5f), new(26f, 7f), new(42f, 11f)
            )
        ];
    }

    [StateGen]
    private ILayoutItem CreateSizeSample() {
        return new BsPrimaryTextButton {
            Text = "Play",
            sColors = Palette.Map(ShowcaseStyles.PrimaryButton)
        };
    }
}

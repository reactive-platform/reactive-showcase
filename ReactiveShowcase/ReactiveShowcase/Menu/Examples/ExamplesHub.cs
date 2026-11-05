using System;
using System.Collections.Generic;
using System.Linq;
using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Components;
using Reactive.Components.Basic;
using Reactive.Yoga;
using ReactiveShowcase.Showcase;
using TMPro;
using UnityEngine;
using Label = Reactive.BeatSaber.Components.Label;
using ScrollArea = Reactive.BeatSaber.Components.ScrollArea;

namespace ReactiveShowcase.Examples;

internal class ExamplesHub : ReactiveComponent {
    public Action? OnShowcaseSelected { get; set; }
    public Action<Example>? OnExampleSelected { get; set; }

    private const float CardWidth = 47f;
    private const float CardGap = 1.5f;
    private const int CardsPerRow = 3;

    private const float ContentWidth = CardWidth * CardsPerRow + CardGap * (CardsPerRow - 1);
    private static readonly Color ShowcaseColor = new(0.75f, 0.55f, 1f);

    private readonly record struct Card(string Title, string Summary, string? Tag, Color TagColor, Action OnClick);

    private readonly record struct Group(string Title, string Description, Color Color, IReadOnlyList<Card> Cards, bool Centered = false);

    protected override GameObject Construct() {
        var context = new ScrollContext();

        return new Layout {
            FlexController = {
                JustifyContent = Justify.Center,
                AlignItems = Align.Stretch,
                Gap = 1.pt
            },

            Children = {
                new ScrollArea {
                    FlexItem = {
                        Size = new() { x = ContentWidth.pt }
                    },

                    ScrollContext = context,

                    ScrollContent = new Repeater<Group, Layout> {
                        FlexController = {
                            FlexDirection = FlexDirection.Column,
                            AlignItems = Align.Stretch,
                            Gap = 2.pt,
                            ConstrainVertical = false
                        },

                        SelectionMode = SelectionMode.None,
                        Items = CreateGroups(),
                        ConstructCell = CreateGroup
                    }
                },

                new Scrollbar {
                    ScrollContext = context
                }
            }
        }.Use();
    }

    private IReadOnlyList<Group> CreateGroups() {
        var showcase = new Group(
            "★  Showcase",
            "Every predefined component in one place.",
            ShowcaseColor,
            [new Card("Component Showcase", "Every predefined component with its options, sizes and states.", null, default, () => OnShowcaseSelected?.Invoke())],
            Centered: true
        );

        var topics = ExampleCatalog.All
            .GroupBy(x => x.Topic)
            .OrderBy(x => x.Key)
            .Select(x => new Group(
                TopicInfo.Name(x.Key),
                TopicInfo.Description(x.Key),
                TopicInfo.Color(x.Key),
                x.Select(CreateCardData).ToArray()
            ));

        return topics.Prepend(showcase).ToArray();
    }

    private Card CreateCardData(Example example) {
        return new Card(
            example.Title,
            example.Summary,
            $"{DifficultyInfo.Dots(example.Difficulty)}  {DifficultyInfo.Name(example.Difficulty)}",
            DifficultyInfo.Color(example.Difficulty),
            () => OnExampleSelected?.Invoke(example)
        );
    }

    [StateGen]
    private static Layout CreateGroup(CellContext<Group> context) {
        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Stretch,
                Gap = 0.5f.pt
            },

            FlexItem = {
                Margin = new() { top = 1.pt }
            },

            Children = {
                new Label {
                    FontSize = 4.2f,
                    FontStyle = FontStyles.Bold,
                    sAlignment = context.Map(x => x.Item.Centered ? TextAlignmentOptions.Capline : TextAlignmentOptions.CaplineLeft),
                    sText = context.Map(x => x.Item.Title),
                    sColor = context.Map(x => x.Item.Color)
                },

                new Label {
                    FontSize = 2.8f,
                    sAlignment = context.Map(x => x.Item.Centered ? TextAlignmentOptions.Capline : TextAlignmentOptions.CaplineLeft),
                    Color = Color.white with { a = 0.6f },
                    sText = context.Map(x => x.Item.Description)
                },

                new Repeater<Card, BsButton> {
                    FlexController = {
                        FlexDirection = FlexDirection.Row,
                        FlexWrap = Wrap.Wrap,
                        sJustifyContent = context.Map(x => x.Item.Centered ? Justify.Center : Justify.FlexStart),
                        Gap = CardGap.pt
                    },

                    FlexItem = {
                        Margin = new() { top = 1.pt }
                    },

                    SelectionMode = SelectionMode.None,
                    sItems = context.Map(x => x.Item.Cards),
                    ConstructCell = CreateCard
                }
            }
        };
    }

    [StateGen]
    private static BsButton CreateCard(CellContext<Card> context) {
        return new BsButton {
            FlexController = {
                FlexDirection = FlexDirection.Column,
                AlignItems = Align.Stretch,
                JustifyContent = Justify.FlexStart,
                Padding = new() { left = 2.pt, right = 2.pt, top = 1.5f.pt, bottom = 1.5f.pt }
            },

            FlexItem = {
                Size = new() { x = CardWidth.pt, y = 17.pt }
            },

            Skew = 0f,
            ShowUnderline = false,
            sColors = ShowcaseTheme.Palette.Map(ShowcaseStyles.Button),
            OnClick = () => context.Item.OnClick(),

            ConstructContent = (color, _) => new Layout {
                FlexItem = {
                    Flex = 1
                },

                FlexController = {
                    FlexDirection = FlexDirection.Column,
                    AlignItems = Align.Stretch,
                    Gap = 0.75f.pt
                },

                Children = {
                    new Label {
                        FontSize = 2.4f,
                        Alignment = TextAlignmentOptions.CaplineLeft,
                        sEnabled = context.Map(x => x.Item.Tag != null),
                        sText = context.Map(x => x.Item.Tag ?? string.Empty),
                        sColor = context.Map(x => x.Item.TagColor)
                    },

                    new Label {
                        FontSize = 3.6f,
                        FontStyle = FontStyles.Bold,
                        Alignment = TextAlignmentOptions.CaplineLeft,
                        EnableWrapping = false,
                        Overflow = TextOverflowModes.Ellipsis,
                        sText = context.Map(x => x.Item.Title),
                        sColor = color.In()
                    },

                    new Label {
                        FlexItem = {
                            Flex = 1
                        },

                        EnableWrapping = true,
                        Overflow = TextOverflowModes.Ellipsis,
                        EnableAutoSizing = true,
                        FontSizeMin = 2.2f,
                        FontSizeMax = 2.6f,
                        Alignment = TextAlignmentOptions.TopLeft,
                        Color = Color.white with { a = 0.65f },
                        sText = context.Map(x => x.Item.Summary)
                    }
                }
            }
        };
    }
}

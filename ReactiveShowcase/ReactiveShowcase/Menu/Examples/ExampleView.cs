using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using Background = Reactive.BeatSaber.Components.Background;
using Label = Reactive.BeatSaber.Components.Label;
using ScrollArea = Reactive.BeatSaber.Components.ScrollArea;

namespace ReactiveShowcase.Examples;

internal class ExampleView : ReactiveComponent {
    private const float CodeWidth = 82f;
    private const float CodeFontSize = 2.3f;

    // A line of code is about 1.3 font sizes tall, every scroll moves two of them
    private const float CodeScrollStep = CodeFontSize * 1.3f * 2f;

    private readonly Example _initial;
    private State<Example> _example = null!;
    private Background _demoArea = null!;
    private ScrollContext _codeScroll = null!;
    private IReactiveComponent? _demo;

    public ExampleView(Example example) : base(false) {
        _initial = example;
        ConstructAndInit();
    }

    public void Show(Example example) {
        // Reopening the same example still has to notify, so its demo starts over
        if (_example.Value == example) {
            _example.EnforceInvalidation();
        } else {
            _example.Value = example;
        }
    }

    protected override GameObject Construct() {
        _example = Remember(_initial);
        _codeScroll = new ScrollContext();

        var go = new Layout {
            FlexController = {
                AlignItems = Align.Stretch,
                Gap = 3.pt
            },

            Children = {
                new Layout {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        AlignItems = Align.Stretch,
                        Gap = 1.5f.pt
                    },

                    FlexItem = {
                        Flex = 1
                    },

                    Children = {
                        new Label {
                            FontSize = 5.5f,
                            FontStyle = FontStyles.Bold,
                            Alignment = TextAlignmentOptions.CaplineLeft,
                            sText = _example.Map(x => x.Title)
                        },

                        new Label {
                            FontSize = 3f,
                            Alignment = TextAlignmentOptions.CaplineLeft,
                            sText = _example.Map(x => $"{TopicInfo.Name(x.Topic)}   {DifficultyInfo.Dots(x.Difficulty)}  {DifficultyInfo.Name(x.Difficulty)}"),
                            sColor = _example.Map(x => DifficultyInfo.Color(x.Difficulty))
                        },

                        new Background {
                            FlexController = {
                                JustifyContent = Justify.Center,
                                sAlignItems = _example.Map(x => x.Centered ? Align.Center : Align.FlexStart),
                                Padding = 3.pt
                            },

                            FlexItem = {
                                Flex = 1,
                                Margin = new() { top = 1.pt }
                            },

                            Sprite = BeatSaberResources.Sprites.background,
                            PixelsPerUnit = 10f,
                            Color = Color.black with { a = 0.35f }
                        }.Bind(ref _demoArea)
                    }
                },

                new Background {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        AlignItems = Align.Stretch,
                        Padding = 2.pt,
                        Gap = 1.pt
                    },

                    FlexItem = {
                        Size = new() { x = CodeWidth.pt }
                    },

                    Sprite = BeatSaberResources.Sprites.background,
                    PixelsPerUnit = 10f,
                    Color = new Color(0.07f, 0.07f, 0.09f, 0.85f),

                    Children = {
                        new Label {
                            FontSize = 2.8f,
                            EnableWrapping = true,
                            Alignment = TextAlignmentOptions.TopLeft,
                            Color = Color.white with { a = 0.85f },
                            sText = _example.Map(x => x.Explanation)
                        },

                        new Label {
                            FlexItem = {
                                Margin = new() { top = 1.pt }
                            },

                            FontSize = 2.8f,
                            Alignment = TextAlignmentOptions.CaplineLeft,
                            Color = Color.white with { a = 0.5f },
                            sText = _example.Map(x => x.SourceFile)
                        },

                        new Layout {
                            FlexController = {
                                AlignItems = Align.Stretch,
                                Gap = 1.pt
                            },

                            FlexItem = {
                                Flex = 1
                            },

                            Children = {
                                new ScrollArea {
                                    FlexItem = {
                                        Flex = 1
                                    },

                                    LineSize = CodeScrollStep,
                                    ScrollContext = _codeScroll,

                                    ScrollContent = new Layout {
                                        FlexController = {
                                            FlexDirection = FlexDirection.Column,
                                            AlignItems = Align.Stretch,
                                            ConstrainVertical = false
                                        },

                                        Children = {
                                            new Label {
                                                FontSize = CodeFontSize,
                                                RichText = true,
                                                EnableWrapping = true,
                                                Alignment = TextAlignmentOptions.TopLeft,
                                                Color = new Color(0.86f, 0.86f, 0.86f),
                                                sText = _example.Map(x => ExampleSource.Highlight(ExampleSource.Load(x.SourceFile)))
                                            }
                                        }
                                    }
                                },

                                new Scrollbar {
                                    ScrollContext = _codeScroll,
                                    ScrollMode = ScrollbarScrollMode.Fixed,
                                    FixedScrollSize = CodeScrollStep
                                }
                            }
                        }
                    }
                }
            }
        }.Use();

        _example.AddCallback(this, static (view, example) => view.ReplaceDemo(example));

        return go;
    }

    private void ReplaceDemo(Example example) {
        _codeScroll.ScrollTo(0f, true);

        // Every visit starts from a fresh demo, so state from the last one doesn't leak in
        if (_demo != null) {
            _demoArea.Children.Remove(_demo);
            (_demo as ReactiveComponent)?.Destroy();
        }

        _demo = example.CreateDemo();
        _demoArea.Children.Add(_demo);
    }
}

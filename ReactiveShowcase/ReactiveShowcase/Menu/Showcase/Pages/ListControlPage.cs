using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Components;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;

namespace ReactiveShowcase.Showcase;

internal class ListControlPage : ShowcasePage {
    private static readonly Dictionary<string, Color> namedColors = new() {
        ["Red"] = new(1f, 0.25f, 0.25f),
        ["Orange"] = new(1f, 0.6f, 0.1f),
        ["Yellow"] = new(1f, 0.95f, 0.2f),
        ["Green"] = new(0.3f, 1f, 0.3f),
        ["Cyan"] = new(0.2f, 0.9f, 1f),
        ["Violet"] = new(0.7f, 0.4f, 1f)
    };

    private static readonly string[] difficulties = ["Easy", "Normal", "Hard", "Expert", "Expert+"];

    private BsListControl<string> _autoList = null!;
    private bool _autoplay;
    private float _autoTimer;
    private int _autoIndex;

    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var colors = Palette.Map(ShowcaseStyles.ListControl);
        var interactable = Remember(true);
        var colorKey = Remember("Cyan");

        var difficultyItems = new List<BsListControlItem<string>>();
        foreach (var difficulty in difficulties) {
            difficultyItems.Add(new(difficulty, difficulty, null));
        }

        var colorItems = new List<BsListControlItem<string>>();
        foreach (var name in namedColors.Keys) {
            colorItems.Add(new(name, name, null));
        }

        return [
            Section(
                "Basics",
                "Arrows cycle through the items, wrapping around at the ends.",
                Row(
                    Labeled("Text", new BsListControl<string> {
                        sColors = colors,
                        sInteractable = interactable,
                        Items = difficultyItems,
                        Key = "Expert"
                    }),

                    Labeled("Icons", new BsListControl<int> {
                        sColors = colors,
                        sInteractable = interactable,
                        Items = [
                            new(0, "Edit", GameResources.EditIcon),
                            new(1, "Search", GameResources.SearchIcon),
                            new(2, "Close", GameResources.CloseIcon)
                        ],
                        Key = 0
                    }),

                    Labeled("No skew", new BsListControl<string> {
                        sColors = colors,
                        sInteractable = interactable,
                        Skew = 0f,
                        Items = difficultyItems,
                        Key = "Normal"
                    })
                ),
                ToggleRow("Interactable", true, x => interactable.Value = x)
            ),

            Section(
                "Colors from the key",
                "The content color is derived from the selected item, so the text takes the color it names.",
                new BsListControl<string> {
                    sColors = RememberDerived(Palette, colorKey, (palette, key) => {
                        var c = ShowcaseStyles.ListControl(palette);
                        return c with { ContentColors = new() { Color = namedColors[key] } };
                    }),
                    Items = colorItems,
                    Key = colorKey.Value,
                    OnKeyChanged = x => colorKey.Value = x
                }
            ),

            Section(
                "Autoplay",
                "Setting Key from code works just like clicking the arrows.",
                new BsListControl<string> {
                    sColors = colors,
                    Items = difficultyItems,
                    Key = difficulties[0]
                }.Bind(ref _autoList),
                ToggleRow("Autoplay", false, x => _autoplay = x)
            ),

            Sizes(
                "The arrows keep their size, the text in between takes the rest.",
                CreateSizeSample,
                new(16f, 4f),
                new(70f, 12f),
                new(24f, 5f), new(36f, 7f), new(56f, 9f)
            )
        ];
    }

    [StateGen]
    private ILayoutItem CreateSizeSample() {
        var items = new List<BsListControlItem<string>>();

        foreach (var difficulty in difficulties) {
            items.Add(new(difficulty, difficulty, null));
        }

        return new BsListControl<string> {
            sColors = Palette.Map(ShowcaseStyles.ListControl),
            Items = items,
            Key = "Expert"
        };
    }

    protected override void OnUpdate() {
        if (!_autoplay) {
            return;
        }

        _autoTimer += Time.deltaTime;

        if (_autoTimer >= 0.8f) {
            _autoTimer = 0f;
            _autoIndex = (_autoIndex + 1) % difficulties.Length;
            _autoList.Key = difficulties[_autoIndex];
        }
    }
}

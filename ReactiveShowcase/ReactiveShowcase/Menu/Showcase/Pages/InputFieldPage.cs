using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Compiler;
using Reactive.Yoga;
using TMPro;
using UnityEngine;
using static ReactiveShowcase.Showcase.ShowcaseUi;
using Label = Reactive.BeatSaber.Components.Label;

namespace ReactiveShowcase.Showcase;

internal class InputFieldPage : ShowcasePage {
    private const int MaxDigits = 6;

    [StateGen]
    protected override IEnumerable<ILayoutItem> ConstructSections() {
        var colors = Palette.Map(ShowcaseStyles.InputField);
        var keyboardColors = Palette.Map(ShowcaseStyles.Keyboard);
        var numpadColors = Palette.Map(ShowcaseStyles.Numpad);
        var interactable = Remember(true);
        var showNumbers = Remember(false);
        var numpadText = Remember(string.Empty);
        BsNumpad numpad = null!;

        var searchContext = new BsInputFieldContext();
        var searchAnchor = Ref<RectTransform>();

        // Rejected values never reach the field
        var pinContext = new BsInputFieldContext {
            Validate = x => x == null || (x.Length <= MaxDigits && IsDigits(x))
        };
        var pinAnchor = Ref<RectTransform>();

        var typed = Remember(string.Empty);
        BsKeyboard keyboard = null!;

        return [
            Section(
                "Input field",
                "Click the field to open the keyboard modal, the toggle adds a row of digits to it. The label mirrors the shared context.",
                Row(
                    new BsInputField {
                        FlexItem = {
                            Size = new() { x = 50.pt, y = 8.pt }
                        },


                        Context = searchContext,
                        Placeholder = "Search songs",
                        Icon = GameResources.SearchIcon,
                        sInteractable = interactable,
                        sColors = colors
                    }.Bind(searchAnchor),

                    new Label {
                        Alignment = TextAlignmentOptions.Left,
                        sText = searchContext.Map(x => x.Focused ? "Typing..." : $"Query: {(string.IsNullOrEmpty(x.Text) ? "-" : x.Text)}")
                    }
                ),
                Row(
                    ToggleRow("Interactable", true, x => interactable.Value = x),
                    ToggleRow("Number row", false, x => showNumbers.Value = x)
                ),

                new BsKeyboardModal {
                    Context = searchContext,
                    PlacementAnchor = searchAnchor,
                    sShowNumbersRow = showNumbers,
                    sKeyboardColors = keyboardColors
                }
            ),

            Section(
                "Validation",
                $"Opens a numpad modal. Validate rejects anything that isn't up to {MaxDigits} digits, the counter turns to the secondary color once it's full.",
                Row(
                    new BsInputField {
                        FlexItem = {
                            Size = new() { x = 30.pt, y = 8.pt }
                        },


                        Context = pinContext,
                        Placeholder = "PIN",
                        Icon = GameResources.EditIcon,
                        sColors = colors
                    }.Bind(pinAnchor),

                    new Label {
                        sText = pinContext.Map(x => $"{x.Text?.Length ?? 0}/{MaxDigits}"),
                        sColor = RememberDerived(
                            pinContext,
                            Palette,
                            (context, palette) => context.Text?.Length >= MaxDigits ? palette.Secondary : Color.white
                        )
                    }
                ),

                new BsNumpadModal {
                    Context = pinContext,
                    PlacementAnchor = pinAnchor,
                    sNumpadColors = numpadColors
                }
            ),

            Section(
                "Standalone keyboard",
                "BsKeyboard can be placed anywhere without a modal. Hover the keys to see the themed key colors.",
                new Label {
                    FontSize = 4f,
                    sText = typed.Map(x => string.IsNullOrEmpty(x) ? "Type something below" : x),
                    sColor = Palette.Map(x => x.Primary)
                },
                new BsKeyboard {
                    sColors = keyboardColors,
                    OnTextChanged = x => typed.Value = x ?? string.Empty,
                    OnOkClicked = () => keyboard.Text = null
                }.Bind(ref keyboard)
            ),

            Section(
                "Standalone numpad",
                "BsNumpad is a separate keyboard type for number-only inputs. OK clears it here.",
                Row(
                    new BsNumpad {
                        sColors = numpadColors,
                        OnTextChanged = x => numpadText.Value = x ?? string.Empty,
                        OnOkClicked = () => numpad.Text = null
                    }.Bind(ref numpad),

                    new Label {
                        FontSize = 6f,
                        sText = numpadText.Map(x => string.IsNullOrEmpty(x) ? "0" : x),
                        sColor = Palette.Map(x => x.Primary)
                    }
                )
            ),

            Sizes(
                "The icon stays on the left, the text and placeholder get the remaining width.",
                CreateSizeSample,
                new(16f, 5f),
                new(80f, 14f),
                new(24f, 6f), new(40f, 8f), new(60f, 10f)
            )
        ];
    }

    [StateGen]
    private ILayoutItem CreateSizeSample() {
        return new BsInputField {
            Context = new BsInputFieldContext(),
            Placeholder = "Search songs",
            Icon = GameResources.SearchIcon,
            sColors = Palette.Map(ShowcaseStyles.InputField)
        };
    }

    private static bool IsDigits(string text) {
        foreach (var c in text) {
            if (c is < '0' or > '9') {
                return false;
            }
        }

        return true;
    }
}

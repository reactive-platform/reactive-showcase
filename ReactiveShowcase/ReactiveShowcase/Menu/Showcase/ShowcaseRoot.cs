using System.Collections.Generic;
using Reactive;
using Reactive.BeatSaber;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using Reactive.Yoga;
using UnityEngine;
using Image = Reactive.BeatSaber.Components.Image;

namespace ReactiveShowcase.Showcase;

/// <summary>
/// Vertical page tabs on the left, the selected page on the right and the palette button in the corner.
/// </summary>
internal class ShowcaseRoot : ReactiveComponent {
    private enum Page {
        Buttons,
        Primary,
        Aero,
        Toggle,
        Slider,
        Segmented,
        Dropdown,
        ListControl,
        Input,
        Color,
        Modals,
        Images,
        Labels
    }

    private static readonly Dictionary<Page, string> pageNames = new() {
        [Page.Buttons] = "Buttons",
        [Page.Primary] = "Primary",
        [Page.Aero] = "Aero",
        [Page.Toggle] = "Toggle",
        [Page.Slider] = "Slider",
        [Page.Segmented] = "Segmented",
        [Page.Dropdown] = "Dropdown",
        [Page.ListControl] = "List",
        [Page.Input] = "Input",
        [Page.Color] = "Color",
        [Page.Modals] = "Modals",
        [Page.Images] = "Images",
        [Page.Labels] = "Labels"
    };

    private ThemeMenu _menu = null!;
    private Image _wheel = null!;

    protected override GameObject Construct() {
        var page = Remember(Page.Buttons);
        var menuOpened = Remember(false);

        var items = new Dictionary<Page, BsSegmentedControlItem>();
        foreach (var pair in pageNames) {
            items[pair.Key] = new(pair.Value, null);
        }

        return new Layout {
            FlexController = {
                FlexDirection = FlexDirection.Row,
                AlignItems = Align.Stretch,
                Gap = 2.pt
            },

            Children = {
                new BsSegmentedControl<Page> {
                    FlexController = {
                        FlexDirection = FlexDirection.Column,
                        Gap = 0.5f.pt
                    },

                    FlexItem = {
                        Size = new() { x = 22.pt, y = 100.pct }
                    },

                    sColors = ShowcaseTheme.Palette.Map(ShowcaseStyles.SegmentedControl),
                    Skew = 0f,
                    Items = items,
                    Key = page.Value,
                    OnKeyChanged = x => page.Value = x
                },

                new KeyedContainer<Page> {
                    FlexController = {
                        AlignItems = Align.Stretch
                    },

                    FlexItem = {
                        Flex = 1
                    },

                    sKey = page,

                    // Every page is built the first time its tab gets selected
                    Views = new Dictionary<Page, IReactiveComponent> {
                        [Page.Buttons] = new LazyView { Factory = () => new ButtonsPage() },
                        [Page.Primary] = new LazyView { Factory = () => new PrimaryButtonsPage() },
                        [Page.Aero] = new LazyView { Factory = () => new AeroButtonPage() },
                        [Page.Toggle] = new LazyView { Factory = () => new TogglePage() },
                        [Page.Slider] = new LazyView { Factory = () => new SliderPage() },
                        [Page.Segmented] = new LazyView { Factory = () => new SegmentedControlPage() },
                        [Page.Dropdown] = new LazyView { Factory = () => new DropdownPage() },
                        [Page.ListControl] = new LazyView { Factory = () => new ListControlPage() },
                        [Page.Input] = new LazyView { Factory = () => new InputFieldPage() },
                        [Page.Color] = new LazyView { Factory = () => new ColorPickerPage() },
                        [Page.Modals] = new LazyView { Factory = () => new ModalsPage() },
                        [Page.Images] = new LazyView { Factory = () => new ImagesPage() },
                        [Page.Labels] = new LazyView { Factory = () => new LabelsPage() }
                    }
                },

                new BsPrimaryImageButton {
                    FlexItem = {
                        PositionType = PositionType.Absolute,
                        Position = new() { top = 0.pt, right = 0.pt },
                        Size = 8.pt
                    },

                    Sprite = GameResources.HueWheelIcon,
                    PreserveAspect = true,
                    Skew = 0f,
                    sColors = ShowcaseTheme.Palette.Map(ShowcaseStyles.PrimaryButton),
                    OnClick = () => menuOpened.Value = true
                }.Bind(ref _wheel),

                new ThemeMenu {
                    sIsPushed = menuOpened,
                    OnClickOutside = () => menuOpened.Value = false,
                    OnCloseClicked = () => menuOpened.Value = false
                }.Bind(ref _menu)
            }
        }.Use();
    }

    protected override void OnInitialize() {
        _menu.PlacementAnchor = ContentTransform;
    }

    protected override void OnUpdate() {
        ShowcaseTheme.Tick();

        if (ShowcaseTheme.Rainbow.Value) {
            _wheel.ContentTransform.localEulerAngles -= new Vector3(0f, 0f, Time.deltaTime * ShowcaseTheme.RainbowSpeed.Value * 360f);
        }
    }
}

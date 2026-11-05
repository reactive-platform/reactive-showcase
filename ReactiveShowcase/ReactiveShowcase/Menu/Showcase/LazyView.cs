using System;
using Reactive;
using Reactive.Yoga;
using UnityEngine;

namespace ReactiveShowcase.Showcase;

/// <summary>
/// A placeholder that builds its view the first time it gets enabled, so views nobody opens are never constructed.
/// </summary>
internal class LazyView : ReactiveComponent {
    /// <summary>
    /// Builds the view. Called once, when the placeholder is enabled for the first time.
    /// </summary>
    public Func<IReactiveComponent>? Factory { get; set; }

    private Layout _layout = null!;
    private IReactiveComponent? _view;

    protected override GameObject Construct() {
        return new Layout {
            FlexItem = {
                Flex = 1f
            },
            
            FlexController = {
                AlignItems = Align.Stretch
            }
        }.Bind(ref _layout).Use();
    }

    protected override void OnEnable() {
        if (_view != null || Factory == null) {
            return;
        }

        _view = Factory();
        _view.FlexItem.Flex = 1f;
        _layout.Children.Add(_view);
    }
}

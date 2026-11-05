using System;
using HMUI;
using ReactiveShowcase.Examples;

namespace ReactiveShowcase;

public class HubViewController : ViewController {
    internal Action? OnShowcaseSelected { get; set; }
    internal Action<Example>? OnExampleSelected { get; set; }

    private void Awake() {
        new ExamplesHub {
            OnShowcaseSelected = () => OnShowcaseSelected?.Invoke(),
            OnExampleSelected = x => OnExampleSelected?.Invoke(x)
        }.Use(transform);
    }
}

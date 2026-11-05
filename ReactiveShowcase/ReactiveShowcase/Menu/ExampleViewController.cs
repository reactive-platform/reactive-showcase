using HMUI;
using ReactiveShowcase.Examples;

namespace ReactiveShowcase;

public class ExampleViewController : ViewController {
    private ExampleView? _view;

    // Built on first use: the controller is created inactive, so Awake only runs once it's presented,
    // which is after the example is handed over
    internal void Show(Example example) {
        if (_view == null) {
            _view = new ExampleView(example);
            _view.Use(transform);
        } else {
            _view.Show(example);
        }
    }
}

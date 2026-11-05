using HMUI;
using ReactiveShowcase.Showcase;

namespace ReactiveShowcase;

public class ReactiveViewController : ViewController {
    private void Awake() {
        new ShowcaseRoot().Use(transform);
    }
}

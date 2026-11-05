using HMUI;
using ReactiveShowcase.Examples;
using Zenject;

namespace ReactiveShowcase;

public class ReactiveFlowCoordinator : FlowCoordinator {
    [Inject] private readonly HubViewController _hubViewController = null!;
    [Inject] private readonly ReactiveViewController _showcaseViewController = null!;
    [Inject] private readonly ExampleViewController _exampleViewController = null!;

    private const string HubTitle = "Reactive";

    public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
        if (!firstActivation) return;

        _hubViewController.OnShowcaseSelected = () => Open(_showcaseViewController, "Component Showcase");
        _hubViewController.OnExampleSelected = HandleExampleSelected;

        ProvideInitialViewControllers(_hubViewController);
        SetTitle(HubTitle);
        showBackButton = true;
    }

    private void HandleExampleSelected(Example example) {
        _exampleViewController.Show(example);
        Open(_exampleViewController, example.Title);
    }

    private void Open(ViewController viewController, string title) {
        ReplaceTopViewController(viewController, animationType: ViewController.AnimationType.In);
        SetTitle(title);
    }

    public override void BackButtonWasPressed(ViewController topViewController) {
        // Anything opened from the hub goes back to it, the hub itself closes the playground
        if (topViewController != _hubViewController) {
            ReplaceTopViewController(_hubViewController, animationType: ViewController.AnimationType.Out);
            SetTitle(HubTitle);
            return;
        }

        _parentFlowCoordinator.DismissFlowCoordinator(this);
    }
}

using System;
using BeatSaberMarkupLanguage.MenuButtons;
using UnityEngine;
using Zenject;

namespace ReactiveShowcase;

internal class MenuButtonManager : MonoBehaviour {
    [Inject] private readonly ReactiveFlowCoordinator _reactiveFlowCoordinator = null!;
    [Inject] private readonly MainFlowCoordinator _mainFlowCoordinator = null!;
        
    #region MenuButton
        
    private static event Action? MenuButtonClickedEvent;
        
    private static readonly MenuButton menuButton = new(
        "Reactive", "Hello world ig", 
        () => MenuButtonClickedEvent?.Invoke());

    #endregion

    #region Init & Dispose

    private void OnEnable() {
        MenuButtons.Instance.RegisterButton(menuButton);
        MenuButtonClickedEvent += HandleMenuButtonClicked;
    }

    private void OnDisable() {
        MenuButtons.Instance.UnregisterButton(menuButton);
        MenuButtonClickedEvent -= HandleMenuButtonClicked;  
    }

    #endregion

    #region Callbacks

    private void HandleMenuButtonClicked() {
        _mainFlowCoordinator.PresentFlowCoordinator(_reactiveFlowCoordinator);
    }

    #endregion
}
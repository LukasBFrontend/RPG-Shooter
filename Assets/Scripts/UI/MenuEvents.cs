#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MenuEvents : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] PageTree pageStructure;
    [Header("Sound")]
    [SerializeField] AudioClip clip;
    [Range(0f, 100f)][SerializeField] float volume = 50f;
    UIDocument document;
    Dictionary<string, PageNode> pages;
    Dictionary<string, EventCallback<ClickEvent>> buttonActions;
    List<Button> menuButtons;
    static VisualElement pauseMenu;
    PageNode currentPage;

    void Awake()
    {
        Cache();

        buttonActions = new Dictionary<string, EventCallback<ClickEvent>>
        {
            ["StartButton"] = e => SceneManager.LoadScene("MainScene"),
            ["ResumeButton"] = e => ToggleLvlMenu(),
            ["OptionsButton"] = e => Navigate(pages!["OptionsPage"], e),
            ["LoadButton"] = e => Navigate(pages!["SavesPage"], e),
            ["BackButton"] = e => { if (currentPage?.Parent != null) Navigate(currentPage.Parent, e); }
        };

        RegisterCallbacks();
    }

    void Cache()
    {
        document = GetComponent<UIDocument>();
        pages = PageNode.GeneratePageLookup(document, pageStructure);
        currentPage = pages[pageStructure.Name];
        menuButtons = document.rootVisualElement.Query<Button>().ToList();

        pauseMenu = document.rootVisualElement.Query<VisualElement>(name: "PauseMenu");
    }

    void OnDisable()
    {
        UnregisterCallbacks();
    }

    void RegisterCallbacks()
    {
        document.rootVisualElement.RegisterCallback<KeyDownEvent>(OnEnterPressed);

        foreach (var button in menuButtons!)
        {
            if (buttonActions.TryGetValue(button.name, out var action))
                button.RegisterCallback(action);

            button.RegisterCallback<FocusEvent>(OnButtonFocused);
            button.RegisterCallback<ClickEvent>(OnButtonClicked);
        }
    }

    void UnregisterCallbacks()
    {
        foreach (var button in menuButtons!)
        {
            if (buttonActions.TryGetValue(button.name, out var action))
                button.UnregisterCallback(action);

            button.UnregisterCallback<FocusEvent>(OnButtonFocused);
            button.UnregisterCallback<ClickEvent>(OnButtonClicked);
        }
    }

    void OnEnterPressed(KeyDownEvent e)
    {
        if (e.keyCode != KeyCode.Return && e.keyCode != KeyCode.KeypadEnter)
        {
            return;
        }

        var focused = document.rootVisualElement.panel.focusController.focusedElement;

        if (focused is Button b)
        {
            OnButtonClicked(null);

            if (buttonActions.TryGetValue(b.name, out var action))
                action.Invoke(null);

            e.StopPropagation();
        }
    }

    public static void ToggleLvlMenu()
    {
        if (pauseMenu == null)
        {
            return;
        }

        if (pauseMenu.ClassListContains("hidden"))
        {
            pauseMenu.RemoveFromClassList("hidden");
            GameState.Pause();
        }
        else
        {
            pauseMenu.AddToClassList("hidden");
            GameState.Unpause();
        }
    }

    void OnButtonFocused(FocusEvent e)
    {
        SoundFXManager.Instance.PlaySoundFXClip(clip, transform, volume / 4);
    }

    void OnButtonClicked(ClickEvent? e)
    {
        SoundFXManager.Instance.PlaySoundFXClip(clip, transform, volume);
    }

    void Navigate(PageNode target, ClickEvent? e)
    {
        currentPage?.VisualElement?.AddToClassList("hidden");
        target.VisualElement?.RemoveFromClassList("hidden");
        currentPage = target;
    }
}

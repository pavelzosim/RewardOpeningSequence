using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Collections.Generic;

public class Unboxing : MonoBehaviour
{
    // Public fields for settings
    public Texture2D spriteSheet; // Sprite sheet containing all frames for animation
    public int frameWidth = 256; // Width of a single frame
    public int frameHeight = 256; // Height of a single frame
    public int startFrame = 0; // First frame index for the animation
    public int endFrame = 15; // Last frame index for the animation
    public float frameRate = 0.1f; // Time interval between frames
    public bool loopAnimation = true; // Whether to loop the animation

    // GameObjects and UI Elements
    public GameObject PF_PresentBox; // Reference to the present box GameObject
    public GameObject OpenBox_EventObjects; // Event objects shown when box opens
    public GameObject Prizes; // Container for prize items
    private Animator animator; // Animator to control box animations
    private Button buttonOpenBox; // UI Button to open the box
    private Button buttonReset; // UI Button to reset the box
    private VisualElement prizeLabel; // UI Label to display prize information

    // Internal variables
    private int currentFrame; // Current frame index for the animation
    private float frameTimer; // Timer to manage frame switching
    private List<Texture2D> frames = new(); // List to store extracted frames
    private bool isOpening = false; // State to check if the box is opening
    private List<GameObject> prizeItems = new(); // List of prize GameObjects

    private void Start()
    {
        // UI initialization and element bindings
        var root = GetComponent<UIDocument>().rootVisualElement;
        buttonOpenBox = root.Q<Button>("ButtonOpenBox");
        buttonReset = root.Q<Button>("ButtonReset");
        prizeLabel = root.Q<VisualElement>("PrizeLabel");

        // Initial UI setup
        prizeLabel.style.display = DisplayStyle.None; // Hide prize label initially
        prizeLabel.style.opacity = 0f; // Make label fully transparent
        OpenBox_EventObjects.SetActive(false); // Hide event objects at start

        // Generate frames from sprite sheet
        CreateSpriteSheetFrames();
        if (frames.Count > 0)
            buttonOpenBox.style.backgroundImage = new StyleBackground(frames[startFrame]);

        // Attach button click events
        buttonOpenBox.clicked += OnOpenBoxClicked;
        buttonReset.clicked += OnResetClicked;

        // Animator initialization
        animator = PF_PresentBox.GetComponent<Animator>();
        InitializePrizeItems();
    }

    private void Update()
    {
        // Update the sprite animation if the box is opening
        if (isOpening)
            UpdateSpriteAnimation();
    }

    private void OnOpenBoxClicked()
    {
        if (isOpening) return; // Prevent multiple clicks

        isOpening = true;
        currentFrame = startFrame; // Reset to the first frame
        frameTimer = 0f; // Reset the timer

        FadeOutButton(); // Start fading out the button
        ExecuteTrigger("TrOpen"); // Trigger the box opening animation
        ShowOpenBox_EventObjects(); // Display objects for the open box event
        ShowRandomPrize(); // Randomly display a prize
    }

    private void OnResetClicked()
    {
        isOpening = false;
        currentFrame = startFrame; // Reset to the first frame
        frameTimer = 0f; // Reset the timer

        prizeLabel.style.display = DisplayStyle.None; // Hide prize label
        prizeLabel.style.opacity = 0f; // Reset label opacity

        if (frames.Count > 0)
            buttonOpenBox.style.backgroundImage = new StyleBackground(frames[startFrame]);

        buttonOpenBox.style.display = DisplayStyle.Flex; // Show the button
        buttonOpenBox.style.opacity = 1f; // Reset button opacity
        DOTween.To(() => buttonOpenBox.style.opacity.value, x => buttonOpenBox.style.opacity = x, 1f, 1f);

        ExecuteTrigger("TrReset"); // Trigger the box reset animation
        RemoveOpenBox_EventObjects(); // Hide the open box event objects
        HideAllPrizes(); // Hide all prizes
    }

    private void InitializePrizeItems()
    {
        // Collect all prize items into a list
        prizeItems.Clear();

        foreach (Transform child in Prizes.transform)
        {
            if (child.gameObject.name.StartsWith("Item"))
            {
                prizeItems.Add(child.gameObject);
                child.gameObject.SetActive(false); // Hide prizes initially
            }
        }

        Debug.Log($"Initialized {prizeItems.Count} prize items.");
    }

    private void ShowRandomPrize()
    {
        // Display a random prize
        if (prizeItems.Count == 0) return;

        int randomIndex = Random.Range(0, prizeItems.Count);
        prizeItems[randomIndex].SetActive(true);
    }

    private void HideAllPrizes()
    {
        // Hide all prizes
        foreach (GameObject prize in prizeItems)
        {
            prize.SetActive(false);
        }
    }

    private void RemoveOpenBox_EventObjects()
    {
        // Hide objects shown during the box opening event
        OpenBox_EventObjects.SetActive(false);
    }

    private void ExecuteTrigger(string trigger)
    {
        // Set a trigger for the animator
        animator?.SetTrigger(trigger);
    }

    private void UpdateSpriteAnimation()
    {
        // Update the animation frame based on time
        if (frames.Count == 0 || buttonOpenBox == null) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer -= frameRate;
            currentFrame++;

            // Loop or stop the animation at the last frame
            if (currentFrame > endFrame)
            {
                currentFrame = loopAnimation ? startFrame : endFrame;
                isOpening = loopAnimation;
            }

            buttonOpenBox.style.backgroundImage = new StyleBackground(frames[currentFrame]);
        }
    }

    private void CreateSpriteSheetFrames()
    {
        // Generate frames from the sprite sheet
        if (spriteSheet == null) return;

        frames.Clear();
        int columns = spriteSheet.width / frameWidth;
        int rows = spriteSheet.height / frameHeight;

        for (int i = startFrame; i <= endFrame; i++)
        {
            int row = i / columns;
            int col = i % columns;

            Rect frameRect = new Rect(col * frameWidth, (rows - row - 1) * frameHeight, frameWidth, frameHeight);
            Texture2D frameTexture = new Texture2D(frameWidth, frameHeight);

            frameTexture.SetPixels(spriteSheet.GetPixels((int)frameRect.x, (int)frameRect.y, frameWidth, frameHeight));
            frameTexture.Apply();

            frames.Add(frameTexture);
        }

        Debug.Log($"Created {frames.Count} frames.");
    }

    private void FadeOutButton()
    {
        // Fade out the open button
        DOTween.To(() => buttonOpenBox.style.opacity.value, x => buttonOpenBox.style.opacity = x, 0f, 1f)
            .SetDelay(0.5f)
            .OnComplete(() =>
            {
                buttonOpenBox.style.display = DisplayStyle.None; // Hide the button
                ShowPrizeLabel(); // Show the prize label
            });
    }

    private void ShowPrizeLabel()
    {
        // Display and fade in the prize label
        prizeLabel.style.display = DisplayStyle.Flex;
        DOTween.To(() => prizeLabel.style.opacity.value, x => prizeLabel.style.opacity = x, 1f, 1f);
    }

    private void ShowOpenBox_EventObjects()
    {
        // Display objects for the box opening event
        OpenBox_EventObjects.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class Unboxing : MonoBehaviour
{
    // Public fields to define the sprite sheet, animation frames, and settings
    public Texture2D spriteSheet; // The sprite sheet containing animation frames
    public int frameWidth = 256;  // Width of a single frame in the sprite sheet
    public int frameHeight = 256; // Height of a single frame in the sprite sheet
    public int startFrame = 0;    // The starting frame index
    public int endFrame = 15;     // The ending frame index
    public float frameRate = 0.1f; // Time between each frame update (frame rate)
    public bool loopAnimation = true; // Whether the animation should loop

    // References for UI and animation components
    public GameObject PF_PresentBox; // The object for the present box (used to trigger animation)
    public GameObject OpenBox_EventObjects; // The object for the cone inside the box (should be initially hidden)
    private Animator animator; // The animator component attached to the present box
    private Button buttonOpenBox; // Button to trigger opening the box
    private VisualElement prizeLabel; // UI element showing the prize after the box is opened

    // Internal variables to control animation
    private int currentFrame; // The current frame in the animation
    private float frameTimer; // Timer to track frame rate
    private List<Texture2D> frames; // List to store the individual frames
    private bool isOpening = false; // Flag to track if the box is being opened

    private void Start()
    {
        // Get references to UI elements from the UIDocument
        var root = GetComponent<UIDocument>().rootVisualElement;
        buttonOpenBox = root.Q<Button>("ButtonOpenBox"); // Get the "Open Box" button
        var buttonReset = root.Q<Button>("ButtonReset"); // Get the "Reset" button
        prizeLabel = root.Q<VisualElement>("PrizeLabel"); // Get the prize label UI element

        // Initially hide the prize label and set its opacity to 0
        prizeLabel.style.display = DisplayStyle.None;
        prizeLabel.style.opacity = 0f;

        // Initially hide the cone object
        OpenBox_EventObjects.SetActive(false);

        // Create sprite frames from the sprite sheet for the animation
        CreateSpriteSheetFrames();

        // Set the initial background image of the "Open Box" button to the first frame
        if (frames.Count > 0)
        {
            buttonOpenBox.style.backgroundImage = new StyleBackground(frames[startFrame]);
        }

        // Attach event listeners for button clicks
        buttonOpenBox.clicked += OnOpenBoxClicked;
        buttonReset.clicked += OnResetClicked;

        // Get the animator component from the present box
        animator = PF_PresentBox.GetComponent<Animator>();
    }

    private void Update()
    {
        // This update is now replaced by the coroutine, no need to handle the animation here anymore
    }

    // Called when the "Open Box" button is clicked
    private void OnOpenBoxClicked()
    {
        // Only start the animation if the box isn't already opening
        if (!isOpening)
        {
            StartCoroutine(StartAnimation());
        }
    }

    // Coroutine for the animation
    private IEnumerator StartAnimation()
    {
        currentFrame = startFrame;
        frameTimer = 0f;
        isOpening = true;

        // Fade out the "Open Box" button
        FadeOutButton();
        ExecuteTrigger("TrOpen");

        // Show the cone after the animation starts
        ShowOpenBox_EventObjects();

        // Continue updating frames while the box is opening
        while (isOpening)
        {
            frameTimer += Time.deltaTime; // Update frame timer

            if (frameTimer >= frameRate)
            {
                frameTimer -= frameRate; // Reset frame timer

                currentFrame++; // Go to the next frame
                if (currentFrame > endFrame)
                {
                    currentFrame = loopAnimation ? startFrame : endFrame;
                    isOpening = !loopAnimation; // Stop opening if not looping
                }

                // Update button background only if the frame has changed
                var currentFrameTexture = frames[currentFrame];
                if (buttonOpenBox.style.backgroundImage != currentFrameTexture)
                {
                    buttonOpenBox.style.backgroundImage = new StyleBackground(currentFrameTexture);
                }
            }

            yield return null; // Wait until the next frame
        }
    }

    // Called when the "Reset" button is clicked
    private void OnResetClicked()
    {
        // Reset animation and UI elements to initial state
        isOpening = false;
        currentFrame = startFrame;
        frameTimer = 0f;

        // Hide and reset the prize label
        prizeLabel.style.display = DisplayStyle.None;
        prizeLabel.style.opacity = 0f;

        // Reset the button background image to the first frame
        if (frames.Count > 0)
        {
            buttonOpenBox.style.backgroundImage = new StyleBackground(frames[startFrame]);
        }

        // Reset the button visibility with a fade-in effect
        buttonOpenBox.style.display = DisplayStyle.Flex;
        buttonOpenBox.style.opacity = 1f;
        DOTween.To(() => buttonOpenBox.style.opacity.value, x => buttonOpenBox.style.opacity = x, 1f, 1f);

        // Trigger the "Reset" animation
        ExecuteTrigger("TrReset");

        // Remove the cone (PF_Cone) when the box is reset
        RemoveOpenBox_EventObjects();
    }

    // Method to remove the PF_Cone (deactivate or destroy it)
    private void RemoveOpenBox_EventObjects()
    {
        // Deactivate the cone object (hides it without destroying it)
        OpenBox_EventObjects.SetActive(false);
    }

    // Executes an animation trigger for the present box
    private void ExecuteTrigger(string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger); // Trigger the animation in the animator
        }
        else
        {
            Debug.LogWarning($"Animator is null. Trigger: {trigger} not executed.");
        }
    }

    // Creates sprite frames from the sprite sheet
    private void CreateSpriteSheetFrames()
    {
        if (spriteSheet == null) return; // Exit if sprite sheet is null

        frames = new List<Texture2D>(); // Initialize the list to hold frames
        int columns = spriteSheet.width / frameWidth; // Calculate the number of columns
        int rows = spriteSheet.height / frameHeight; // Calculate the number of rows

        // Loop through each frame in the sprite sheet
        for (int i = startFrame; i <= endFrame; i++)
        {
            int row = i / columns; // Calculate the row index of the frame
            int col = i % columns; // Calculate the column index of the frame

            // Define the rectangle for the frame in the sprite sheet
            Rect frameRect = new Rect(col * frameWidth, (rows - row - 1) * frameHeight, frameWidth, frameHeight);

            // Create a new texture for the frame from the sprite sheet
            Texture2D frameTexture = new Texture2D(frameWidth, frameHeight);

            // Extract pixels using GetPixels with a Rect parameter
            frameTexture.SetPixels(spriteSheet.GetPixels((int)frameRect.x, (int)frameRect.y, frameWidth, frameHeight));
            frameTexture.Apply(); // Apply changes to the texture

            frames.Add(frameTexture); // Add the frame to the list of frames
        }

        Debug.Log($"Created {frames.Count} frames."); // Log the number of frames created
    }

    // Fades out the "Open Box" button with a delay
    private void FadeOutButton()
    {
        DOTween.To(() => buttonOpenBox.style.opacity.value, x => buttonOpenBox.style.opacity = x, 0f, 1f) // Fade out over 1 second
            .SetDelay(0.5f) // Apply a short delay before fading
            .OnComplete(() =>
            {
                buttonOpenBox.style.display = DisplayStyle.None; // Hide the button after fade out
                ShowPrizeLabel(); // Show the prize label after the button fades out
            });
    }

    // Displays the prize label after the box is opened
    private void ShowPrizeLabel()
    {
        prizeLabel.style.display = DisplayStyle.Flex; // Show the prize label
        DOTween.To(() => prizeLabel.style.opacity.value, x => prizeLabel.style.opacity = x, 1f, 1f); // Fade in the prize label
    }

    // Shows the PF_Cone object (cone) when the box is opened (triggered via animation event)
    private void ShowOpenBox_EventObjects()
    {
        // We trigger this method via the animation event after the box is opened
        OpenBox_EventObjects.SetActive(true); // Activate the cone
    }
}

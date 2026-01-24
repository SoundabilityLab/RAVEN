using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GoogleTextToSpeech.Scripts;

public class menu : MonoBehaviour
{
    public GameObject dimensionItemContainer;
    public GameObject categoryItemContainer;

    public GameObject descriptionItemContainer;
    public GameObject promptItemContainer;
    public GameObject questionItemContainer;

    public GameObject dimensionTitleContainer;
    public GameObject categoryTitleContainer;
    public GameObject descriptionTitleContainer;
    public GameObject promptTitleContainer;
    public GameObject questionTitleContainer;

    private TextToSpeech textToSpeech;

    


    // The currently visible container (Dimension or Category).
    // Once we hit third-level, we treat them differently with 'inThirdLevel' flag.
    private GameObject currentContainer;

    // Where we store the current container's text items
    private List<Text> currentMenuItems = new List<Text>();
    private int selectedIndex = 0;

    // Keep track of indices so we can restore them after going back
    private Dictionary<GameObject, int> menuSelections = new Dictionary<GameObject, int>();

    // Map each container to its title container (for showing/hiding)
    private Dictionary<GameObject, GameObject> menuTitleContainers;

    // Flag to indicate we are in the combined third column
    private bool inThirdLevel = false;

    // store current column level
    private int currentLevel = 0;

    // The entire combined list of text items from Description, Prompt, and Question
    private List<ThirdLevelMenuItem> thirdLevelItems = new List<ThirdLevelMenuItem>();
    private int thirdLevelIndex = 0; // which item in the third-level list is selected

    // Keep track of which dimension is selected
    private string selectedDimension = "";

    // -----------------------------------------
    // Data structure holding everything
    // -----------------------------------------
    private Dictionary<string, List<CategoryData>> dimensionData = new Dictionary<string, List<CategoryData>>()
    {
        {
            "Visual", new List<CategoryData>
            {
                new CategoryData
                {
                    categoryName = "Change Color",
                    description = new List<string>
                    {
                        "Change the color or color scheme of an item (for color blindness or increased color contrast)"
                    },
                    prompts = new List<string>
                    {
                        "Make the big vase bright yellow",
                        "Change the color of the chairs to make them more accessible for someone with red-green color blindness",
                        "Change the color of the wooden key to increase color contrast from its surroundings"
                    },
                    questions = new List<string>
                    {
                        "What is the color of object A?",
                        "Are Object A and Object B close in color?",
                        "Is Object A close in color to its surroundings?",
                        "Does a group of objects look similar to someone with a certain color deficiency?"
                    }
                },
                new CategoryData
                {
                    categoryName = "Change Object Location",
                    description = new List<string>
                    {
                        "Move an object close to the user, or move the user close to an object"
                    },
                    prompts = new List<string>
                    {
                        "Move me next to the briefcase",
                        "Move the Big Vase in front of me",
                        "Move the big vase next to the briefcase"
                    },
                    questions = new List<string>
                    {
                        "Explore the location and spatial relationship of objects using the selection screen."
                    }
                },
                new CategoryData
                {
                    categoryName = "Change Sizes",
                    description = new List<string>
                    {
                        "Make the size of objects or the font of texts bigger or smaller"
                    },
                    prompts = new List<string>
                    {
                        "Make the text (much) bigger",
                        "Make the small vase (much) bigger",
                        "Make the small vase bigger than the big vase"
                    },
                    questions = new List<string>
                    {
                        "How big is Object A?",
                        "What is Object A’s size compared to Object B?",
                        "How big are the texts?"
                    }
                },
                new CategoryData
                {
                    categoryName = "Change Scene Brightness",
                    description = new List<string>
                    {
                        "Make the scene or certain light sources brighter or dimmer"
                    },
                    prompts = new List<string>
                    {
                        "Make the scene brighter",
                        "Make the ceiling lights brighter",
                        "Create a light next to the laptop"
                    },
                    questions = new List<string>
                    {
                        "How many light sources are there in the scene?",
                        "Is Light A brighter or dimmer than Light B?"
                    }
                }
            }
        },
        {
            "Audio", new List<CategoryData>
            {
                new CategoryData
                {
                    categoryName = "Change Volume",
                    description = new List<string>
                    {
                        "Increase or decrease the volume of a certain sound source/ a certain group of sound sources"
                    },
                    prompts = new List<string>
                    {
                        "Increase (to maximum) the volume of Ari’s voice",
                        "Mute Everyone except Ari"
                    },
                    questions = new List<string>
                    {
                        "Who is currently the loudest?",
                        "Is Sound Source A muted right now?",
                        "How loud is Sound Source A?"
                    }
                },
                new CategoryData
                {
                    categoryName = "Change Pitch",
                    description = new List<string>
                    {
                        "Increase or decrease the pitch of a certain sound source/ a certain group of sound sources"
                    },
                    prompts = new List<string>
                    {
                        "Lower the pitch of Alice’s voice",
                        "Lower the pitch of all women"
                    },
                    questions = new List<string>
                    {
                        "Has Alice’s voice pitch been shifted down?",
                        "Can you further lower Alice’s pitch?"
                    }
                },
                // new CategoryData
                // {
                //     categoryName = "Change Range",
                //     description = new List<string>
                //     {
                //         "Increase or decrease the spatial range of a certain sound source/ a certain group of sound sources"
                //     },
                //     prompts = new List<string>
                //     {
                //         "(Greatly) increase the range of Adam’s voice",
                //         "Decrease everyone's voice range in the scene"
                //     },
                //     questions = new List<string>
                //     {
                //         "Am I within the range of Sound Source A?",
                //         "How far is the audio range of Sound Source A?"
                //     }
                // },
                // new CategoryData
                // {
                //     categoryName = "Transcript Understanding",
                //     description = new List<string>
                //     {
                //         "The tool has access to all transcripts of characters, which you can ask questions about and prompt changes based on."
                //     },
                //     prompts = new List<string>
                //     {
                //         "Increase the volume of people talking about phones",
                //         "Move me next to the people talking about the weather",
                //         "Highlight the person talking about football"
                //     },
                //     questions = new List<string>
                //     {
                //         "Which pair of people are talking about phones?",
                //         "What are Jade and Emily talking about?",
                //         "What is the person closest to me talking about?"
                //     }
                // }
            }
        }
    };

    void Start()
    {
        textToSpeech = FindObjectOfType<TextToSpeech>();
        // Initialize dictionary that maps each container to its title
        menuTitleContainers = new Dictionary<GameObject, GameObject>
        {
            { dimensionItemContainer, dimensionTitleContainer },
            { categoryItemContainer, categoryTitleContainer },
            { descriptionItemContainer, descriptionTitleContainer },
            { promptItemContainer, promptTitleContainer },
            { questionItemContainer, questionTitleContainer }
        };

        // Start at dimension container
        currentContainer = dimensionItemContainer;

        // Set up layout groups
        InitializeLayoutGroup(dimensionItemContainer, 10, 10);
        InitializeLayoutGroup(categoryItemContainer, 10, 10);
        // Smaller spacing/padding for the third-level containers
        InitializeLayoutGroup(descriptionItemContainer, 4, 4);
        InitializeLayoutGroup(promptItemContainer, 4, 4);
        InitializeLayoutGroup(questionItemContainer, 4, 4);

        // Initialize selection dictionaries
        menuSelections[dimensionItemContainer] = 0;
        menuSelections[categoryItemContainer] = 0;
        menuSelections[descriptionItemContainer] = 0;
        menuSelections[promptItemContainer] = 0;
        menuSelections[questionItemContainer] = 0;

        // Fill dimension container with "Visual" / "Audio"
        PopulateMenu(dimensionItemContainer, new List<string> { "Visual", "Audio" }, 36);
        ShowMenu(dimensionItemContainer);

        // Hide deeper menus
        HideMenu(categoryItemContainer);
        HideMenu(descriptionItemContainer);
        HideMenu(promptItemContainer);
        HideMenu(questionItemContainer);

        UpdateMenuItems();
        HighlightSelectedItem();
    }

    void Update()
    {
        // W = up, S = down in the current container or third-level combined list
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveSelection(1);
        }
        // D = select if not in third-level
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentLevel += 1;
            if (!inThirdLevel)
            {
                SelectItem($"You are now in level {currentLevel+1} submenu.");
            }
            else
            {
                textToSpeech.PlayTtsAudio($"You are now in level {currentLevel+1} submenu. This is the last level.");
            }

        }
        // A = go back
        else if (Input.GetKeyDown(KeyCode.A))
        {
            currentLevel -= 1;
            GoBack($"You are now in level {currentLevel+1} submenu.");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SpeakCurrentItem();
        }
    }

    void MoveSelection(int direction)
    {
        if (inThirdLevel)
        {
            // We are navigating the combined list of third-level items
            if (thirdLevelItems.Count == 0) return;

            // Un-highlight current
            thirdLevelItems[thirdLevelIndex].textComponent.color = Color.white;

            // Move index
            thirdLevelIndex = (thirdLevelIndex + direction + thirdLevelItems.Count) % thirdLevelItems.Count;

            // Highlight new
            thirdLevelItems[thirdLevelIndex].textComponent.color = Color.yellow;

            // Speak new
            SpeakCurrentItem();
        }
        else
        {
            // First or second column (Dimension / Category)
            if (currentMenuItems.Count == 0) return;

            currentMenuItems[selectedIndex].color = Color.white;
            selectedIndex = (selectedIndex + direction + currentMenuItems.Count) % currentMenuItems.Count;
            menuSelections[currentContainer] = selectedIndex;

            HighlightSelectedItem();
        }
    }

    void HighlightSelectedItem(string additional_tts_text = "")
    {
        if (!inThirdLevel)
        {
            if (currentMenuItems.Count > 0)
            {
                currentMenuItems[selectedIndex].color = Color.yellow;
            }
        }
        else
        {
            // If we're in third-level, we highlight via thirdLevelItems
            if (thirdLevelItems.Count > 0)
            {
                thirdLevelItems[thirdLevelIndex].textComponent.color = Color.yellow;
            }
        }
        // Speak new
        SpeakCurrentItem(additional_tts_text);
    }

    void SelectItem(string additional_tts_text = "")
    {
        // Selecting while in dimension or category
        if (currentMenuItems.Count == 0) return;

        Text item = currentMenuItems[selectedIndex];
        string selectedText = item.text;

        // If we are in the dimension container
        if (currentContainer == dimensionItemContainer)
        {
            if (dimensionData.ContainsKey(selectedText))
            {
                selectedDimension = selectedText;
                currentContainer = categoryItemContainer;

                // Fill with categories
                var catList = dimensionData[selectedDimension];
                List<string> catNames = new List<string>();
                foreach (var c in catList)
                {
                    catNames.Add(c.categoryName);
                }

                PopulateMenu(categoryItemContainer, catNames, 36);
                ShowMenu(categoryItemContainer);

                // Move index/cursor
                selectedIndex = menuSelections[categoryItemContainer];
                UpdateMenuItems();
                HighlightSelectedItem(additional_tts_text);
            }
        }
        else if (currentContainer == categoryItemContainer)
        {
            // We picked a category. Let's load the third column data
            var catList = dimensionData[selectedDimension];
            CategoryData chosenCategory = new CategoryData();

            foreach (var c in catList)
            {
                if (c.categoryName == selectedText)
                {
                    chosenCategory = c;
                    break;
                }
            }

            // Populate the three containers
            PopulateMenu(descriptionItemContainer, chosenCategory.description, 24);
            PopulateMenu(promptItemContainer, chosenCategory.prompts, 24);
            PopulateMenu(questionItemContainer, chosenCategory.questions, 24);

            ShowMenu(descriptionItemContainer);
            ShowMenu(promptItemContainer);
            ShowMenu(questionItemContainer);

            // Now combine all text items into thirdLevelItems
            CombineThirdLevelItems();

            // Enter the third-level mode
            inThirdLevel = true;

            // Start highlight
            thirdLevelIndex = 0;
            if (thirdLevelItems.Count > 0)
            {
                thirdLevelItems[thirdLevelIndex].textComponent.color = Color.yellow;
                SpeakCurrentItem(additional_tts_text);
            }
        }
    }

    /// <summary>
    /// Combine the text items from Description, Prompt, and Question containers into one list for linear navigation.
    /// </summary>
    void CombineThirdLevelItems()
    {
        thirdLevelItems.Clear();

        // Description first
        foreach (Transform child in descriptionItemContainer.transform)
        {
            Text txt = child.GetComponent<Text>();
            if (txt != null)
            {
                ThirdLevelMenuItem item = new ThirdLevelMenuItem
                {
                    textComponent = txt,
                    sectionType = "description"  // We'll use this to speak "Type: <text>"
                };
                thirdLevelItems.Add(item);
            }
        }
        // Then Prompt
        foreach (Transform child in promptItemContainer.transform)
        {
            Text txt = child.GetComponent<Text>();
            if (txt != null)
            {
                ThirdLevelMenuItem item = new ThirdLevelMenuItem
                {
                    textComponent = txt,
                    sectionType = "prompt" // We'll say "Example change prompt: <text>"
                };
                thirdLevelItems.Add(item);
            }
        }
        // Then Question
        foreach (Transform child in questionItemContainer.transform)
        {
            Text txt = child.GetComponent<Text>();
            if (txt != null)
            {
                ThirdLevelMenuItem item = new ThirdLevelMenuItem
                {
                    textComponent = txt,
                    sectionType = "question" // We'll say "Example related question: <text>"
                };
                thirdLevelItems.Add(item);
            }
        }

        // De-highlight them all
        foreach (var item in thirdLevelItems)
        {
            item.textComponent.color = Color.white;
        }
    }


    void GoBack(string additional_tts_text = "")
    {
        if (inThirdLevel)
        {
            // Go back to category container
            inThirdLevel = false;
            currentContainer = categoryItemContainer;

            // Clear third-level
            HideMenu(descriptionItemContainer);
            HideMenu(promptItemContainer);
            HideMenu(questionItemContainer);

            thirdLevelItems.Clear();

            // Restore category container selection
            selectedIndex = menuSelections[categoryItemContainer];
            UpdateMenuItems();
            HighlightSelectedItem(additional_tts_text);
        }
        else if (currentContainer == categoryItemContainer)
        {
            // Go back to dimension container
            currentContainer = dimensionItemContainer;
            HideMenu(categoryItemContainer);

            selectedIndex = menuSelections[dimensionItemContainer];
            UpdateMenuItems();
            HighlightSelectedItem(additional_tts_text);
        }
    }

    void UpdateMenuItems()
    {
        // Only do this if not in third-level
        if (inThirdLevel) return;

        currentMenuItems.Clear();

        foreach (Transform child in currentContainer.transform)
        {
            Text txt = child.GetComponent<Text>();
            if (txt != null)
            {
                currentMenuItems.Add(txt);
            }
        }

        selectedIndex = menuSelections[currentContainer];
    }

    void ShowMenu(GameObject container)
    {
        container.SetActive(true);
        if (menuTitleContainers.ContainsKey(container))
        {
            menuTitleContainers[container].SetActive(true);
        }
    }

    void HideMenu(GameObject container)
    {
        container.SetActive(false);
        if (menuTitleContainers.ContainsKey(container))
        {
            menuTitleContainers[container].SetActive(false);
        }
        ClearContainer(container);
    }

    void ClearContainer(GameObject container)
    {
        for (int i = container.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Creates a VerticalLayoutGroup with given spacing/padding.
    /// </summary>
    void InitializeLayoutGroup(GameObject container, int spacing, int padding)
    {
        VerticalLayoutGroup vlg = container.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
        {
            vlg = container.AddComponent<VerticalLayoutGroup>();
        }
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        // Force children to expand to fill all available space
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;
        vlg.spacing = spacing;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.padding = new RectOffset(padding, padding, padding, padding);
    }

    /// <summary>
    /// Populates a container with a list of strings, each as a separate Text UI.
    /// </summary>
    void PopulateMenu(GameObject container, List<string> lines, int fontSize)
    {
        ClearContainer(container);
        foreach (string line in lines)
        {
            GameObject newItem = new GameObject("Item");
            newItem.transform.SetParent(container.transform);

            Text txt = newItem.AddComponent<Text>();
            txt.text = line;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.fontSize = fontSize;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
        }
    }

    void SpeakCurrentItem(string additional_tts_text = "")
    {
        if (textToSpeech == null) return;

        if (!inThirdLevel)
        {
            // Dimensions or Categories
            // string prefix = (currentContainer == dimensionItemContainer) ? "Level 1 Submenu" : "Level 2 Submenu"; //renamed dimension to type for user study
            string text = currentMenuItems[selectedIndex].text;
            textToSpeech.PlayTtsAudio($"{additional_tts_text}................ {text}. Press D for more information about {text}.");
        }
        else
        {
            // third level
            // was previously:
            //   string text = thirdLevelItems[thirdLevelIndex].text;
            //   ttsManager.SynthesizeAndPlay(text);

            // Now we have a struct with textComponent + sectionType
            var item = thirdLevelItems[thirdLevelIndex];
            string rawText = item.textComponent.text; // the actual text
            string prefix = "";

            switch (item.sectionType)
            {
                case "description":
                    prefix = "Description";
                    break;
                case "prompt":
                    prefix = "Example change prompt";
                    break;
                case "question":
                    prefix = "Example change question";
                    break;
            }

            textToSpeech.PlayTtsAudio($"{additional_tts_text}................ {prefix}: {rawText}");
        }
    }

}

// A simple struct to hold all lines for each Category
[System.Serializable]
public struct CategoryData
{
    public string categoryName;
    public List<string> description;
    public List<string> prompts;
    public List<string> questions;
}


[System.Serializable]
public struct ThirdLevelMenuItem
{
    public Text textComponent;
    public string sectionType; // "description", "prompt", or "question"
}

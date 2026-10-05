using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//
// The Imgui Input Box for Debug...THERE IS NO IMMEDIATE MODE GUI INPUT BOX ?
// SelectedIndex is set when an item is selected. -1 means no item is selected.
//
public class ImguiInputBoxEventArgs
{
    public int selectedIndex;

    public string selectedItem; 
}; 

public class ImguiInputBox
{
    private List<string> _items = new();
    public List<string> items
    {
        get
        {
            return _items;
        }
        set
        {
            _items = value;

            if (_items == null)
            {
                selectedIndex = -1; // nothing selecetd
                return;
            }

            selectedIndex = items.Count;
        }
    }

    public int selectedIndex { get; private set; }


    /// <summary>
    /// the label the *overall* input box will use
    /// </summary>
    public string labelName = "Input Box";

    /// <summary>
    /// the label the buttonw will use *while open"
    /// </summary>
    public string labelNameButtonOpen = "Hide";

    /// <summary>
    /// the label the buttons will use *while closed*
    /// </summary>
    public string labelNameButtonClosed = "Show";

    public Vector2 position; /// position dropdown
    public Vector2 size; /// size of the dropdown
    /// <summary>
    /// bozo error suppressor for invalid size
    /// </summary>
    private bool bozoErrorSuppressorInvalidSize = false; 
    private bool dropdownOpen = false; 

    private Vector2 scrollPosition;

    //
    // EVENTS
    //
    public delegate void OnItemEventHandler(object sender, ImguiInputBoxEventArgs e);

    public event OnItemEventHandler onItemClicked;

    /// <summary>
    /// this is the main function that runs. put it in your OnGUI function
    /// </summary>
    public void Update()
    {
        if (size.x <= 0
        || size.y <= 0)
        {
            if (!bozoErrorSuppressorInvalidSize)
            {
                Debug.LogError("ImguiInputBox: size must be a positive float!");
                bozoErrorSuppressorInvalidSize = true; 
            }

            return; 
        }

        ///GUI.Box(new Rect(position.x, position.y, size.x, size.y), labelNameOverall);
        
        string buttonLabelToUse = (dropdownOpen) ? labelNameButtonOpen : labelNameButtonClosed;

        if (buttonLabelToUse == null)
            buttonLabelToUse = "buttonLabelToUse is null for some reason";

        if (GUI.Button(new(position.x, position.y, size.x, 25), buttonLabelToUse))
            dropdownOpen = !dropdownOpen;

        if (dropdownOpen)
        {
            scrollPosition = GUI.BeginScrollView(new Rect(position.x, position.y + 30, size.x, size.y), scrollPosition, new Rect(0, 0, size.x, size.y));
            selectedIndex = GUILayout.SelectionGrid(-1, items.ToArray(), 1);

            onItemClicked(this, new ImguiInputBoxEventArgs
            {
                selectedIndex = this.selectedIndex,
                selectedItem = (selectedIndex >= 0) ? items[selectedIndex] : null
            });

            GUI.EndScrollView();
        }

    }

    // Getters for privates
    // Setters for privates

}
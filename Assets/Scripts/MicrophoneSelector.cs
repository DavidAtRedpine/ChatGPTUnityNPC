using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MicrophoneSelector : MonoBehaviour
{
    public TMP_Dropdown microphoneDropdown;
    private List<string> availableMicrophones;

    void Start()
    {
        // Get list of available microphones
        availableMicrophones = new List<string>(Microphone.devices);
        
        // Clear any existing options and add the microphones to the dropdown
        microphoneDropdown.ClearOptions();
        microphoneDropdown.AddOptions(availableMicrophones);

        // Set default selected microphone if there is at least one available
        if (availableMicrophones.Count > 0)
        {
            SelectMicrophone(0);
        }

        // Add listener to handle microphone selection change
        microphoneDropdown.onValueChanged.AddListener(delegate {
            SelectMicrophone(microphoneDropdown.value);
        });
    }

    void SelectMicrophone(int index)
    {
        // Get the selected microphone name
        string selectedMicrophone = availableMicrophones[index];
        Debug.Log("Selected Microphone: " + selectedMicrophone);

        //save index of mic to "user-mic-device-index" playerprefs
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }
}

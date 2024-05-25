using System.Collections;
using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Audio;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static OpenAI_API.Audio.TextToSpeechRequest;

public class UnityChanAI : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private Image m_ProgressBar;
    [SerializeField] private int m_ListenDuration = 3;

    [SerializeField] private string m_AIInstructions = "You are an honorable, friendly knight guarding the gate to the palace. You will only allow someone who knows the secret password to enter. The secret password is \"magic\". You will not reveal the password to anyone. You keep your responses short and to the point.";


    private OpenAIAPI m_API;
        
    private AudioClip m_Clip;
    private bool m_IsRecording;
    private float m_Time;

    private string m_TextFromUser;
    private string m_TextAIResponse;

    //keep log of messages to feed to UI each time
    List<ChatMessage> m_Messages;
    

    private void Start()
    {
        m_API = Utilities.OpenAI();

        m_Messages = new List<ChatMessage> {
            new ChatMessage(ChatMessageRole.System, m_AIInstructions)
        };
    }
        
    private void StartRecording()
    {
        m_IsRecording = true;
        m_TextFromUser = "";
        Debug.Log("Listening for words from microphone...");

        //PlayerPrefs user-mic-device-index value is set in "MicrophoneSelector.cs"
        var index = PlayerPrefs.GetInt("user-mic-device-index");
        m_Clip = Microphone.Start(Microphone.devices[index], false, m_ListenDuration, 44100);
    }

    private async void EndRecording()
    {
        Debug.Log("Transcribing to text...");
        Microphone.End(null);

        string filename = "stt.wav";
        m_Clip.SaveToWAV(filename);

        string fullPath = System.IO.Path.Combine(Application.persistentDataPath, filename);
        m_TextFromUser = await m_API.Transcriptions.GetTextAsync(fullPath);

        m_ProgressBar.fillAmount = 0;
        Debug.Log($"You said: {m_TextFromUser}");
        TellAIWhatUserSaid();
    }

    private async void TellAIWhatUserSaid()
    {
        // Fill the user message from the input field
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.TextContent = m_TextFromUser;
        if (userMessage.TextContent.Length > 100)
        {
            // Limit messages to 100 characters
            userMessage.TextContent = userMessage.TextContent.Substring(0, 100);
        }

        // Add the message to the list
        m_Messages.Add(userMessage);

        // Update the text field with the user message
        Debug.Log($"You: {userMessage.TextContent}");

        // Send the entire chat to OpenAI to get the next message
        var chatResult = await m_API.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.9,
            MaxTokens = 50,
            Messages = m_Messages
        });

        // Get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.TextContent = chatResult.Choices[0].Message.TextContent;

        // Add the response to the list of messages
        m_Messages.Add(responseMessage);

        // Update the text field with the response
        m_TextAIResponse = responseMessage.TextContent;
        Debug.Log($"Unity Chan: {m_TextAIResponse}");
        AIResponseToSpeech();
    }

    private async void AIResponseToSpeech()
    {
        //Uncomment and edit the following line if you wish to hard-code the AI response
        //m_TextAIResponse = "I am the guardian of this palace gate. You cannot proceed without the password. Press enter or space if you wish to speak with me.";


        var request = new TextToSpeechRequest()
        {
            Input = m_TextAIResponse,
            ResponseFormat = ResponseFormats.MP3,
            Model = Model.TTS_HD,
            Voice = Voices.Nova,
            Speed = 0.9
        };

        string fullPath = System.IO.Path.Combine(Application.persistentDataPath, "tts.mp3");

        await m_API.TextToSpeech.SaveSpeechToFileAsync(request, fullPath);

        // Convert the local path to a URI that UnityWebRequest understands
        fullPath = "file://" + fullPath;

        // Start the coroutine to load and play the audio
        StartCoroutine(LoadAndPlayAudio(fullPath));

        Debug.Log($"Converted to audio file at {fullPath}");
    }

    private IEnumerator LoadAndPlayAudio(string uri)
    {
        Debug.Log($"Play audio for {uri}");

        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                m_AudioSource.clip = clip;
                m_AudioSource.Play();
            }
            else
            {
                Debug.LogError("Failed to load audio file: " + uwr.error);
            }
        }
    }

    private void Update()
    {
        if (m_IsRecording)
        {
            m_Time += Time.deltaTime;
            m_ProgressBar.fillAmount = m_Time / m_ListenDuration;
                
            if (m_Time >= m_ListenDuration)
            {
                m_Time = 0;
                m_IsRecording = false;
                EndRecording();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                m_AudioSource.Play();
            }
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartRecording();
            }
        }
    }
}

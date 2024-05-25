# ChatGPTUnityNPC

This project integrates several tools and libraries to create a ChatGPT-based NPC in Unity.

## Features

- **Lip Syncing**: Utilizes [uLipSync](https://github.com/hecomi/uLipSync/) for syncing UnityChat's mouth movements.
- **API Interface**: Implements [OkGoDoIt's C# wrapper](https://github.com/OkGoDoIt/OpenAI-API-dotnet) to interface with OpenAI's API.
- **Audio Generation**: Audio files are generated using OpenAI's API.
- **NPC Setup**: Based on a tutorial by Adam Kelly, available [here](https://www.youtube.com/watch?v=gI9QSHpiMW0).

## Requirements

- Unity v2022.3.29f1

## Setup Instructions

1. Clone this repository to your local machine.
2. Add a file named `ApiKey.txt` inside the `Assets` folder containing your ChatGPT API key.
3. Open the "TalkingUnityChan" scene and press play.

## Credits

- **uLipSync**: [hecomi](https://github.com/hecomi/uLipSync/)
- **OpenAI API .NET Wrapper**: [OkGoDoIt](https://github.com/OkGoDoIt/OpenAI-API-dotnet)
- **Tutorial by Adam Kelly**: [YouTube Link](https://www.youtube.com/watch?v=gI9QSHpiMW0)
- **UnityChan Model**: See the readme file in the assets folder for details.

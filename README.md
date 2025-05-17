# StereoTanks API wrapper in C# for HackArena 2.5

This C#-based WebSocket client was developed for the HackArena 2.5, organized
by kn init. It serves as a api wrapper for participants to create AI bots that
can play the game.

To fully test and run the game, you will also need the game server and GUI
client, as the GUI provides a visual representation of gameplay. You can find
more information about the server and GUI client in the following repository:

- [Server and GUI Client Repository](https://github.com/INIT-SGGW/HackArena2.5-StereoTanks)

## Development

Clone this repo using git:
```sh
git clone https://github.com/INIT-SGGW/HackArena2.5-StereoTanks-CSharp
```

The bot logic you are going to implement is located in
`Bot/Bot.cs`: <br>
**Bot example** can be found in `Bot/Bot.cs` file.
```C#
using StereoTanksBotLogic;
using StereoTanksBotLogic.Enums;
using StereoTanksBotLogic.Models;

namespace Bot;

public class Bot : IBot
{
    public Bot(LobbyData lobbyData)
    {
        // Initialize your bot. 
    }

    public void OnSubsequentLobbyData(LobbyData lobbyData)
    {
        // Define what should happen when lobby is changed.
        // For example when somebody new joins or disconnects
        // new LobbyData is sent and this method is run.
    }

    public BotResponse NextMove(GameState gameState)
    {
        // Define behaviour of your bot in this method. 
        // This method will be called every game tick with most recent game state.
        // Use BotResponse to return your action for each game tick.

        return BotResponse.Pass();
    }

    public void OnGameEnd(GameEnd gameEnd)
    {
        // Define what your program should do when game is finished.
    }

    public void OnGameStarting()
    {
        // Define what your program should do when game is starting.
    }

    public void OnWarningReceived(Warning warning, string? message)
    {
        // Define what your program should do when game is warning is recieved.
    }
}
```


The `Bot` class implements the `IBot` interface and defines the behavior of the bot.
 - The constructor `Bot(LobbyData lobbyData)` is called when the bot is created to initialize its state based on the initial lobby data.
 - The `OnSubsequentLobbyData(LobbyData lobbyData)` method is triggered whenever there is an update to the lobby data, such as when players join or leave the game.
 - The `NextMove(GameState gameState)` method is called every game tick to determine the bot's next action. This is where the bot’s behavior logic is implemented. It returns an BotResponse that defines what action the bot will take during the game tick.
 - The `OnGameEnd(GameEnd gameEnd)` method is called when the game finishes, allowing the bot to handle any final actions or cleanup.
 - The `OnGameStarting()` method is called when the game is about to start, letting the bot perform any preparations.
 - The `OnWarningReceived(Warning warning, string? message)` method is invoked when the game issues a warning, allowing the bot to handle warnings appropriately.

The `NextMove` method returns an `BotResponse` object, which can be one of the following:
 - `MoveResponse`: Move the tank forward or backward. The `MoveDirection` enum defines the options: `FORWARD` or `BACKWARD`.
 - `RotationResponse`: Rotate the tank or turret. The `RotationDirection` enum specifies the direction: `LEFT` or `RIGHT`.
 - `AbilityUseResponse`: Use an ability such as firing a bullet or using a radar. The `AbilityType` enum includes various options, such as `FireBullet`, `UseLaser`, `FireDoubleBullet`, `UseRadar`, `DropMine`, `FireHealingBullet`, and `FireStunBullet`.
 - `PassResponse`: Do nothing for the current game tick.
 - `GoTo`: Move the tank to specified coordinates on a board. You will need to send this response every tick until bot will reach specified destination. You can also use `Costs` and `Penalties` objects in order to fine tune pathfinding to your needs!
 - `CaptureZone`: Command the tank to attempt to capture the zone it is currently on. This is only effective if the tank is on a capturable zone tile. Use this method to increase your share in zone capture.

You can create these responses using static factory methods in the `BotResponse` class:
 - `BotResponse.Move()`
 - `BotResponse.Rotate()`
 - `BotResponse.UseAbility()`
 - `BotResponse.Pass()`
 - `BotResponse.GoTo()`
 - `BotResponse.CaptureZone()`

You can modify the mentioned file and create more files in the
`Bot` directory. Do not
modify any other files, as this may prevent us from running your bot during
the competition.
### Including Static Files

If you need to include static files that your program should access during
testing or execution, place them in the `data` folder. This folder is copied
into the Docker image and will be accessible to your application at runtime.
For example, you could include configuration files, pre-trained models, or any
other data your bot might need.

## Running the bot

You can run this bot in three different ways: locally, within a VS Code
development container, or manually using Docker.

### 1. Running locally (on Windows using Visual Studio).
Simply open project in Visual Studio and use IDE interface to run and debug your bot!

### 2. Running locally (on Linux or in other IDEs)

To run the bot locally, you must have dotnet SDK 8.0 or later installed and dotnet runtime 8.0 or later. <br><br>
Verify your dotnet SDK version by running:
```sh
dotnet --list-sdks
```
Verify your dotnet SDK runtime by running:
```sh
dotnet --list-runtimes
```

To build your solution use in `Debug` configuration use:
```sh
dotnet build HackArena2.5-StereoTanks-CSharp.sln
```
**Remember** we will test your bot in optimized `Release` configuration so make sure everything is correct. You can build `Release` version using command below:
```sh
dotnet build HackArena2.5-StereoTanks-CSharp.sln -c Release
```

Assuming the game server is running on `localhost:5000` (refer to the server
repository's README for setup instructions), start the bot by running in build directory `StereoTanksBot/bin/Debug/net8.0`:

```sh
./StereoTanksBot -n CSharpBot -t heavy
```

The `-n` argument is required and must be unique. The `-t` specifies which type of tank will be hosted. Acceptable value are: `heavy` or `light`. For additional configuration options, run:

```sh
./StereoTanksBot --help
```

### 3. Running in a Docker Container (Manual Setup)

To run the bot manually in a Docker container, ensure Docker is installed on
your system.

Steps:

1. Build the Docker image:
   ```sh
   docker build -t bot .
   ```
2. Run the Docker container:
   ```sh
   docker run --rm bot --host host.docker.internal -n TEAM_NAME -t heavy
   ```

If the server is running on your local machine, use the
`--host host.docker.internal` flag to connect the Docker container to your local
host.

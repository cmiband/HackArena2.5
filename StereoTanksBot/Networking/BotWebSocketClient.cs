using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StereoTanksBotLogic;
using StereoTanksBotLogic.Enums;
using StereoTanksBotLogic.Models;
using StereoTanksBotLogic.Networking;

namespace StereoTanksBot.Networking;

/// <summary>
/// Represents custom web socket client that comunicates bot with game server.
/// </summary>
internal class BotWebSocketClient : IDisposable
{
    private readonly IContractResolver contractResolver = new CamelCasePropertyNamesContractResolver();
    private readonly uint incomingMessageBufferSize = 32 * 1024;
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

    private readonly ClientWebSocket clientWebSocket;
    private readonly Uri serverURI;

    private IBot? bot;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotWebSocketClient"/> class.
    /// </summary>
    /// <param name="host">Represents host of a WebSocket server.</param>
    /// <param name="port">Represents port of a WebSocket server.</param>
    /// <param name="teamName">Represents team name of bot.</param>
    /// <param name="tankType">Represents tank type of bot.</param>
    /// <param name="code">Represents join code.</param>
    public BotWebSocketClient(string host = "localhost", string port = "5000", string teamName = "", TankType tankType = TankType.Light, string code = "")
    {
        this.clientWebSocket = new ClientWebSocket();

        string parsedTankType = tankType switch
        {
            TankType.Light => "light",
            TankType.Heavy => "heavy",
            _ => throw new NotSupportedException(),
        };

        StringBuilder link = new($"ws://{host}:{port}/?teamName={teamName}&tankType={parsedTankType}&playerType=hackathonBot");

        if (code.Length != 0)
        {
            _ = link.Append($"&joinCode={code}");
        }

        this.serverURI = new(link.ToString());
    }

    /// <summary>
    /// Gets a value indicating whether connection is open or not.
    /// </summary>
    public bool IsConnected { get => this.clientWebSocket.State == WebSocketState.Open; }

    /// <summary>
    ///  Connects to a WebSocket server as an asynchronous operation.
    /// </summary>
    /// <param name="uri">Represents uri of a WebSocket server.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task ConnectAsync()
    {
        try
        {
            await this.clientWebSocket.ConnectAsync(this.serverURI, CancellationToken.None);
            Console.WriteLine("[System] Successfully connected to the server");

            // TODO: Run this in a new thread.
            await this.ListenForMessagesAsync();
        }
        catch (Exception)
        {
            Console.WriteLine("[System] Connection Failed.");
        }
    }

    /// <summary>
    ///  Close the <see cref="BotWebSocketClient" /> instance as an asynchronous operation.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task CloseAsync()
    {
        await this.clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
        Console.WriteLine("[System] WebSocket connection closed.");
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.clientWebSocket.Dispose();
    }

    /// <summary>
    /// Send message using clientWebSocket.
    /// </summary>
    /// <param name="message">Message to send. </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    private async Task SendMessageAsync(string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        await this.clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task ListenForMessagesAsync()
    {
        while (this.clientWebSocket.State == WebSocketState.Open)
        {
            byte[] buffer = new byte[this.incomingMessageBufferSize];
            WebSocketReceiveResult result;
            try
            {
                result = await this.clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("[ERROR] Receiving a message failed: ");
                Console.WriteLine("[^^^^^] {0}", ex.Message);
                Console.WriteLine("[^^^^^] Closing the connection with InternalServerError.");

                await this.clientWebSocket.CloseAsync(
                    WebSocketCloseStatus.InternalServerError,
                    "Internal server error",
                    CancellationToken.None);

                break;
            }

            if (!result.EndOfMessage)
            {
                Console.WriteLine("[WARN] Received message is too big");
                Console.WriteLine("[^^^^] Closing the connection with MessageTooBig.");

                await this.clientWebSocket.CloseAsync(
                    WebSocketCloseStatus.MessageTooBig,
                    "Message too big",
                    CancellationToken.None);

                break;
            }

            if (result.MessageType == WebSocketMessageType.Text)
            {
                _ = this.HandleBuffer(buffer, result.Count);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await this.clientWebSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closing",
                    CancellationToken.None);

                Console.WriteLine("[System] WebSocket connection closed.");
            }
        }
    }

    private async Task HandleBuffer(byte[] buffer, int bytesRecieved)
    {
        Packet packet;

        try
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = this.contractResolver,
            };

            packet = JsonConvert.DeserializeObject<Packet>(Encoding.UTF8.GetString(buffer, 0, bytesRecieved), settings)!;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Packet deserialization failed: ");
            Console.WriteLine("[^^^^^] Message: {0}", ex.Message);

            throw;
        }

        switch (packet.Type)
        {
            case PacketType.LobbyData:
            case PacketType.GameStarting:
            case PacketType.GameState:
            case PacketType.GameEnded:
            case PacketType.PlayerAlreadyMadeActionWarning:
            case PacketType.SlowResponseWarning:
            case PacketType.ActionIgnoredDueToDeadWarning:
            case PacketType.CustomWarning:
                {
                    if (this.semaphore.Wait(new TimeSpan(0, 0, 1)))
                    {
                        try
                        {
                            await this.ProcessPacket(packet);
                        }
                        finally
                        {
                            _ = this.semaphore.Release();
                        }
                    }
                    else
                    {
                        Console.WriteLine("[System] Skipping packet due to previous packet not being processed in time.");
                        return;
                    }

                    break;
                }

            default:
                {
                    _ = this.ProcessPacket(packet);
                    break;
                }
        }
    }

    private async Task ProcessPacket(Packet packet)
    {
        switch (packet.Type)
        {
            case PacketType.Ping:
                {
                    Packet pongPacket = new Packet() { Type = PacketType.Pong, Payload = [] };
                    await this.SendMessageAsync(JsonConvert.SerializeObject(pongPacket));
                    break;
                }

            case PacketType.ConnectionAccepted:
                {
                    Console.WriteLine("[System] Connection accepted");
                    Packet lobbyDataRequest = new Packet() { Type = PacketType.LobbyDataRequest, Payload = [] };
                    await this.SendMessageAsync(JsonConvert.SerializeObject(lobbyDataRequest));
                    break;
                }

            case PacketType.ConnectionRejected:
                {
                    var connectionRejected = packet.GetPayload<ConnectionRejected>();
                    Console.WriteLine($"[System] Connection rejected -> {connectionRejected.Reason}");
                    break;
                }

            case PacketType.LobbyData:
                {
                    Console.WriteLine("[System] Lobby data received");
                    var lobbyData = packet.GetPayload<LobbyData>();

                    if (this.bot == null)
                    {
                        this.bot = new Bot.Bot(lobbyData);

                        if (lobbyData.ServerSettings.SandboxMode)
                        {
                            Console.WriteLine("[SYSTEM] Sandbox mode enabled");

                            Packet readyToReceivePacket = new Packet() { Type = PacketType.ReadyToReceiveGameState, Payload = [] };
                            await this.SendMessageAsync(JsonConvert.SerializeObject(readyToReceivePacket));
                            Packet gameStatusRequestPacket = new Packet() { Type = PacketType.GameStatusRequest, Payload = [] };
                            await this.SendMessageAsync(JsonConvert.SerializeObject(gameStatusRequestPacket));
                        }
                    }
                    else
                    {
                        this.bot.OnSubsequentLobbyData(lobbyData);
                    }

                    break;
                }

            case PacketType.GameStarting:
                {
                    Console.WriteLine("[System] Game starting");

                    if (this.bot == null)
                    {
                        while (this.bot == null)
                        {
                            try
                            {
                                Thread.Sleep(100);
                            }
                            catch (ThreadInterruptedException e)
                            {
                                Thread.CurrentThread.Interrupt();
                                Console.WriteLine("[ERROR] Interrupted while waiting for bot initialization.");
                                Console.WriteLine($"[^^^^^] {e.Message}");
                            }
                        }

                        try
                        {
                            Packet readyToReceivePacket = new Packet() { Type = PacketType.ReadyToReceiveGameState, Payload = [] };
                            await this.SendMessageAsync(JsonConvert.SerializeObject(readyToReceivePacket));
                            Console.WriteLine("[SYSTEM] Sent ReadyToReceiveGameState after bot initialization.");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("[ERROR] Error while sending ReadyToReceiveGameState");
                            Console.WriteLine($"[^^^^^] {e.Message}");
                        }
                    }
                    else
                    {
                        Packet readyToReceivePacket = new Packet() { Type = PacketType.ReadyToReceiveGameState, Payload = [] };
                        await this.SendMessageAsync(JsonConvert.SerializeObject(readyToReceivePacket));
                    }

                    break;
                }

            case PacketType.GameState:
                {
                    try
                    {
                        var gameState = packet.GetPayload<GameState>();

                        BotResponse botResponse = this.bot!.NextMove(gameState);
                        botResponse.Payload["gameStateId"] = gameState.Id;
                        await this.SendMessageAsync(JsonConvert.SerializeObject(botResponse));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[ERROR] Error while processing game state: ");
                        Console.WriteLine($"[^^^^^] {e.Message}");
                        throw;
                    }

                    break;
                }

            case PacketType.GameEnded:
                {
                    Console.WriteLine("[SYSTEM] Game ended!");
                    var gameEnd = packet.GetPayload<GameEnd>();
                    this.bot!.OnGameEnd(gameEnd);
                    break;
                }

            case PacketType.PlayerAlreadyMadeActionWarning:
                {
                    this.bot!.OnWarningReceived(Warning.PlayerAlreadyMadeActionWarning, null);
                    break;
                }

            case PacketType.SlowResponseWarning:
                {
                    this.bot!.OnWarningReceived(Warning.SlowResponseWarning, null);
                    break;
                }

            case PacketType.ActionIgnoredDueToDeadWarning:
                {
                    this.bot!.OnWarningReceived(Warning.ActionIgnoredDueToDeadWarning, null);
                    break;
                }

            case PacketType.InvalidPacketTypeError:
                {
                    Console.WriteLine("[ERROR] Invalid packet type error");
                    break;
                }

            case PacketType.InvalidPacketUsageError:
                {
                    Console.WriteLine("[ERROR] Invalid packet usage error");
                    break;
                }

            case PacketType.InvalidPayloadError:
                {
                    Console.WriteLine("[ERROR] Invalid payload error");
                    break;
                }

            case PacketType.InternalError:
                {
                    Console.WriteLine("[ERROR] Internal server error");
                    break;
                }

            case PacketType.CustomWarning:
                {
                    var customWarning = packet.GetPayload<CustomWarning>();
                    this.bot!.OnWarningReceived(Warning.CustomWarning, customWarning.Message);
                    break;
                }

            // Should never happen
            case PacketType.Pong: break;
            case PacketType.Movement: break;
            case PacketType.Rotation: break;
            case PacketType.AbilityUse: break;
            case PacketType.Pass: break;
            case PacketType.GoTo: break;
            case PacketType.ReadyToReceiveGameState: break;
            case PacketType.InvalidPacketTypeErrorWithPayload: break; // ADDED TEMPORARILY TODO: CHANGE
            case PacketType.InvalidPacketUsageErrorWithPayload: break; // ADDED TEMPORARILY TODO: CHANGE
            case PacketType.InvalidPayloadErrorWithPayload: break; // ADDED TEMPORARILY TODO: CHANGE
            case PacketType.InternalErrorWithPayload: break; // ADDED TEMPORARILY TODO: CHANGE
            default: throw new NotSupportedException();
        }
    }
}

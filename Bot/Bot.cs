using StereoTanksBotLogic;
using StereoTanksBotLogic.Enums;
using StereoTanksBotLogic.Models;
using System.Collections.Generic;
using System.Text;

namespace Bot;

public class Bot : IBot
{
    private string myId;
    private string myTeamName;
    private State currentState;

    private TankType? currentTankType;

    public Bot(LobbyData lobbyData)
    {
        this.myId = lobbyData.PlayerId;
        this.myTeamName = lobbyData.TeamName;

        Console.WriteLine(lobbyData);
        Console.WriteLine("players");
        Console.WriteLine(lobbyData.Teams);
        Console.WriteLine("ep");

        List<LobbyPlayer> allPlayers = concatPlayers(lobbyData.Teams);
        this.currentTankType = this.GetTankType(allPlayers);

        Console.WriteLine(this.currentTankType.ToString());

    }

    private List<LobbyPlayer> concatPlayers(LobbyTeam[] teams)
    {
        List<LobbyPlayer> lps = new List<LobbyPlayer>();
        foreach (var team in teams)
        {
            foreach(var player in team.Players)
            {
                lps.Add(player);
            }
        }

        return lps;
    }

    private TankType? GetTankType(List<LobbyPlayer> players)
    {
        Console.WriteLine("My id: " + this.myId);
        TankType? myTankType = this.FindTypeInArray(players);
        if(myTankType == null)
        {
            throw new Exception("chuj");
        }

        return myTankType;
    }

    private TankType? FindTypeInArray(List<LobbyPlayer> players)
    {
        for(int i = 0; i<players.Count; i++)
        {
            if (players[i].Id == this.myId)
            {
                return players[i].TankType;
            }
        }

        return null;
    }

    private int[] findMyLocation(GameState gameState)
    {
        for (int y = 0; y < gameState.Map.GetLength(0); y++)
        {
            for (int x = 0; x < gameState.Map.GetLength(1); x++)
            {
                Tile tile = gameState.Map[y, x];
                foreach (var entity in gameState.Map[y, x].Entities)
                {
                    if (entity is Tile.OwnLightTank || entity is Tile.OwnHeavyTank)
                    {
                        return new int[] { y, x };
                    }
                }
            }
                    
        }
        return null;
    }

    private bool MatchId(LobbyPlayer lp)
    {
        return lp.Id == this.myId;
    }


    public enum State
    {
        GAZ,
        ZONE,
        DEF
    }

    public void checkState()
    {
        // TODO: zrobić ustawianie na podstawie pozycji strefa czy nie strefa
        currentState = State.GAZ;
    }

    public void OnSubsequentLobbyData(LobbyData lobbyData) { }

    public BotResponse NextMove(GameState gameState)
    {
        int[] myLocation = findMyLocation(gameState);

        if (myLocation != null)
        {
            Console.WriteLine($"Moja pozycja: Y={myLocation[0]}, X={myLocation[1]}");
        }
        else
        {
            Console.WriteLine("Nie znaleziono mojego czołgu.");
        }

        /*

        Console.WriteLine($"GameStateId: {gameState.Id}");
        Console.WriteLine($"PlayerId: {gameState.PlayerId}");
        Console.WriteLine($"Tick: {gameState.Tick}");

        Console.WriteLine("I SEE THESE TEAMS:");
        for (int i = 0; i < gameState.Teams.Length; i++)
        {
            var team = gameState.Teams[i];
            Console.WriteLine($"TEAM: {i}");
            Console.WriteLine($"  Name: {team.Name}");
            Console.WriteLine($"  Color: {team.Color}");

            if(team is OwnTeam ot)
            {
                Console.WriteLine($"  Score: {ot.Score}");
            }
            
            Console.WriteLine($"  Players:");
            for (int j = 0; j < team.Players.Length; j++)
            {
                var player = team.Players[j];
                if (player is OwnPlayer op)
                {
                    Console.WriteLine($"  OwnPlayer:");
                    Console.WriteLine($"    Id: {op.Id}");
                    Console.WriteLine($"    Ping: {op.Ping}");
                    Console.WriteLine($"    TicksToRegen: {op.TicksToRegen}");
                }
                else if (player is EnemyPlayer ep)
                {
                    Console.WriteLine($"  OwnPlayer:");
                    Console.WriteLine($"    Id: {ep.Id}");
                    Console.WriteLine($"    Ping: {ep.Ping}");
                }
            }
        }

        Console.WriteLine("Map:");
        for (int y = 0; y < gameState.Map.GetLength(0); y++)
        {
            for (int x = 0; x < gameState.Map.GetLength(1); x++)
            {
                Tile tile = gameState.Map[y, x];
                char symbol = ' ';

                if (tile.ZoneIndex != null)
                {
                    int zoneIndex = (int)gameState.Map[y, x].ZoneIndex!;
                    symbol = (char)zoneIndex;
                }

                foreach (var entity in gameState.Map[y, x].Entities)
                {
                    if (entity is Tile.Wall wall)
                    {
                        if(wall.Type == WallType.Solid)
                        {
                            symbol = '#';
                        }
                        else
                        {
                            symbol = 'O';
                        }
                    }
                    else if (entity is Tile.OwnLightTank ownLightTank)
                    {
                        symbol = ownLightTank.Direction switch
                        {
                            Direction.Down => 'V',
                            Direction.Left => '<',
                            Direction.Up => '^',
                            Direction.Right => '>',
                            _ => throw new NotSupportedException()
                        };

                        // There is also turret direction
                        // ownTank.Turret.Direction
                    }
                    else if (entity is Tile.OwnHeavyTank ownHeavyTank)
                    {
                        symbol = ownHeavyTank.Direction switch
                        {
                            Direction.Down => 'V',
                            Direction.Left => '<',
                            Direction.Up => '^',
                            Direction.Right => '>',
                            _ => throw new NotSupportedException()
                        };

                        // There is also turret direction
                        // ownTank.Turret.Direction
                    }
                    else if (entity is Tile.EnemyHeavyTank || entity is Tile.EnemyLightTank)
                    {
                        symbol = '@';
                    }
                    else if (entity is Tile.Bullet bullet)
                    {
                        if (bullet.Type == BulletType.Stun)
                        {
                            symbol = bullet.Direction switch
                            {
                                Direction.Down => '↓',
                                Direction.Left => '←',
                                Direction.Up => '↑',
                                Direction.Right => '→',
                                _ => throw new NotSupportedException()
                            };
                        }
                        else if(bullet.Type == BulletType.Double)
                        {
                            symbol = bullet.Direction switch
                            {
                                Direction.Down => '⇊',
                                Direction.Left => '⇇',
                                Direction.Up => '⇈',
                                Direction.Right => '⇉',
                                _ => throw new NotSupportedException()
                            };
                        }
                        else if(bullet.Type == BulletType.Stun)
                        {
                            symbol = bullet.Direction switch
                            {
                                Direction.Down => '|',
                                Direction.Left => '-',
                                Direction.Up => '|',
                                Direction.Right => '-',
                                _ => throw new NotSupportedException()
                            };
                        }
                        else if (bullet.Type == BulletType.Healing)
                        {
                            symbol = bullet.Direction switch
                            {
                                Direction.Down => 'H',
                                Direction.Left => 'H',
                                Direction.Up => 'H',
                                Direction.Right => 'H',
                                _ => throw new NotSupportedException()
                            };
                        }
                    }
                    else if (entity is Tile.Laser laser)
                    {
                        symbol = laser.Orientation switch
                        {
                            LaserDirection.Horizontal => '-',
                            LaserDirection.Vertical => '|',
                            _ => throw new NotSupportedException()
                        };
                    }
                    else if (entity is Tile.Mine mine)
                    {
                        symbol = 'X';
                    }
                }
                Console.Write(symbol);
                Console.Write(' ');
            }
            Console.Write("\n");
        }

        Console.WriteLine("ZONES:");
        for (int i = 0; i < gameState.Zones.Length; i++)
        {
            var zone = gameState.Zones[i];
            Console.WriteLine($"   X: {zone.X}");
            Console.WriteLine($"   Y: {zone.Y}");
            Console.WriteLine($"   Index: {zone.Index}");
            Console.WriteLine($"   Width: {zone.Width}");
            Console.WriteLine($"   Shares:");
            for (int j = 0; j < zone.Shares.Count; j++)
            {
                var key = zone.Shares.Keys.ToList()[j];
                var value = zone.Shares.Values.ToList()[j];
                Console.WriteLine($"      Key: {key} Value: {value}");
            }
        */
        this.currentState = this.CalculateCurrentState();
        BotResponse response = this.GetBotResponseBasedOnState(this.currentState, gameState);

        /*
        //Bot that randomly choses one of all possible bot responses.
        var rand = new Random();
        var X = rand.Next(0,22);
        var Y = rand.Next(0,22);
        return rand.Next(0, 19) switch
        {
            0 => BotResponse.Pass(),
            1 => BotResponse.Move(MovementDirection.Backward),
            2 => BotResponse.Move(MovementDirection.Forward),
            3 => BotResponse.Rotate(null, null),
            4 => BotResponse.Rotate(null, Rotation.Left),
            5 => BotResponse.Rotate(null, Rotation.Right),
            6 => BotResponse.Rotate(Rotation.Left, null),
            7 => BotResponse.Rotate(Rotation.Left, Rotation.Left),
            8 => BotResponse.Rotate(Rotation.Left, Rotation.Right),
            9 => BotResponse.Rotate(Rotation.Right, null),
            10 => BotResponse.Rotate(Rotation.Right, null),
            11 => BotResponse.Rotate(Rotation.Right, Rotation.Left),
            12 => BotResponse.Rotate(Rotation.Right, Rotation.Right),
            13 => BotResponse.UseAbility(AbilityType.DropMine),
            14 => BotResponse.UseAbility(AbilityType.FireBullet),
            15 => BotResponse.UseAbility(AbilityType.FireDoubleBullet),
            16 => BotResponse.UseAbility(AbilityType.UseLaser),
            17 => BotResponse.UseAbility(AbilityType.UseRadar),
            18 => BotResponse.GoTo(X, Y, Rotation.Left, new(0, 0, 0), new(null, null, null, null, null, null)),
            19 => BotResponse.CaptureZone(),
            _ => throw new NotSupportedException(),
        };*/


        return response;
    }

    private BotResponse GetBotResponseBasedOnState(State playerState, GameState gameState)
    {
        switch(playerState)
        {
            case State.DEF:
                return this.HandleDefense(gameState);

            default:
                return BotResponse.Pass();
        }
    }

    private State CalculateCurrentState()
    {
        State result = State.DEF;



        return result;
    }

    private BotResponse HandleDefense(GameState gameState)
    {
        List<EnemyWrapper> enemies = this.GetEnemyPositions(gameState);
        PositionWrapper currentPlayerPosition = this.GetPlayerPosition(gameState);

        return BotResponse.Pass();         
    }

    private List<EnemyWrapper> GetEnemyPositions(GameState gameState)
    {
        List<EnemyWrapper> result= new List<EnemyWrapper>();

        for (int y = 0; y < gameState.Map.GetLength(0); y++)
        {
            for (int x = 0; x < gameState.Map.GetLength(1); x++)
            {
                Tile tile = gameState.Map[y, x];

                foreach (var entity in gameState.Map[y, x].Entities)
                {
                    if (entity is Tile.EnemyHeavyTank || entity is Tile.EnemyLightTank)
                    {
                        EnemyWrapper enemy = new EnemyWrapper();
                        PositionWrapper position = new PositionWrapper();

                        position.x = x;
                        position.y = y;
                        enemy.position = position;
                        result.Add(enemy);
                    }
                }
            }
        }

        return result;
    }

    private PositionWrapper? GetPlayerPosition(GameState gameState)
    {
        for (int y = 0; y < gameState.Map.GetLength(0); y++)
        {
            for (int x = 0; x < gameState.Map.GetLength(1); x++)
            {
                Tile tile = gameState.Map[y, x];

                foreach (var entity in gameState.Map[y, x].Entities)
                {
                    if (entity is Tile.OwnHeavyTank)
                    {
                        Tile.OwnHeavyTank? t = entity as Tile.OwnHeavyTank;
                        
                        if(t.OwnerId == this.myId)
                        {
                            PositionWrapper ps = new PositionWrapper();

                            ps.x = x;
                            ps.y = y;

                            return ps;
                        }


                    } else if (entity is Tile.OwnLightTank)
                    {
                        Tile.OwnLightTank? t = entity as Tile.OwnLightTank;

                        if (t.OwnerId == this.myId)
                        {
                            PositionWrapper ps = new PositionWrapper();

                            ps.x = x;
                            ps.y = y;

                            return ps;
                        }
                    }
                }
            }
        }

        return null;
    }

    public void OnGameEnd(GameEnd gameEnd)
    {
        // Define what your program should do when game is finished.
        GameEndTeam winner = gameEnd.Teams[0];

        foreach (var team in gameEnd.Teams)
        {
            Console.WriteLine($"{team.Name} - {team.Score}");
        }
    }

    public void OnGameStarting()
    {
        // Define what your program should do when game is starting.
    }

    public void OnWarningReceived(Warning warning, string? message)
    {
        // Define what your program should do when game is warning is recieved.

        switch (warning)
        {
            case Warning.PlayerAlreadyMadeActionWarning:
                {
                    Console.WriteLine("Player already made action warning");
                    break;
                }
            case Warning.SlowResponseWarning:
                {
                    Console.WriteLine("Slow response warning");
                    break;
                }
            case Warning.ActionIgnoredDueToDeadWarning:
                {
                    Console.WriteLine("Action ignored due to dead warning");
                    break;
                }
            case Warning.CustomWarning:
                {
                    Console.WriteLine($"Custom warning: {message ?? "no message"}");
                    break;
                }
        }
    }

    class EnemyWrapper
    {
        public PositionWrapper? position;
    }

    class PositionWrapper {
        public int x;
        public int y;
    }
}
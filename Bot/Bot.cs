using StereoTanksBotLogic;
using StereoTanksBotLogic.Enums;
using StereoTanksBotLogic.Models;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Bot;

public class Bot : IBot
{
    private string myId;
    private string myTeamName;
    private State currentState;

    private TankType? currentTankType;
    private List<PositionWrapper> queuedPositions = new List<PositionWrapper>();
    private bool wasAlreadyInZone = false;

    private int lightCounter = 0;
    private int lightSpinCounter = 0;

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

    public enum State
    {
        GAZ,
        ZONE,
        DEF_HEAVY,
        DEF_LIGHT,
        OFF
    }

    public bool AmInZone(GameState gameState)
    {
        var pos = GetPlayerPosition(gameState);
        if (pos == null)
        {
            return false;
        }
        foreach (var zone in gameState.Zones)
        {
            if (((zone.X <= pos.x) && pos.x < (zone.X + zone.Width)) && ((zone.Y <= pos.y) && (pos.y < zone.Y + zone.Height)))
            {
                return true;
            }
        }
        return false;
    }

    public void OnSubsequentLobbyData(LobbyData lobbyData) { }

    public BotResponse NextMove(GameState gameState)
    {
        if(gameState == null)
        {
            return BotResponse.Pass();
        }

        this.currentState = this.CalculateCurrentState(gameState);
        BotResponse response = this.GetBotResponseBasedOnState(this.currentState, gameState);

        return response;
    }

    private BotResponse GetBotResponseBasedOnState(State playerState, GameState gameState)
    {
        switch (playerState)
        {
            case State.DEF_LIGHT:
                Console.WriteLine("DEFL");
                return this.HandleLightDefense(gameState);
            case State.DEF_HEAVY:
                Console.WriteLine("DEFH");
                return this.HandleHeavyDefense(gameState);
            case State.GAZ:
                Console.WriteLine("GAZ");
                return this.HandleGaz(gameState);

            case State.OFF:
                Console.WriteLine("OFF");
                return this.HandleOffense(gameState);

            default:
                return BotResponse.Pass();
        }
    }

    public BotResponse HandleOffense(GameState gameState)
    {
        PositionWrapper myLocation = this.GetPlayerPosition(gameState);

        Direction? currentTurretDirection = null;

        foreach (var entity in gameState.Map[myLocation.y, myLocation.x].Entities)
        {
            if (entity is Tile.OwnLightTank lightTank)
            {
                currentTurretDirection = lightTank.Turret.Direction;
                break;
            }
            else if (entity is Tile.OwnHeavyTank heavyTank)
            {
                currentTurretDirection = heavyTank.Turret.Direction;
                break;
            }
        }

        if (GetEnemyPositions(gameState).Count > 0)
        {
            List<EnemyWrapper> listOfEnemies = GetEnemyPositions(gameState);
            EnemyWrapper closestEnemy = FindClosestEnemy(listOfEnemies, gameState);
            int turretDirection = FindEnemyDirection(closestEnemy, gameState);

            return FaceEnemy((int)currentTurretDirection, turretDirection, closestEnemy, gameState);
        }

        return BotResponse.Pass();
    }


    private BotResponse HandleGaz(GameState gameState)
    {
        // jak jestesmy LIGHT i zapierdalamy do strefy to sprawdza czy może radaru użyć (jeżeli da cokolwiek) i używa
        if (currentTankType == TankType.Light && GetEnemyPositions(gameState).Count < 2)
        {
            for (int y = 0; y < gameState.Map.GetLength(0); y++)
            {
                for (int x = 0; x < gameState.Map.GetLength(1); x++)
                {
                    Tile tile = gameState.Map[y, x];

                    foreach (var entity in gameState.Map[y, x].Entities)
                    {
                        if (entity is Tile.OwnLightTank tank)
                        {
                            if (tank.TicksToRadar == null || tank.TicksToRadar <= 0)
                            {
                                return BotResponse.UseAbility(AbilityType.UseRadar);
                            }
                        }
                    }
                }
            }
        }

        // defaultowo zapierdala na pierwszy możliwy punkt w strefie
        PositionWrapper zoneCoord = this.GetValidZoneCoord(gameState);
        return BotResponse.GoTo(zoneCoord.x, zoneCoord.y, null, new(0, 1, 2), new(0, 2, 2, 2, 10, null));
    }

    private PositionWrapper GetValidZoneCoord(GameState gameState)
    {
        List<PositionWrapper> zoneCoords = new List<PositionWrapper>();
        Zone z = gameState.Zones[0];

        for (int i = z.X; i<z.X+z.Width; i++)
        {
            for(int j = z.Y; j<z.Y+z.Height; j++)
            {
                PositionWrapper pw = new PositionWrapper();
                pw.x = i;
                pw.y = j;

                zoneCoords.Add(pw);
            }
        }

        List<PositionWrapper> filteredResult = new List<PositionWrapper>();

        for(int i = 0; i<zoneCoords.Count; i++)
        {
            PositionWrapper currentCoords = zoneCoords[i];
            Tile.TileEntity[] te = gameState.Map[currentCoords.y, currentCoords.x].Entities;

            if(te.Length == 0)
            {
                filteredResult.Add(currentCoords);
                continue;
            }

            foreach(Tile.TileEntity t in te) {
                if (!(t is Tile.Wall))
                {
                    
                    filteredResult.Add(currentCoords);
                }
            }
        }

        return filteredResult[0];
    }

    private State CalculateCurrentState(GameState gameState)
    {
        List<EnemyWrapper> enemies = this.GetEnemyPositions(gameState);
        EnemyWrapper? closest = this.FindClosestEnemy(enemies, gameState);
        bool isEnemyOnSameAxis = closest != null ? this.isEnemyOnTheSameAxis(closest, gameState) : false;
        bool isInZone = AmInZone(gameState);

        if (isInZone || wasAlreadyInZone)
        {
            if (currentTankType == TankType.Light)
            {
               wasAlreadyInZone = true;
            }

            if(!isEnemyOnSameAxis)
            {
                if(this.currentTankType == TankType.Heavy)
                {
                    return State.DEF_HEAVY;
                }
                else
                {
                    return State.DEF_LIGHT;
                }
            } 
            else
            {
                return State.OFF;
            }
        } 

        return State.GAZ;
    }

    private BotResponse HandleHeavyDefense(GameState gameState)
    {
        return BotResponse.CaptureZone();
    }

    private List<PositionWrapper> getPositionsAroundZone(GameState gameState)
    {
        Zone z = gameState.Zones[0];

        List<PositionWrapper> positionsToCheck = new List<PositionWrapper>
        {
            new PositionWrapper(z.X, z.Y),
            new PositionWrapper(z.X+1, z.Y),
            new PositionWrapper(z.X+2, z.Y),
            new PositionWrapper(z.X+3, z.Y),
            new PositionWrapper(z.X+3, z.Y+1),
            new PositionWrapper(z.X+3, z.Y+2),
            new PositionWrapper(z.X+3, z.Y+3),
            new PositionWrapper(z.X+2, z.Y+3),
            new PositionWrapper(z.X+1, z.Y+3),
            new PositionWrapper(z.X, z.Y+3),
            new PositionWrapper(z.X, z.Y+2),
            new PositionWrapper(z.X, z.Y+1)
        };
        List<PositionWrapper> filteredResult = new List<PositionWrapper>();

        for (int i = 0; i < positionsToCheck.Count; i++)
        {
            PositionWrapper currentCoords = positionsToCheck[i];
            Tile.TileEntity[] te = gameState.Map[currentCoords.y, currentCoords.x].Entities;

            if (te.Length == 0)
            {
                filteredResult.Add(currentCoords);
                continue;
            }

            foreach (Tile.TileEntity t in te)
            {
                if ((t is not Tile.Wall) && (t is not Tile.OwnHeavyTank) && (t is not Tile.EnemyHeavyTank) && (t is not Tile.EnemyLightTank))
                {

                    filteredResult.Add(currentCoords);
                }
            }
        }

        if(filteredResult.Count == 0)
        {
            filteredResult.Add(new PositionWrapper(z.X+2, z.Y+2));
        }
        return filteredResult;
    }

    private BotResponse HandleLightDefense(GameState gameState)
    {
        PositionWrapper playerPos = this.GetPlayerPosition(gameState);
        if(playerPos == null)
        {
            return BotResponse.Pass();
        }

        Tile.OwnLightTank myTank = this.GetMyLightTank(playerPos, gameState);

        if (myTank == null)
        {
            return BotResponse.Pass();
        }

        if(myTank.TicksToRadar <= 0 || myTank.TicksToRadar == null)
        {
            return BotResponse.UseAbility(AbilityType.UseRadar);
        }

        if(this.lightCounter == 0)
        {
            Console.WriteLine("cap");
            lightCounter = 1;
            if (this.AmInZone(gameState))
            {
                return BotResponse.CaptureZone();
            }
        }
        else if(this.lightCounter == 1)
        {
            Console.WriteLine("rot");
            if (lightSpinCounter < 5)
            {
                lightSpinCounter++;
                return BotResponse.Rotate(null, Rotation.Right);
            } else {
                lightSpinCounter = 0;
                lightCounter = 2;
            }
        } 
        else if(this.lightCounter == 2)
        {
            Console.WriteLine("go on");
            lightCounter = 0;
        }

        PositionWrapper? currentPos = new PositionWrapper(1,1);
        if (this.queuedPositions.Count == 0)
        {
            this.queuedPositions = this.getPositionsAroundZone(gameState);
        } 
        else
        {
            currentPos = this.queuedPositions[0];

            if(playerPos.x == currentPos.x && playerPos.y == currentPos.y)
            {
                if(this.queuedPositions.Count > 0)
                {
                    currentPos = this.queuedPositions[0];

                    this.queuedPositions.RemoveAt(0);
                } else
                {
                    currentPos = this.GetValidZoneCoord(gameState);
                }
            }
        }

        Console.WriteLine("targeted pos" + currentPos.x + " " + currentPos.y);        

        return BotResponse.GoTo(currentPos.x, currentPos.y, Rotation.Left, new(0, 1, 1), new(0, 2, 2, 2, 10, null));
    }

    private List<string> GetEnemiesIntersectingWithPlayers(List<EnemyWrapper> enemies, PositionWrapper player, GameState gameState)
    {
        List<string> result = new List<string>();

        for(int i = 0; i<enemies.Count; i++)
        {
            EnemyWrapper enemy = enemies[i];
            string axis = this.CheckIfCoordsMatch(enemy.position, player);

            if (axis != "")
            {
                bool isIntersecting = this.CheckIfThereIsNoBlocksBetweenTanks(gameState, enemy.position, player, axis);

                if(isIntersecting)
                {
                    result.Add(axis);
                }
            }
        }

        return result;
    }

    private bool CheckIfThereIsNoBlocksBetweenTanks(GameState gameState, PositionWrapper enemy, PositionWrapper player, string axis)
    {
        if(axis == "x")
        {
            int yPos = player.y;
            int smallerPosition = Math.Min(player.x, enemy.x);
            int biggerPosition = Math.Max(player.x, enemy.x);

            if(biggerPosition-smallerPosition == 1)
            {
                return true;
            }

            for(int i = smallerPosition+1; i < biggerPosition; i++)
            {
                Tile checkedTile = gameState.Map[yPos, i];
                
                if(!this.CheckIfTileIsClear(checkedTile))
                {
                    return false;
                }
            }
        } 
        else
        {
            int xPos = player.x;
            int smallerPosition = Math.Min(player.y, enemy.y);
            int biggerPosition = Math.Max(player.y, enemy.y);

            if (biggerPosition - smallerPosition == 1)
            {
                return true;
            }

            for (int i = smallerPosition + 1; i < biggerPosition; i++)
            {
                Tile checkedTile = gameState.Map[i, xPos];

                if (!this.CheckIfTileIsClear(checkedTile))
                {
                    return false;
                }
            }
        }

        return false;
    }

    private bool CheckIfTileIsClear(Tile t)
    {
        foreach (var entity in t.Entities)
        {
            if (entity is Tile.OwnHeavyTank || entity is Tile.OwnLightTank || entity is Tile.Wall)
            {
                return false;
            }
        }

        return true;
    }

    private string CheckIfCoordsMatch(PositionWrapper enemy, PositionWrapper player)
    {
        if(enemy.y == player.y)
        {
            return "x";
        }

        if (enemy.x == player.x)
        {
            return "y";
        }

        return "";
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

    private EnemyWrapper? FindClosestEnemy(List<EnemyWrapper> enemies, GameState gameState)
    {
        if(enemies.Count == 0)
        {
            return null;
        }

        PositionWrapper myPosition = GetPlayerPosition(gameState);
        List<(int distance, EnemyWrapper enemy)> distances = new List<(int, EnemyWrapper)>();
        int i = 0;
        foreach (EnemyWrapper enemy in enemies)
        {
            int distance = Math.Abs(enemy.position.x - myPosition.x) + Math.Abs(enemy.position.y - myPosition.y);
            distances.Add((distance, enemy));
        }

        return distances.OrderBy(e => e.distance).First().enemy;
    }

    private int FindEnemyDirection(EnemyWrapper enemy, GameState gameState)
    {
        PositionWrapper myPosition = GetPlayerPosition(gameState);

        int dx = enemy.position.x - myPosition.x;
        int dy = enemy.position.y - myPosition.y;

        int turretDirection;

        
        int distanceToEnemy = Math.Abs(dx) + Math.Abs(dy);

        if (dy == 0)
        {
            turretDirection = dx < 0 ? 3 : 1;
        }
        else if (dx == 0)
        {

            turretDirection = dy < 0 ? 0 : 2;
        }
        else
        {

            double angle = Math.Atan2(dy, dx);

            if (angle >= -Math.PI / 4 && angle < Math.PI / 4)
            {
                turretDirection = 1;
            }
            else if (angle >= Math.PI / 4 && angle < 3 * Math.PI / 4)
            {
                turretDirection = 2;
            }
            else if (angle >= -3 * Math.PI / 4 && angle < -Math.PI / 4)
            {
                turretDirection = 0;
            }
            else
            {
                turretDirection = 3;
            }
        }
        return turretDirection;
    }

    private bool isEnemyOnTheSameAxis(EnemyWrapper enemy, GameState gameState)
    {
        PositionWrapper myPosition = GetPlayerPosition(gameState);
        if (enemy.position.x == myPosition.x || enemy.position.y == myPosition.y) return true;
        return false;
    }

    private BotResponse FaceEnemy(int currentTurretDirection, int turretDirection, EnemyWrapper closestEnemy, GameState gameState)
    {
        if ((int)currentTurretDirection != turretDirection)
        {
            int difference = (int)currentTurretDirection - turretDirection;

            if (Math.Abs(difference) == 2)
            {
                BotResponse.Rotate(null, Rotation.Left);
                return BotResponse.Rotate(null, Rotation.Left);
            }
            else if (difference == -3 || difference == 1)
            {
                return BotResponse.Rotate(null, Rotation.Left);
            }
            return BotResponse.Rotate(null, Rotation.Right);
        }
        return Attack(closestEnemy, gameState);
    }

    private BotResponse Attack(EnemyWrapper enemy, GameState gameState)
    {
        for (int y = 0; y < gameState.Map.GetLength(0); y++)
        {
            for (int x = 0; x < gameState.Map.GetLength(1); x++)
            {
                Tile tile = gameState.Map[y, x];

                foreach (var entity in gameState.Map[y, x].Entities)
                {
                    if (entity is Tile.OwnHeavyTank heavy && isEnemyOnTheSameAxis(enemy, gameState))
                    {
                        if (heavy.Turret.TicksToStunBullet == null || heavy.Turret.TicksToBullet <= 0)
                        {
                            return BotResponse.UseAbility(AbilityType.FireStunBullet);
                        }
                        else if(heavy.Turret.TicksToLaser == null || heavy.Turret.TicksToBullet <= 0)
                        {
                            return BotResponse.UseAbility(AbilityType.UseLaser);
                        }
                        else if (heavy.Turret.BulletCount != 0)
                        {
                            return BotResponse.UseAbility(AbilityType.FireBullet);
                        }
                        else
                        {
                            return HandleHeavyDefense(gameState);
                        }

                    }
                    else if (entity is Tile.OwnLightTank light && isEnemyOnTheSameAxis(enemy, gameState))
                    {
                        if (light.Turret.TicksToStunBullet == null)
                        {
                            return BotResponse.UseAbility(AbilityType.FireStunBullet);
                        }
                        else if (light.Turret.BulletCount == 0 && light.Turret.TicksToDoubleBullet == null)
                        {
                            return BotResponse.UseAbility(AbilityType.FireDoubleBullet);

                        }
                        else if (light.Turret.BulletCount != 0)
                        {
                            return BotResponse.UseAbility(AbilityType.FireBullet);
                        }
                        else {
                            return HandleLightDefense(gameState);
                        }

                    }
                }
            }
        }
        return BotResponse.Pass();
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

    private Tile.OwnLightTank? GetMyLightTank(PositionWrapper playerPos, GameState gameState)
    {
        Tile t = gameState.Map[playerPos.y, playerPos.x];

        foreach(Tile.TileEntity te in t.Entities) {
            if(te is Tile.OwnLightTank)
            {
                return (Tile.OwnLightTank) te;
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
        currentState = State.GAZ;
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
        public PositionWrapper() { }

        public PositionWrapper(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x;
        public int y;
    }
}
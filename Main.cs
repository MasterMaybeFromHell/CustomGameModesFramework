using Il2Cpp;
using MelonLoader;
using System.Reflection;

[assembly: MelonInfo(typeof(CustomGameModesFramework.Main), "CustomGameModesFramework", "1.0.0", "MasterHell", null)]
[assembly: MelonGame("ZeoWorks", "Slendytubbies 3")]
[assembly: MelonColor(255, 0, 128, 255)]

namespace CustomGameModesFramework
{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoadExternalGameModes();
        }

        private void LoadExternalGameModes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    if (IsSystemOrUnityAssembly(assembly))
                        continue;

                    RegisterGameModesFromAssembly(assembly);
                }
                catch (ReflectionTypeLoadException e)
                {
                    MelonLogger.Warning($"Failed to load types from assembly {assembly.GetName().Name}: {e.Message}");
                }
                catch (Exception e)
                {
                    MelonLogger.Error($"Error loading external game modes from {assembly.GetName().Name}: {e.Message}");
                }
            }
        }

        private void RegisterGameModesFromAssembly(Assembly assembly)
        {
            var gameModeTypes = assembly.GetTypes()
                .Where(type => typeof(CustomGameMode).IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var type in gameModeTypes)
            {
                var gameModeInstance = (CustomGameMode)Activator.CreateInstance(type);
                GameModeManager.Instance.RegisterGameMode(gameModeInstance);

                if (type.GetCustomAttribute(typeof(GameModeInfo)) is GameModeInfo gameModeInfoAttribute)
                {
                    GameModeManager.Instance.GameModeInfos.Add(gameModeInfoAttribute);
                }
                else
                {
                    MelonLogger.Warning($"No GameModeInfo attribute found for {type.Name}");
                }

                MelonLogger.Msg($"Registered game mode: {type.Name}");
            }
        }

        private bool IsSystemOrUnityAssembly(Assembly assembly)
        {
            return assembly.GetName().Name.StartsWith("UnityEngine") || 
                assembly.GetName().Name.StartsWith("Unity.") || 
                assembly.GetName().Name.StartsWith("System.") || 
                assembly.GetName().Name.StartsWith("mscorlib") || 
                assembly.GetName().Name.StartsWith("netstandard");
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class GameModeInfo : Attribute
    {
        public string ModeID;
        public string ModeName;
        public string Players;
        public string SmallMapPlayerCount;
        public string MediumMapPlayerCount;
        public string LargeMapPlayerCount;

        public GameModeInfo(string modeID, string modeName, string players, string smallMapPlayerCount, 
            string mediumMapPlayerCount, string largeMapPlayerCount)
        {
            ModeID = modeID;
            ModeName = modeName;
            Players = players;
            SmallMapPlayerCount = smallMapPlayerCount;
            MediumMapPlayerCount = mediumMapPlayerCount;
            LargeMapPlayerCount = largeMapPlayerCount;
        }
    }

    public abstract class CustomGameMode
    {
        public virtual void LobbyMenuOptions(LobbyMenu instance) { }
        public virtual void OnAwake(RoomMultiplayerMenu instance) { }
        public virtual void OnStartGame(RoomMultiplayerMenu instance) { }
        public virtual void OnGUI(RoomMultiplayerMenu instance) { }
        public virtual void OnFixedUpdate(RoomMultiplayerMenu instance) { }
        public virtual void OnSpawnPlayer(RoomMultiplayerMenu instance) { }
        public virtual void OnRoundEnded(RoomMultiplayerMenu instance) { }
        public virtual void OnRestart(RoomMultiplayerMenu instance) { }
        public virtual void OnRespawnPlayerGUI(RagdollController instance) { }
        public virtual void OnRespawnPlayer(RagdollController instance) { }
    }

    public class GameModeManager
    {
        public static GameModeManager Instance { get; private set; }
        public List<CustomGameMode> _gameModes = [];
        public List<GameModeInfo> GameModeInfos = [];

        static GameModeManager()
        {
            Instance = new GameModeManager();
        }

        public void RegisterGameMode(CustomGameMode customGameMode)
        {
            _gameModes.Add(customGameMode);
        }

        public IEnumerable<Action> GetActionsForAwake(RoomMultiplayerMenu instance) => _gameModes.Select(mode => new Action(() => mode.OnAwake(instance)));
        public IEnumerable<Action> GetActionsForStartGame(RoomMultiplayerMenu instance) => _gameModes.Select(mode => new Action(() => mode.OnStartGame(instance)));
        public IEnumerable<Action> GetActionsForOnGUI(RoomMultiplayerMenu instance) => _gameModes.Select(mode => new Action(() => mode.OnGUI(instance)));
        public IEnumerable<Action> GetActionsForFixedUpdate(RoomMultiplayerMenu instance) => _gameModes.Select(mode => new Action(() => mode.OnFixedUpdate(instance)));
        public IEnumerable<Action> GetActionsForSpawnPlayer(RoomMultiplayerMenu instance) => _gameModes.Select(mode => new Action(() => mode.OnSpawnPlayer(instance)));
        public IEnumerable<Action> GetActionsForRoundEnded(RoomMultiplayerMenu instance) => _gameModes.Select(mode => new Action(() => mode.OnRoundEnded(instance)));
        public IEnumerable<Action> GetActionsForRestart(RoomMultiplayerMenu instance) => _gameModes.Select(mode => new Action(() => mode.OnRestart(instance)));
        public IEnumerable<Action> GetActionsForRespawnPlayerGUI(RagdollController instance) => _gameModes.Select(mode => new Action(() => mode.OnRespawnPlayerGUI(instance)));
        public IEnumerable<Action> GetActionsForRespawnPlayer(RagdollController instance) => _gameModes.Select(mode => new Action(() => mode.OnRespawnPlayer(instance)));
        public IEnumerable<Action> GetActionsForLobbyMenuOptions(LobbyMenu instance) => _gameModes.Select(mode => new Action(() => mode.LobbyMenuOptions(instance)));
    }
}
using Il2Cpp;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using Il2CppCodeStage.AntiCheat.Storage;
using Il2CppExitGames.Client.Photon;

namespace CustomGameModesFramework.Patches
{
    public class AllSizes
    {
        public string sizeName;

		public List<string> playerCount;
    }

    [HarmonyPatch(typeof(RoomMultiplayerMenu), "LOHLFHPLKKD")]
    public static class AwakePatch
    {
        [HarmonyPostfix]
        public static void Postfix(RoomMultiplayerMenu __instance)
        {
            OnGUIPatch.IsTimerDisabled = __instance.KCIGKBNBPNN == "SUR" || __instance.KCIGKBNBPNN == "SBX" || 
                __instance.KCIGKBNBPNN == "THR";

            foreach (var action in GameModeManager.Instance.GetActionsForAwake(__instance))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"Error while performing action for Awake: {e.Message}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(RoomMultiplayerMenu), "INBHDPAHOGC")]
    public static class StartGamePatch
    {
        [HarmonyPostfix]
        public static void Postfix(RoomMultiplayerMenu __instance)
        {
            foreach (var action in GameModeManager.Instance.GetActionsForStartGame(__instance))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"Error while performing action for StartGame: {e.Message}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(RoomMultiplayerMenu), "OnGUI")]
    public static class OnGUIPatch
    {
        public static bool IsTimerDisabled = false;
        public static bool ShowLeadingPlayerText = false;
        public static bool ShowWaitingForPlayersText = false;
        public static string RoundEndedText = string.Empty;

        [HarmonyPrefix]
        public static bool Prefix(RoomMultiplayerMenu __instance)
        {
            GUI.skin = __instance.BIPMFIBNFBC;
            GUI.color = new Color(1f, 1f, 1f, 0.7f);
            float num = Mathf.CeilToInt(__instance.DIGCEKFFHPP);
            int seconds = Mathf.FloorToInt(num % 60f);
            int minutes = Mathf.FloorToInt(num / 60f % 60f);
            string gameMode = __instance.KCIGKBNBPNN;
            string timeText = $"{minutes:00}:{seconds:00}";

            GUIStyle guiStyle = GUI.skin.GetStyle("Label");
            int fontSize = Screen.height / 20;

            if (!ShowWaitingForPlayersText)
            {
                guiStyle.alignment = TextAnchor.MiddleCenter;

                DrawTextWithOutline(__instance, new Rect(0f, 45f, Screen.width, fontSize), 
                    __instance.OEOFGGDEFPP, (float)(fontSize / 1.5f), guiStyle);
            }
            else
            {
                if (!IsTimerDisabled)
                {
                    guiStyle.alignment = TextAnchor.MiddleCenter;

                    DrawTextWithOutline(__instance, new Rect(0f, 45f, Screen.width, fontSize), 
                        timeText, (float)(fontSize / 1.5f), guiStyle);
                }
            }

            if (ShowLeadingPlayerText && gameMode != "INF")
                HandleLeadingPlayerText(__instance, fontSize, guiStyle);

            if (gameMode == "INF")
                HandleInfectedGameMode(__instance, fontSize, guiStyle);

            if (gameMode == "SUR")
                HandleSurvivalGameMode(__instance, guiStyle);

            if (__instance.DAEFCOIPNCP)
                HandleRoundEnded(__instance, fontSize, guiStyle);

            ExecuteOnGUIActions(__instance);
            __instance?.LOMMCEFHELI();

            return false;
        }

        private static void DrawTextWithOutline(RoomMultiplayerMenu instance, Rect rect, string text, float textSize, GUIStyle guiStyle)
        {
            instance?.PFDDJOJEKHK(rect, $"<size={textSize}>{text}</size>", 1, guiStyle);
        }

        private static void HandleInfectedGameMode(RoomMultiplayerMenu instance, int fontSize, GUIStyle guiStyle)
        {
            ShowWaitingForPlayersText = true;

            if (!instance.AHHCEEGJKPB)  
                HandleLeadingPlayerText(instance, fontSize, guiStyle);

            // You are Infected Text
            if (PhotonNetwork.player.GetCustomPropertiesV2("TeamName")?.ToString() == instance.KGLOGDGOELM.teamName)
            {
                guiStyle.alignment = TextAnchor.LowerRight;

                GUI.Label(new Rect(0f, Screen.height - fontSize, Screen.width, fontSize),
                    $"<size={fontSize / 1.5f}><color=red>{instance.HCOOKIFOCIP}</color></size>", 
                guiStyle);

                GUI.color = Color.white;
            }

            // Fight Off Infected Text
            if (instance.BMKKAAIFFFI < 15f && 
                PhotonNetwork.player.GetCustomPropertiesV2("TeamName")?.ToString() == instance.OJPBAOICLJK.teamName)
            {
                guiStyle.alignment = TextAnchor.LowerRight;

                GUI.Label(new Rect(0f, Screen.height - fontSize - 2, Screen.width, fontSize),
                    $"<size={fontSize / 1.75f}><color=white>{instance?.EPHAFCFHNFE}</color></size>", 
                guiStyle);

                instance.BMKKAAIFFFI += Time.deltaTime;
            }
        }

        private static void HandleLeadingPlayerText(RoomMultiplayerMenu instance, int fontSize, GUIStyle guiStyle)
        {
            if (!instance.DAEFCOIPNCP && instance.HGKEEDKPGOD?.Length > 2)
            {
                GUI.color = Color.white;

                if (instance.HGKEEDKPGOD.EndsWith("|2"))
                    instance.HGKEEDKPGOD = instance.HGKEEDKPGOD.Split("|2")[0];

                DrawTextWithOutline(instance, new Rect(0f, 45 + fontSize, Screen.width, fontSize), 
                    $"{instance.HGKEEDKPGOD}{instance.POCPFDCEKIL}", (float)(fontSize / 1.75f), guiStyle);
            }
        }

        private static void HandleSurvivalGameMode(RoomMultiplayerMenu instance, GUIStyle guiStyle)
        {
            guiStyle.alignment = TextAnchor.MiddleLeft;

            // Toast Count Text
            DrawTextWithOutline(instance, new Rect(Screen.width - 80, Screen.height - 58, 70f, 48f), 
                ObscuredPrefs.GetInt("Toast", 0).ToString(), 20f, guiStyle);

            // Toast Icon
            GUI.DrawTexture(new Rect(Screen.width - 128, Screen.height - 58, 48f, 48f), instance.FLCFMCJMNCN);
        }

        private static void HandleRoundEnded(RoomMultiplayerMenu instance, int fontSize, GUIStyle guiStyle)
        {
            guiStyle.alignment = TextAnchor.MiddleCenter;

            if (!string.IsNullOrEmpty(instance.NJMDNCGLNHH) && string.IsNullOrEmpty(RoundEndedText))
                RoundEndedText = instance?.NJMDNCGLNHH;

            // Final Text
            DrawTextWithOutline(instance, new Rect(0f, Screen.height / 2, Screen.width, fontSize), 
                RoundEndedText, (float)(fontSize / 1.5f), guiStyle);

            // Restarting Text
            if (instance.KCIGKBNBPNN == "INF")
                DrawTextWithOutline(instance, new Rect(0f, Screen.height / 2 - fontSize, Screen.width, fontSize), 
                    instance.LOLBLDOFPPN, (float)(fontSize / 1.25f), guiStyle);
        }

        private static void ExecuteOnGUIActions(RoomMultiplayerMenu instance)
        {
            foreach (var action in GameModeManager.Instance.GetActionsForOnGUI(instance))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"Error while performing action for OnGUI: {e.Message}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(RoomMultiplayerMenu), "FixedUpdate")]
    public static class FixedUpdatePatch
    {
        [HarmonyPostfix]
        public static void Postfix(RoomMultiplayerMenu __instance)
        {
            if (PhotonNetwork.playerList.Count < 1)
                OnGUIPatch.ShowWaitingForPlayersText = false;

            foreach (var action in GameModeManager.Instance.GetActionsForFixedUpdate(__instance))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"Error while performing action for FixedUpdate: {e.Message}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(RoomMultiplayerMenu), "SpawnPlayer")]
    public static class SpawnPlayerPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(string teamName, RoomMultiplayerMenu __instance)
        {
            // If Player != null then destroy
            if (__instance.CNLHJAICIBH != null)
                PhotonNetwork.Destroy(__instance.CNLHJAICIBH);

            string gameMode = __instance.KCIGKBNBPNN;           // team1
            teamName = string.IsNullOrEmpty(teamName) ? __instance.OJPBAOICLJK.teamName : teamName;

            SetTeamNameInPhotonProperties(teamName);

            // If teamName equals team 1
            if (teamName == __instance.OJPBAOICLJK.teamName)
                HandleTeamOneSpawning(__instance, gameMode);
            else
                HandleTeamTwoSpawning(__instance, gameMode);

            ExecuteSpawnPlayerActions(__instance);

            __instance.DIAFJILHGPC.SetActive(false); // roomCamera

            return false;
        }

        private static void SetTeamNameInPhotonProperties(string teamName)
        {
            Hashtable hashtable = new();
            hashtable.Add("TeamName", teamName);
            PhotonNetwork.player.SetCustomPropertiesV2(hashtable, null, false);
        }

        private static void HandleTeamOneSpawning(RoomMultiplayerMenu instance, string gameMode)
        {
            if (gameMode == "SUR")
                ObscuredPrefs.SetInt("Toast", 200);
            
            if (gameMode == "INF")
                instance.BMKKAAIFFFI = 0f; // fightOffTime

            switch (gameMode)
            {
                case "COOP":
                case "VS":
                case "INF":
                case "SBX":
                case "SUR":
                    SpawnPrefab(instance, instance.BGOEEADMCBE.name, instance.OJPBAOICLJK);
                    break;
            }
        }

        private static void HandleTeamTwoSpawning(RoomMultiplayerMenu instance, string gameMode)
        {
            switch (gameMode)
            {
                case "VS":
                    SpawnPrefab(instance, "VS/" + instance.GetComponent<ClassicMechanics>().FKEIPFJBJHP, 
                        instance.KGLOGDGOELM);
                    break;
                case "INF":
                    SpawnPrefab(instance, "INF/PlayerNewborn", instance.KGLOGDGOELM);
                    break;
            }
        }

        private static void SpawnPrefab(RoomMultiplayerMenu instance, string prefabName, RoomMultiplayerMenu.AllTeams team)
        {
            int index = UnityEngine.Random.Range(0, team.spawnPoints.Length);
            instance.CNLHJAICIBH = PhotonNetwork.NOOU(prefabName, team.spawnPoints[index].position + Vector3.up, 
               team.spawnPoints[index].rotation, 0);
            instance.CNLHJAICIBH.name = PhotonNetwork.player.name;
        }

        private static void ExecuteSpawnPlayerActions(RoomMultiplayerMenu instance)
        {
            foreach (var action in GameModeManager.Instance.GetActionsForSpawnPlayer(instance))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"Error while performing action for SpawnPlayer: {e.Message}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(RoomMultiplayerMenu.ELNOHBJFICO), "MoveNext")]
    public static class RoundEndedPatch
    {
        [HarmonyPostfix]
        public static void Postfix(RoomMultiplayerMenu __instance)
        {
            if (__instance == null)
                return;

            foreach (var action in GameModeManager.Instance.GetActionsForRoundEnded(__instance))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"Error while performing action for RoundEnded: {e.Message}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(RoomMultiplayerMenu.PMJNPNPLPHA), "MoveNext")]
    public static class RestartPatch
    {
        [HarmonyPostfix]
        public static void Postfix(RoomMultiplayerMenu __instance)
        {
            if (__instance == null)
                return;

            foreach (var action in GameModeManager.Instance.GetActionsForRestart(__instance))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"Error while performing action for Restart: {e.Message}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(RagdollController), "OnGUI")]
    public static class RespawnPlayerGUIPatch
    {
        public static bool ShowRespawnText = true;

        [HarmonyPrefix]
        public static bool Prefix(RagdollController __instance)
        {
            if (Camera.main)
            {
                UnityEngine.Object.Destroy(__instance.JKBNHPGGIDE);
                UnityEngine.Object.Destroy(__instance);
            }

            if (__instance.KCIGKBNBPNN != "SUR" && __instance.OGLEOGIHGLH && __instance.JKBNHPGGIDE)
            {
                GUI.skin = __instance.LCBGEPHMBMK;

                foreach (var action in GameModeManager.Instance.GetActionsForRespawnPlayerGUI(__instance))
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Warning($"Error while performing action for RespawnPlayerGUI: {e.Message}");
                    }
                }

                string labelText = PhotonNetwork.offlineMode && __instance.KCIGKBNBPNN != "SBX"
                    ? __instance.AHOHGGNPBLG
                    : $"{__instance.AHOHGGNPBLG}: {__instance.NADEDDKIIJL}";

                if (ShowRespawnText)
                    GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 15, 150f, 30f), labelText);

                if (__instance.KCIGKBNBPNN == "INF" && ShowRespawnText)
                    GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 45, 150f, 30f), $"<color=red>{__instance.DKJLDNCOANF}</color>");
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(RagdollController), "BLACJABKHFN")]
    public static class RespawnPlayerPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(RagdollController __instance)
        {
            if (--__instance.NADEDDKIIJL == 0)
            {
                UnityEngine.Object.Destroy(__instance?.JKBNHPGGIDE);

                string gameMode = __instance?.KCIGKBNBPNN;
                string teamName = GetTeamName(gameMode);

                if (PhotonNetwork.offlineMode && IsGameModeWithDisconnect(gameMode))
                    PhotonNetwork.Disconnect();

                SpawnPlayer(gameMode, teamName);
                ExecuteRespawnPlayerActions(__instance);
                UnityEngine.Object.Destroy(__instance);
            }

            return false;
        }

        private static string GetTeamName(string gameMode)
        {
            string teamName = PhotonNetwork.player.GetCustomPropertiesV2("TeamName").ToString();

            if (gameMode == "INF")
                teamName = GameObject.FindWithTag("Network").GetComponent<RoomMultiplayerMenu>().KGLOGDGOELM.teamName;

            return teamName;
        }

        private static bool IsGameModeWithDisconnect(string gameMode)
        {
            return gameMode switch
            {
                "INF" or "SUR" or "COOP" or "VS" => true,
                _ => false
            };
        }

        private static void SpawnPlayer(string gameMode, string teamName)
        {
            switch (gameMode)
            {
                case "SBX":
                    GameObject.FindWithTag("Network").SendMessage("RespawnPlayer2");
                    break;
                case "INF":
                case "SUR":
                case "COOP":
                case "VS":
                    GameObject.FindWithTag("Network").SendMessage("SpawnPlayer", teamName);
                    break;
            }
        }

        private static void ExecuteRespawnPlayerActions(RagdollController instance)
        {
            foreach (var action in GameModeManager.Instance.GetActionsForRespawnPlayer(instance))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"Error while performing action for RespawnPlayer: {e.Message}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerNetworkController), "Update")]
    public static class FriendlyFirePatch
    {
        public static bool IsFriendlyFireDisabled = true;

        [HarmonyPostfix]
        public static void Postfix(PlayerNetworkController __instance)
        {
            if ((bool)!__instance?.photonView?.isMine)
                HandleFriendlyFire(__instance, IsFriendlyFireDisabled);
        }

        private static void HandleFriendlyFire(PlayerNetworkController instance, bool isDisabled)
        {
            if (isDisabled)
            {
                instance.AHCJMNLNOGI.enabled = true;
                instance.FJEJDPCPOJC.IACKFCDKMLP = true;
            }
            else
            {
                HitBox hitBox = instance.gameObject.GetComponent<HitBox>();

                instance.FJEJDPCPOJC.IACKFCDKMLP = false;                
                instance.AHCJMNLNOGI.enabled = false;

                if (hitBox == null)
                {
                    instance.gameObject.AddComponent<HitBox>();
                    hitBox = instance.gameObject.GetComponent<HitBox>();
                    hitBox.FJEJDPCPOJC = instance.FJEJDPCPOJC;
                    hitBox.DHGDMKPLBJH = 10f;
                }
            }
        }
    }

    [HarmonyPatch(typeof(LobbyMenu), "Start")]
    public static class LobbyMenuStartPatch
    {
        [HarmonyPrefix]
        public static void Prefix(LobbyMenu __instance)
        {
            MigrateAllSizes(__instance);
        }

        [HarmonyPostfix]
        public static void Postfix(LobbyMenu __instance)
        {
            foreach (var gameModeInfo in GameModeManager.Instance.GameModeInfos)
            {
                try
                {
                    AddGameMode(__instance, gameModeInfo);
                }
                catch (Exception ex)
                {
                    MelonLogger.Msg($"Error while performing AddGameMode for LobbyMenuStart: {ex}");
                }
            }
        }

        private static void MigrateAllSizes(LobbyMenu instance)
        {
            foreach (var oldSize in instance.NMEONIIJMNO)
            {
                var newSize = new AllSizes
                {
                    sizeName = oldSize.sizeName,
                    playerCount = [.. oldSize.playerCount]
                };

                LobbyMenuOptionsPatch.AllSizes.Add(newSize);
            }
        }

        private static void AddGameMode(LobbyMenu instance, GameModeInfo gameModeInfo)
        {
            LobbyMenu.AllModes gameMode = new()
            {
                modeID = gameModeInfo.ModeID,
                modeName = gameModeInfo.ModeName,
                players = gameModeInfo.Players
            };

            instance.MHDLCNKEEGN.Add(gameMode);
            AddMapSizes(instance, gameModeInfo);
        }

        private static void AddMapSizes(LobbyMenu instance, GameModeInfo gameModeInfo)
        {
            CreateMapSize(instance, "Small", gameModeInfo.SmallMapPlayerCount);
            CreateMapSize(instance, "Medium", gameModeInfo.MediumMapPlayerCount);
            CreateMapSize(instance, "Large", gameModeInfo.LargeMapPlayerCount);
        }

        private static void CreateMapSize(LobbyMenu instance, string sizeName, string playerCount)
        {
            for (int i = 0; i < instance.IILJODEKLIC.Count; i++)
            {
                if (LobbyMenuOptionsPatch.AllSizes[instance.IILJODEKLIC[i].size].sizeName != sizeName)
                    continue;

                LobbyMenuOptionsPatch.AllSizes[instance.IILJODEKLIC[i].size].playerCount.Add(playerCount);
            }
        }
    }

    [HarmonyPatch(typeof(LobbyMenu), "AJKAOGDBDMH")]
	public static class LobbyMenuOptionsPatch
	{
        public static bool IsTimerOptionDisabled = false;
        public static List<AllSizes> AllSizes = [];

        [HarmonyPrefix]
        private static bool Prefix(LobbyMenu __instance)
        {
            int num = Screen.height / 17;
            int num2 = Screen.width / 17;
            int num3 = Screen.height / 17;
            int fontSize = (int)(num3 / 1.05f);
            GUI.skin = __instance.BIPMFIBNFBC;

            DrawMapButtons(__instance, num, fontSize);

            GUI.color = Color.white;

            DrawBackButton(__instance, num, num2);
            DrawGameSettingsBox(__instance, num);
            DrawModeOptions(__instance, num);
            DrawMultiplayerOptions(__instance, num, num2);

            if (GUI.Button(new Rect(Screen.width - num2 * 6, Screen.height - num * 1.5f, num2 * 5, num), 
                GetStyledText(__instance.DJKFLNNGFOO, num / 1.25f), __instance.BIPMFIBNFBC.customStyles[3]))
            {
                CreateRoom(__instance);
            }

            __instance.MEDMJPKCBGG();
            DrawS3MapsButton(__instance, num, num2);
            DrawS2MapsButton(__instance, num, num2);

            return false;
        }

        private static void DrawMapButtons(LobbyMenu instance, int num, int fontSize)
        {
            for (int i = 0; i < instance.IILJODEKLIC.Count; i++)
            {
                GUI.color = i == instance.MCAINIELPCK ? Color.red : Color.white;

                if (i == instance.MCAINIELPCK)
                    instance.ENLNIIAJNOC.sprite = instance.IILJODEKLIC[instance.MCAINIELPCK].mapPreview;

                int row = i >= 10 ? (int)(num * 4f) : 0;
                int column = i >= 10 ? i - 10 : i;

                if (GUI.Button(new Rect(num * 1.25f + row, num * 4.7f + fontSize * column, num * 5f, fontSize), 
                    GetStyledText(instance.IILJODEKLIC[i].mapName, num / 2), instance.BIPMFIBNFBC.customStyles[0]))
                {
                    instance.MCAINIELPCK = i;
                }
            }
        }

        private static void DrawBackButton(LobbyMenu instance, int num, int num2)
        {
            if (GUI.Button(new Rect(num2, Screen.height - num * 1.5f, Screen.width / 4, num), 
                GetStyledText(instance.PJIAFMHOEDL, num / 1.25f), instance.BIPMFIBNFBC.customStyles[0]))
            {
                instance.FANLKBJODCL = false;

                if (PhotonNetwork.isOfflineMode)
                {
                    if (PhotonNetwork.connected)
                        PhotonNetwork.Disconnect();

                    instance.CPFFBHEDEPF.ReturnToMenu();
                }
            }
        }

        private static void DrawGameSettingsBox(LobbyMenu instance, int num)
        {
            GUIStyle guiStyle = instance.BIPMFIBNFBC.customStyles[1];

            GUI.Box(new Rect(Screen.width - num * 13, Screen.height - num * 15, num * 11, num * 13), 
                GetStyledText(instance.LPFLICFGDCN, num / 2));

            GUI.Label(new Rect(Screen.width - num * 13, Screen.height - num * 14, num * 11, num),
                GetMapSizeText(instance, num), guiStyle);

            GUI.Label(new Rect(Screen.width - num * 13, Screen.height - num * 13, num * 11, num), 
                GetRecommendedPlayersText(instance, num), guiStyle);
        }

        private static void DrawModeOptions(LobbyMenu instance, int num)
        {
            GUIStyle guiStyle = instance.BIPMFIBNFBC.customStyles[2];

            if (!instance.EGDMPMDBJIK)
            {
                DrawTimerOptions(instance, num, guiStyle);
                DrawCustardAmountOptions(instance, num, guiStyle);
                DrawDifficultyOptions(instance, num, guiStyle);
                ExecuteLobbyMenuOptionsActions(instance);
                DrawMuteGuestToggle(instance, num);
            }
        }

        private static void DrawTimerOptions(LobbyMenu instance, int num, GUIStyle guiStyle)
        {
            if (instance.NNKFEAPBLCK != 2 && instance.NNKFEAPBLCK != 4 && !IsTimerOptionDisabled)
            {
                int maxTimer = instance.NNKFEAPBLCK == 3 ? 9 : 25;

                if (instance.NADEDDKIIJL > 9 && maxTimer == 9)
                    instance.NADEDDKIIJL = 5;

                GUI.Label(new Rect(Screen.width - num * 13, Screen.height - num * 8, num * 11, num), 
                    GetStyledText($"{instance.MGMIKBHOHCD}: {instance.NADEDDKIIJL}{instance.JCNBBNDKBPE}", num / 1.75f), guiStyle);

                instance.NADEDDKIIJL = (int)GUI.HorizontalSlider(new Rect(Screen.width - num * 12, Screen.height - num * 7, num * 9, num), 
                    (float)instance.NADEDDKIIJL, 5f, maxTimer);
            }
        }

        private static void DrawCustardAmountOptions(LobbyMenu instance, int num, GUIStyle guiStyle)
        {
            if (instance.NNKFEAPBLCK < 2)
            {
                GUI.Label(new Rect(Screen.width - num * 13, Screen.height - num * 6, num * 11, num), 
                    GetStyledText($"{instance.LCADCNGODCG}: {instance.OJKBDFPLNNG}", num / 1.75f), guiStyle);

                instance.OJKBDFPLNNG = (int)GUI.HorizontalSlider(new Rect(Screen.width - num * 12, Screen.height - num * 5,
                    num * 9, num), (float)instance.OJKBDFPLNNG, 5f, 25f);
            }
        }

        private static void DrawDifficultyOptions(LobbyMenu instance, int num, GUIStyle guiStyle)
        {
            if (instance.NNKFEAPBLCK == 2 || instance.NNKFEAPBLCK == 4)
            {
                string[] difficulties = ["(Easy)", "☠ (Normal)", "☠☠ (Hard)", "☠☠☠ (Impossible)"];

                GUI.Label(new Rect(Screen.width - num * 13, Screen.height - num * 6, num * 11, num), 
                    GetStyledText($"Difficulty: {difficulties[instance.LBOEJBAJEGJ]}", num / 1.75f), guiStyle);

                instance.LBOEJBAJEGJ = (int)GUI.HorizontalSlider(new Rect(Screen.width - num * 12, Screen.height - num * 5,
                    num * 9, num), instance.LBOEJBAJEGJ, 0f, 3f);
            }
        }

        private static void ExecuteLobbyMenuOptionsActions(LobbyMenu instance)
        {
            foreach (var action in GameModeManager.Instance.GetActionsForLobbyMenuOptions(instance))
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"Error while performing action for LobbyMenuOptions: {e.Message}");
                }
            }
        }

        private static void DrawMuteGuestToggle(LobbyMenu instance, int num)
        {
            if (!PhotonNetwork.offlineMode)
            {
                string toggleText = instance.NPBLIJCOGKM ? "<color=green>☑</color>" : "<color=grey>☐</color>";

                instance.NPBLIJCOGKM = GUI.Toggle(new Rect(Screen.width - num * 13, Screen.height - num * 3,
                    num * 9, num), instance.NPBLIJCOGKM, GetStyledText($"{toggleText} {instance.FEKLHMMHODK}", num / 1.75f));
            }
        }

        private static void DrawMultiplayerOptions(LobbyMenu instance, int num, int num2)
        {
            if (!PhotonNetwork.offlineMode)
            {
                DrawPrivateRoomToggle(instance, num, num2);
                DrawRoomNameOptions(instance, num, num2);
            }
        }

        private static void DrawPrivateRoomToggle(LobbyMenu instance, int num, int num2)
        {
            string toggleText = instance.ILKABAHDHBK ? "<color=green>☑</color>" : "☐";

            instance.ILKABAHDHBK = GUI.Toggle(new Rect(num2 + num * 7, num * 2, num * 5, num),
                instance.ILKABAHDHBK, string.Concat(
                [
                    "<size=",
                    (num / 1.5f),
                    ">",
                    toggleText,
                    string.Empty,
                    instance.NDEHPFDBJGF,
                    "</size>"
                ]));

            ObscuredPrefs.SetBool("Private", instance.ILKABAHDHBK);
        }

        private static void DrawRoomNameOptions(LobbyMenu instance, int num, int num2)
        {
            GUI.Label(new Rect(num2, num, num * 6, num), GetStyledText(instance.KJBOAFHJBEH, num / 1.5f),
                instance.BIPMFIBNFBC.customStyles[0]);

            GUIStyle guiStyle = GUI.skin.GetStyle("TextField");
            guiStyle.fontSize = (int)(num / 1.5f);

            instance.JEGIAEANOII = GUI.TextField(new Rect(num2, num * 2, num * 7, num), instance.JEGIAEANOII,
                30, guiStyle);
        }

        private static void CreateRoom(LobbyMenu instance)
        {
            Il2CppSystem.Int32 difficulty = new() { m_value = instance.LBOEJBAJEGJ };
            Il2CppSystem.Int32 custards = new() { m_value = instance.OJKBDFPLNNG };
            Il2CppSystem.Int32 timer = new() { m_value = instance.NADEDDKIIJL * 60 };
            Il2CppSystem.Int32 muteGuest = new() { m_value = instance.NPBLIJCOGKM ? 1 : 0 };
            Il2CppSystem.Single refTime = new() { m_value = (float)instance.NADEDDKIIJL * 60f };
            Il2CppSystem.Int32 team1Score = new() { m_value = 0 };
            Il2CppSystem.Int32 team2Score = new() { m_value = 0 };

            Hashtable customRoomProperties = new();
            customRoomProperties["MN002'"] = GetMapName(instance);
            customRoomProperties["RD004'"] = timer.BoxIl2CppObject();
            customRoomProperties["GM001'"] = instance.MHDLCNKEEGN[instance.NNKFEAPBLCK].modeID;
            customRoomProperties["Custards"] = custards.BoxIl2CppObject();
            customRoomProperties["DY003'"] = difficulty.BoxIl2CppObject();
            customRoomProperties["RefTime"] = refTime.BoxIl2CppObject();
            customRoomProperties["Team1Score"] = team1Score.BoxIl2CppObject();
            customRoomProperties["Team2Score"] = team2Score.BoxIl2CppObject();
            customRoomProperties["MG"] = muteGuest.BoxIl2CppObject();

            string[] customRoomPropertiesForLobby = ["MN002'", "DY003'", "GM001'"];
            string roomName = GetRoomName(instance);

            RoomOptions roomOptions = new() 
            {
                IsOpen = true,
                IsVisible = !instance.ILKABAHDHBK,
                MaxPlayers = byte.Parse(instance.MHDLCNKEEGN[instance.NNKFEAPBLCK].players),
                customRoomProperties = customRoomProperties,
                customRoomPropertiesForLobby = customRoomPropertiesForLobby
            };

            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        }

        private static void DrawS3MapsButton(LobbyMenu instance, int num, int num2)
        {
            GUI.backgroundColor = instance.AEIEMBGPDFA == 0 ? Color.red : Color.white;

            if (GUI.Button(new Rect(num2, (float)(num * 3f), (float)(num * 3f), num), "<size=42>S3 MAPS</size>",
                instance.BIPMFIBNFBC.customStyles[6]))
            {
                instance.AEIEMBGPDFA = 0;
                instance.JHENFCKJNMJ();
            }
        }

        private static void DrawS2MapsButton(LobbyMenu instance, int num, int num2)
        {
            GUI.backgroundColor = instance.AEIEMBGPDFA == 1 ? Color.red : Color.white;

            if (GUI.Button(new Rect((float)(num2 * 2.69f), (float)(num * 3f), (float)(num * 3f), num), "<size=42>S2 MAPS</size>", 
                instance.BIPMFIBNFBC.customStyles[6]))
            {
                instance.AEIEMBGPDFA = 1;
                instance.JHENFCKJNMJ();
            }
        }

        private static string GetStyledText(string text, float size)
        {
            return $"<size={size}>{text}</size>";
        }

        private static string GetMapSizeText(LobbyMenu instance, int num)
        {
            return GetStyledText($"{instance.IIFDPOOCJID}: " + 
                $"{instance.NMEONIIJMNO[instance.IILJODEKLIC[instance.MCAINIELPCK].size].sizeName}", 
            num / 1.75f);
        }

        private static string GetRecommendedPlayersText(LobbyMenu instance, int num)
        {
            string text = GetStyledText($"{instance.DLJAALKHONJ}: " +
                $"{AllSizes[instance.IILJODEKLIC[instance.MCAINIELPCK].size].playerCount[instance.NNKFEAPBLCK]}", 
            num / 1.75f);

            return text;
        }

        private static string GetMapName(LobbyMenu instance)
        {
            string mapName = instance.IILJODEKLIC[instance.MCAINIELPCK].mapName;

            if (instance.IILJODEKLIC[instance.MCAINIELPCK].useDayAndNight)
            {
                string timeSuffix = instance.GGMDFHLODMF ? " (Night)" : instance.NLAEKGFANJA ? " (Dusk)" : " (Day)";
                mapName += timeSuffix;
            }

            return mapName;
        }

        private static string GetRoomName(LobbyMenu instance)
        {
            if (string.IsNullOrEmpty(instance.JEGIAEANOII))
                instance.JEGIAEANOII = $"Room {UnityEngine.Random.Range(0, 999)}";

            return instance.ILKABAHDHBK ? instance.JEGIAEANOII : 
                $"{instance.JEGIAEANOII}|" + 
                $"{GetMapName(instance)}|" + 
                $"{instance.NADEDDKIIJL}|" + 
                $"{instance.MHDLCNKEEGN[instance.NNKFEAPBLCK].modeID}|" + 
                $"{instance.OJKBDFPLNNG}|" +
                $"{instance.LBOEJBAJEGJ}|" + 
                $"{instance.MHDLCNKEEGN[instance.NNKFEAPBLCK].players.ToString()}";
        }
    }
}
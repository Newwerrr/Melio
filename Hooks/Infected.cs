using GorillaGameModes;
using GorillaTagScripts;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Melio.Hooks
{
    public class Hooks
    {
        private static MethodInfo TagManagerChangeItMethod;
        private static MethodInfo AmbushManagerChangeItMethod;

        static Hooks()
        {
            TagManagerChangeItMethod = typeof(GorillaTagManager).GetMethod(
                "ChangeCurrentIt",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                new Type[] { typeof(NetPlayer), typeof(bool) },
                null
            );

            AmbushManagerChangeItMethod = typeof(GorillaAmbushManager).GetMethod(
                "ChangeCurrentIt",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                new Type[] { typeof(NetPlayer), typeof(bool) },
                null
            );
        }

        public static List<NetPlayer> InfectedList()
        {
            List<NetPlayer> infected = new List<NetPlayer>();

            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return infected;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    if (GorillaGameManager.instance is GorillaTagManager tagManager)
                    {
                        if (tagManager.isCurrentlyTag)
                        {
                            if (tagManager.currentIt != null)
                                infected.Add(tagManager.currentIt);
                        }
                        else if (tagManager.currentInfected != null)
                        {
                            infected.AddRange(tagManager.currentInfected.Where(element => element != null));
                        }
                    }
                    break;

                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    if (GorillaGameManager.instance is GorillaAmbushManager ghostManager)
                    {
                        if (ghostManager.isCurrentlyTag)
                        {
                            if (ghostManager.currentIt != null)
                                infected.Add(ghostManager.currentIt);
                        }
                        else if (ghostManager.currentInfected != null)
                        {
                            infected.AddRange(ghostManager.currentInfected.Where(element => element != null));
                        }
                    }
                    break;

                case GameModeType.Paintbrawl:
                    if (GorillaGameManager.instance is GorillaPaintbrawlManager paintbrawlManager && paintbrawlManager.playerLives != null)
                    {
                        infected.AddRange(
                            paintbrawlManager.playerLives
                                .Where(element => element.Value <= 0)
                                .Select(element => PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(element.Key))
                                .Where(dummy => dummy != null)
                                .Select(dummy => (NetPlayer)dummy)
                        );

                        if (NetworkSystem.Instance?.LocalPlayer != null && !infected.Contains(NetworkSystem.Instance.LocalPlayer))
                            infected.Add(NetworkSystem.Instance.LocalPlayer);
                    }
                    break;
            }

            return infected;
        }

        public static void AddInfected(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null || plr == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    if (GorillaGameManager.instance is GorillaTagManager tagManager)
                    {
                        if (tagManager.isCurrentlyTag)
                            TagManagerChangeItMethod?.Invoke(tagManager, new object[] { plr, true });
                        else if (tagManager.currentInfected != null && !tagManager.currentInfected.Contains(plr))
                            tagManager.AddInfectedPlayer(plr);
                    }
                    break;

                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    if (GorillaGameManager.instance is GorillaAmbushManager ghostManager)
                    {
                        if (ghostManager.isCurrentlyTag)
                            AmbushManagerChangeItMethod?.Invoke(ghostManager, new object[] { plr, true });
                        else if (ghostManager.currentInfected != null && !ghostManager.currentInfected.Contains(plr))
                            ghostManager.AddInfectedPlayer(plr);
                    }
                    break;

                case GameModeType.Paintbrawl:
                    if (GorillaGameManager.instance is GorillaPaintbrawlManager paintbrawlManager && paintbrawlManager.playerLives != null)
                        paintbrawlManager.playerLives[plr.ActorNumber] = 0;
                    break;
            }
        }

        public static void RemoveInfected(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null || plr == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    if (GorillaGameManager.instance is GorillaTagManager tagManager)
                    {
                        switch (tagManager.isCurrentlyTag)
                        {
                            case true when tagManager.currentIt == plr:
                                TagManagerChangeItMethod?.Invoke(tagManager, new object[] { null, true });
                                break;
                            case false when tagManager.currentInfected != null && tagManager.currentInfected.Contains(plr):
                                tagManager.currentInfected.Remove(plr);
                                break;
                        }
                    }
                    break;

                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    if (GorillaGameManager.instance is GorillaAmbushManager ghostManager)
                    {
                        switch (ghostManager.isCurrentlyTag)
                        {
                            case true when ghostManager.currentIt == plr:
                                AmbushManagerChangeItMethod?.Invoke(ghostManager, new object[] { null, true });
                                break;
                            case false when ghostManager.currentInfected != null && ghostManager.currentInfected.Contains(plr):
                                ghostManager.currentInfected.Remove(plr);
                                break;
                        }
                    }
                    break;

                case GameModeType.Paintbrawl:
                    if (GorillaGameManager.instance is GorillaPaintbrawlManager paintbrawlManager && paintbrawlManager.playerLives != null)
                        paintbrawlManager.playerLives[plr.ActorNumber] = 3;
                    break;
            }
        }

        public static void AddRock(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null || plr == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    if (GorillaGameManager.instance is GorillaTagManager tagManager)
                        TagManagerChangeItMethod?.Invoke(tagManager, new object[] { plr, true });
                    break;

                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    if (GorillaGameManager.instance is GorillaAmbushManager ghostManager)
                        AmbushManagerChangeItMethod?.Invoke(ghostManager, new object[] { plr, true });
                    break;

                case GameModeType.Paintbrawl:
                    if (GorillaGameManager.instance is GorillaPaintbrawlManager paintbrawlManager && paintbrawlManager.playerLives != null)
                        paintbrawlManager.playerLives[plr.ActorNumber] = 0;
                    break;
            }
        }

        public static void RemoveRock(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null || plr == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    if (GorillaGameManager.instance is GorillaTagManager tagManager && tagManager.currentIt == plr)
                        TagManagerChangeItMethod?.Invoke(tagManager, new object[] { null, true });
                    break;

                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    if (GorillaGameManager.instance is GorillaAmbushManager ghostManager && ghostManager.currentIt == plr)
                        AmbushManagerChangeItMethod?.Invoke(ghostManager, new object[] { null, true });
                    break;

                case GameModeType.Paintbrawl:
                    if (GorillaGameManager.instance is GorillaPaintbrawlManager paintbrawlManager && paintbrawlManager.playerLives != null)
                        paintbrawlManager.playerLives[plr.ActorNumber] = 3;
                    break;
            }
        }
    }
}
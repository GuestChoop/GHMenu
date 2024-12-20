using HarmonyLib;
using Steamworks;

public class NetworkUtilities
{
    public static string GetSteamLocalName()
    {
        return SteamFriends.GetPersonaName();
    }

    public static string GetFriendsName(int index)
    {
        P2PSession p2psession = P2PSession.Instance;
        var p2peer = p2psession.m_RemotePeers;

        if (index < 0 || index > p2peer.Count - 1)
        {
            return "Not founded name in index";
        }

        var p2paddress = p2peer[index].m_Address as P2PAddressSteam;
        var steamID = p2paddress.m_SteamID;
        return SteamFriends.GetFriendPersonaName(steamID);
    }

    public static CSteamID GetSteamID(string nameplayer)
    {
        CSteamID steamID = default;
        ReplicatedLogicalPlayer[] logicalnetwork = ReplicatedLogicalPlayer.s_AllLogicalPlayers.ToArray();
        for (int i = 0; i < logicalnetwork.Length; i++)
        {
            var P2Pplayer = logicalnetwork[i].ReplGetOwner();
            if (nameplayer.ToLower() == P2Pplayer.GetDisplayName().ToLower())
            {
                var p2paddress = P2Pplayer.m_Address as P2PAddressSteam;
                steamID = p2paddress.m_SteamID;
                break;
            }
        }
        return steamID;
    }

    public static P2PConnection GetP2PConnection(P2PPeer p2p)
    {
        P2PSession p2psession = P2PSession.Instance;
        return Traverse.Create(p2psession).Method("FindConnection", p2p).GetValue() as P2PConnection;
    }

    public static P2PLobbyMemberInfo GetMemberInfo(int index)
    {
        return P2PTransportLayer.Instance.GetCurrentLobbyMembers()[index];
    }
}


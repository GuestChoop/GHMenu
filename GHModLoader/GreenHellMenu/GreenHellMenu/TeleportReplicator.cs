using System.Linq;
using UnityEngine;

public class TeleportReplicator : ReplicatedBehaviour
{
    private static TeleportReplicator s_Instance;
    private Vector3 m_Position;
    private Quaternion m_Rotation;
    //private short id;
    private void Awake()
    {
        s_Instance = this;
    }
    public static TeleportReplicator Get()
    {
        return s_Instance;
    }
    public override void OnReplicationSerialize(P2PNetworkWriter writer, bool initial_state)
    {
        base.OnReplicationSerialize(writer, initial_state);
        writer.Write(P2PSession.Instance.m_RemotePeers.First().GetHostId());
        writer.Write(m_Position);
        writer.Write(m_Rotation);
    }
    public override void OnReplicationDeserialize(P2PNetworkReader reader, bool initial_state)
    {
        base.OnReplicationDeserialize(reader, initial_state);
        short id = reader.ReadInt16();
        m_Position = reader.ReadVector3();
        m_Rotation = reader.ReadQuaternion();
        if (P2PSession.Instance.LocalPeer.GetHostId() == id)
        {
            Player.Get().TeleportTo(m_Position, m_Rotation, false);
            //HNotify.Get().AddNotification(HNotify.NotificationType.scaling, "You have been summoned by host", 2, HNotify.CheckSprite);
        }
    }
    public void OnTeleportTarget(Vector3 pos, Quaternion rot)
    {
        m_Position = pos;
        m_Rotation = rot;
        if (!ReplIsOwner())
        {
            ReplRequestOwnership();
        }
        this.ReplSetDirty();
    }
}

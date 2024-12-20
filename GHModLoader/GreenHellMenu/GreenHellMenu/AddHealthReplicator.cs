using System.Linq;

public class AddHealthReplicator : ReplicatedBehaviour
{
    private static AddHealthReplicator s_Instance;
    private float m_health;
    //private short playerneedHeal;
    //private float m_maxHealth;
    //private float m_Health_Repl;
    //private float m_maxHealth_Repl;
    private void Awake()
    {
        s_Instance = this;
    }
    public static AddHealthReplicator Get()
    {
        return s_Instance;
    }
    public override void OnReplicationSerialize(P2PNetworkWriter writer, bool initial_state)
    {
        base.OnReplicationSerialize(writer, initial_state);
        //writer.Write(m_maxHealth);
        writer.Write(P2PSession.Instance.m_RemotePeers.First().GetHostId());
        writer.Write(m_health);
        //writer.Write(m_Health_Repl);
        //writer.Write(m_maxHealth_Repl);
    }
    public override void OnReplicationDeserialize(P2PNetworkReader reader, bool initial_state)
    {
        base.OnReplicationDeserialize(reader, initial_state);
        //float maxHP = reader.ReadFloat();
        short playerneedHeal = reader.ReadInt16();
        m_health = reader.ReadFloat();
        if (P2PSession.Instance.LocalPeer.GetHostId() == playerneedHeal)
        {
            PlayerConditionModule.Get().m_HP += PlayerConditionModule.Get().GetMaxHP() - m_health;
            //HNotify.Get().AddNotification(HNotify.NotificationType.scaling, "You have been heal by host", 2, HNotify.CheckSprite);
        }

    }
    //public override void OnReplicationPrepare()
    //{
    //    base.OnReplicationPrepare();
    //    m_Health_Repl = m_health;
    //    m_maxHealth_Repl = m_maxHealth;
    //    this.ReplSetDirty();
    //}
    public void OnAddHealth(float hp)
    {
        m_health = hp;
        if (!ReplIsOwner())
        {
            ReplRequestOwnership();
        }
        this.ReplSetDirty();
    }
}

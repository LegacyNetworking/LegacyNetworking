namespace LegacyNetworking
{
    public enum NetworkTags {
        ServerId = -1,
    }
    public enum NetworkMessages : ushort {
        SceneMessage,
        SpawnMessage,
        RpcMessage,
        StreamMessage,
        ChangeOwnerMessage,
        RequestOwnerMessage,
    }
    public enum NetworkTarget : byte {
        All = 0,
        Others,
        Server,
        Targeted,
    }
}

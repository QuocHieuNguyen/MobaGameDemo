using Colyseus.Schema;

public partial class GameRoomState : Schema
{
    [Type(0, "map", typeof(MapSchema<Player>))]
    public MapSchema<Player> players = new MapSchema<Player>();
}

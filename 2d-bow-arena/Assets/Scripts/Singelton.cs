using System.Collections.Generic;

public class Singelton
{
    private static List<Player> players = new List<Player>();

    public static void AddPlayer(Player _player)
    {
        players.Add(_player);
    }

    public static void RemovePlayer(Player _player)
    {
        players.Remove(_player);
    }

    public static List<Player> GetPlayers()
    {
        return players;
    }
}

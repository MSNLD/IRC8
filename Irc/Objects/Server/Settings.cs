using System.Transactions;

namespace Irc.Objects.Server;

public class Settings
{
    public string Id { get; set; } = "DefaultServerId";
    public string Name { get; set; } = "DefaultServerName";
    public string Title { get; set; } = "Default Chat Service";
    public DateTime Creation = DateTime.Now;
    public string[] Motd { get; set; } = new string[] { "*** Welcome to the Server ***" };
    public string[] Packages { get; set; } = new string[] { "ANON", "GateKeeper", "NTLM" };
    public string PassportKey { get; set; } = "mysecret";
    public string PassportAppId { get; set; } = "myappid";
    public string PassportAppSecret { get; set; } = "mysecret";
    public string WebIrcUser { get; set; } = "user";
    public string WebIrcPassword { get; set; } = "password";
    public string WebIrcIp { get; set; } = "::ffff:127.0.0.1";
    public int MaxInputBytes { get; set; } = 1024;
    public int MaxOutputBytes { get; set; } = 4096;
    public int PingInterval { get; set; } = 60;
    public int PingAttempts { get; set; } = 2;
    public int MaxChannels { get; set; } = 1;
    public int MaxConnections { get; set; } = 10000;
    public int MaxAuthenticatedConnections { get; set; } = 1000;
    public int MaxAnonymousConnections { get; set; } = 100;
    public int MaxGuestConnections { get; set; } = 100;
    public bool BasicAuthentication { get; set; } = true;
    public bool AnonymousConnections { get; set; } = true;
    public string AdminInfo1 { get; set; } = "Your Admin is";
    public string AdminInfo2 { get; set; } = "amazing";
    public string AdminInfo3 { get; set; } = "example@domain.com";
}
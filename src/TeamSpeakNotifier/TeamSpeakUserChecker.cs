using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentScheduler;

using TeamSpeak3QueryApi.Net.Specialized;
using TeamSpeak3QueryApi.Net.Specialized.Responses;

using TeamSpeakNotifier.Senders;

namespace TeamSpeakNotifier
{
    public class TeamSpeakUserChecker : IJob
    {
        private readonly List<GetClientInfo> _clients;
        private readonly string _password;
        private readonly string _serverIp;
        private readonly int _serverPort;
        private readonly AppSettings _settings;
        private readonly string _username;

        private TeamSpeakClient _tsClientConnection;

        public TeamSpeakUserChecker(AppSettings settings)
        {
            _settings = settings;

            _username = settings.TeamSpeakUserName;
            _password = settings.TeamSpeakPassword;

            _serverPort = settings.TeamSpeakServerPort;
            _serverIp = settings.TeamSpeakServerIp;

            _tsClientConnection = CreateConnection().Result;

            _clients = new List<GetClientInfo>();
        }

        public void Execute()
        {
            ExecuteAsync().GetAwaiter().GetResult();
        }

        private async Task<TeamSpeakClient> CreateConnection()
        {
            try
            {
                var conn = new TeamSpeakClient(_serverIp, _serverPort);
                await conn.Connect();
                await conn.Login(_username, _password);
                await conn.UseServer(1);

                return conn;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public async Task<bool> EnsureConnected()
        {
            if (_tsClientConnection == null)
            {
                _tsClientConnection = await CreateConnection();
            }

            return _tsClientConnection != null;
        }

        public async Task ExecuteAsync()
        {
            if (!await EnsureConnected())
            {
                return;
            }

            var clients = await _tsClientConnection.GetClients();

            var newClients = clients.Where(c => !c.NickName.Contains(_username) && _clients.Count(i => i.NickName.Equals(c.NickName)) == 0).ToList();
            var removedClients = _clients.Where(c => !c.NickName.Contains(_username) && clients.Count(i => i.NickName.Equals(c.NickName)) == 0).ToList();

            foreach (var client in newClients)
            {
                MessageSender.SendMessage($"{client.NickName} Connected!!");
                _clients.Add(client);
            }

            foreach (var client in removedClients)
            {
                MessageSender.SendMessage($"{client.NickName} Disconnected!!");
                _clients.Remove(client);
            }
        }
    }
}
using DEA.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class RoleEvents
    {
        private readonly IDependencyMap _map;
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _loggingService;

        public RoleEvents(IDependencyMap map)
        {
            _map = map;
            _loggingService = _map.Get<LoggingService>();
            _client = _map.Get<DiscordSocketClient>();
            _client.RoleCreated += HandleRoleCreated;
            _client.RoleUpdated += HandleRoleUpdated;
            _client.RoleDeleted += HandleRoleDeleted;
        }

        private Task HandleRoleCreated(SocketRole role)
            => _loggingService.DetailedLogAsync(role.Guild, "Action", "Role Creation", "Role", role.Name, role.Id, new Color(12, 255, 129));

        private Task HandleRoleUpdated(SocketRole roleBefore, SocketRole roleAfter)
            => _loggingService.DetailedLogAsync(roleAfter.Guild, "Action", "Role Modification", "Role", roleAfter.Name, roleAfter.Id, new Color(12, 255, 129));

        private Task HandleRoleDeleted(SocketRole role)
            => _loggingService.DetailedLogAsync(role.Guild, "Action", "Role Deletion", "Role", role.Name, role.Id, new Color(255, 0, 0));
    }
}

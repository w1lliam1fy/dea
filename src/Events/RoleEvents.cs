using DEA.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class RoleEvents
    {
        private IDependencyMap _map;
        private DiscordSocketClient _client;
        private LoggingService _loggingService;

        public RoleEvents(IDependencyMap map)
        {
            _map = map;
            _loggingService = _map.Get<LoggingService>();
            _client = _map.Get<DiscordSocketClient>();
            _client.RoleCreated += HandleRoleCreated;
            _client.RoleUpdated += HandleRoleUpdated;
            _client.RoleDeleted += HandleRoleDeleted;
        }

        private async Task HandleRoleCreated(SocketRole role)
        {
            await _loggingService.DetailedLogAsync(role.Guild, "Action", "Role Creation", "Role", role.Name, role.Id, new Color(12, 255, 129));
        }

        private async Task HandleRoleUpdated(SocketRole roleBefore, SocketRole roleAfter)
        {
            await _loggingService.DetailedLogAsync(roleAfter.Guild, "Action", "Role Modification", "Role", roleAfter.Name, roleAfter.Id, new Color(12, 255, 129));
        }

        private async Task HandleRoleDeleted(SocketRole role)
        {
            await _loggingService.DetailedLogAsync(role.Guild, "Action", "Role Deletion", "Role", role.Name, role.Id, new Color(255, 0, 0));
        }
    }
}

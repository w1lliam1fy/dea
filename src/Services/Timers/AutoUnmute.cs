using DEA.Common.Data;
using DEA.Database.Repositories;
using DEA.Services.Static;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services.Timers
{
    class AutoUnmute
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly GuildRepository _guildRepo;
        private readonly MuteRepository _muteRepo;
        private readonly ModerationService _moderationService;
        private readonly Timer _timer;

        public AutoUnmute(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = _serviceProvider.GetService<DiscordSocketClient>();
            _guildRepo = _serviceProvider.GetService<GuildRepository>();
            _muteRepo = _serviceProvider.GetService<MuteRepository>();
            _moderationService = _serviceProvider.GetService<ModerationService>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(Unmute);

            _timer = new Timer(TimerDelegate, StateObj, TimeSpan.FromMilliseconds(1000), Config.AUTO_UNMUTE_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private void Unmute(object stateObj)
        {
            Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Timers", "Auto Unmute");

                foreach (var mute in await _muteRepo.AllAsync())
                {
                    if (DateTime.UtcNow.Subtract(mute.MutedAt).TotalMilliseconds <= mute.MuteLength)
                    {
                        return;
                    }

                    try
                    {
                        var guild = await (_client as IDiscordClient).GetGuildAsync(mute.GuildId);
                        var user = await guild.GetUserAsync(mute.UserId);
                        var dbGuild = await _guildRepo.GetGuildAsync(guild.Id);
                        var mutedRole = guild.GetRole(dbGuild.MutedRoleId);

                        await user.RemoveRoleAsync(mutedRole);
                        await _moderationService.ModLogAsync(dbGuild, guild, "Automatic Unmute", new Color(12, 255, 129), string.Empty, null, user);
                    }
                    catch
                    {
                        //Ignored.
                    }
                    finally
                    {
                        await _muteRepo.DeleteAsync(x => x.Id == mute.Id);
                    }
                }
            });
        }
    }
}

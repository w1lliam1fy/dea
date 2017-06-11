using DEA.Database.Repositories;
using DEA.Services.Static;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using DEA.Database.Models;

namespace DEA.Services.Timers
{
    internal sealed class AutoUnmute
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

            _timer = new Timer(TimerDelegate, StateObj, TimeSpan.Zero, Config.AutoUnmuteCooldown);

            StateObj.TimerReference = _timer;
        }

        private void Unmute(object stateObj)
        {
            Task.Run(async () =>
            {
                Logger.Log(LogSeverity.Debug, $"Timers", "Auto Unmute");

                List<Mute> collection = null;

                try
                {
                    collection = await _muteRepo.AllAsync();
                }
                catch (Exception e)
                {
                    Logger.Log(LogSeverity.Error, "Attempting to all async the mute repo.", e.StackTrace);
                }

                if (collection == null)
                {
                    Logger.Log(LogSeverity.Error, "Collection is null", $"Stopping Auto Unmute");
                    return;
                }

                Logger.Log(LogSeverity.Debug, "Successfully loaded collection", $"Collection count: {collection.Count}");

                foreach (var mute in collection)
                {
                    if (DateTime.UtcNow.Subtract(mute.MutedAt).TotalMilliseconds <= mute.MuteLength)
                    {
                        return;
                    }

                    Logger.Log(LogSeverity.Debug, "Mute is passed it's length", $"More information soon...");

                    try
                    {
                        var guild = await (_client as IDiscordClient).GetGuildAsync(mute.GuildId);

                        Logger.Log(LogSeverity.Debug, "Mute Guild:", guild.Name + $" ({mute.GuildId})");

                        var user = await guild.GetUserAsync(mute.UserId);

                        Logger.Log(LogSeverity.Debug, "Mute User:", $"{user} ({mute.UserId})");

                        var dbGuild = await _guildRepo.GetGuildAsync(guild.Id);

                        Logger.Log(LogSeverity.Debug, "DbGuild Fetched", $"Mod Log Channel Id: {dbGuild.ModLogChannelId}");

                        var mutedRole = guild.GetRole(dbGuild.MutedRoleId);

                        Logger.Log(LogSeverity.Debug, "Muted role fetched", mutedRole.Name + $" ({mutedRole.Id})");

                        await user.RemoveRoleAsync(mutedRole);

                        Logger.Log(LogSeverity.Debug, "Successfully removed a role", $"Mute: {mute}");

                        var result = await _moderationService.TryModLogAsync(dbGuild, guild, "Automatic Unmute", new Color(12, 255, 129), string.Empty, null, user);

                        Logger.Log(LogSeverity.Debug, "Mod log attempt", $"Succes: {result}");

                        await Task.Delay(500);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(LogSeverity.Error, "Auto Unmute role removal", e.StackTrace);
                    }
                    finally
                    {
                        await _muteRepo.DeleteAsync(mute);
                        Logger.Log(LogSeverity.Debug, "Mute entry deleted", "Passed it's length");
                    }
                }
            });
        }
    }
}

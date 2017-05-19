using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DEA.Common.Data;

namespace DEA.Modules.General
{
    public partial class General
    {
        [Command("Cooldowns")]
        [Summary("Check when you can sauce out more cash.")]
        public async Task Cooldowns([Remainder] IGuildUser user = null)
        {
            user = user ?? Context.GUser;
            var dbUser = Context.User.Id == user.Id ? Context.DbUser : await _userRepo.GetUserAsync(user);

            var cooldowns = new Dictionary<String, TimeSpan>
            {
                { "Whore", Config.WHORE_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(dbUser.Whore)) },
                { "Jump", Config.JUMP_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(dbUser.Jump)) },
                { "Steal", Config.STEAL_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(dbUser.Steal)) },
                { "Rob", Config.ROB_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(dbUser.Rob)) },
                { "Withdraw", Config.WITHDRAW_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(dbUser.Withdraw)) },
                { "Hunt", Config.HUNT_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(dbUser.Hunt)) },
                { "Fish", Config.FISH_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(dbUser.Fish)) },
                { "Collect", Config.COLLECT_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(dbUser.Collect)) }
            };

            if (await _gangRepo.InGangAsync(user))
            {
                cooldowns.Add("Raid", Config.RAID_COOLDOWN.Subtract(DateTime.UtcNow.Subtract(Context.User.Id == user.Id ? Context.Gang.Raid : (await _gangRepo.GetGangAsync(user)).Raid)));
            }

            var description = string.Empty;
            foreach (var cooldown in cooldowns)
            {
                if (cooldown.Value.TotalMilliseconds > 0)
                {
                    description += $"{cooldown.Key}: {cooldown.Value.Hours}:{cooldown.Value.Minutes.ToString("D2")}:{cooldown.Value.Seconds.ToString("D2")}\n";
                }
            }

            if (description.Length == 0)
            {
                ReplyError("All commands are available for use!");
            }

            await SendAsync(description, $"All cooldowns for {user}");
        }
    }
}

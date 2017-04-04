using DEA.Database.Repository;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace DEA.Services
{
    public static class ModuleMethods
    {
        public static async Task UnchillAsync(SocketCommandContext context, ITextChannel channel, OverwritePermissions permissions, Timer timer, int seconds, string reason)
        {
            await channel.AddPermissionOverwriteAsync(context.Guild.EveryoneRole, new OverwritePermissions().Modify(permissions.CreateInstantInvite, permissions.ManageChannel, permissions.AddReactions, permissions.ReadMessages, PermValue.Allow));
            await Logger.ModLog(context, "Chill", new Color(34, 59, 255), reason, null, $"\n**Length:** {seconds} seconds");
            timer.Stop();
        }

        public static async Task<bool> IsModAsync(SocketCommandContext context, IGuildUser user)
        {
            if (user.GuildPermissions.Administrator) return true;
            foreach (var role in (await GuildRepository.FetchGuildAsync(context.Guild.Id)).ModRoles)
            {
                if (user.Guild.GetRole((ulong)role.RoleId) != null)
                {
                    if (user.RoleIds.Any(x => x == role.RoleId)) return true;
                }
            }
            return false;
        }

        public static async Task InformSubjectAsync(IUser moderator, string action, IUser subject, string reason)
        {
            try
            {
                var channel = await subject.CreateDMChannelAsync();
                if (reason == "No reason.")
                    await channel.SendMessageAsync($"{moderator.Mention} has attempted to {action.ToLower()} you.");
                else
                    await channel.SendMessageAsync($"{moderator.Mention} has attempted to {action.ToLower()} you for the following reason: \"{reason}\"");
            }
            catch { }
        }

        public static async Task Gamble(SocketCommandContext context, double bet, double odds, double payoutMultiplier)
        {
            var user = await UserRepository.FetchUserAsync(context);
            var guild = await GuildRepository.FetchGuildAsync(context.Guild.Id);
            if (context.Guild.GetTextChannel((ulong)guild.GambleId) != null && context.Channel.Id != guild.GambleId)
                throw new Exception($"You may only gamble in {context.Guild.GetTextChannel((ulong)guild.GambleId).Mention}!");
            if (bet < Config.BET_MIN) throw new Exception($"Lowest bet is {Config.BET_MIN}$.");
            if (bet > user.Cash) throw new Exception($"You do not have enough money. Balance: {user.Cash.ToString("C", Config.CI)}.");
            double roll = new Random().Next(1, 10001) / 100.0;
            if (roll >= odds * 100)
            {
                await UserRepository.EditCashAsync(context, (bet * payoutMultiplier));
                await context.Channel.SendMessageAsync($"{context.User.Mention}, you rolled: {roll.ToString("N2")}. Congrats, you won" + 
                                                       $"{(bet * payoutMultiplier).ToString("C", Config.CI)}! Balance: {user.Cash.ToString("C", Config.CI)}.");
            }
            else
            {
                await UserRepository.EditCashAsync(context, -bet);
                await context.Channel.SendMessageAsync($"{context.User.Mention}, you rolled: {roll.ToString("N2")}. Unfortunately, you lost " + 
                                                       $"{bet.ToString("C", Config.CI)}. Balance: {user.Cash.ToString("C", Config.CI)}.");
            }
        }
    }
}

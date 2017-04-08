using DEA.Database.Repository;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Services
{
    public static class ModuleMethods
    {
        public static bool IsMod(SocketCommandContext context, IGuildUser user)
        {
            if (user.GuildPermissions.Administrator) return true;
            var guild = GuildRepository.FetchGuild(context.Guild.Id);
            if (guild.ModRoles != null)
                foreach (var role in guild.ModRoles)
                    if (user.Guild.GetRole(Convert.ToUInt64(role.Value)) != null)
                        if (user.RoleIds.Any(x => x.ToString() == role.Value.AsString)) return true;
            return false;
        }

        public static async Task InformSubjectAsync(IUser moderator, string action, IUser subject, string reason)
        {
            try
            {
                var channel = await subject.CreateDMChannelAsync();
                if (reason == "No reason.")
                    await ResponseMethods.DM(channel, $"{moderator} has attempted to {action.ToLower()} you.");
                else
                    await ResponseMethods.DM(channel, $"{moderator} has attempted to {action.ToLower()} you for the following reason: \"{reason}\"");
            }
            catch { }
        }

        public static async Task Gamble(SocketCommandContext context, decimal bet, decimal odds, decimal payoutMultiplier)
        {
            var user = UserRepository.FetchUser(context);
            var guild = GuildRepository.FetchGuild(context.Guild.Id);
            if (context.Guild.GetTextChannel(guild.GambleId) != null && context.Channel.Id != guild.GambleId)
                await ResponseMethods.Error(context, $"You may only gamble in {context.Guild.GetTextChannel(guild.GambleId).Mention}!");
            if (bet < Config.BET_MIN) await ResponseMethods.Error(context, $"Lowest bet is {Config.BET_MIN}$.");
            if (bet > user.Cash) await ResponseMethods.Error(context, $"You do not have enough money. Balance: {user.Cash.ToString("C", Config.CI)}.");
            decimal roll = new Random().Next(1, 10001) / 100m;
            if (roll >= odds)
            {
                await UserRepository.EditCashAsync(context, (bet * payoutMultiplier));
                await ResponseMethods.Reply(context, $"You rolled: {roll.ToString("N2")}. Congrats, you won " + 
                                                     $"{(bet * payoutMultiplier).ToString("C", Config.CI)}! Balance: {(user.Cash + (bet * payoutMultiplier)).ToString("C", Config.CI)}.");
            }
            else
            {
                await UserRepository.EditCashAsync(context, -bet);
                await ResponseMethods.Reply(context, $"You rolled: {roll.ToString("N2")}. Unfortunately, you lost " + 
                                                     $"{bet.ToString("C", Config.CI)}. Balance: {(user.Cash - bet).ToString("C", Config.CI)}.");
            }
        }

    }
}

using DEA.Common;
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
        public static bool IsMod(IGuildUser user)
        {
            if (user.GuildPermissions.Administrator) return true;
            var guild = GuildRepository.FetchGuild(user.GuildId);
            if (guild.ModRoles.ElementCount != 0)
                foreach (var role in guild.ModRoles)
                    if (user.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                        if (user.RoleIds.Any(x => x.ToString() == role.Name)) return true;
            return false;
        }

        public static bool IsHigherMod(IGuildUser mod, IGuildUser user)
        {
            var guild = GuildRepository.FetchGuild(mod.GuildId);
            int highest = mod.GuildPermissions.Administrator ? 2 : 0;
            int highestForUser = user.GuildPermissions.Administrator ? 2 : 0;
            if (guild.ModRoles.ElementCount == 0) return highest > highestForUser;

            foreach (var role in guild.ModRoles.OrderBy(x => x.Value))
                if (mod.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                    if (mod.RoleIds.Any(x => x.ToString() == role.Name)) highest = role.Value.AsInt32;

            foreach (var role in guild.ModRoles.OrderBy(x => x.Value))
                if (user.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                    if (user.RoleIds.Any(x => x.ToString() == role.Name)) highestForUser = role.Value.AsInt32;

            return highest > highestForUser;
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
                throw new DEAException($"You may only gamble in {context.Guild.GetTextChannel(guild.GambleId).Mention}!");
            if (bet < Config.BET_MIN) throw new DEAException($"Lowest bet is {Config.BET_MIN}$.");
            if (bet > user.Cash) throw new DEAException($"You do not have enough money. Balance: {user.Cash.ToString("C", Config.CI)}.");
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

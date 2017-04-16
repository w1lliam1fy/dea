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
        public static bool IsMod(DEAContext context, IGuildUser user)
        {
            if (user.GuildPermissions.Administrator) return true;
            if (context.DbGuild.ModRoles.ElementCount != 0)
                foreach (var role in context.DbGuild.ModRoles)
                    if (user.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                        if (user.RoleIds.Any(x => x.ToString() == role.Name)) return true;
            return false;
        }

        public static bool IsHigherMod(DEAContext context, IGuildUser mod, IGuildUser user)
        {
            int highest = mod.GuildPermissions.Administrator ? 2 : 0;
            int highestForUser = user.GuildPermissions.Administrator ? 2 : 0;
            if (context.DbGuild.ModRoles.ElementCount == 0) return highest > highestForUser;

            foreach (var role in context.DbGuild.ModRoles.OrderBy(x => x.Value))
                if (mod.Guild.GetRole(Convert.ToUInt64(role.Name)) != null)
                    if (mod.RoleIds.Any(x => x.ToString() == role.Name)) highest = role.Value.AsInt32;

            foreach (var role in context.DbGuild.ModRoles.OrderBy(x => x.Value))
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

        public static async Task Gamble(DEAContext context, decimal bet, decimal odds, decimal payoutMultiplier)
        {
            if (context.Guild.GetTextChannel(context.DbGuild.GambleId) != null && context.Channel.Id != context.DbGuild.GambleId)
                throw new DEAException($"You may only gamble in {context.Guild.GetTextChannel(context.DbGuild.GambleId).Mention}!");
            if (bet < Config.BET_MIN) throw new DEAException($"Lowest bet is {Config.BET_MIN}$.");
            if (bet > context.DbUser.Cash) throw new DEAException($"You do not have enough money. Balance: {context.DbUser.Cash.ToString("C", Config.CI)}.");
            decimal roll = new Random().Next(1, 10001) / 100m;
            if (roll >= odds)
            {
                await UserRepository.EditCashAsync(context, (bet * payoutMultiplier));
                await ResponseMethods.Reply(context, $"You rolled: {roll.ToString("N2")}. Congrats, you won " + 
                                                     $"{(bet * payoutMultiplier).ToString("C", Config.CI)}! Balance: {(context.DbUser.Cash + (bet * payoutMultiplier)).ToString("C", Config.CI)}.");
            }
            else
            {
                await UserRepository.EditCashAsync(context, -bet);
                await ResponseMethods.Reply(context, $"You rolled: {roll.ToString("N2")}. Unfortunately, you lost " + 
                                                     $"{bet.ToString("C", Config.CI)}. Balance: {(context.DbUser.Cash - bet).ToString("C", Config.CI)}.");
            }
        }

    }
}

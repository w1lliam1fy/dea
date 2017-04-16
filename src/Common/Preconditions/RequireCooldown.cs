using DEA.Services;
using DEA.Database.Repository;
using System;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Database.Models;
using Discord;

namespace DEA.Common.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireCooldownAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            TimeSpan cooldown;
            DateTime lastUse;
            var deaContext = context as DEAContext;
            switch (command.Name.ToLower())
            {
                case "whore":
                    cooldown = Config.WHORE_COOLDOWN;
                    lastUse = deaContext.DbUser.Whore;
                    break;
                case "jump":
                    cooldown = Config.JUMP_COOLDOWN;
                    lastUse = deaContext.DbUser.Jump;
                    break;
                case "steal":
                    cooldown = Config.STEAL_COOLDOWN;
                    lastUse = deaContext.DbUser.Steal;
                    break;
                case "rob":
                    cooldown = Config.ROB_COOLDOWN;
                    lastUse = deaContext.DbUser.Rob;
                    break;
                case "withdraw":
                    cooldown = Config.WITHDRAW_COOLDOWN;
                    lastUse = deaContext.DbUser.Withdraw;
                    break;
                case "raid":
                    cooldown = Config.RAID_COOLDOWN;
                    lastUse = deaContext.Gang.Raid;
                    break;
                default:
                    return PreconditionResult.FromSuccess();
            }
            if (DateTime.UtcNow.Subtract(lastUse).TotalMilliseconds > cooldown.TotalMilliseconds)
                return PreconditionResult.FromSuccess();
            else
            {
                await Logger.CooldownAsync(deaContext, command.Name, cooldown.Subtract(DateTime.UtcNow.Subtract(lastUse)));
                return PreconditionResult.FromError(string.Empty);
            }
        }
    }
}
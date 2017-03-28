using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireRankAttribute : PreconditionAttribute
    {
        int rank;
        public RequireRankAttribute(int rank) {
            this.rank = rank;
        }

        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (Config.SPONSOR_IDS.Any(x => x == context.User.Id)) return PreconditionResult.FromSuccess();
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                var user = await context.Guild.GetUserAsync(context.User.Id) as IGuildUser;
                var guild = await guildRepo.FetchGuildAsync(context.Guild.Id);
                if (!(rank >= 1 && rank <= guild.RankIds.Length)) // To fix this error, set the [RequireRank] of the given command to a valid rank
                    return PreconditionResult.FromError($"Rank {rank} does not exist in your guild.\n```diff\n" +
                                                        $"- This is an error with DEA itself and is not your fault\n```\n"+
                                                        $"Please report this error to https://github.com/RealBlazeIt/DEA/issues"
                                                          + $" along with the command you used to cause this error");

                if (context.Guild.GetRole(guild.RankIds[rank-1]) == null) 
                    return PreconditionResult.FromError($"This command may not be used if the {rank} rank role does not exist.\n" +
                                                        $"Use the `{guild.Prefix}SetRankRoles {rank}` command to change that.");

                if (user.RoleIds.All(x => x != guild.RankIds[rank-1])) // This isn't really needed, all that's needed is the cash check. Why check twice?
                    return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired role: {context.Guild.GetRole(guild.RankIds[rank-1]).Mention}");

                if (!(rank >= 1 && rank <= Config.RANKS.Length)) // To fix the error below, in Config.cs add the rank's price to the ranks array.
                    return PreconditionResult.FromError($"Rank {rank} exists in your guild but not in the config.\n" +
                                                        $"This is an error with DEA itself and is not your fault\n" +
                                                        $"Please report this error to https://github.com/RealBlazeIt/DEA/issues"); 

                if (await userRepo.GetCashAsync(user.Id) < Config.RANKS[rank - 1])
                    return PreconditionResult.FromError($"Hmmm.... It seems you did not get that rank legitimately.");

                return PreconditionResult.FromSuccess();
            }
        }
    }
}
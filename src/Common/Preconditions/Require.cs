using DEA.Database.Repository;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Common.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireAttribute : PreconditionAttribute
    {
        private Attributes[] attributes;

        public RequireAttribute(params Attributes[] attributes)
        {
            this.attributes = attributes;
        }
        
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var user = context.User as IGuildUser;
            var deaContext = context as DEAContext;
            foreach (var attribute in attributes)
                switch (attribute)
                {
                    case Attributes.BotOwner:
                        if (!DEABot.Credentials.OwnerIds.Any(x => x == context.User.Id))
                            return PreconditionResult.FromError("Only an owner of this bot may use this command.");
                        break;
                    case Attributes.ServerOwner:
                        if (user.Guild.OwnerId != user.Id && deaContext.DbGuild.ModRoles.ElementCount == 0)
                            return PreconditionResult.FromError("Only the owners of this server may use this command.");
                        else if (user.Guild.OwnerId != user.Id && deaContext.DbGuild.ModRoles != null && !user.RoleIds.Any(x => deaContext.DbGuild.ModRoles.Any(y => y.Name == x.ToString() && y.Value.AsInt32 >= 3)))
                            return PreconditionResult.FromError("Only the owners of this server may use this command.");
                        break;
                    case Attributes.Admin:
                        if (!(context.User as IGuildUser).GuildPermissions.Administrator && deaContext.DbGuild.ModRoles.ElementCount == 0)
                            return PreconditionResult.FromError("The administrator permission is required to use this command.");
                        else if (!(context.User as IGuildUser).GuildPermissions.Administrator && deaContext.DbGuild.ModRoles.ElementCount != 0 && !user.RoleIds.Any(x => deaContext.DbGuild.ModRoles.Any(y => y.Name == x.ToString() && y.Value.AsInt32 >= 2)))
                            return PreconditionResult.FromError("The administrator permission is required to use this command.");
                        break;
                    case Attributes.Moderator:
                        if (!(context.User as IGuildUser).GuildPermissions.Administrator && deaContext.DbGuild.ModRoles.ElementCount == 0)
                            return PreconditionResult.FromError("Only a moderator may use this command.");
                        else if (!(context.User as IGuildUser).GuildPermissions.Administrator && deaContext.DbGuild.ModRoles.ElementCount != 0 && !user.RoleIds.Any(x => deaContext.DbGuild.ModRoles.Any(y => y.Name == x.ToString())))
                            return PreconditionResult.FromError("Only a moderator may use this command.");
                        break;
                    case Attributes.Nsfw:
                        if (!deaContext.DbGuild.Nsfw)
                            return PreconditionResult.FromError($"This command may not be used while NSFW is disabled. An administrator may enable with the " +
                                                                $"`{deaContext.Prefix}ChangeNSFWSettings` command.");
                        var nsfwChannel = await context.Guild.GetChannelAsync(deaContext.DbGuild.NsfwId);
                        if (nsfwChannel != null && context.Channel.Id != deaContext.DbGuild.NsfwId)
                            return PreconditionResult.FromError($"You may only use this command in {(nsfwChannel as ITextChannel).Mention}.");
                        var nsfwRole = context.Guild.GetRole(deaContext.DbGuild.NsfwRoleId);
                        if (nsfwRole != null && (context.User as IGuildUser).RoleIds.All(x => x != deaContext.DbGuild.NsfwRoleId))
                            return PreconditionResult.FromError($"You do not have permission to use this command.\nRequired role: {nsfwRole.Mention}");
                        break;
                    case Attributes.InGang:
                        if (!await GangRepository.InGangAsync(context.User as IGuildUser))
                            return PreconditionResult.FromError("You must be in a gang to use this command.");
                        break;
                    case Attributes.NoGang:
                        if (await GangRepository.InGangAsync(context.User as IGuildUser))
                            return PreconditionResult.FromError("You may not use this command while in a gang.");
                        break;
                    case Attributes.GangLeader:
                        if ((await GangRepository.FetchGangAsync(context.User as IGuildUser)).LeaderId != context.User.Id)
                            return PreconditionResult.FromError("You must be the leader of a gang to use this command.");
                        break;
                    case Attributes.Jump:
                        if (deaContext.DbUser.Cash < Config.JUMP_REQUIREMENT)
                            return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.JUMP_REQUIREMENT.ToString("C", Config.CI)}.");
                        break;
                    case Attributes.Steal:
                        if (deaContext.DbUser.Cash < Config.STEAL_REQUIREMENT)
                            return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.STEAL_REQUIREMENT.ToString("C", Config.CI)}.");
                        break;
                    
                    case Attributes.Rob:
                        if (deaContext.DbUser.Cash < Config.ROB_REQUIREMENT)
                            return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.ROB_REQUIREMENT.ToString("C", Config.CI)}.");
                        break;
                    case Attributes.Bully:
                        if (deaContext.DbUser.Cash < Config.BULLY_REQUIREMENT)
                            return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.BULLY_REQUIREMENT.ToString("C", Config.CI)}.");
                        break;
                    case Attributes.FiftyX2:
                        if (deaContext.DbUser.Cash < Config.FIFTYX2_REQUIREMENT)
                            return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.FIFTYX2_REQUIREMENT.ToString("C", Config.CI)}.");
                        break;
                    default:
                        return PreconditionResult.FromError($"ERROR: The {attribute} attribute is not being handled!");
                }
            return PreconditionResult.FromSuccess();
        }
    }
}
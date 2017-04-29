using DEA.Common.Extensions;
using DEA.Database.Repositories;
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
        private IDependencyMap _map;
        
        private Credentials _credentials;
        private UserRepository _userRepo;
        private GuildRepository _guildRepo;
        private GangRepository _gangRepo;

        private Attributes[] _attributes;

        public RequireAttribute(params Attributes[] attributes)
        {
            _attributes = attributes;
        }
        
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            _map = map;
            _credentials = _map.Get<Credentials>();
            _userRepo = _map.Get<UserRepository>();
            _guildRepo = _map.Get<GuildRepository>();
            _gangRepo = _map.Get<GangRepository>();

            var guildUser = context.User as IGuildUser;
            var DbUser = await _userRepo.FetchUserAsync(guildUser);
            var DbGuild = await _guildRepo.FetchGuildAsync(context.Guild.Id);
            foreach (var attribute in _attributes)
                switch (attribute)
                {
                    case Attributes.BotOwner:
                        if (!_credentials.OwnerIds.Any(x => x == context.User.Id))
                            return PreconditionResult.FromError("Only an owner of this bot may use this command.");
                        break;
                    case Attributes.ServerOwner:
                        if (context.Guild.OwnerId != guildUser.Id && DbGuild.ModRoles.ElementCount == 0)
                            return PreconditionResult.FromError("Only the owners of this server may use this command.");
                        else if (guildUser.Guild.OwnerId != context.User.Id && DbGuild.ModRoles != null && !guildUser.RoleIds.Any(x => DbGuild.ModRoles.Any(y => y.Name == x.ToString() && y.Value.AsInt32 >= 3)))
                            return PreconditionResult.FromError("Only the owners of this server may use this command.");
                        break;
                    case Attributes.Admin:
                        if (!guildUser.GuildPermissions.Administrator && DbGuild.ModRoles.ElementCount == 0)
                            return PreconditionResult.FromError("The administrator permission is required to use this command.");
                        else if (!guildUser.GuildPermissions.Administrator && DbGuild.ModRoles.ElementCount != 0 && !guildUser.RoleIds.Any(x => DbGuild.ModRoles.Any(y => y.Name == x.ToString() && y.Value.AsInt32 >= 2)))
                            return PreconditionResult.FromError("The administrator permission is required to use this command.");
                        break;
                    case Attributes.Moderator:
                        if (!guildUser.GuildPermissions.Administrator && DbGuild.ModRoles.ElementCount == 0)
                            return PreconditionResult.FromError("Only a moderator may use this command.");
                        else if (!guildUser.GuildPermissions.Administrator && DbGuild.ModRoles.ElementCount != 0 && !guildUser.RoleIds.Any(x => DbGuild.ModRoles.Any(y => y.Name == x.ToString())))
                            return PreconditionResult.FromError("Only a moderator may use this command.");
                        break;
                    case Attributes.Nsfw:
                        if (!DbGuild.Nsfw)
                            return PreconditionResult.FromError($"This command may not be used while NSFW is disabled. An administrator may enable with the " +
                                                                $"`{DbGuild.Prefix}ChangeNSFWSettings` command.");
                        var nsfwChannel = await context.Guild.GetChannelAsync(DbGuild.NsfwChannelId);
                        if (nsfwChannel != null && context.Channel.Id != DbGuild.NsfwChannelId)
                            return PreconditionResult.FromError($"You may only use this command in {(nsfwChannel as ITextChannel).Mention}.");
                        break;
                    case Attributes.InGang:
                        if (!await _gangRepo.InGangAsync(guildUser))
                            return PreconditionResult.FromError("You must be in a gang to use this command.");
                        break;
                    case Attributes.NoGang:
                        if (await _gangRepo.InGangAsync(guildUser))
                            return PreconditionResult.FromError("You may not use this command while in a gang.");
                        break;
                    case Attributes.GangLeader:
                        if ((await _gangRepo.FetchGangAsync(guildUser)).LeaderId != context.User.Id)
                            return PreconditionResult.FromError("You must be the leader of a gang to use this command.");
                        break;
                    case Attributes.Jump:
                        if (DbUser.Cash < Config.JUMP_REQUIREMENT)
                            return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.JUMP_REQUIREMENT.USD()}.");
                        break;
                    case Attributes.Steal:
                        if (DbUser.Cash < Config.STEAL_REQUIREMENT)
                            return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.STEAL_REQUIREMENT.USD()}.");
                        break;
                    
                    case Attributes.Rob:
                        if (DbUser.Cash < Config.ROB_REQUIREMENT)
                            return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.ROB_REQUIREMENT.USD()}.");
                        break;
                    case Attributes.Bully:
                        if (DbUser.Cash < Config.BULLY_REQUIREMENT)
                            return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.BULLY_REQUIREMENT.USD()}.");
                        break;
                    case Attributes.FiftyX2:
                        if (DbUser.Cash < Config.FIFTYX2_REQUIREMENT)
                            return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.FIFTYX2_REQUIREMENT.USD()}.");
                        break;
                    default:
                        return PreconditionResult.FromError($"ERROR: The {attribute} attribute is not being handled!");
                }
            return PreconditionResult.FromSuccess();
        }
    }
}
using DEA.Common.Extensions;
using DEA.Common.Utilities;
using DEA.Database.Repositories;
using DEA.Services;
using Discord.Commands;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DEA.Common.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal sealed class Require : PreconditionAttribute
    {
        private IServiceProvider _serviceProvider;
        private Credentials _credentials;
        private UserRepository _userRepo;
        private GameService _gameService;
        private ModerationService _moderationService;
        private Attributes[] _attributes;

        public Require(params Attributes[] attributes)
        {
            _attributes = attributes;
        }
        
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            return Task.Run(() =>
            {
                _serviceProvider = serviceProvider;
                _userRepo = _serviceProvider.GetService<UserRepository>();
                _credentials = _serviceProvider.GetService<Credentials>();
                _moderationService = _serviceProvider.GetService<ModerationService>();
                _gameService = _serviceProvider.GetService<GameService>();

                Context deaContext = context as Context;

                var permLevel = _moderationService.GetPermLevel(deaContext.DbGuild, deaContext.GUser);
                var invData = _gameService.InventoryData(deaContext.DbUser);

                foreach (var attribute in _attributes)
                {
                    switch (attribute)
                    {
                        case Attributes.BotOwner:
                            if (!_credentials.OwnerIds.Any(x => x == context.User.Id))
                            {
                                return PreconditionResult.FromError("Only an owner of this bot may use this command.");
                            }
                            break;
                        case Attributes.ServerOwner:
                            if (permLevel != 3)
                            {
                                return PreconditionResult.FromError("Only the owners of this server may use this command.");
                            }
                            break;
                        case Attributes.Admin:
                            if (permLevel < 2)
                            {
                                return PreconditionResult.FromError("The administrator permission is required to use this command.");
                            }
                            break;
                        case Attributes.Moderator:
                            if (permLevel == 0)
                            {
                                return PreconditionResult.FromError("Only a moderator may use this command.");
                            }
                            break;
                        case Attributes.InGang:
                            if (deaContext.Gang == null)
                            {
                                return PreconditionResult.FromError("You must be in a gang to use this command.");
                            }
                            break;
                        case Attributes.NoGang:
                            if (deaContext.Gang != null)
                            {
                                return PreconditionResult.FromError("You may not use this command while in a gang.");
                            }
                            break;
                        case Attributes.GangLeader:
                            if (deaContext.Gang == null || deaContext.Gang.LeaderId != context.User.Id)
                            {
                                return PreconditionResult.FromError("You must be the leader of a gang to use this command.");
                            }
                            break;
                        case Attributes.Jump:
                            if (deaContext.Cash < Config.JumpRequirement)
                            {
                                return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.JumpRequirement.USD()}.");
                            }
                            break;
                        case Attributes.Steal:
                            if (deaContext.Cash < Config.StealRequirement)
                            {
                                return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.StealRequirement.USD()}.");
                            }
                            break;
                        case Attributes.Rob:
                            if (deaContext.Cash < Config.RobRequirement)
                            {
                                return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.RobRequirement.USD()}.");
                            }
                            break;
                        case Attributes.Bully:
                            if (deaContext.Cash < Config.BullyRequirement)
                            {
                                return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.BullyRequirement.USD()}.");
                            }
                            break;
                        case Attributes.FiftyX2:
                            if (deaContext.Cash < Config.FiftyX2Requirement)
                            {
                                return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired cash: {Config.FiftyX2Requirement.USD()}.");
                            }
                            break;
                        default:
                            return PreconditionResult.FromError($"ERROR: The {attribute} attribute is not being handled!");
                    }
                }
                return PreconditionResult.FromSuccess();
            });
        }
    }
}
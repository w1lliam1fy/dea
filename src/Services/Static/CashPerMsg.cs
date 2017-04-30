using DEA.Common;
using DEA.Database.Repositories;
using DEA.Services.Handlers;
using System;
using System.Threading.Tasks;

namespace DEA.Services.Static
{
    public static class CashPerMsg
    {
        /// <summary>
        /// Provides the user in question with money and an increased rate if the cooldown has finished.
        /// </summary>
        /// <param name="userRepo">The user repository object to modify the user's cash.</param>
        /// <param name="context">The context to get the user's data information.</param>
        /// <returns></returns>
        public static async Task Apply(UserRepository userRepo, RankHandler rankHandler, DEAContext context)
        {
            if (DateTime.UtcNow.Subtract(context.DbUser.Message).TotalMilliseconds > context.DbUser.MessageCooldown)
            {
                await userRepo.ModifyAsync(context.DbUser, x => 
                {
                    x.TemporaryMultiplier = context.DbUser.TemporaryMultiplier + context.DbGuild.TempMultiplierIncreaseRate;
                    x.Message = DateTime.UtcNow;
                    x.Cash += context.DbGuild.GlobalChattingMultiplier * context.DbUser.TemporaryMultiplier * context.DbUser.InvestmentMultiplier;
                });
                await rankHandler.HandleAsync(context.Guild, context.GUser, context.DbGuild, context.DbUser);
            }
        }
    }
}

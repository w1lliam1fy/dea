using DEA.Common;
using DEA.Database.Repository;
using System;
using System.Threading.Tasks;

namespace DEA.Services.Static
{
    public static class CashPerMsg
    {
        public static async Task Apply(UserRepository userRepo, DEAContext context)
        {
            if (DateTime.UtcNow.Subtract(context.DbUser.Message).TotalMilliseconds > context.DbUser.MessageCooldown)
            {
                await userRepo.ModifyAsync(context.DbUser.UserId, context.DbGuild.Id, x => x.TemporaryMultiplier, context.DbUser.TemporaryMultiplier + context.DbGuild.TempMultiplierIncreaseRate);
                await userRepo.ModifyAsync(context.DbUser.UserId, context.DbGuild.Id, x => x.Message, DateTime.UtcNow);
                await userRepo.EditCashAsync(context, context.DbGuild.GlobalChattingMultiplier * context.DbUser.TemporaryMultiplier * context.DbUser.InvestmentMultiplier + context.DbUser.Cash);
            }
        }
    }
}

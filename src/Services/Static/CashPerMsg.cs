using DEA.Database.Models;
using DEA.Database.Repository;
using System;
using System.Threading.Tasks;

namespace DEA.Services.Static
{
    public static class CashPerMsg
    {
        public static async Task Apply(UserRepository userRepo, Guild dbGuild, User dbUser)
        {
            if (DateTime.UtcNow.Subtract(dbUser.Message).TotalMilliseconds > dbUser.MessageCooldown)
            {
                await userRepo.ModifyAsync(dbUser.UserId, dbGuild.Id, x => x.TemporaryMultiplier, dbUser.TemporaryMultiplier + dbGuild.TempMultiplierIncreaseRate);
                await userRepo.ModifyAsync(dbUser.UserId, dbGuild.Id, x => x.Message, DateTime.UtcNow);
                await userRepo.ModifyAsync(dbUser.UserId, dbGuild.Id, x => x.Cash, dbGuild.GlobalChattingMultiplier * dbUser.TemporaryMultiplier * dbUser.InvestmentMultiplier + dbUser.Cash);
            }
        }
    }
}

using DEA.Database.Models;
using DEA.Database.Repository;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public static class CashPerMsg
    {
        public static async Task Apply(Guild dbGuild, User dbUser)
        {
            if (DateTime.UtcNow.Subtract(dbUser.Message).TotalMilliseconds > dbUser.MessageCooldown)
            {
                await UserRepository.ModifyAsync(dbUser.UserId, dbGuild.Id, x => x.TemporaryMultiplier, dbUser.TemporaryMultiplier + dbGuild.TempMultiplierIncreaseRate);
                await UserRepository.ModifyAsync(dbUser.UserId, dbGuild.Id, x => x.Message, DateTime.UtcNow);
                await UserRepository.ModifyAsync(dbUser.UserId, dbGuild.Id, x => x.Cash, dbGuild.GlobalChattingMultiplier * dbUser.TemporaryMultiplier * dbUser.InvestmentMultiplier + dbUser.Cash);
            }
        }
    }
}

const db = require('../database');
const util = require('../utility');
const config = require('../config.json');

module.exports = async (client) => {
  client.setInterval(async () => {
    const users = await db.userRepo.findMany({ cash: { $gte: config.minRich * 100 } });

    for (const dbUser of users) {
      const dbGuild = await db.guildRepo.getGuild(dbUser.guildId);

      if (!dbGuild.settings.fines) {
        break;
      }

      const additionalOdds = dbUser.cash * config.additionalFineOdds;

      if (config.fineOdds + additionalOdds >= util.Random.roll()) {
        const user = client.users.get(dbUser.userId);

        const guild = client.guilds.get(dbUser.guildId);

        if (user === undefined || guild === undefined) {
          continue;
        }

        const member = guild.member(user);

        if (member === null) {
          continue;
        }

        const fine = util.NumberUtil.realValue(dbUser.cash) * config.fineCut;

        await db.userRepo.modifyCash(dbGuild, member, -fine);

        await util.Messenger.tryDM(user, util.StringUtil.format(util.Random.arrayElement(config.fineMessages), util.NumberUtil.USD(fine)), guild);
      }
    }
  }, config.fineInterval);
};

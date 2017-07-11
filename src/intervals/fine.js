const db = require('../database');
const util = require('../utility');
const config = require('../config.json');

module.exports = async (client) => {
  client.setInterval(async () => {
    const users = await db.userRepo.findMany({ cash: { $gte: config.minRich * 100 } });

    for (const dbUser of users) {
      if (config.fineOdds >= util.Random.roll()) {
        const user = client.users.get(dbUser.userId);

        if (user === undefined) {
          return;
        }

        const fine = util.NumberUtil.realValue(dbUser.cash) * config.fineCut;

        await db.userRepo.updateUser(dbUser.userId, dbUser.guildId, new db.updates.IncMoney('cash', -fine));

        await util.Messenger.tryDM(user, util.StringUtil.format(util.Random.arrayElement(config.fineMessages), util.NumberUtil.USD(fine)));
      }
    }
  }, config.fineInterval);
};
const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');
const config = require('../../config.json');
const NoSelf = require('../../preconditions/NoSelf.js');

class GiveReputation extends patron.Command {
  constructor() {
    super({
      name: 'rep',
      aliases: ['givereputation', 'giverep'],
      group: 'reputation',
      description: 'Give reputation to any user.',
      cooldown: 21600000,
      args: [
        new patron.Argument({
          name: 'user',
          key: 'user',
          type: 'user',
          example:'Cheese Burger#6666',
          isRemainder: true,
          preconditions: [NoSelf]
        })
      ]
    });
  }

  async run(msg, args) {
    const reppedDbUser = await db.userRepo.findUserAndUpsert(args.user.id, msg.guild.id, { $inc: { reputation: 1 } });

    const reward = util.Random.nextFloat(config.repRewardMin, config.repRewardMax);

    const newDbUser = await db.userRepo.findAndModifyCash(msg.dbGuild, msg.member, reward);

    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully given reputation to ' + util.StringUtil.boldify(args.user.tag) + ' raising it to ' + reppedDbUser.reputation + '.\n\nIn exchange for having contributed to this community, you have been awarded ' + util.NumberUtil.USD(reward) + '. Balance: ' + util.NumberUtil.format(newDbUser.cash) + '.');
  }
}

module.exports = new GiveReputation();

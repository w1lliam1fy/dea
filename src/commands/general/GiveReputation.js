const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');
const NoSelf = require('../../preconditions/NoSelf.js');

class GiveReputation extends patron.Command {
  constructor() {
    super({
      name: 'rep',
      aliases: ['givereputation', 'giverep'],
      group: 'general',
      description: 'Give reputation to any user.',
      cooldown: 21600000,
      args: [
        new patron.Argument({
          name: 'user',
          key: 'user',
          type: 'user',
          example:'Cheese Burger Hours#6666',
          preconditions: [NoSelf],
          isRemainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    const newDbUser = await db.userRepo.findUserAndUpsert(args.user.id, msg.guild.id, { $inc: { reputation: 1 } });

    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully given reputation to ' + util.StringUtil.boldify(args.user.tag) + ' raising it to ' + newDbUser.reputation + '.');
  }
}

module.exports = new GiveReputation();

const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');
const config = require('../../config.json');
const NoSelf = require('../../preconditions/NoSelf.js');
const Reputation = require('../../preconditions/Reputation.js');

class RemoveReputation extends patron.Command {
  constructor() {
    super({
      name: 'unrep',
      aliases: ['removereputation', 'removerep'],
      group: 'reputation',
      description: 'Remove reputation from any user.',
      cooldown: 21600000,
      preconditions: [new Reputation(config.minRepForRemove)],
      args: [
        new patron.Argument({
          name: 'user',
          key: 'user',
          type: 'user',
          example:'Blast My Ass#6969',
          isRemainder: true,
          preconditions: [NoSelf]
        })
      ]
    });
  }

  async run(msg, args) {
    const newDbUser = await db.userRepo.findUserAndUpsert(args.user.id, msg.guild.id, { $inc: { reputation: -1 } });

    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully unrepped ' + util.StringUtil.boldify(args.user.tag) + ' lowering their reputation ' + newDbUser.reputation + '.');
  }
}

module.exports = new RemoveReputation();

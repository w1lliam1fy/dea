const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class ResetUser extends patron.Command {
  constructor() {
    super({
      name: 'resetuser',
      group: 'owners',
      description: 'Reset any member\'s data.',
      args: [
        new patron.Argument({
          name: 'member',
          key: 'member',
          type: 'member',
          default: patron.Default.Member,
          example: 'Jesus Christ#4444',
          remainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    await db.userRepo.deleteUser(args.member.id, msg.guild.id);

    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully reset all of ' + (args.member.id === msg.author.id ? 'your' : util.StringUtil.boldify(args.member.user.tag) + '\'s') + ' data.');
  }
}

module.exports = new ResetUser();

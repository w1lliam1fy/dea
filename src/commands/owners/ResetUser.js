const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class ResetUser extends patron.Command {
  constructor() {
    super({
      name: 'resetuser',
      group: 'owners',
      description: 'Reset any users data.',
      args: [
        new patron.Argument({
          name: 'user',
          key: 'user',
          type: 'user',
          default: patron.Default.Author,
          example: 'Supa Hot Fire#0911'
        })
      ]
    });
  }

  async run(context, args) {
    await db.userRepo.deleteUser(args.user.id, context.guild.id);
  
    return util.Messenger.reply(context.channel, context.author, 'You have successfully reset all of ' + (args.user.id === context.author.id ? 'your' : util.StringUtil.boldify(args.user.tag) + '\'s') + ' data.');
  }
}

module.exports = new ResetUser();

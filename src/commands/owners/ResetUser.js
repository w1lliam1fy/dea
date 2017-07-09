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

  async run(msg, args){
    await db.userRepo.deleteUser(args.user.id, msg.guild.id);
  
    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully reset all of ' + (args.user.id === msg.author.id ? 'your' : util.StringUtil.boldify(args.user.tag) + '\'s') + ' data.');
  }
}

module.exports = new ResetUser();

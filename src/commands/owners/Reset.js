const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class Reset extends patron.Command {
  constructor() {
    super({
      name: 'reset',
      group: 'owners',
      description: 'Resets all user data in your server.'
    });
  }

  async run(context, args){
    await util.Messenger.reply(context.channel, context.author, 'Are you sure you wish to reset all DEA related data within your server? Reply with "yes" to continue.');

    const filter = x => x.content.toLowerCase() === 'yes' && x.author.id === context.author.id;
    const result = await context.channel.awaitMessages(filter, { max: 1, time: 30000 });

    if (result.size === 1) {
      await db.userRepo.deleteUsers(context.guild.id);
      return util.Messenger.reply(context.channel, context.author, 'You have successfully reset all data in your server.');
    } 
  }
}

module.exports = new Reset();
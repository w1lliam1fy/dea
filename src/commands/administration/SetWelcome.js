const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');

class SetWelcome extends patron.Command {
  constructor() {
    super({
      name: 'setwelcome',
      aliases: ['setwelcomemessage', 'welcome', 'addwelcome'],
      group: 'administration',
      description: 'Sets the welcome message.',
      args: [
        new patron.Argument({
          name: 'message',
          key: 'message',
          type: 'string',
          example: 'Hey! Welcome to our server!',
          isRemainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    await db.guildRepo.upsertGuild(msg.guild.id, new db.updates.Set('settings.welcomeMessage', args.message));

    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully set the welcome message to "' + args.message + '".');
  }
}

module.exports = new SetWelcome();

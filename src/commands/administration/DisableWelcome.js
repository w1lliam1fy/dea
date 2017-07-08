const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');

class DisableWelcome extends patron.Command {
  constructor() {
    super({
      name: 'disablewelcome',
      aliases: ['disablewelcomemessage'],
      group: 'administration',
      description: 'Disables the welcome message.'
    });
  }

  async run(context) {
    await db.guildRepo.upsertGuild(context.guild.id, new db.updates.Set('settings.welcomeMessage', null));

    return util.Messenger.reply(context.channel, context.author, 'You have successfully disabled the welcome message.');
  }
}

module.exports = new DisableWelcome();

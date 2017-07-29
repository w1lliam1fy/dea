const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');

class ChangeFineSettings extends patron.Command {
  constructor() {
    super({
      name: 'changefinesettings',
      aliases: ['enablefines', 'disablefines'],
      group: 'administration',
      description: 'Toggles the fine settings.'
    });
  }

  async run(msg) {
    await db.guildRepo.upsertGuild(msg.guild.id, new db.updates.Set('settings.fines', !msg.dbGuild.settings.fines));

    return util.Messenger.reply(msg.channel, msg.author, 'You have successfully ' + (msg.dbGuild.settings.fines ? 'dis' : 'en') + 'abled fines.');
  }
}

module.exports = new ChangeFineSettings();

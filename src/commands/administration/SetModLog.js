const patron = require('patron.js');
const db = require('../../database');
const util = require('../../utility');

class SetModLog extends patron.Command {
  constructor() {
    super({
      name: 'setmodlogchannel',
      aliases: ['modlog', 'logs', 'setmodlog', 'setmodlogs', 'setlog', 'setlogs'],
      group: 'administration',
      description: 'Sets the mod log channel.',
      args: [
        new patron.Argument({
          name: 'mod log channel',
          key: 'channel',
          type: 'channel',
          example: 'Mod Log',
          isRemainder: true
        })
      ]
    });
  }

  async run(context, args) {
    await db.guildRepo.upsertGuild(context.guild.id, new db.updates.Set('channels.modLog', args.channel.id));

    return util.Messenger.reply(context.channel, context.author, 'You have successfully set the mod log channel to ' + args.channel + '.');
  }
}

module.exports = new SetModLog();

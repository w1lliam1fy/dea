const patron = require('patron.js');
const util = require('../../utility');

class Help extends patron.Command {
  constructor() {
    super({
      name: 'statistics',
      aliases: ['stats'],
      group: 'system',
      description: 'Statistics about DEA.',
      guildOnly: false
    });
  }

  async run(context, args) {
    const uptime = util.NumberUtil.msToTime(context.client.uptime);

    await util.Messenger.DMFields(context.author, 
      [
        'Author', 'John#0969', 'Framework', 'patron.js', 'Memory', (process.memoryUsage().rss / 1000000).toFixed(2) + ' MB', 'Servers', context.client.guilds.size,
        'Users', context.client.users.size, 'Uptime', 'Days: ' + uptime.days + '\nHours: '+ uptime.hours + '\nMinutes: ' + uptime.minutes
      ]);

    if (context.channel.type !== 'dm') {
      return util.Messenger.reply(context.channel, context.author, 'You have been DMed with all DEA Statistics!');
    }
  }
}

module.exports = new Help();

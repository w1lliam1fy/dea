const patron = require('patron.js');
const util = require('../../utility');

class Statistics extends patron.Command {
  constructor() {
    super({
      name: 'statistics',
      aliases: ['stats'],
      group: 'system',
      description: 'Statistics about DEA.',
      guildOnly: false
    });
  }

  async run(msg, args) {
    const uptime = util.NumberUtil.msToTime(msg.client.uptime);

    let users = 0;

    for (const guild of msg.client.guilds.values()) {
      users += guild.memberCount;
    }

    await util.Messenger.DMFields(msg.author,
      [
        'Author', 'John#0969', 'Framework', 'patron.js', 'Memory', (process.memoryUsage().rss / 1000000).toFixed(2) + ' MB', 'Servers', msg.client.guilds.size,
        'Users', users, 'Uptime', 'Days: ' + uptime.days + '\nHours: '+ uptime.hours + '\nMinutes: ' + uptime.minutes
      ]);

    if (msg.channel.type !== 'dm') {
      return util.Messenger.reply(msg.channel, msg.author, 'You have been DMed with all DEA Statistics!');
    }
  }
}

module.exports = new Statistics();

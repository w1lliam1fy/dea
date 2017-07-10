const db = require('../../database');
const config = require('../../config.json');
const patron = require('patron.js');
const util = require('../../utility');

class LowRepLeaderboards extends patron.Command {
  constructor() {
    super({
      name: 'lowrepleaderboards',
      aliases: ['lowredlb', 'lowreps', 'lowrep'],
      group: 'reputation',
      description: 'View the users with the lowest reputation.'
    });
  }

  async run(msg) {
    const users = await db.userRepo.findMany({ guildId: msg.guild.id });
		
    users.sort((a, b) => a.reputation - b.reputation);

    let message = '';
    let position = 1;

    for (let i = 0; i < users.length; i++) {
      if (position > config.leaderboardCap) {
        break;
      }

      const user = msg.client.users.get(users[i].userId);

      if (user === undefined) {
        continue;
      }

      message += position++ + '. ' + util.StringUtil.boldify(user.tag) + ': ' + users[i].reputation + '\n';
    }

    if (util.StringUtil.isNullOrWhiteSpace(message)) {
      return util.Messenger.replyError(msg.channel, msg.author, 'There is nobody on the leaderboards.');
    }

    return util.Messenger.send(msg.channel, message, 'The Least Respected Users');
  }
}

module.exports = new LowRepLeaderboards();
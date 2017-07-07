const db = require('../../database');
const config = require('../../config.json');
const patron = require('patron.js');
const util = require('../../utility');

class Leaderboards extends patron.Command {
  constructor() {
    super({
      name: 'leaderboards',
      aliases: ['lb', 'highscores', 'highscore', 'leaderboard'],
      group: 'general',
      description: 'View the richest Drug Traffickers.'
    });
  }

  async run(context) {
    const users = await db.userRepo.findMany({ guildId: context.guild.id });
		
    users.sort((a, b) => b.cash - a.cash);

    let message = '';
    let position = 1;

    for (let i = 0; i < users.length; i++) {
      if (position > config.leaderboardCap) {
        break;
      }

      const user = context.client.users.get(users[i].userId);

      if (user === undefined) {
        continue;
      }

      message += position++ + '. ' + util.StringUtil.boldify(user.tag) + ': ' + util.NumberUtil.format(users[i].cash) + '\n';
    }

    if (util.StringUtil.isNullOrWhiteSpace(message)) {
      return util.Messenger.replyError(context.channel, context.author, 'There is nobody on the leaderboards.');
    }

    return util.Messenger.send(context.channel, message, 'The Richest Traffickers');
  }
}

module.exports = new Leaderboards();

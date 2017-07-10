const db = require('../../database');
const patron = require('patron.js');
const util = require('../../utility');
const RankService = require('../../services/RankService.js');

class Rank extends patron.Command {
  constructor() {
    super({
      name: 'rank',
      group: 'general',
      description: 'View the rank of anyone.',
      args: [
        new patron.Argument({
          name: 'user',
          key: 'user',
          type: 'user',
          default: patron.Default.Author,
          example:'Supa Hot Fire#0911',
          isRemainder: true
        })
      ]
    });
  }

  async run(msg, args) {
    const dbUser = msg.author.id === args.user.id ? msg.dbUser : await db.userRepo.getUser(args.user.id, msg.guild.id);
    const sortedUsers = (await db.userRepo.findMany({ guildId: msg.guild.id })).sort((a, b) => b.cash - a.cash);
    const rank = RankService.getRank(dbUser, msg.dbGuild, msg.guild);

    return util.Messenger.send(msg.channel, '**Balance:** ' + util.NumberUtil.format(dbUser.cash) + '\n' +
                              '**Position:** ' + (sortedUsers.findIndex((v) => v.userId === dbUser.userId) + 1) + '\n' +
                              (rank !== undefined ? '**Rank:** #' + rank + '\n' : ''), args.user.tag + '\'s Rank');
  }
}

module.exports = new Rank();

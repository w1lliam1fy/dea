const db = require('../database');
const config = require('../config.json');
const util = require('../utility');

class ChatService {
  constructor() {
    this.messages = new Map();
  }

  // TODO: CHECK IF RICH TO DEFLATE ECONOMY!!!

  async applyCash(msg) {
    const lastMessage = this.messages.get(msg.author.id);
    const isMessageCooldownOver = lastMessage === undefined || Date.now() - lastMessage > config.messageCooldown;
    const isLongEnough = msg.content.length <= config.minCharLength;

    if (isMessageCooldownOver && isLongEnough) {
      this.messages.set(msg.author.id, Date.now());

      if (config.lotteryOdds >= util.Random.roll()) {
        const winnings = util.Random.nextFloat(config.lotteryMin, config.lotteryMax);
        await db.userRepo.findAndModifyCash(msg.dbGuild, msg.member, winnings);
        return util.Messenger.tryReply(msg.channel, msg.author, util.StringUtil.format(util.Random.arrayElement(config.lotteryResponses), util.NumberUtil.USD(winnings)));
      } else {
        return db.userRepo.findAndModifyCash(msg.dbGuild, msg.member, config.cashPerMessage);
      }
    }
  }

  async gangBang(msg) {
    const cash = util.NumberUtil.realValue(msg.dbUser.cash);

    if (cash >= config.gangBangMin && config.gangBangOdds >= util.Random.roll()) {
      const boldifiedVictim = util.StringUtil.boldify(msg.author.tag);

      const result = await util.Messenger.trySend(msg.channel, 'GUYS, ' + boldifiedVictim + ' is a rich nigga who is vulnerable to a gang bang! '+ 
                       'If three people use the `' + config.prefix + 'gangbang` command, you can clean this shady drug dealer\'s bank account!');

      if (result) {
        const filter = (m) => m.content.toLowerCase().startsWith(config.prefix + 'gangbang') && m.author.id !== msg.author.id;

        const collection = await msg.channel.awaitMessages(filter, { maxMatches: 3, time: 15000 });

        if (collection.size >= 3) {
          let gangBangers = '';

          const stolen = cash * config.gangBangSteal;
          const split = stolen / 6;

          for (const message of collection.values()) {
            if (gangBangers.includes(message.author.tag)) {
              continue;
            }

            gangBangers += util.StringUtil.boldify(message.author.tag) + ', ';

            await db.userRepo.findAndModifyCash(msg.dbGuild, msg.guild.member(message.author), split);
          }

          await db.userRepo.findAndModifyCash(msg.dbGuild, msg.member, -stolen);

          await util.Messenger.trySend(msg.channel, gangBangers.slice(0, -2) + ' just gang banged ' + boldifiedVictim + ' and managed to each walk away with ' + util.NumberUtil.USD(split) + 
            '. Some of the cash got destroyed in the fight, but hey, a gang bang is a gang bang.');
        } else {
          await util.Messenger.trySend(msg.channel, 'You guys were too damn slow! The filthy scrub just made it out in time!');
        }
      }
    }
  }
}

module.exports = new ChatService();

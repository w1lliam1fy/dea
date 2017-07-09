const db = require('../database');
const config = require('../config.json');
const util = require('../utility');

class ChatService {
  constructor() {
    this.messages = new Map();
  }

  async applyCash(msg) {
    const lastMessage = this.messages.get(msg.author.id);
    const isMessageCooldownOver = lastMessage === undefined || Date.now() - lastMessage > config.messageCooldown;
    const isLongEnough = msg.content.length >= config.minCharLength;
    const inGuild = msg.content.guild !== null;

    if (isMessageCooldownOver && isLongEnough && inGuild) {
      this.messages.set(msg.author.id, Date.now());

      if (config.lotteryOdds >= util.Random.roll()) {
        const winnings = util.Random.nextFloat(config.lotteryMin, config.lotteryMax);
        await db.userRepo.modifyCash(msg.author.id, msg.guild.id, winnings);
        return util.Messenger.tryReply(msg.channel, msg.author, util.StringUtil.format(util.Random.arrayElement(config.lotteryResponse), util.NumberUtil.USD(winnings)));
      } else {
        return db.userRepo.modifyCash(msg.author.id, msg.guild.id, config.cashPerMessage);
      }  
    }
  }
}

module.exports = new ChatService();

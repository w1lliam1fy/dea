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

    if (isMessageCooldownOver && isLongEnough) {
      this.messages.set(msg.author.id, Date.now());

      if (config.lotteryOdds >= util.Random.roll()) {
        const winnings = util.Random.nextFloat(config.lotteryMin, config.lotteryMax);
        await db.userRepo.modifyCash(msg.dbGuild, msg.member, winnings);
        return util.Messenger.tryReply(msg.channel, msg.author, util.StringUtil.format(util.Random.arrayElement(config.lotteryMessages), util.NumberUtil.USD(winnings)));
      }

      return db.userRepo.modifyCash(msg.dbGuild, msg.member, config.cashPerMessage);
    }
  }
}

module.exports = new ChatService();

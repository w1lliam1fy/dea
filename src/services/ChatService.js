const db = require('../database');
const config = require('../config.json');

class ChatService {
  static async applyCash(msg, lastMessage) {
    const statsWithPrefix = msg.content.startsWith(config.defaultPrefix);
    const isLongEnough = msg.content.length >= config.minCharLength;
    const isMessageCooldownOver = Date.now() - lastMessage > config.messageCooldown;

    if (!statsWithPrefix && isLongEnough && isMessageCooldownOver) {
      db.userRepo.modifyCash(msg.author.id, msg.guild.id, config.cashPerMessage)
        .then(() => msg.client.cooldowns.message.set(msg.author.id, Date.now()))
        .catch(console.error);
    }
  }
}

module.exports = ChatService;
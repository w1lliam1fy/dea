const db = require('../database');
const config = require('../config.json');
const messages = new Map();

class ChatService {
  static async applyCash(msg) {
    const lastMessage = messages.get(msg.author.id);
    const isMessageCooldownOver = lastMessage === undefined || Date.now() - lastMessage > config.messageCooldown;
    const isLongEnough = msg.content.length >= config.minCharLength;

    if (isMessageCooldownOver && isLongEnough) {
      await db.userRepo.modifyCash(msg.author.id, msg.guild.id, config.cashPerMessage);
      messages.set(msg.author.id, Date.now());
    }
  }
}

module.exports = ChatService;
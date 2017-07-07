const db = require('../database');
const config = require('../config.json');

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
      await db.userRepo.modifyCash(msg.author.id, msg.guild.id, config.cashPerMessage);
      this.messages.set(msg.author.id, Date.now());
    }
  }
}

module.exports = new ChatService();

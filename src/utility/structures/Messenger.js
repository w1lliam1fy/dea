const config = require('../../config.json');
const discord = require('discord.js');
const Random = require('./Random.js');
const StringUtil = require('./StringUtil.js');

class Messenger {
  static send(channel, description, title = '', color = null) {
    const embed = new discord.RichEmbed()
      .setColor(color || Random.arrayElement(config.embedColors))
      .setDescription(description);

    if (!StringUtil.isNullOrWhiteSpace(title)) {
      embed.setTitle(title);
    }

    return channel.send({ embed: embed });
  }

  static reply(channel, user, description, title = '', color = null) {
    return this.send(channel, StringUtil.boldify(user.tag) + ', ' + description, title, color);
  }

  static sendError(channel, description, title = '') {
    return this.send(channel, description, title, config.errorColor);
  }

  static replyError(channel, user, description, title = '') {
    return this.reply(channel, user, description, title, config.errorColor);
  }

  static DM(user, description, title = '', color = null) {
    return this.send(user, description, title, color);
  }

  static async tryDM(user, description, title = '', color = null) {
    try {
      await this.DM(user, description, title, color);
      return true;
    } catch (err) {
      return false;
    }
  }
}

module.exports = Messenger;

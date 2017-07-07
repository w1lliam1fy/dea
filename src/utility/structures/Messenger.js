const config = require('../../config.json');
const discord = require('discord.js');
const Random = require('./Random.js');
const StringUtil = require('./StringUtil.js');

class Messenger {
  static async trySendEmbed(channel, embed) {
    try {
      await channel.send({ embed: embed });
      return true;
    } catch (err) {
      return false;
    }
  }

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

  static async tryReplyError(channel, user, description, title = '') {
    try {
      await this.replyError(channel, user, description, title);
      return true;
    } catch (err) {
      return false;
    }
  }

  static DM(user, description, guild = null, title = '', color = null) {
    const embed = new discord.RichEmbed()
      .setColor(color || Random.arrayElement(config.embedColors))
      .setDescription(description);

    if (!StringUtil.isNullOrWhiteSpace(title)) {
      embed.setTitle(title);
    }

    if (guild !== null) {
      embed.setFooter(guild.name, guild.iconURL);
    }

    return user.send({ embed: embed});
  }

  static async tryDM(user, description, guild = null, title = '', color = null) {
    try {
      await this.DM(user, description, guild, title, color);
      return true;
    } catch (err) {
      return false;
    }
  }
}

module.exports = Messenger;

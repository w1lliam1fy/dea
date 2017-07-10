const config = require('../../config.json');
const discord = require('discord.js');
const Random = require('./Random.js');
const StringUtil = require('./StringUtil.js');
const NumberUtil = require('./NumberUtil.js');

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

    return channel.send({ embed });
  }

  static async trySend(channel, description, title = '', color = null) {
    try {
      await this.send(channel, description, title, color);
      return true;
    } catch (err) {
      return false;
    }
  }

  static reply(channel, user, description, title = '', color = null) {
    return this.send(channel, StringUtil.boldify(user.tag) + ', ' + description, title, color);
  }

  static tryReply(channel, user, description, title = '', color = null) {
    return this.trySend(channel, StringUtil.boldify(user.tag) + ', ' + description, title, color);
  }

  static sendFields(channel, fieldsAndValues, inline = true, color = null) {
    const embed = new discord.RichEmbed()
      .setColor(color || Random.arrayElement(config.embedColors));

    if (!NumberUtil.isEven(fieldsAndValues.length)) {
      throw new TypeError('The fieldsAndValues length must be even.');
    }

    for (let i = 0; i < fieldsAndValues.length - 1; i++) {
      if (NumberUtil.isEven(i)) {
        embed.addField(fieldsAndValues[i], fieldsAndValues[i + 1], inline);
      }
    }

    return channel.send({ embed });
  }

  static DMFields(user, fieldsAndValues, inline = true, color = null) {
    return this.sendFields(user, fieldsAndValues, inline, color);
  }

  static sendError(channel, description, title = '') {
    return this.send(channel, description, title, config.errorColor);
  }

  static trySendError(channel, description, title = '') {
    return this.trySend(channel, description, title, config.errorColor);
  }

  static replyError(channel, user, description, title = '') {
    return this.reply(channel, user, description, title, config.errorColor);
  }

  static tryReplyError(channel, user, description, title = '') {
    return this.tryReply(channel, user, description, title, config.errorColor);
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

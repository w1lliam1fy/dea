const patron = require('patron.js');
const db = require('../database');
const ModerationService = require('../services/ModerationService.js');

class Owner extends patron.Precondition {
  async run(command, msg) {
    const dbGuild = await db.guildRepo.getGuild(msg.guild.id);

    if (ModerationService.getPermLevel(dbGuild, msg.guild.member(msg.author)) === 3) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'You must be an owner in order to use this command.');
  }
}

module.exports = new Owner();

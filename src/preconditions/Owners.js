const patron = require('patron.js');
const db = require('../database');
const ModerationService = require('../services/ModerationService.js');

class Owners extends patron.Precondition {
  async run(command, context, args) {
    const dbGuild = db.guildRepo.getGuild(context.guild.id);

    if (ModerationService.getPermLevel(dbGuild, context.guild.member(context.author)) == 3) {
      return patron.PreconditionResult.fromSuccess();
    }

    return patron.PreconditionResult.fromError(command, 'You must be an owner in order to use this command.');
  }
}

module.exports = new Owners();

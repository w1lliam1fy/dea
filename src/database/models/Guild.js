class Guild {
  constructor(guildId) {
    this.guildId = guildId;
    this.roles = {
      mod: [],
      rank: [],
      muted: null
    };
    this.channels = {
      gambling: null,
      modLog: null,
      welcome: null
    };
    this.settings = {
      globalChattingMultiplier: 1,
      fines: false,
      welcomeMessage: null
    };
    this.misc = {
      caseNumber: 1,
      trivia: []
    };
  }
}

module.exports = Guild;

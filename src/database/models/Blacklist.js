class Blacklist {
  constructor(userId, username, avatarUrl) {
    this.userId = userId;
    this.username = username;
    this.avatarUrl = avatarUrl;
    this.guildIds = [];
  }
}

module.exports = Blacklist;
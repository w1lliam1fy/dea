const BaseRepository = require('./BaseRepository.js');
const GuildQuery = require('../queries/GuildQuery.js');
const Guild = require('../models/Guild.js');

class GuildRepository extends BaseRepository {
  constructor(collection) { 
    super(collection);
  }

  anyGuild(guildId) {
    return this.any(new GuildQuery(guildId));
  }

  async getGuild(guildId) {
    const fetchedGuild = await this.findOne(new GuildQuery(guildId));
    
    return fetchedGuild !== null ? fetchedGuild : this.insertOne(new Guild(guildId));
  }

  updateGuild(guildId, update) {
    return this.updateOne(new GuildQuery(guildId), update);
  }

  findGuildAndUpdate(guildId, update) {
    return this.findOneAndUpdate(new GuildQuery(guildId), update);
  }

  async upsertGuild(guildId, update) {
    if (await this.anyGuild(guildId)) {
      return this.updateGuild(guildId, update);
    } else {
      return this.updateOne(new Guild(guildId), update, true);
    }
  }

  async findGuildAndUpsert(guildId, update) {
    if (await this.anyGuild(guildId)) {
      return this.findGuildAndUpdate(guildId, update);
    } else {
      return this.findOneAndUpdate(new Guild(guildId), update, true);
    }
  }
}

module.exports = GuildRepository;
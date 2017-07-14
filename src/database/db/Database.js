const MongoClient = require('mongodb').MongoClient;
const UserRepository = require('../repositories/UserRepository.js');
const GuildRepository = require('../repositories/GuildRepository.js');
const MuteRepository = require('../repositories/MuteRepository.js');
const BlacklistRepository = require('../repositories/BlacklistRepository.js');

class Database {
  constructor() {
    this.queries = {
      Blacklist: require('../queries/BlacklistQuery.js'),
      Guild: require('../queries/GuildQuery.js'),
      Id: require('../queries/IdQuery.js'),
      Mute: require('../queries/MuteQuery.js'),
      User: require('../queries/UserQuery.js')
    };

    this.updates = {
      IncMoney: require('../updates/IncMoneyUpdate.js'),
      Pull: require('../updates/PullUpdate.js'),
      Push: require('../updates/PushUpdate.js'),
      Set: require('../updates/SetUpdate.js')
    };

    this.models = {
      Blacklist: require('../models/Blacklist.js'),
      Guild: require('../models/Guild.js'),
      Mute: require('../models/Mute.js'),
      User: require('../models/User.js')
    };
  }

  async connect(connectionURL) {
    const db = await MongoClient.connect(connectionURL);

    this.blacklistRepo = new BlacklistRepository(await db.createCollection('blacklists',
      {
        validator: { $or:
          [
            { userId: { $type: 'string', $exists: true } },
            { guildIds: { $type: 'array', $exists: true } },
            { username: { $type: 'string', $exists: true } },
            { avatarURL: { $type: 'string', $exists: true } }
          ]
        }
      }));

    await db.collection('blacklists').createIndex('userId', { unique: true });

    this.guildRepo = new GuildRepository(await db.createCollection('guilds',
      {
        validator: { $or:
          [
            { guildId: { $type: 'string', $exists: true } },
            { roles: { $type: 'object', $exists: true } },
            { channels: { $type: 'object', $exists: true } },
            { settings: { $type: 'object', $exists: true } },
            { misc: { $type: 'object', $exists: true } }
          ]
        }
      }));

    await db.collection('guilds').createIndex('guildId', { unique: true });

    this.muteRepo = new MuteRepository(await db.createCollection('mutes',
      {
        validator: { $or:
          [
            { userId: { $type: 'string', $exists: true } },
            { guildId: { $type: 'string', $exists: true } },
            { muteLength: { $type: 'int', $exists: true, $gte: 0 } },
            { mutedAt: { $type: 'int', $exists: true, $gte: 0 } }
          ]
        }
      }));

    this.userRepo = new UserRepository(await db.createCollection('users',
      {
        validator: { $or:
          [
            { userId: { $type: 'string', $exists: true } },
            { guildId: { $type: 'string', $exists: true } },
            { cash: { $type: 'int', $exists: true } },
            { reputation: { $type: 'int', $exists: true } },
            { bounty: { $type: 'int', $exists: true, $gte: 0 } },
            { health: { $type: 'int', $exists: true, $gt: 0 } },
            { slaveOwnerId: { $type: 'string', $exists: true } },
            { inventory: { $type: 'array', $exists: true } }
          ]
        }
      }));
  }
}

module.exports = Database;

const path = require('path');
const patron = require('patron.js');
const discord = require('discord.js');
const db = require('../src/database');
const config = require('../src/config.json');
const credentials = require('../src/credentials.json');
const Documentation = require('../src/services/Documentation.js');

async function initiate() {
  const client = new discord.Client({ fetchAllMembers: true, messageCacheMaxSize: 5, messageCacheLifetime: 30, messageSweepInterval: 1800, disabledEvents: config.disabledEvents, restTimeOffset: 150 });
  const registry = new patron.Registry();

  registry.registerDefaultTypeReaders();
  registry.registerGroupsIn(path.join(__dirname, '../src/groups'));
  registry.registerCommandsIn(path.join(__dirname, '../src/commands'));

  await Documentation.createAndSave(registry);
  await db.connect(credentials.mongodbConnectionURL);
  await client.login(credentials.token);
}

initiate()
  .then(() => process.exit(0))
  .catch((err) => {
    console.error(err);
    process.exit(1);
  });

const path = require('path');
const patron = require('patron.js');
const discord = require('discord.js');
const db = require('./database');
const EventService = require('./services/EventService.js');
const CommandService = require('./services/CommandService.js');
const config = require('./config.json');
const credentials = require('./credentials.json');

const client = new discord.Client({ fetchAllMembers: true, messageCacheMaxSize: 5, messageCacheLifetime: 30, messageSweepInterval: 1800, disabledEvents: config.disabledEvents, restTimeOffset: 150 });

const registry = new patron.Registry();

registry.registerDefaultTypeReaders();
registry.registerGroupsIn(path.join(__dirname, 'groups'));
registry.registerCommandsIn(path.join(__dirname, 'commands'));

client.registry = Object.freeze(registry);

EventService.initiate(client);

CommandService.run(client, new patron.Handler(registry));

initiate();

async function initiate() {
  await db.connect(credentials.mongodbConnectionUrl);
  await db.guildRepo.updateMany({}, { $rename: { 'roles.ranks': 'roles.rank' } });
  await client.login(credentials.token);
}
